using System;
using Adic.Examples.Factory.Commands;

namespace Adic.Examples.Factory
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			this.dispatcher = base.AddContainer<InjectionContainer>().RegisterExtension<CommanderContainerExtension>().RegisterExtension<EventCallerContainerExtension>().RegisterExtension<UnityBindingContainerExtension>().SetupBindings("Adic.Examples.Factory.Bindings").RegisterCommands("Adic.Examples.Factory.Commands").GetCommandDispatcher();
		}

		public override void Init()
		{
			this.dispatcher.Dispatch<SpawnObjectsCommand>(Array.Empty<object>());
		}

		protected ICommandDispatcher dispatcher;
	}
}
