using System;

namespace Adic
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class BindingPriority : Attribute
	{
		public BindingPriority()
		{
			this.priority = 1;
		}

		public BindingPriority(int priority)
		{
			this.priority = priority;
		}

		public int priority;
	}
}
