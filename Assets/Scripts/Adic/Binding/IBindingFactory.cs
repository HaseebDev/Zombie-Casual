using System;

namespace Adic.Binding
{
	public interface IBindingFactory
	{
		IBinder binder { get; }

		Type bindingType { get; }

		IBindingConditionFactory ToSelf();

		IBindingConditionFactory ToSingleton();

		IBindingConditionFactory ToSingleton<T>() where T : class;

		IBindingConditionFactory ToSingleton(Type type);

		IBindingConditionFactory To<T>() where T : class;

		IBindingConditionFactory To(Type type);

		IBindingConditionFactory To<T>(T instance);

		IBindingConditionFactory To(Type type, object instance);

		IBindingConditionFactory ToNamespace(string namespaceName);

		IBindingConditionFactory ToNamespace(string namespaceName, bool includeChildren);

		IBindingConditionFactory ToNamespaceSingleton(string namespaceName);

		IBindingConditionFactory ToNamespaceSingleton(string namespaceName, bool includeChildren);

		IBindingConditionFactory ToFactory<T>() where T : IFactory;

		IBindingConditionFactory ToFactory(Type type);

		IBindingConditionFactory ToFactory(IFactory factory);

		IBindingConditionFactory AddBinding(object value, BindingInstance instanceType);
	}
}
