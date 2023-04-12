using System;
using System.Collections.Generic;
using Adic.Binding;
using Adic.Container;
using Adic.Injection;
using UnityEngine;

namespace Adic
{
	public class EventCallerContainerExtension : IContainerExtension
	{
		public List<IDisposable> disposable { get; private set; }

		public List<IUpdatable> updateable { get; private set; }

		public List<ILateUpdatable> lateUpdateable { get; private set; }

		public List<IFixedUpdatable> fixedUpdateable { get; private set; }

		public List<IFocusable> focusable { get; private set; }

		public List<IPausable> pausable { get; private set; }

		public List<IQuitable> quitable { get; private set; }

		public EventCallerBehaviour behaviour { get; private set; }

		public EventCallerContainerExtension()
		{
			this.disposable = new List<IDisposable>();
			this.updateable = new List<IUpdatable>();
			this.lateUpdateable = new List<ILateUpdatable>();
			this.fixedUpdateable = new List<IFixedUpdatable>();
			this.focusable = new List<IFocusable>();
			this.pausable = new List<IPausable>();
			this.quitable = new List<IQuitable>();
		}

		public void Init(IInjectionContainer container)
		{
		}

		public void OnRegister(IInjectionContainer container)
		{
			this.CreateBehaviour(container.identifier);
			if (container.ContainsBindingFor<ICommandDispatcher>())
			{
				ICommandDispatcher instance = container.Resolve<ICommandDispatcher>();
				this.BindUnityExtension<IDisposable>(this.disposable, instance);
			}
			container.afterAddBinding += this.OnAfterAddBinding;
			container.bindingResolution += this.OnBindingResolution;
		}

		public void OnUnregister(IInjectionContainer container)
		{
			container.afterAddBinding -= this.OnAfterAddBinding;
			container.bindingResolution -= this.OnBindingResolution;
			if (this.behaviour != null && this.behaviour.gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(this.behaviour.gameObject);
			}
			this.behaviour = null;
			this.disposable.Clear();
			this.updateable.Clear();
			this.lateUpdateable.Clear();
			this.fixedUpdateable.Clear();
			this.focusable.Clear();
			this.pausable.Clear();
			this.quitable.Clear();
		}

		private void CreateBehaviour(object containerID)
		{
			if (this.behaviour == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = string.Format("EventCaller ({0})", containerID);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				this.behaviour = gameObject.AddComponent<EventCallerBehaviour>();
				this.behaviour.extension = this;
			}
		}

		protected void OnAfterAddBinding(IBinder source, ref BindingInfo binding)
		{
			if (binding.instanceType == BindingInstance.Singleton)
			{
				if (binding.value is ICommand)
				{
					return;
				}
				this.BindUnityExtension<IDisposable>(this.disposable, binding.value);
				this.BindUnityExtension<IUpdatable>(this.updateable, binding.value);
				this.BindUnityExtension<ILateUpdatable>(this.lateUpdateable, binding.value);
				this.BindUnityExtension<IFixedUpdatable>(this.fixedUpdateable, binding.value);
				this.BindUnityExtension<IFocusable>(this.focusable, binding.value);
				this.BindUnityExtension<IPausable>(this.pausable, binding.value);
				this.BindUnityExtension<IQuitable>(this.quitable, binding.value);
			}
		}

		protected void OnBindingResolution(IInjector source, ref BindingInfo binding, ref object instance)
		{
			if (binding.instanceType == BindingInstance.Singleton || instance is ICommand)
			{
				return;
			}
			this.BindUnityExtension<IDisposable>(this.disposable, instance);
			this.BindUnityExtension<IUpdatable>(this.updateable, instance);
			this.BindUnityExtension<ILateUpdatable>(this.lateUpdateable, instance);
			this.BindUnityExtension<IFixedUpdatable>(this.fixedUpdateable, instance);
			this.BindUnityExtension<IFocusable>(this.focusable, instance);
			this.BindUnityExtension<IPausable>(this.pausable, instance);
			this.BindUnityExtension<IQuitable>(this.quitable, instance);
		}

		protected void BindUnityExtension<T>(List<T> instances, object instance)
		{
			if (instance is T && !instances.Contains((T)((object)instance)))
			{
				instances.Add((T)((object)instance));
			}
		}
	}
}
