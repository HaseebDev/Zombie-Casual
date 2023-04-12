using System;
using System.Linq;
using Adic.Container;
using Adic.Util;

namespace Adic
{
	public static class BindingsSetupExtension
	{
		public static IInjectionContainer SetupBindings<T>(this IInjectionContainer container) where T : IBindingsSetup, new()
		{
			container.SetupBindings(typeof(T));
			return container;
		}

		public static IInjectionContainer SetupBindings(this IInjectionContainer container, Type type)
		{
			object obj = container.Resolve(type);
			container.SetupBindings((IBindingsSetup)obj);
			return container;
		}

		public static IInjectionContainer SetupBindings(this IInjectionContainer container, IBindingsSetup setup)
		{
			setup.SetupBindings(container);
			return container;
		}

		public static IInjectionContainer SetupBindings(this IInjectionContainer container, string namespaceName)
		{
			container.SetupBindings(namespaceName, true);
			return container;
		}

		public static IInjectionContainer SetupBindings(this IInjectionContainer container, string namespaceName, bool includeChildren)
		{
			Type[] assignableTypes = TypeUtils.GetAssignableTypes(typeof(IBindingsSetup), namespaceName, includeChildren);
			BindingsSetupExtension.PrioritizedBindingSetup[] array = new BindingsSetupExtension.PrioritizedBindingSetup[assignableTypes.Length];
			for (int i = 0; i < assignableTypes.Length; i++)
			{
				IBindingsSetup bindingsSetup = (IBindingsSetup)container.Resolve(assignableTypes[i]);
				object[] customAttributes = bindingsSetup.GetType().GetCustomAttributes(typeof(BindingPriority), true);
				if (customAttributes.Length != 0)
				{
					BindingPriority bindingPriority = customAttributes[0] as BindingPriority;
					array[i] = new BindingsSetupExtension.PrioritizedBindingSetup
					{
						setup = bindingsSetup,
						priority = bindingPriority.priority
					};
				}
				else
				{
					array[i] = new BindingsSetupExtension.PrioritizedBindingSetup
					{
						setup = bindingsSetup,
						priority = 0
					};
				}
			}
			array = (from setup in array
			orderby setup.priority descending
			select setup).ToArray<BindingsSetupExtension.PrioritizedBindingSetup>();
			for (int j = 0; j < array.Length; j++)
			{
				array[j].setup.SetupBindings(container);
			}
			return container;
		}

		private class PrioritizedBindingSetup
		{
			public IBindingsSetup setup;

			public int priority;
		}
	}
}
