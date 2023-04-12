using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	internal static class TypeSystem
	{
		internal static Type GetElementType(Type seqType)
		{
			Type type = TypeSystem.FindIEnumerable(seqType);
			if (type == null)
			{
				return seqType;
			}
			return type.GetGenericArguments()[0];
		}

		private static Type FindIEnumerable(Type seqType)
		{
			if (seqType == null || seqType == typeof(string))
			{
				return null;
			}
			if (seqType.IsArray)
			{
				return typeof(IEnumerable<>).MakeGenericType(new Type[]
				{
					seqType.GetElementType()
				});
			}
			if (seqType.IsGenericType)
			{
				foreach (Type type in seqType.GetGenericArguments())
				{
					Type type2 = typeof(IEnumerable<>).MakeGenericType(new Type[]
					{
						type
					});
					if (type2.IsAssignableFrom(seqType))
					{
						return type2;
					}
				}
			}
			Type[] interfaces = seqType.GetInterfaces();
			if (interfaces != null && interfaces.Length != 0)
			{
				Type[] array = interfaces;
				for (int i = 0; i < array.Length; i++)
				{
					Type type3 = TypeSystem.FindIEnumerable(array[i]);
					if (type3 != null)
					{
						return type3;
					}
				}
			}
			if (seqType.BaseType != null && seqType.BaseType != typeof(object))
			{
				return TypeSystem.FindIEnumerable(seqType.BaseType);
			}
			return null;
		}

		public static bool IsEnumerableType(Type type)
		{
			return type.GetInterface("IEnumerable") != null;
		}

		public static bool IsCollectionType(Type type)
		{
			return type.GetInterface("ICollection") != null;
		}
	}
}
