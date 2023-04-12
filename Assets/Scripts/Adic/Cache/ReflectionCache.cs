using System;
using System.Collections.Generic;
using Adic.Binding;

namespace Adic.Cache
{
	public class ReflectionCache : IReflectionCache
	{
		public ReflectedClass this[Type type]
		{
			get
			{
				return this.GetClass(type);
			}
		}

		public IReflectionFactory reflectionFactory { get; set; }

		public ReflectionCache()
		{
			this.reflectionFactory = this.ReflectionFactoryProvider();
		}

		public void Add(Type type)
		{
			if (type == null)
			{
				return;
			}
			if (!this.Contains(type))
			{
				this.classes.Add(type, this.reflectionFactory.Create(type));
			}
		}

		public void Remove(Type type)
		{
			if (this.Contains(type))
			{
				this.classes.Remove(type);
			}
		}

		public ReflectedClass GetClass(Type type)
		{
			if (!this.Contains(type))
			{
				this.Add(type);
			}
			return this.classes[type];
		}

		public bool Contains(Type type)
		{
			return this.classes.ContainsKey(type);
		}

		public void CacheFromBinder(IBinder binder)
		{
			IList<BindingInfo> bindings = binder.GetBindings();
			for (int i = 0; i < bindings.Count; i++)
			{
				BindingInfo bindingInfo = bindings[i];
				if (bindingInfo.instanceType == BindingInstance.Transient && bindingInfo.value is Type)
				{
					this.Add(bindingInfo.value as Type);
				}
				else if (bindingInfo.instanceType == BindingInstance.Singleton)
				{
					this.Add(bindingInfo.value.GetType());
				}
			}
		}

		protected virtual IReflectionFactory ReflectionFactoryProvider()
		{
			return new ReflectionFactory();
		}

		private Dictionary<Type, ReflectedClass> classes = new Dictionary<Type, ReflectedClass>();
	}
}
