using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Adic.Util
{
	public static class MethodUtils
	{
		public static ConstructorCall CreateConstructor(Type type, ConstructorInfo constructor)
		{
			return null;
		
		}

		public static ParamsConstructorCall CreateConstructorWithParams(Type type, ConstructorInfo constructor)
		{
			return null;

		}

		public static SetterCall CreateFieldSetter(Type type, FieldInfo fieldInfo)
		{
			return null;

		}

		public static GetterCall CreateFieldGetter(Type type, FieldInfo fieldInfo)
		{
			return (object instance) => fieldInfo.GetValue(instance);
		}

		public static SetterCall CreatePropertySetter(Type type, PropertyInfo propertyInfo)
		{
			return null;

		}

		public static GetterCall CreatePropertyGetter(Type type, PropertyInfo propertyInfo)
		{
			if (propertyInfo.CanRead)
			{
				return (object instance) => propertyInfo.GetValue(instance, null);
			}
			return null;
		}

		public static MethodCall CreateParameterlessMethod(Type type, MethodInfo methodInfo)
		{
			return null;

		}

		public static ParamsMethodCall CreateParameterizedMethod(Type type, MethodInfo methodInfo)
		{
			return delegate(object instance, object[] parameters)
			{
				methodInfo.Invoke(instance, parameters);
			};
		}

		private static Type OBJECT_TYPE = typeof(object);
	}
}
