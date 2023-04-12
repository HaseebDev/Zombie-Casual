using System;
using Adic.Util;

namespace Adic.Cache
{
	public class ReflectedClass
	{
		public Type type { get; set; }

		public ConstructorCall constructor { get; set; }

		public ParamsConstructorCall paramsConstructor { get; set; }

		public ParameterInfo[] constructorParameters { get; set; }

		public MethodInfo[] methods { get; set; }

		public AcessorInfo[] properties { get; set; }

		public AcessorInfo[] fields { get; set; }
	}
}
