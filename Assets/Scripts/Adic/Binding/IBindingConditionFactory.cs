using System;

namespace Adic.Binding
{
	public interface IBindingConditionFactory : IBindingCreator
	{
		IBindingConditionFactory As(object identifier);

		IBindingConditionFactory When(BindingCondition condition);

		IBindingConditionFactory WhenInto<T>();

		IBindingConditionFactory WhenInto(Type type);

		IBindingConditionFactory WhenIntoInstance(object instance);

		IBindingConditionFactory Tag(params string[] tags);
	}
}
