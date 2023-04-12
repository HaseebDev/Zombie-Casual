using System;
using Adic.Binding;
using Adic.Exceptions;
using Adic.Util;
using UnityEngine;

namespace Adic
{
	public static class UnityBindingExtension
	{
		public static UnityBindingConditionFactory ToGameObject(this IBindingFactory bindingFactory)
		{
			return bindingFactory.ToGameObject(bindingFactory.bindingType, string.Empty);
		}

		public static UnityBindingConditionFactory ToGameObject<T>(this IBindingFactory bindingFactory) where T : Component
		{
			return bindingFactory.ToGameObject(typeof(T), string.Empty);
		}

		public static UnityBindingConditionFactory ToGameObject(this IBindingFactory bindingFactory, Type type)
		{
			return bindingFactory.ToGameObject(type, string.Empty);
		}

		public static UnityBindingConditionFactory ToGameObject(this IBindingFactory bindingFactory, string name)
		{
			return bindingFactory.ToGameObject(bindingFactory.bindingType, name);
		}

		public static UnityBindingConditionFactory ToGameObject<T>(this IBindingFactory bindingFactory, string name) where T : Component
		{
			return bindingFactory.ToGameObject(typeof(T), name);
		}

		public static UnityBindingConditionFactory ToGameObject(this IBindingFactory bindingFactory, Type type, string name)
		{
			if (!TypeUtils.IsAssignable(bindingFactory.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			GameObject gameObject;
			if (string.IsNullOrEmpty(name))
			{
				gameObject = new GameObject(type.Name);
			}
			else
			{
				gameObject = GameObject.Find(name);
				if (gameObject == null)
				{
					gameObject = new GameObject(name);
				}
			}
			return UnityBindingExtension.CreateSingletonBinding(bindingFactory, gameObject, type, flag);
		}

		public static UnityBindingConditionFactory ToGameObject(this IBindingFactory bindingFactory, GameObject gameObject)
		{
			return bindingFactory.ToGameObject(bindingFactory.bindingType, gameObject);
		}

		public static UnityBindingConditionFactory ToGameObject<T>(this IBindingFactory bindingFactory, GameObject gameObject)
		{
			return bindingFactory.ToGameObject(typeof(T), gameObject);
		}

		public static UnityBindingConditionFactory ToGameObject(this IBindingFactory bindingFactory, Type type, GameObject gameObject)
		{
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			return UnityBindingExtension.CreateSingletonBinding(bindingFactory, gameObject, type, flag);
		}

		public static UnityBindingConditionFactory ToGameObjectWithTag(this IBindingFactory bindingFactory, string tag)
		{
			return bindingFactory.ToGameObjectWithTag(bindingFactory.bindingType, tag);
		}

		public static UnityBindingConditionFactory ToGameObjectWithTag<T>(this IBindingFactory bindingFactory, string tag) where T : Component
		{
			return bindingFactory.ToGameObjectWithTag(typeof(T), tag);
		}

		public static UnityBindingConditionFactory ToGameObjectWithTag(this IBindingFactory bindingFactory, Type type, string tag)
		{
			if (!TypeUtils.IsAssignable(bindingFactory.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			GameObject gameObject = GameObject.FindWithTag(tag);
			if (gameObject == null)
			{
				throw new BindingException(string.Format("There's no GameObject tagged \"{0}\" to bind the type {1} to.", tag, type.ToString()));
			}
			return UnityBindingExtension.CreateSingletonBinding(bindingFactory, gameObject, type, flag);
		}

		public static IBindingConditionFactory ToGameObjectsWithTag(this IBindingFactory bindingFactory, string tag)
		{
			return bindingFactory.ToGameObjectsWithTag(bindingFactory.bindingType, tag);
		}

		public static IBindingConditionFactory ToGameObjectsWithTag<T>(this IBindingFactory bindingFactory, string tag) where T : Component
		{
			return bindingFactory.ToGameObjectsWithTag(typeof(T), tag);
		}

		public static IBindingConditionFactory ToGameObjectsWithTag(this IBindingFactory bindingFactory, Type type, string tag)
		{
			if (!TypeUtils.IsAssignable(bindingFactory.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
			IBindingConditionFactory[] array2 = new IBindingConditionFactory[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = UnityBindingExtension.CreateSingletonBinding(bindingFactory, array[i], type, flag);
			}
			return new MultipleBindingConditionFactory(array2, bindingFactory.binder);
		}

		public static UnityBindingConditionFactory ToPrefab(this IBindingFactory bindingFactory, GameObject prefab)
		{
			return bindingFactory.ToPrefab(bindingFactory.bindingType, prefab);
		}

		public static UnityBindingConditionFactory ToPrefab<T>(this IBindingFactory bindingFactory, GameObject prefab) where T : Component
		{
			return bindingFactory.ToPrefab(typeof(T), prefab);
		}

		public static UnityBindingConditionFactory ToPrefab(this IBindingFactory bindingFactory, Type type, GameObject prefab)
		{
			if (!TypeUtils.IsAssignable(bindingFactory.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			return new UnityBindingConditionFactory(bindingFactory.AddBinding(new PrefabBinding(prefab, type), BindingInstance.Transient), prefab.name);
		}

		public static UnityBindingConditionFactory ToPrefabSingleton(this IBindingFactory bindingFactory, GameObject prefab)
		{
			return bindingFactory.ToPrefabSingleton(bindingFactory.bindingType, prefab);
		}

		public static UnityBindingConditionFactory ToPrefabSingleton<T>(this IBindingFactory bindingFactory, GameObject prefab) where T : Component
		{
			return bindingFactory.ToPrefabSingleton(typeof(T), prefab);
		}

		public static UnityBindingConditionFactory ToPrefabSingleton(this IBindingFactory bindingFactory, Type type, GameObject prefab)
		{
			if (!TypeUtils.IsAssignable(bindingFactory.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			return UnityBindingExtension.CreateSingletonBinding(bindingFactory, gameObject, type, flag);
		}

		[Obsolete("Loading from Resources is not recommended by Unity. Please use ToPrefab(GameObject) instead.")]
		public static UnityBindingConditionFactory ToPrefab(this IBindingFactory bindingFactory, string name)
		{
			return bindingFactory.ToPrefab(bindingFactory.bindingType, name);
		}

		[Obsolete("Loading from Resources is not recommended by Unity. Please use ToPrefab<T>(GameObject) instead.")]
		public static UnityBindingConditionFactory ToPrefab<T>(this IBindingFactory bindingFactory, string name) where T : Component
		{
			return bindingFactory.ToPrefab(typeof(T), name);
		}

		[Obsolete("Loading from Resources is not recommended by Unity. Please use ToPrefab(Type, GameObject) instead.")]
		public static UnityBindingConditionFactory ToPrefab(this IBindingFactory bindingFactory, Type type, string name)
		{
			if (!TypeUtils.IsAssignable(bindingFactory.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			UnityEngine.Object @object = Resources.Load(name);
			if (@object == null)
			{
				throw new BindingException(string.Format("There's no prefab named \"{0}\" to bind the type {1} to.", name, type.ToString()));
			}
			PrefabBinding value = new PrefabBinding(@object, type);
			return new UnityBindingConditionFactory(bindingFactory.AddBinding(value, BindingInstance.Transient), @object.name);
		}

		[Obsolete("Loading from Resources is not recommended by Unity. Please use ToPrefabSingleton(GameObject) instead.")]
		public static UnityBindingConditionFactory ToPrefabSingleton(this IBindingFactory bindingFactory, string name)
		{
			return bindingFactory.ToPrefabSingleton(bindingFactory.bindingType, name);
		}

		[Obsolete("Loading from Resources is not recommended by Unity. Please use ToPrefabSingleton(GameObject) instead.")]
		public static UnityBindingConditionFactory ToPrefabSingleton<T>(this IBindingFactory bindingFactory, string name) where T : Component
		{
			return bindingFactory.ToPrefabSingleton(typeof(T), name);
		}

		[Obsolete("Loading from Resources is not recommended by Unity. Please use ToPrefabSingleton(GameObject) instead.")]
		public static UnityBindingConditionFactory ToPrefabSingleton(this IBindingFactory bindingFactory, Type type, string name)
		{
			if (!TypeUtils.IsAssignable(bindingFactory.bindingType, type))
			{
				throw new BindingException("The related type is not assignable from the source type.");
			}
			bool flag = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_GAME_OBJECT, type);
			bool flag2 = TypeUtils.IsAssignable(UnityBindingExtension.TYPE_COMPONENT, type);
			if (!flag && !flag2)
			{
				throw new BindingException("The type must be derived from UnityEngine.Component.");
			}
			UnityEngine.Object @object = Resources.Load(name);
			if (@object == null)
			{
				throw new BindingException(string.Format("There's no prefab named \"{0}\" to bind the type {1} to.", name, type.ToString()));
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(@object);
			return UnityBindingExtension.CreateSingletonBinding(bindingFactory, gameObject, type, flag);
		}

		[Obsolete("Loading from Resources is not recommended by Unity. Please use other binding methods.")]
		public static UnityBindingConditionFactory ToResource(this IBindingFactory bindingFactory, string name)
		{
			if (!TypeUtils.IsAssignable(typeof(UnityEngine.Object), bindingFactory.bindingType))
			{
				throw new BindingException("The type must be derived from UnityEngine.Object.");
			}
			UnityEngine.Object @object = Resources.Load(name);
			if (@object == null)
			{
				throw new BindingException("There's no resource to bind the type to.");
			}
			return new UnityBindingConditionFactory(bindingFactory.AddBinding(@object, BindingInstance.Singleton), @object.name);
		}

		private static UnityBindingConditionFactory CreateSingletonBinding(IBindingFactory bindingFactory, GameObject gameObject, Type type, bool typeIsGameObject)
		{
			if (gameObject == null)
			{
				throw new BindingException("There's no GameObject to bind the type to.");
			}
			if (typeIsGameObject)
			{
				return new UnityBindingConditionFactory(bindingFactory.AddBinding(gameObject, BindingInstance.Singleton), gameObject.name);
			}
			Component component = gameObject.GetComponent(type);
			if (component == null)
			{
				component = gameObject.AddComponent(type);
			}
			return new UnityBindingConditionFactory(bindingFactory.AddBinding(component, BindingInstance.Singleton), gameObject.name);
		}

		private const string TYPE_NOT_OBJECT = "The type must be derived from UnityEngine.Object.";

		private const string TYPE_NOT_COMPONENT = "The type must be derived from UnityEngine.Component.";

		private const string GAMEOBJECT_IS_NULL = "There's no GameObject to bind the type to.";

		private const string GAMEOBJECT_TAG_TYPE_IS_NULL = "There's no GameObject tagged \"{0}\" to bind the type {1} to.";

		private const string PREFAB_NAME_TYPE_IS_NULL = "There's no prefab named \"{0}\" to bind the type {1} to.";

		private const string RESOURCE_IS_NULL = "There's no resource to bind the type to.";

		private static readonly Type TYPE_GAME_OBJECT = typeof(GameObject);

		private static readonly Type TYPE_COMPONENT = typeof(Component);
	}
}
