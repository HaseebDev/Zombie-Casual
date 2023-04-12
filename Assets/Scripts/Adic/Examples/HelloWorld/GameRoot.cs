using System;
using Adic.Container;

namespace Adic.Examples.HelloWorld
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			IInjectionContainer injectionContainer = base.AddContainer<InjectionContainer>();
			injectionContainer.Bind<HelloWorld>().ToSelf();
			injectionContainer.Resolve<HelloWorld>().DisplayHelloWorld();
		}

		public override void Init()
		{
		}
	}
}
