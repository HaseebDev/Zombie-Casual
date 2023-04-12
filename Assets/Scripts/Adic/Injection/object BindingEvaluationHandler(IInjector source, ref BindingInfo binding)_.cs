using System;
using Adic.Binding;

namespace Adic.Injection
{
	public delegate object BindingEvaluationHandler(IInjector source, ref BindingInfo binding);
}
