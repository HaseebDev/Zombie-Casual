using System;

namespace Adic.Cache
{
	public interface IReflectionFactory
	{
		ReflectedClass Create(Type type);
	}
}
