using System;
using System.Collections.Generic;
using Adic.Binding;
using Adic.Container;

namespace Adic.Extensions.MonoInjection
{
	public static class InjectionUtil
	{
		public static void Inject(object obj)
		{
			object[] customAttributes = obj.GetType().GetCustomAttributes(true);
			if (customAttributes.Length == 0)
			{
				InjectionUtil.Inject(obj, null);
				return;
			}
			bool flag = false;
			foreach (object obj2 in customAttributes)
			{
				if (obj2 is InjectFromContainer)
				{
					InjectionUtil.Inject(obj, (obj2 as InjectFromContainer).identifier);
					flag = true;
				}
			}
			if (!flag)
			{
				InjectionUtil.Inject(obj, null);
			}
		}

		public static void Inject(object obj, object identifier)
		{
			List<ContextRoot.InjectionContainerData> containersData = ContextRoot.containersData;
			for (int i = 0; i < containersData.Count; i++)
			{
				IInjectionContainer container = containersData[i].container;
				bool flag = container.identifier != null && container.identifier.Equals(identifier);
				if ((identifier == null || flag) && !InjectionUtil.IsSingletonOnContainer(obj, container))
				{
					container.Inject(obj);
				}
			}
		}

		public static bool IsSingletonOnContainer(object obj, IInjectionContainer container)
		{
			bool result = false;
			IList<BindingInfo> bindingsFor = container.GetBindingsFor(obj.GetType());
			if (bindingsFor == null)
			{
				return false;
			}
			for (int i = 0; i < bindingsFor.Count; i++)
			{
				if (bindingsFor[i].value == obj)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
