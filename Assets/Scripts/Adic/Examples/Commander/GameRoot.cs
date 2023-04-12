using System;
using Adic.Container;
using Adic.Examples.Commander.Commands;
using UnityEngine;

namespace Adic.Examples.Commander
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			IInjectionContainer injectionContainer = base.AddContainer<InjectionContainer>();
			injectionContainer.RegisterExtension<CommanderContainerExtension>().RegisterExtension<EventCallerContainerExtension>().RegisterExtension<UnityBindingContainerExtension>().RegisterCommands("Adic.Examples.Commander.Commands").Bind<Transform>().ToPrefab(this.prismPrefab);
			this.dispatcher = injectionContainer.GetCommandDispatcher();
		}

		public override void Init()
		{
			this.dispatcher.Dispatch<SpawnGameObjectCommand>(Array.Empty<object>());
			base.Invoke("StopRotation", 1f);
		}

		private void StopRotation()
		{
			this.dispatcher.ReleaseAll("Rotator");
		}

		public const string ROTATOR_COMMAND_TAG = "Rotator";

		[Tooltip("Prefab for the prism.")]
		public GameObject prismPrefab;

		protected ICommandDispatcher dispatcher;
	}
}
