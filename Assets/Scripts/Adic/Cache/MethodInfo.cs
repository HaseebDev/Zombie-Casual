using System;
using Adic.Util;

namespace Adic.Cache
{
	public class MethodInfo
	{
		public MethodInfo(string name, ParameterInfo[] parameters)
		{
			this.name = name;
			this.parameters = parameters;
		}

		public MethodCall method;

		public string name;

		public ParamsMethodCall paramsMethod;

		public ParameterInfo[] parameters;
	}
}
