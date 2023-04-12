using System;
using Adic.Exceptions;
using Adic.Util;

namespace Adic.Binding
{
	public class BindingFactory : IBindingFactory
	{
		public IBinder binder { get; private set; }

		public Type bindingType { get; private set; }

		public BindingFactory(Type bindingType, IBinder binder)
		{
			this.bindingType = bindingType;
			this.binder = binder;
		}

		public IBindingConditionFactory ToSelf()
		{
			return this.AddBinding(this.bindingType, BindingInstance.Transient);
		}

		public IBindingConditionFactory ToSingleton()
		{
			return this.AddBinding(this.bindingType, BindingInstance.Singleton);
		}

		public IBindingConditionFactory ToSingleton<T>() where T : class
		{
			return this.ToSingleton(typeof(T));
		}

		public IBindingConditionFactory ToSingleton(Type type)
		{
			if (!TypeUtils.IsAssignable(this.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			return this.AddBinding(type, BindingInstance.Singleton);
		}

		public IBindingConditionFactory To<T>() where T : class
		{
			return this.To(typeof(T));
		}

		public IBindingConditionFactory To(Type type)
		{
			if (!TypeUtils.IsAssignable(this.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			return this.AddBinding(type, BindingInstance.Transient);
		}

		public IBindingConditionFactory To<T>(T instance)
		{
			return this.To(typeof(T), instance);
		}

		public IBindingConditionFactory To(Type type, object instance)
		{
			if (!TypeUtils.IsAssignable(this.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			if (!TypeUtils.IsAssignable(type, instance.GetType()))
			{
				throw new BindingException("The instance is not of the given type.");
			}
			return this.AddBinding(instance, BindingInstance.Singleton);
		}

		public IBindingConditionFactory ToNamespace(string namespaceName)
		{
			return this.ToNamespace(namespaceName, BindingInstance.Transient, false);
		}

		public IBindingConditionFactory ToNamespace(string namespaceName, bool includeChildren)
		{
			return this.ToNamespace(namespaceName, BindingInstance.Transient, includeChildren);
		}

		public IBindingConditionFactory ToNamespaceSingleton(string namespaceName)
		{
			return this.ToNamespace(namespaceName, BindingInstance.Singleton, false);
		}

		public IBindingConditionFactory ToNamespaceSingleton(string namespaceName, bool includeChildren)
		{
			return this.ToNamespace(namespaceName, BindingInstance.Singleton, includeChildren);
		}

		protected IBindingConditionFactory ToNamespace(string namespaceName, BindingInstance bindingInstance, bool includeChildren)
		{
			Type[] assignableTypes = TypeUtils.GetAssignableTypes(this.bindingType, namespaceName, includeChildren);
			IBindingConditionFactory[] array = new IBindingConditionFactory[assignableTypes.Length];
			for (int i = 0; i < assignableTypes.Length; i++)
			{
				array[i] = this.AddBinding(assignableTypes[i], bindingInstance);
			}
			return this.CreateBindingConditionFactoryProvider(array);
		}

		public IBindingConditionFactory ToFactory<T>() where T : IFactory
		{
			return this.ToFactory(typeof(T));
		}

		public IBindingConditionFactory ToFactory(Type type)
		{
			if (!TypeUtils.IsAssignable(typeof(IFactory), type))
			{
				throw new BindingException("The type doesn't implement Adic.IFactory.");
			}
			return this.AddBinding(type, BindingInstance.Factory);
		}

		public IBindingConditionFactory ToFactory(IFactory factory)
		{
			return this.AddBinding(factory, BindingInstance.Factory);
		}

		public IBindingConditionFactory AddBinding(object value, BindingInstance instanceType)
		{
			BindingInfo binding = new BindingInfo(this.bindingType, value, instanceType);
			this.binder.AddBinding(binding);
			return this.CreateBindingConditionFactoryProvider(binding);
		}

		protected virtual IBindingConditionFactory CreateBindingConditionFactoryProvider(BindingInfo binding)
		{
			return new SingleBindingConditionFactory(binding, this.binder);
		}

		protected virtual IBindingConditionFactory CreateBindingConditionFactoryProvider(IBindingConditionFactory[] bindingConditionFactories)
		{
			return new MultipleBindingConditionFactory(bindingConditionFactories, this.binder);
		}
	}
}
