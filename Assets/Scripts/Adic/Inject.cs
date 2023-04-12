using System;

namespace Adic
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public class Inject : Attribute
	{
		public Inject()
		{
			this.identifier = null;
		}

		public Inject(object identifier)
		{
			this.identifier = identifier;
		}

		public object identifier;
	}
}
