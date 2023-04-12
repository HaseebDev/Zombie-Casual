using System;
using UnityEngine;

namespace Adic.Examples.BindingGameObjects
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			base.AddContainer<InjectionContainer>().RegisterExtension<UnityBindingContainerExtension>().Bind<Transform>().ToGameObject("Cube").Bind<GameObjectRotator>().ToGameObject();
		}

		public override void Init()
		{
		}
	}
}
