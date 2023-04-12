using System;
using System.Collections.Generic;
using System.Linq;
using Adic.Binding;
using Adic.Cache;
using Adic.Container;
using Adic.Injection;

namespace Adic
{
	public class InjectionContainer : Injector, IInjectionContainer, IBinder, IBindingCreator, IInjector, IDisposable
	{
		public object identifier { get; private set; }

		public InjectionContainer() : this(InjectionContainer.GenerateIdentifier(), new ReflectionCache(), new Binder(), ResolutionMode.ALWAYS_RESOLVE)
		{
		}

		public InjectionContainer(object identifier) : this(identifier, new ReflectionCache(), new Binder(), ResolutionMode.ALWAYS_RESOLVE)
		{
		}

		public InjectionContainer(IReflectionCache cache) : this(InjectionContainer.GenerateIdentifier(), cache, new Binder(), ResolutionMode.ALWAYS_RESOLVE)
		{
		}

		public InjectionContainer(ResolutionMode resolutionMode) : this(InjectionContainer.GenerateIdentifier(), new ReflectionCache(), new Binder(), resolutionMode)
		{
		}

		public InjectionContainer(object identifier, ResolutionMode resolutionMode) : this(identifier, new ReflectionCache(), new Binder(), resolutionMode)
		{
		}

		public InjectionContainer(object identifier, IReflectionCache cache) : this(identifier, cache, new Binder(), ResolutionMode.ALWAYS_RESOLVE)
		{
		}

		public InjectionContainer(IReflectionCache cache, ResolutionMode resolutionMode) : this(InjectionContainer.GenerateIdentifier(), cache, new Binder(), resolutionMode)
		{
		}

		public InjectionContainer(IReflectionCache cache, IBinder binder) : this(InjectionContainer.GenerateIdentifier(), cache, binder, ResolutionMode.ALWAYS_RESOLVE)
		{
		}

		public InjectionContainer(object identifier, IReflectionCache cache, IBinder binder) : this(identifier, cache, binder, ResolutionMode.ALWAYS_RESOLVE)
		{
		}

		public InjectionContainer(object identifier, IReflectionCache cache, ResolutionMode resolutionMode) : this(identifier, cache, new Binder(), resolutionMode)
		{
		}

		public InjectionContainer(object identifier, IReflectionCache cache, IBinder binder, ResolutionMode resolutionMode) : base(cache, binder, resolutionMode)
		{
			this.identifier = identifier;
			this.RegisterItself();
		}

		public IInjectionContainer Init()
		{
			if (this.isInitialized)
			{
				return this;
			}
			base.cache.CacheFromBinder(this);
			if (this.extensions != null)
			{
				foreach (IContainerExtension containerExtension in this.extensions)
				{
					containerExtension.Init(this);
				}
			}
			this.isInitialized = true;
			return this;
		}

		public void Dispose()
		{
			if (this.extensions != null)
			{
				foreach (IContainerExtension containerExtension in this.extensions)
				{
					containerExtension.OnUnregister(this);
				}
				this.extensions.Clear();
				this.extensions = null;
			}
			base.cache = null;
			base.binder = null;
		}

		public IInjectionContainer RegisterExtension<T>() where T : IContainerExtension
		{
			if (this.extensions == null)
			{
				this.extensions = new List<IContainerExtension>();
			}
			IContainerExtension containerExtension = base.Resolve<T>();
			if (!this.HasExtension(containerExtension.GetType()))
			{
				this.extensions.Add(containerExtension);
				containerExtension.OnRegister(this);
			}
			return this;
		}

		public IInjectionContainer UnregisterExtension<T>() where T : IContainerExtension
		{
			foreach (T t in this.extensions.OfType<T>().ToList<T>())
			{
				this.extensions.Remove(t);
				t.OnUnregister(this);
			}
			return this;
		}

		public T GetExtension<T>() where T : IContainerExtension
		{
			return (T)((object)this.GetExtension(typeof(T)));
		}

		public IContainerExtension GetExtension(Type type)
		{
			IContainerExtension result = null;
			if (this.extensions != null)
			{
				foreach (IContainerExtension containerExtension in this.extensions)
				{
					if (containerExtension.GetType().Equals(type))
					{
						result = containerExtension;
						break;
					}
				}
			}
			return result;
		}

		public bool HasExtension<T>()
		{
			return this.HasExtension(typeof(T));
		}

		public bool HasExtension(Type type)
		{
			return this.GetExtension(type) != null;
		}

		public void Clear()
		{
			foreach (BindingInfo bindingInfo in base.binder.GetBindings())
			{
				base.binder.Unbind(bindingInfo.type);
			}
		}

		protected void RegisterItself()
		{
			this.Bind<IInjectionContainer>().To<InjectionContainer>(this);
		}

		public event BindingAddedHandler beforeAddBinding;

		public event BindingAddedHandler afterAddBinding;

		public event BindingRemovedHandler beforeRemoveBinding;

		public event BindingRemovedHandler afterRemoveBinding;

		public IBindingFactory Bind<T>()
		{
			return base.binder.Bind<T>();
		}

		public IBindingFactory Bind(Type type)
		{
			return base.binder.Bind(type);
		}

		public void AddBinding(BindingInfo binding)
		{
			base.binder.AddBinding(binding);
		}

		public IList<BindingInfo> GetBindings()
		{
			return base.binder.GetBindings();
		}

		public IList<BindingInfo> GetBindingsFor<T>()
		{
			return base.binder.GetBindingsFor<T>();
		}

		public IList<BindingInfo> GetBindingsFor(Type type)
		{
			return base.binder.GetBindingsFor(type);
		}

		public IList<BindingInfo> GetBindingsFor(object identifier)
		{
			return base.binder.GetBindingsFor(identifier);
		}

		public IList<BindingInfo> GetBindingsTo<T>()
		{
			return base.binder.GetBindingsTo<T>();
		}

		public IList<BindingInfo> GetBindingsTo(Type type)
		{
			return base.binder.GetBindingsTo(type);
		}

		public bool ContainsBindingFor<T>()
		{
			return base.binder.ContainsBindingFor<T>();
		}

		public bool ContainsBindingFor(Type type)
		{
			return base.binder.ContainsBindingFor(type);
		}

		public bool ContainsBindingFor(object identifier)
		{
			return base.binder.ContainsBindingFor(identifier);
		}

		public void Unbind<T>()
		{
			base.binder.Unbind<T>();
		}

		public void Unbind(Type type)
		{
			base.binder.Unbind(type);
		}

		public void Unbind(object identifier)
		{
			base.binder.Unbind(identifier);
		}

		public void UnbindInstance(object instance)
		{
			base.binder.UnbindInstance(instance);
		}

		public void UnbindByTag(string tag)
		{
			base.binder.UnbindByTag(tag);
		}

		private static string GenerateIdentifier()
		{
			return Guid.NewGuid().ToString();
		}

		protected const ResolutionMode DEFAULT_RESOLUTION_MODE = ResolutionMode.ALWAYS_RESOLVE;

		private bool isInitialized;

		private List<IContainerExtension> extensions;
	}
}
