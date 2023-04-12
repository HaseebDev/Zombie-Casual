using System;
using Adic.Binding;
using Adic.Container;
using Adic.Exceptions;
using Adic.Injection;
using Adic.Util;
using UnityEngine;

namespace Adic
{
	public class UnityBindingContainerExtension : IContainerExtension
	{
		public void Init(IInjectionContainer container)
		{
		}

		public void OnRegister(IInjectionContainer container)
		{
			container.beforeAddBinding += this.OnBeforeAddBinding;
			container.bindingEvaluation += this.OnBindingEvaluation;
		}

		public void OnUnregister(IInjectionContainer container)
		{
			container.beforeAddBinding -= this.OnBeforeAddBinding;
			container.bindingEvaluation -= this.OnBindingEvaluation;
		}

		protected void OnBeforeAddBinding(IBinder source, ref BindingInfo binding)
		{
			if (binding.value is Type && TypeUtils.IsAssignable(typeof(MonoBehaviour), binding.value as Type))
			{
				throw new BindingException("A MonoBehaviour cannot be resolved directly.");
			}
		}

		protected object OnBindingEvaluation(IInjector source, ref BindingInfo binding)
		{
			if (!(binding.value is PrefabBinding) || binding.instanceType != BindingInstance.Transient)
			{
				return null;
			}
			PrefabBinding prefabBinding = (PrefabBinding)binding.value;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(prefabBinding.prefab);
			if (prefabBinding.type.Equals(typeof(GameObject)))
			{
				return gameObject;
			}
			Component component = gameObject.GetComponent(prefabBinding.type);
			if (component == null)
			{
				component = gameObject.AddComponent(prefabBinding.type);
			}
			return component;
		}

		protected const string CANNOT_RESOLVE_MONOBEHAVIOUR = "A MonoBehaviour cannot be resolved directly.";
	}
}
