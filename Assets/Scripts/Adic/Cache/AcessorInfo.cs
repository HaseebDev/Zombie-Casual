using System;
using Adic.Util;

namespace Adic.Cache
{
	public class AcessorInfo : ParameterInfo
	{
		public AcessorInfo(Type type, string name, object identifier, GetterCall getter, SetterCall setter) : base(type, name, identifier)
		{
			this.getter = getter;
			this.setter = setter;
		}

		public GetterCall getter;

		public SetterCall setter;
	}
}
