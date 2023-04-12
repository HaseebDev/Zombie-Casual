using System;

namespace Adic.Binding
{
	public class MultipleBindingConditionFactory : IBindingConditionFactory, IBindingCreator
	{
		public MultipleBindingConditionFactory(IBindingConditionFactory[] bindingConditionFactories, IBindingCreator bindindCreator)
		{
			this.bindingConditionFactories = bindingConditionFactories;
			this.bindindCreator = bindindCreator;
		}

		public IBindingConditionFactory As(object identifier)
		{
			for (int i = 0; i < this.bindingConditionFactories.Length; i++)
			{
				this.bindingConditionFactories[i].As(identifier);
			}
			return this;
		}

		public IBindingConditionFactory When(BindingCondition condition)
		{
			for (int i = 0; i < this.bindingConditionFactories.Length; i++)
			{
				this.bindingConditionFactories[i].When(condition);
			}
			return this;
		}

		public IBindingConditionFactory WhenInto<T>()
		{
			for (int i = 0; i < this.bindingConditionFactories.Length; i++)
			{
				this.bindingConditionFactories[i].WhenInto<T>();
			}
			return this;
		}

		public IBindingConditionFactory WhenInto(Type type)
		{
			for (int i = 0; i < this.bindingConditionFactories.Length; i++)
			{
				this.bindingConditionFactories[i].WhenInto(type);
			}
			return this;
		}

		public IBindingConditionFactory WhenIntoInstance(object instance)
		{
			for (int i = 0; i < this.bindingConditionFactories.Length; i++)
			{
				this.bindingConditionFactories[i].WhenIntoInstance(instance);
			}
			return this;
		}

		public IBindingConditionFactory Tag(params string[] tags)
		{
			for (int i = 0; i < this.bindingConditionFactories.Length; i++)
			{
				this.bindingConditionFactories[i].Tag(tags);
			}
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

		protected IBindingConditionFactory[] bindingConditionFactories;

		protected IBindingCreator bindindCreator;
	}
}
