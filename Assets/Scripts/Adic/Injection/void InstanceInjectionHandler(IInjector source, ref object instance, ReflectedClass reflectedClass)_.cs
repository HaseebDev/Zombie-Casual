using System;
using Adic.Cache;

namespace Adic.Injection
{
	public delegate void InstanceInjectionHandler(IInjector source, ref object instance, ReflectedClass reflectedClass);
}
