using System;
using Adic.Util;
using UnityEngine;

namespace Adic.Extensions.ContextRoots
{
	[RequireComponent(typeof(ContextRoot))]
	public class SceneInjector : MonoBehaviour
	{
		private void Awake()
		{
			ContextRoot component = base.GetComponent<ContextRoot>();
			Type baseType = (component.baseBehaviourTypeName == "UnityEngine.MonoBehaviour") ? typeof(MonoBehaviour) : TypeUtils.GetType(component.baseBehaviourTypeName);
			ContextRoot.MonoBehaviourInjectionType injectionType = component.injectionType;
			if (injectionType == ContextRoot.MonoBehaviourInjectionType.Children)
			{
				this.InjectOnChildren(baseType);
				return;
			}
			if (injectionType != ContextRoot.MonoBehaviourInjectionType.BaseType)
			{
				return;
			}
			this.InjectFromBaseType(baseType);
		}

		public void InjectOnChildren(Type baseType)
		{
			Type type = base.GetType();
			foreach (Component component in base.GetComponent<Transform>().GetComponentsInChildren(baseType, true))
			{
				Type type2 = component.GetType();
				if (!(type2 == type) && !TypeUtils.IsAssignable(typeof(ContextRoot), type2))
				{
					((MonoBehaviour)component).Inject();
				}
			}
		}

		public void InjectFromBaseType(Type baseType)
		{
			MonoBehaviour[] array = (MonoBehaviour[])Resources.FindObjectsOfTypeAll(baseType);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Inject();
			}
		}
	}
}
