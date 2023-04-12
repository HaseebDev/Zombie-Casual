using System;

namespace Adic.Binding
{
	public class BindingInfo
	{
		public Type type { get; private set; }

		public object value { get; set; }

		public BindingInstance instanceType { get; private set; }

		public object identifier { get; set; }

		public BindingCondition condition { get; set; }

		public string[] tags
		{
			get
			{
				if (this.originalBinding != null)
				{
					return this.originalBinding.tags;
				}
				return this.bindingTags;
			}
			set
			{
				this.bindingTags = value;
			}
		}

		public BindingInfo(Type type, object value, BindingInstance instanceType) : this(type, value, instanceType, null)
		{
		}

		public BindingInfo(Type type, object value, BindingInstance instanceType, BindingInfo originalBinding)
		{
			this.type = type;
			this.value = value;
			this.instanceType = instanceType;
			this.originalBinding = originalBinding;
		}

		public Type GetValueType()
		{
			if (!(this.value is Type))
			{
				return this.value.GetType();
			}
			return (Type)this.value;
		}

		public override string ToString()
		{
			return string.Format("Type: {0}\nBound to: {1} ({2})\nBinding type: {3}\nIdentifier: {4}\nConditions: {5}\nTags: {6}\n", new object[]
			{
				this.type.FullName,
				(this.value == null) ? "-" : this.value.ToString(),
				(this.value == null) ? "-" : ((this.value is Type) ? "type" : ("instance [" + this.value.GetHashCode() + "]")),
				this.instanceType.ToString(),
				(this.identifier == null) ? "-" : this.identifier.ToString(),
				(this.condition == null) ? "no" : "yes",
				(this.tags == null) ? "[]" : string.Join(",", this.tags)
			});
		}

		private string[] bindingTags;

		private BindingInfo originalBinding;
	}
}
