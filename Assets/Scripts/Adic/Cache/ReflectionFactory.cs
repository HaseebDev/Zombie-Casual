using System;
using System.Collections.Generic;
using System.Reflection;
using Adic.Util;

namespace Adic.Cache
{
	public class ReflectionFactory : IReflectionFactory
	{
		public ReflectedClass Create(Type type)
		{
			ReflectedClass reflectedClass = new ReflectedClass();
			reflectedClass.type = type;
			ConstructorInfo constructorInfo = this.ResolveConstructor(type);
			if (constructorInfo != null)
			{
				if (constructorInfo.GetParameters().Length == 0)
				{
					reflectedClass.constructor = MethodUtils.CreateConstructor(type, constructorInfo);
				}
				else
				{
					reflectedClass.paramsConstructor = MethodUtils.CreateConstructorWithParams(type, constructorInfo);
				}
			}
			reflectedClass.constructorParameters = this.ResolveConstructorParameters(constructorInfo);
			reflectedClass.methods = this.ResolveMethods(type);
			reflectedClass.properties = this.ResolveProperties(type);
			reflectedClass.fields = this.ResolveFields(type);
			return reflectedClass;
		}

		protected ConstructorInfo ResolveConstructor(Type type)
		{
			ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
			if (constructors.Length == 0)
			{
				return null;
			}
			if (constructors.Length == 1)
			{
				return constructors[0];
			}
			ConstructorInfo result = null;
			int i = 0;
			int num = int.MaxValue;
			while (i < constructors.Length)
			{
				ConstructorInfo constructorInfo = constructors[i];
				object[] customAttributes = constructorInfo.GetCustomAttributes(typeof(Construct), true);
				object[] customAttributes2 = constructorInfo.GetCustomAttributes(typeof(Inject), true);
				if (customAttributes.Length != 0 || customAttributes2.Length != 0)
				{
					return constructorInfo;
				}
				int num2 = constructorInfo.GetParameters().Length;
				if (num2 < num)
				{
					num = num2;
					result = constructorInfo;
				}
				i++;
			}
			return result;
		}

		protected ParameterInfo[] ResolveConstructorParameters(ConstructorInfo constructor)
		{
			if (constructor == null)
			{
				return null;
			}
			System.Reflection.ParameterInfo[] parameters = constructor.GetParameters();
			ParameterInfo[] array = new ParameterInfo[parameters.Length];
			for (int i = 0; i < array.Length; i++)
			{
				object identifier = null;
				System.Reflection.ParameterInfo parameterInfo = parameters[i];
				object[] customAttributes = parameterInfo.GetCustomAttributes(typeof(Inject), true);
				if (customAttributes.Length != 0)
				{
					identifier = (customAttributes[0] as Inject).identifier;
				}
				array[i] = new ParameterInfo(parameterInfo.ParameterType, parameterInfo.Name, identifier);
			}
			return array;
		}

		protected MethodInfo[] ResolveMethods(Type type)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			foreach (System.Reflection.MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(PostConstruct), true);
				object[] customAttributes2 = methodInfo.GetCustomAttributes(typeof(Inject), true);
				if (customAttributes.Length != 0 || customAttributes2.Length != 0)
				{
					System.Reflection.ParameterInfo[] parameters = methodInfo.GetParameters();
					ParameterInfo[] array = new ParameterInfo[parameters.Length];
					for (int j = 0; j < array.Length; j++)
					{
						object identifier = null;
						System.Reflection.ParameterInfo parameterInfo = parameters[j];
						object[] customAttributes3 = parameterInfo.GetCustomAttributes(typeof(Inject), true);
						if (customAttributes3.Length != 0)
						{
							identifier = (customAttributes3[0] as Inject).identifier;
						}
						array[j] = new ParameterInfo(parameterInfo.ParameterType, parameterInfo.Name, identifier);
					}
					MethodInfo methodInfo2 = new MethodInfo(methodInfo.Name, array);
					if (array.Length == 0)
					{
						methodInfo2.method = MethodUtils.CreateParameterlessMethod(type, methodInfo);
					}
					else
					{
						methodInfo2.paramsMethod = MethodUtils.CreateParameterizedMethod(type, methodInfo);
					}
					list.Add(methodInfo2);
				}
			}
			return list.ToArray();
		}

		protected AcessorInfo[] ResolveProperties(Type type)
		{
			List<AcessorInfo> list = new List<AcessorInfo>();
			foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(Inject), true);
				if (customAttributes.Length != 0)
				{
					Inject inject = customAttributes[0] as Inject;
					GetterCall getter = MethodUtils.CreatePropertyGetter(type, propertyInfo);
					SetterCall setter = MethodUtils.CreatePropertySetter(type, propertyInfo);
					AcessorInfo item = new AcessorInfo(propertyInfo.PropertyType, propertyInfo.Name, inject.identifier, getter, setter);
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		protected AcessorInfo[] ResolveFields(Type type)
		{
			List<AcessorInfo> list = new List<AcessorInfo>();
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(Inject), true);
				if (customAttributes.Length != 0)
				{
					Inject inject = customAttributes[0] as Inject;
					GetterCall getter = MethodUtils.CreateFieldGetter(type, fieldInfo);
					SetterCall setter = MethodUtils.CreateFieldSetter(type, fieldInfo);
					AcessorInfo item = new AcessorInfo(fieldInfo.FieldType, fieldInfo.Name, inject.identifier, getter, setter);
					list.Add(item);
				}
			}
			return list.ToArray();
		}
	}
}
