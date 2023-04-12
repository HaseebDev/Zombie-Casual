using System;
using Adic.Binding;

namespace Adic
{
	public class UnityBindingConditionFactory : IBindingConditionFactory, IBindingCreator
	{
		public UnityBindingConditionFactory(IBindingConditionFactory bindingConditionFactory, string objectName)
		{
			this.bindingConditionFactory = bindingConditionFactory;
			this.objectName = objectName;
		}

		public IBindingFactory Bind<T>()
		{
			return this.bindingConditionFactory.Bind<T>();
		}

		public IBindingFactory Bind(Type type)
		{
			return this.bindingConditionFactory.Bind(type);
		}

		public IBindingConditionFactory As(object identifier)
		{
			return this.bindingConditionFactory.As(identifier);
		}

		public IBindingConditionFactory AsObjectName()
		{
			return this.bindingConditionFactory.As(this.objectName);
		}

		public IBindingConditionFactory When(BindingCondition condition)
		{
			return this.bindingConditionFactory.When(condition);
		}

		public IBindingConditionFactory WhenInto<T>()
		{
			return this.bindingConditionFactory.WhenInto<T>();
		}

		public IBindingConditionFactory WhenInto(Type type)
		{
			return this.bindingConditionFactory.WhenInto(type);
		}

		public IBindingConditionFactory WhenIntoInstance(object instance)
		{
			return this.bindingConditionFactory.WhenIntoInstance(instance);
		}

		public IBindingConditionFactory Tag(params string[] tags)
		{
			return this.bindingConditionFactory.Tag(tags);
		}

		private IBindingConditionFactory bindingConditionFactory;

		private string objectName;
	}
}
