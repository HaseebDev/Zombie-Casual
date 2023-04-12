using System;
using System.Collections.Generic;
using Adic.Binding;
using Adic.Cache;
using Adic.Exceptions;

namespace Adic.Injection
{
	public class Injector : IInjector
	{
		public event TypeResolutionHandler beforeResolve;

		public event TypeResolutionHandler afterResolve;

		public event BindingEvaluationHandler bindingEvaluation;

		public event BindingResolutionHandler bindingResolution;

		public event InstanceInjectionHandler beforeInject;

		public event InstanceInjectionHandler afterInject;

		public IReflectionCache cache { get; protected set; }

		public IBinder binder { get; protected set; }

		public ResolutionMode resolutionMode { get; set; }

		public Injector(IReflectionCache cache, IBinder binder, ResolutionMode resolutionMode)
		{
			this.cache = cache;
			this.binder = binder;
			this.resolutionMode = resolutionMode;
			this.binder.beforeAddBinding += this.OnBeforeAddBinding;
		}

		public T Resolve<T>()
		{
			return (T)((object)this.Resolve(typeof(T), InjectionMember.None, null, null, null, false));
		}

		public T Resolve<T>(object identifier)
		{
			return (T)((object)this.Resolve(typeof(T), InjectionMember.None, null, null, identifier, false));
		}

		public object Resolve(Type type)
		{
			return this.Resolve(type, InjectionMember.None, null, null, null, false);
		}

		public object Resolve(object identifier)
		{
			object obj = this.Resolve(null, InjectionMember.None, null, null, identifier, false);
			if (obj != null && obj.GetType().IsArray)
			{
				object[] array = (object[])obj;
				if (array.Length == 0)
				{
					return null;
				}
				return array[0];
			}
			else
			{
				if (obj != null)
				{
					return obj;
				}
				return null;
			}
		}

		public object Resolve(Type type, object identifier)
		{
			return this.Resolve(type, InjectionMember.None, null, null, identifier, false);
		}

		public T[] ResolveAll<T>()
		{
			return this.ResolveAll<T>(null);
		}

		public T[] ResolveAll<T>(object identifier)
		{
			object obj = this.Resolve(typeof(T[]), identifier);
			if (obj == null)
			{
				return null;
			}
			if (!obj.GetType().IsArray)
			{
				Array array = Array.CreateInstance(obj.GetType(), 1);
				array.SetValue(obj, 0);
				return (T[])array;
			}
			return (T[])obj;
		}

		public object[] ResolveAll(Type type)
		{
			return this.ResolveAll(type, null);
		}

		public object[] ResolveAll(object identifier)
		{
			return this.ResolveAll(null, identifier);
		}

		public object[] ResolveAll(Type type, object identifier)
		{
			object obj = this.Resolve(type, identifier);
			if (obj == null)
			{
				return null;
			}
			if (!obj.GetType().IsArray)
			{
				Array array = Array.CreateInstance(obj.GetType(), 1);
				array.SetValue(obj, 0);
				return (object[])array;
			}
			return (object[])obj;
		}

		protected object Resolve(Type type, InjectionMember member, string memberName, object parentInstance, object identifier, bool alwaysResolve)
		{
			object result = null;
			if (this.beforeResolve != null)
			{
				Delegate[] invocationList = this.beforeResolve.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					if (!((TypeResolutionHandler)invocationList[i])(this, type, member, parentInstance, identifier, ref result))
					{
						return result;
					}
				}
			}
			bool flag = type == null;
			Type type2;
			IList<BindingInfo> bindingsFor;
			if (flag)
			{
				type2 = typeof(object);
				bindingsFor = this.binder.GetBindingsFor(identifier);
			}
			else
			{
				if (type.IsArray)
				{
					type2 = type.GetElementType();
				}
				else
				{
					type2 = type;
				}
				bindingsFor = this.binder.GetBindingsFor(type2);
			}
			IList<object> list = new List<object>();
			if (bindingsFor == null)
			{
				if (!alwaysResolve && this.resolutionMode != ResolutionMode.ALWAYS_RESOLVE)
				{
					return null;
				}
				if (!type2.IsInterface || !type.IsArray)
				{
					list.Add(this.Instantiate(type2));
				}
			}
			else
			{
				for (int j = 0; j < bindingsFor.Count; j++)
				{
					BindingInfo binding = bindingsFor[j];
					object obj = this.ResolveBinding(binding, type, member, memberName, parentInstance, identifier);
					if (obj != null)
					{
						list.Add(obj);
					}
				}
			}
			if ((flag && list.Count == 1) || (!flag && !type.IsArray && list.Count == 1))
			{
				result = list[0];
			}
			else if ((flag && list.Count > 1) || (!flag && type.IsArray))
			{
				Array array = Array.CreateInstance(type2, list.Count);
				for (int k = 0; k < list.Count; k++)
				{
					array.SetValue(list[k], k);
				}
				result = array;
			}
			if (this.afterResolve != null)
			{
				Delegate[] invocationList2 = this.afterResolve.GetInvocationList();
				for (int l = 0; l < invocationList2.Length; l++)
				{
					if (!((TypeResolutionHandler)invocationList2[l])(this, type, member, parentInstance, identifier, ref result))
					{
						return result;
					}
				}
			}
			return result;
		}

		public T Inject<T>(T instance) where T : class
		{
			ReflectedClass @class = this.cache.GetClass(instance.GetType());
			return (T)((object)this.Inject(instance, @class));
		}

		public object Inject(object instance)
		{
			ReflectedClass @class = this.cache.GetClass(instance.GetType());
			return this.Inject(instance, @class);
		}

		protected object Inject(object instance, ReflectedClass reflectedClass)
		{
			if (this.beforeInject != null)
			{
				this.beforeInject(this, ref instance, reflectedClass);
			}
			if (reflectedClass.fields.Length != 0)
			{
				this.InjectFields(instance, reflectedClass.fields);
			}
			if (reflectedClass.properties.Length != 0)
			{
				this.InjectProperties(instance, reflectedClass.properties);
			}
			if (reflectedClass.methods.Length != 0)
			{
				this.InjectMethods(instance, reflectedClass.methods);
			}
			if (this.afterInject != null)
			{
				this.afterInject(this, ref instance, reflectedClass);
			}
			return instance;
		}

		protected void InjectFields(object instance, AcessorInfo[] fields)
		{
			foreach (AcessorInfo acessorInfo in fields)
			{
				object obj = acessorInfo.getter(instance);
				if (obj == null || obj.Equals(null))
				{
					try
					{
						object value = this.Resolve(acessorInfo.type, InjectionMember.Field, acessorInfo.name, instance, acessorInfo.identifier, false);
						acessorInfo.setter(instance, value);
					}
					catch (Exception ex)
					{
						throw new InjectorException(string.Format("Unable to inject on field {0} at object {1}.\nCaused by: {2}", acessorInfo.name, instance.GetType(), ex.Message), ex);
					}
				}
			}
		}

		protected void InjectProperties(object instance, AcessorInfo[] properties)
		{
			foreach (AcessorInfo acessorInfo in properties)
			{
				object obj = (acessorInfo.getter == null) ? null : acessorInfo.getter(instance);
				if (obj == null || obj.Equals(null))
				{
					try
					{
						object value = this.Resolve(acessorInfo.type, InjectionMember.Property, acessorInfo.name, instance, acessorInfo.identifier, false);
						acessorInfo.setter(instance, value);
					}
					catch (Exception ex)
					{
						throw new InjectorException(string.Format("Unable to inject on property {0} at object {1}.\nCaused by: {2}", acessorInfo.name, instance.GetType(), ex.Message), ex);
					}
				}
			}
		}

		protected void InjectMethods(object instance, MethodInfo[] methods)
		{
			foreach (MethodInfo methodInfo in methods)
			{
				try
				{
					if (methodInfo.parameters.Length == 0)
					{
						methodInfo.method(instance);
					}
					else
					{
						object[] parametersFromInfo = this.GetParametersFromInfo(instance, methodInfo.parameters, InjectionMember.Method);
						methodInfo.paramsMethod(instance, parametersFromInfo);
					}
				}
				catch (Exception ex)
				{
					throw new InjectorException(string.Format("Unable to inject on method {0} at object {1}.\nCaused by: {2}", methodInfo.name, instance.GetType(), ex.Message), ex);
				}
			}
		}

		protected object ResolveBinding(BindingInfo binding, Type type, InjectionMember member, string memberName, object parentInstance, object identifier)
		{
			if (binding.condition != null)
			{
				InjectionContext context = new InjectionContext
				{
					member = member,
					memberType = type,
					memberName = memberName,
					identifier = identifier,
					parentType = ((parentInstance != null) ? parentInstance.GetType() : null),
					parentInstance = parentInstance,
					injectType = binding.type
				};
				if (!binding.condition(context))
				{
					return null;
				}
			}
			bool flag = identifier != null;
			bool flag2 = binding.identifier != null;
			if ((!flag && flag2) || (flag && !flag2) || (flag && flag2 && !binding.identifier.Equals(identifier)))
			{
				return null;
			}
			object obj = null;
			if (this.bindingEvaluation != null)
			{
				Delegate[] invocationList = this.bindingEvaluation.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					obj = ((BindingEvaluationHandler)invocationList[i])(this, ref binding);
				}
			}
			if (obj == null)
			{
				if (binding.instanceType == BindingInstance.Transient)
				{
					obj = this.Instantiate(binding.value as Type);
				}
				else if (binding.instanceType == BindingInstance.Factory)
				{
					InjectionContext context2 = new InjectionContext
					{
						member = member,
						memberType = type,
						memberName = memberName,
						identifier = identifier,
						parentType = ((parentInstance != null) ? parentInstance.GetType() : null),
						parentInstance = parentInstance,
						injectType = binding.type
					};
					obj = (binding.value as IFactory).Create(context2);
				}
				else
				{
					if (binding.value is Type)
					{
						binding.value = this.Instantiate(binding.value as Type);
					}
					obj = binding.value;
				}
			}
			if (this.bindingResolution != null)
			{
				this.bindingResolution(this, ref binding, ref obj);
			}
			return obj;
		}

		protected object Instantiate(Type type)
		{
			if (type.IsInterface)
			{
				throw new InjectorException(string.Format("Interface \"{0}\" cannot be instantiated.", type.ToString()));
			}
			ReflectedClass @class = this.cache.GetClass(type);
			if (@class.constructor == null && @class.paramsConstructor == null)
			{
				throw new InjectorException(string.Format("There are no constructors on the type \"{0}\".", type.ToString()));
			}
			object instance;
			if (@class.constructorParameters.Length == 0)
			{
				instance = @class.constructor();
			}
			else
			{
				object[] parametersFromInfo = this.GetParametersFromInfo(null, @class.constructorParameters, InjectionMember.Constructor);
				instance = @class.paramsConstructor(parametersFromInfo);
			}
			return this.Inject(instance, @class);
		}

		protected object[] GetParametersFromInfo(object instance, ParameterInfo[] parametersInfo, InjectionMember injectionMember)
		{
			object[] array = new object[parametersInfo.Length];
			for (int i = 0; i < array.Length; i++)
			{
				ParameterInfo parameterInfo = parametersInfo[i];
				array[i] = this.Resolve(parameterInfo.type, injectionMember, parameterInfo.name, instance, parameterInfo.identifier, false);
			}
			return array;
		}

		protected void OnBeforeAddBinding(IBinder source, ref BindingInfo binding)
		{
			if (binding.instanceType == BindingInstance.Singleton || binding.instanceType == BindingInstance.Factory)
			{
				IList<BindingInfo> bindings = this.binder.GetBindings();
				bool flag = binding.value is Type;
				BindingInfo bindingInfo = null;
				for (int i = 0; i < bindings.Count; i++)
				{
					BindingInfo bindingInfo2 = bindings[i];
					bool flag2 = bindingInfo2.instanceType == BindingInstance.Singleton || bindingInfo2.instanceType == BindingInstance.Factory;
					bool flag3 = flag2 && flag && bindingInfo2.value != null && bindingInfo2.value.GetType().Equals(binding.value);
					bool flag4 = flag2 && !flag && bindingInfo2.value == binding.value;
					if (flag3 || flag4)
					{
						bindingInfo = bindingInfo2;
						break;
					}
				}
				if (bindingInfo != null)
				{
					binding.value = bindingInfo.value;
					return;
				}
				if (flag)
				{
					object value = this.Resolve(binding.value as Type, InjectionMember.None, null, null, null, true);
					binding.value = value;
					return;
				}
				this.Inject(binding.value);
			}
		}
	}
}
