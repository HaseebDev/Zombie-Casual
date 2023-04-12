using System;
using Adic;

namespace Framework.Common
{
	public abstract class BaseMainContext : ContextRoot
	{
		public override void SetupContainers()
		{
			base.AddContainer<InjectionContainer>().RegisterExtension<UnityBindingContainerExtension>().RegisterExtension<CommanderContainerExtension>().RegisterExtension<EventCallerContainerExtension>();
			this.BindConfigs();
			this.BindComponents();
			this.BindView();
			this.BindManagers();
			this.BindModels();
			this.BindControllers();
			this.BindCommands();
		}

		protected abstract void BindView();

		protected abstract void BindManagers();

		protected abstract void BindConfigs();

		protected abstract void BindModels();

		protected abstract void BindControllers();

		protected abstract void BindComponents();

		protected abstract void BindCommands();

		public override void Init()
		{
		}
	}
}
