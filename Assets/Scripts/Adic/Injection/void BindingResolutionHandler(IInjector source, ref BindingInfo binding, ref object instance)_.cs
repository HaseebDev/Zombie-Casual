using System;
using Adic.Binding;

namespace Adic.Injection
{
	public delegate void BindingResolutionHandler(IInjector source, ref BindingInfo binding, ref object instance);
}
