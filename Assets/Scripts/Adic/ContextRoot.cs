using System;
using System.Collections.Generic;
using Adic.Container;
using Adic.Extensions.ContextRoots;
using UnityEngine;

namespace Adic
{
	public abstract class ContextRoot : MonoBehaviour, IContextRoot
	{
		public static List<ContextRoot.InjectionContainerData> containersData { get; set; }

		public IInjectionContainer[] containers
		{
			get
			{
				IInjectionContainer[] array = new IInjectionContainer[ContextRoot.containersData.Count];
				for (int i = 0; i < ContextRoot.containersData.Count; i++)
				{
					array[i] = ContextRoot.containersData[i].container;
				}
				return array;
			}
		}

		protected void Awake()
		{
			if (ContextRoot.containersData == null)
			{
				ContextRoot.containersData = new List<ContextRoot.InjectionContainerData>(1);
			}
			this.SetupContainers();
			this.InitContainers();
		}

		protected void Start()
		{
			base.gameObject.AddComponent<SceneInjector>();
			this.Init();
		}

		protected void OnDestroy()
		{
			for (int i = 0; i < ContextRoot.containersData.Count; i++)
			{
				ContextRoot.InjectionContainerData injectionContainerData = ContextRoot.containersData[i];
				if (injectionContainerData.destroyOnLoad)
				{
					injectionContainerData.container.Dispose();
					ContextRoot.containersData.Remove(injectionContainerData);
					i--;
				}
			}
		}

		public IInjectionContainer AddContainer<T>() where T : IInjectionContainer, new()
		{
			T t = Activator.CreateInstance<T>();
			return this.AddContainer(t, true);
		}

		public IInjectionContainer AddContainer<T>(object identifier) where T : IInjectionContainer
		{
			return this.AddContainer<T>(new Type[]
			{
				typeof(object)
			}, new object[]
			{
				identifier
			});
		}

		public IInjectionContainer AddContainer<T>(ResolutionMode resolutionMode) where T : IInjectionContainer
		{
			return this.AddContainer<T>(new Type[]
			{
				typeof(ResolutionMode)
			}, new object[]
			{
				resolutionMode
			});
		}

		public IInjectionContainer AddContainer<T>(object identifier, ResolutionMode resolutionMode) where T : IInjectionContainer
		{
			return this.AddContainer<T>(new Type[]
			{
				typeof(object),
				typeof(ResolutionMode)
			}, new object[]
			{
				identifier,
				resolutionMode
			});
		}

		private IInjectionContainer AddContainer<T>(Type[] parameterTypes, object[] parameterValues) where T : IInjectionContainer
		{
			IInjectionContainer container = (IInjectionContainer)typeof(T).GetConstructor(parameterTypes).Invoke(parameterValues);
			return this.AddContainer(container, true);
		}

		public IInjectionContainer AddContainer(IInjectionContainer container)
		{
			return this.AddContainer(container, true);
		}

		public IInjectionContainer AddContainer(IInjectionContainer container, bool destroyOnLoad)
		{
			ContextRoot.containersData.Add(new ContextRoot.InjectionContainerData(container, destroyOnLoad));
			return container;
		}

		public abstract void SetupContainers();

		public abstract void Init();

		private void InitContainers()
		{
			for (int i = 0; i < ContextRoot.containersData.Count; i++)
			{
				ContextRoot.containersData[i].container.Init();
			}
		}

		[Tooltip("Type of injection on MonoBehaviours.")]
		[HideInInspector]
		public ContextRoot.MonoBehaviourInjectionType injectionType;

		[Tooltip("Name of the base behaviour type to perform scene injection.")]
		[HideInInspector]
		public string baseBehaviourTypeName;

		public class InjectionContainerData
		{
			public InjectionContainerData(IInjectionContainer container, bool destroyOnLoad)
			{
				this.container = container;
				this.destroyOnLoad = destroyOnLoad;
			}

			public IInjectionContainer container;

			public bool destroyOnLoad;
		}

		[Serializable]
		public enum MonoBehaviourInjectionType
		{
			Manual,
			Children,
			BaseType
		}
	}
}
