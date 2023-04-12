using System;

namespace Adic.Container
{
	public interface IContainerExtension
	{
		void Init(IInjectionContainer container);

		void OnRegister(IInjectionContainer container);

		void OnUnregister(IInjectionContainer container);
	}
}
