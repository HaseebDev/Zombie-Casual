using System;
using Adic.Container;
using Adic.Util;

namespace Adic
{
	public static class CommanderExtension
	{
		public static ICommandDispatcher GetCommandDispatcher(this IInjectionContainer container)
		{
			return container.Resolve<ICommandDispatcher>();
		}

		public static IInjectionContainer RegisterCommand<T>(this IInjectionContainer container) where T : ICommand, new()
		{
			container.RegisterCommand(typeof(T));
			return container;
		}

		public static IInjectionContainer RegisterCommand(this IInjectionContainer container, Type type)
		{
			container.Resolve<ICommandPool>().AddCommand(type);
			return container;
		}

		public static IInjectionContainer RegisterCommands(this IInjectionContainer container, string namespaceName)
		{
			container.RegisterCommands(namespaceName, true);
			return container;
		}

		public static IInjectionContainer RegisterCommands(this IInjectionContainer container, string namespaceName, bool includeChildren)
		{
			Type[] assignableTypes = TypeUtils.GetAssignableTypes(typeof(ICommand), namespaceName, includeChildren);
			if (assignableTypes.Length != 0)
			{
				ICommandPool commandPool = container.Resolve<ICommandPool>();
				for (int i = 0; i < assignableTypes.Length; i++)
				{
					commandPool.AddCommand(assignableTypes[i]);
				}
			}
			return container;
		}
	}
}
