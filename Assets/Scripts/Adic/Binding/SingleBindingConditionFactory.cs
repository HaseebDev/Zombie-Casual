using System;
using Adic.Injection;

namespace Adic.Binding
{
	public class SingleBindingConditionFactory : IBindingConditionFactory, IBindingCreator
	{
		public SingleBindingConditionFactory(BindingInfo binding, IBindingCreator bindindCreator)
		{
			this.binding = binding;
			this.bindindCreator = bindindCreator;
		}

		public IBindingConditionFactory As(object identifier)
		{
			this.binding.identifier = identifier;
			return this;
		}

		public IBindingConditionFactory When(BindingCondition condition)
		{
			this.binding.condition = condition;
			return this;
		}

		public IBindingConditionFactory WhenInto<T>()
		{
			this.binding.condition = ((InjectionContext context) => context.parentType == typeof(T));
			return this;
		}

		public IBindingConditionFactory WhenInto(Type type)
		{
			this.binding.condition = ((InjectionContext context) => context.parentType == type);
			return this;
		}

		public IBindingConditionFactory WhenIntoInstance(object instance)
		{
			this.binding.condition = ((InjectionContext context) => context.parentInstance == instance);
			return this;
		}

		public IBindingConditionFactory Tag(params string[] tags)
		{
			this.binding.tags = tags;
			return this;
		}

		public IBindingFactory Bind<T>()
		{
			return this.bindindCreator.Bind<T>();
		}

		public IBindingFactory Bind(Type type)
		{
			return this.bindindCreator.Bind(type);
		}

		protected BindingInfo binding;

		protected IBindingCreator bindindCreator;
	}
}
