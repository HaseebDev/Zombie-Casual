using System;
using System.Collections.Generic;
using System.Reflection;
using Adic.Commander.Exceptions;
using Adic.Container;
using Adic.Util;

namespace Adic.Commander
{
	public static class CommanderUtils
	{
		public static Type[] GetAvailableCommands()
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				if (!assembly.FullName.StartsWith("Unity") && !assembly.FullName.StartsWith("Boo") && !assembly.FullName.StartsWith("Mono") && !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("mscorlib"))
				{
					Type typeFromHandle = typeof(ICommand);
					foreach (Type type in assemblies[i].GetTypes())
					{
						if (type.Namespace != "Adic" && type.IsClass && TypeUtils.IsAssignable(typeFromHandle, type))
						{
							list.Add(type);
						}
					}
				}
			}
			return list.ToArray();
		}

		public static void DispatchCommand(Type type, params object[] parameters)
		{
			bool flag = false;
			List<ContextRoot.InjectionContainerData> containersData = ContextRoot.containersData;
			for (int i = 0; i < containersData.Count; i++)
			{
				IInjectionContainer container = containersData[i].container;
				if (container.ContainsBindingFor<ICommandDispatcher>())
				{
					ICommandDispatcher commandDispatcher = container.GetCommandDispatcher();
					if (commandDispatcher.ContainsRegistration(type))
					{
						flag = true;
						commandDispatcher.Dispatch(type, parameters);
						break;
					}
				}
			}
			if (!flag)
			{
				throw new CommandException(string.Format("There is no command registered for the type {0}.", type));
			}
		}

		public static Dictionary<string, IList<string>> GetTypesAsString(Type[] types)
		{
			Dictionary<string, IList<string>> dictionary = new Dictionary<string, IList<string>>();
			foreach (Type type in types)
			{
				string key = "-";
				if (!string.IsNullOrEmpty(type.Namespace))
				{
					key = type.Namespace;
				}
				IList<string> list;
				if (dictionary.ContainsKey(key))
				{
					list = dictionary[key];
				}
				else
				{
					list = new List<string>();
					dictionary.Add(key, list);
				}
				list.Add(type.Name);
			}
			return dictionary;
		}
	}
}
