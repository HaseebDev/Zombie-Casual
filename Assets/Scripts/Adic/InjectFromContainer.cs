using System;

namespace Adic
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class InjectFromContainer : Attribute
	{
		public InjectFromContainer(object identifier)
		{
			this.identifier = identifier;
		}

		public object identifier;
	}
}
