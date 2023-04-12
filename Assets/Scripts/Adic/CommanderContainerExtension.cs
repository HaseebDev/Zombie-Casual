using System;
using Adic.Container;

namespace Adic
{
	public class CommanderContainerExtension : IContainerExtension
	{
		public void Init(IInjectionContainer container)
		{
			container.Resolve<ICommandDispatcher>().Init();
		}

		public void OnRegister(IInjectionContainer container)
		{
			container.Bind<ICommandDispatcher>().ToSingleton<CommandDispatcher>();
			CommandDispatcher instance = (CommandDispatcher)container.Resolve<ICommandDispatcher>();
			container.Bind<ICommandPool>().To<CommandDispatcher>(instance);
		}

		public void OnUnregister(IInjectionContainer container)
		{
			container.Unbind<ICommandDispatcher>();
			container.Unbind<ICommandPool>();
			container.Unbind<ICommand>();
		}
	}
}
