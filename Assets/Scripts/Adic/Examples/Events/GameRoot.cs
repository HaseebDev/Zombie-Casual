using System;

namespace Adic.Examples.Events
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			base.AddContainer<InjectionContainer>().RegisterExtension<EventCallerContainerExtension>().Bind<EventReceiver>().ToSingleton();
		}

		public override void Init()
		{
		}
	}
}
