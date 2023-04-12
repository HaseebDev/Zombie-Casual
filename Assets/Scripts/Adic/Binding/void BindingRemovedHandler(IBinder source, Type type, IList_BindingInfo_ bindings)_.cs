using System;
using System.Collections.Generic;

namespace Adic.Binding
{
	public delegate void BindingRemovedHandler(IBinder source, Type type, IList<BindingInfo> bindings);
}
