using System;
using System.Collections.Generic;
using Adic.Exceptions;
using Adic.Injection;

namespace Adic.Binding
{
	public class Binder : IBinder, IBindingCreator
	{
		public event BindingAddedHandler beforeAddBinding;

		public event BindingAddedHandler afterAddBinding;

		public event BindingRemovedHandler beforeRemoveBinding;

		public event BindingRemovedHandler afterRemoveBinding;

		public IBindingFactory Bind<T>()
		{
			return this.Bind(typeof(T));
		}

		public IBindingFactory Bind(Type type)
		{
			return this.BindingFactoryProvider(type);
		}

		public void AddBinding(BindingInfo binding)
		{
			if (binding == null)
			{
				throw new BinderException("There is no binding to be bound.");
			}
			if (binding.value is Type && (binding.value as Type).IsInterface)
			{
				throw new BinderException("It's not possible to bind a key to an interface.");
			}
			Type valueType = binding.GetValueType();
			if (!valueType.Equals(typeof(InjectionContainer)))
			{
				bool flag = binding.instanceType == BindingInstance.Singleton;
				bool flag2 = !binding.type.Equals(valueType);
				bool flag3 = this.typeBindings.ContainsKey(valueType);
				if (flag && flag2 && !flag3)
				{
					this.AddBindingToDictionary(new BindingInfo(valueType, binding.value, BindingInstance.Singleton, binding));
				}
			}
			this.AddBindingToDictionary(binding);
		}

		protected void AddBindingToDictionary(BindingInfo binding)
		{
			if (this.beforeAddBinding != null)
			{
				this.beforeAddBinding(this, ref binding);
			}
			if (this.typeBindings.ContainsKey(binding.type))
			{
				this.typeBindings[binding.type].Add(binding);
			}
			else
			{
				List<BindingInfo> list = new List<BindingInfo>(1);
				list.Add(binding);
				this.typeBindings.Add(binding.type, list);
			}
			if (this.afterAddBinding != null)
			{
				this.afterAddBinding(this, ref binding);
			}
		}

		public IList<BindingInfo> GetBindings()
		{
			List<BindingInfo> list = new List<BindingInfo>();
			foreach (KeyValuePair<Type, IList<BindingInfo>> keyValuePair in this.typeBindings)
			{
				list.AddRange(keyValuePair.Value);
			}
			return list;
		}

		public IList<BindingInfo> GetBindingsFor<T>()
		{
			return this.GetBindingsFor(typeof(T));
		}

		public IList<BindingInfo> GetBindingsFor(Type type)
		{
			if (this.typeBindings.ContainsKey(type))
			{
				return this.typeBindings[type];
			}
			return null;
		}

		public IList<BindingInfo> GetBindingsFor(object identifier)
		{
			List<BindingInfo> list = new List<BindingInfo>();
			foreach (KeyValuePair<Type, IList<BindingInfo>> keyValuePair in this.typeBindings)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					BindingInfo bindingInfo = keyValuePair.Value[i];
					if (bindingInfo.identifier != null && bindingInfo.identifier.Equals(identifier))
					{
						list.Add(bindingInfo);
					}
				}
			}
			return list;
		}

		public IList<BindingInfo> GetBindingsTo<T>()
		{
			return this.GetBindingsTo(typeof(T));
		}

		public IList<BindingInfo> GetBindingsTo(Type type)
		{
			IList<BindingInfo> list = new List<BindingInfo>();
			foreach (KeyValuePair<Type, IList<BindingInfo>> keyValuePair in this.typeBindings)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					BindingInfo bindingInfo = keyValuePair.Value[i];
					if (bindingInfo.GetValueType().Equals(type))
					{
						list.Add(bindingInfo);
					}
				}
			}
			if (list.Count != 0)
			{
				return list;
			}
			return null;
		}

		public bool ContainsBindingFor<T>()
		{
			return this.ContainsBindingFor(typeof(T));
		}

		public bool ContainsBindingFor(Type type)
		{
			return this.typeBindings.ContainsKey(type);
		}

		public bool ContainsBindingFor(object identifier)
		{
			bool flag = false;
			foreach (KeyValuePair<Type, IList<BindingInfo>> keyValuePair in this.typeBindings)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					object identifier2 = keyValuePair.Value[i].identifier;
					if (identifier2 != null && identifier2.Equals(identifier))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		public void Unbind<T>()
		{
			this.Unbind(typeof(T));
		}

		public void Unbind(Type type)
		{
			if (!this.ContainsBindingFor(type))
			{
				return;
			}
			IList<BindingInfo> list = new List<BindingInfo>();
			IList<Type> list2 = new List<Type>();
			foreach (KeyValuePair<Type, IList<BindingInfo>> keyValuePair in this.typeBindings)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					BindingInfo bindingInfo = keyValuePair.Value[i];
					if (bindingInfo.type.Equals(type) || bindingInfo.GetValueType().Equals(type))
					{
						list.Add(bindingInfo);
						list2.Add(keyValuePair.Key);
					}
				}
			}
			if (this.beforeRemoveBinding != null)
			{
				this.beforeRemoveBinding(this, type, list);
			}
			foreach (Type key in list2)
			{
				this.typeBindings.Remove(key);
			}
			if (this.afterRemoveBinding != null)
			{
				this.afterRemoveBinding(this, type, list);
			}
		}

		public void Unbind(object identifier)
		{
			this.Unbind((BindingInfo binding) => binding.identifier != null && binding.identifier.Equals(identifier));
		}

		public void UnbindInstance(object instance)
		{
			this.Unbind((BindingInfo binding) => binding.value == instance || (binding.condition != null && binding.condition(new InjectionContext
			{
				parentInstance = instance
			})));
		}

		public void UnbindByTag(string tag)
		{
			if (!string.IsNullOrEmpty(tag))
			{
				Predicate<string> _003C_003E9__1;
				this.Unbind(delegate(BindingInfo binding)
				{
					if (binding.tags != null)
					{
						string[] tags = binding.tags;
						Predicate<string> match;
						match = (_003C_003E9__1 = ((string element) => element != null && element.Equals(tag)));
						return Array.Exists<string>(tags, match);
					}
					return false;
				});
			}
		}

		protected void Unbind(Binder.CanRemoveBindingHandler canRemoveBinding)
		{
			List<BindingInfo> list = new List<BindingInfo>();
			foreach (KeyValuePair<Type, IList<BindingInfo>> keyValuePair in this.typeBindings)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					BindingInfo bindingInfo = keyValuePair.Value[i];
					list.Clear();
					if (canRemoveBinding(bindingInfo))
					{
						list.Add(bindingInfo);
						if (this.beforeRemoveBinding != null)
						{
							this.beforeRemoveBinding(this, bindingInfo.type, list);
						}
						keyValuePair.Value.RemoveAt(i--);
						if (this.afterRemoveBinding != null)
						{
							this.afterRemoveBinding(this, bindingInfo.type, list);
						}
					}
				}
			}
		}

		protected virtual IBindingFactory BindingFactoryProvider(Type type)
		{
			return new BindingFactory(type, this);
		}

		protected Dictionary<Type, IList<BindingInfo>> typeBindings = new Dictionary<Type, IList<BindingInfo>>();

		public delegate bool CanRemoveBindingHandler(BindingInfo binding);
	}
}
