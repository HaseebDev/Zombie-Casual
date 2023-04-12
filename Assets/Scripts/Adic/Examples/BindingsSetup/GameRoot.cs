using System;

namespace Adic.Examples.BindingsSetup
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			base.AddContainer<InjectionContainer>().RegisterExtension<UnityBindingContainerExtension>().SetupBindings("Adic.Examples.BindingsSetup.Bindings");
		}

		public override void Init()
		{
		}
	}
}
