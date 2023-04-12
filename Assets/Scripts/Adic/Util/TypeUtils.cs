using System;
using System.Collections.Generic;
using System.Reflection;

namespace Adic.Util
{
	public static class TypeUtils
	{
		public static bool IsAssignable(Type potentialBase, Type potentialDescendant)
		{
			return potentialBase.Equals(potentialDescendant) || potentialBase.IsAssignableFrom(potentialDescendant);
		}

		public static Type[] GetAssignableTypes(Type baseType)
		{
			return TypeUtils.GetAssignableTypes(baseType, string.Empty, false);
		}

		public static Type[] GetAssignableTypes(Type baseType, string namespaceName)
		{
			return TypeUtils.GetAssignableTypes(baseType, namespaceName, false);
		}

		public static Type[] GetAssignableTypes(Type baseType, string namespaceName, bool includeChildren)
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				if (!assembly.FullName.StartsWith("Unity") && !assembly.FullName.StartsWith("Boo") && !assembly.FullName.StartsWith("Mono") && !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("mscorlib"))
				{
					try
					{
						foreach (Type type in assemblies[i].GetTypes())
						{
							if ((string.IsNullOrEmpty(namespaceName) || (includeChildren && !string.IsNullOrEmpty(type.Namespace) && type.Namespace.StartsWith(namespaceName)) || (!includeChildren && type.Namespace == namespaceName)) && type.IsClass && TypeUtils.IsAssignable(baseType, type))
							{
								list.Add(type);
							}
						}
					}
					catch (ReflectionTypeLoadException)
					{
					}
				}
			}
			return list.ToArray();
		}

		public static Type GetType(string typeName)
		{
			return TypeUtils.GetType(string.Empty, typeName);
		}

		public static Type GetType(string namespaceName, string typeName)
		{
			string text = null;
			if (!string.IsNullOrEmpty(typeName))
			{
				if (string.IsNullOrEmpty(namespaceName) || namespaceName == "-")
				{
					text = typeName;
				}
				else
				{
					text = string.Format("{0}.{1}", namespaceName, typeName);
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				if (!assembly.FullName.StartsWith("Unity") && !assembly.FullName.StartsWith("Boo") && !assembly.FullName.StartsWith("Mono") && !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("mscorlib"))
				{
					try
					{
						foreach (Type type in assemblies[i].GetTypes())
						{
							if (type.FullName == text)
							{
								return type;
							}
						}
					}
					catch (ReflectionTypeLoadException)
					{
					}
				}
			}
			return null;
		}
	}
}
