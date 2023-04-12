using System;
using Adic.Container;

namespace Adic
{
	public interface IContextRoot
	{
		IInjectionContainer[] containers { get; }

		IInjectionContainer AddContainer<T>() where T : IInjectionContainer, new();

		IInjectionContainer AddContainer(IInjectionContainer container);

		IInjectionContainer AddContainer(IInjectionContainer container, bool destroyOnLoad);

		void SetupContainers();

		void Init();
	}
}
