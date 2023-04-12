using System;

namespace Adic.Cache
{
	public class ParameterInfo
	{
		public ParameterInfo(Type type, string name, object identifier)
		{
			this.type = type;
			this.name = name;
			this.identifier = identifier;
		}

		public Type type;

		public string name;

		public object identifier;
	}
}
