using System;
using Adic.Container;

namespace Adic
{
	public interface IBindingsSetup
	{
		void SetupBindings(IInjectionContainer container);
	}
}
