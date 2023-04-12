using System;

namespace Adic.Injection
{
	public delegate bool TypeResolutionHandler(IInjector source, Type type, InjectionMember member, object parentInstance, object identifier, ref object resolutionInstance);
}
