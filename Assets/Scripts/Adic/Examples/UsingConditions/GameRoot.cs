using System;
using UnityEngine;

namespace Adic.Examples.UsingConditions
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			base.AddContainer<InjectionContainer>().RegisterExtension<UnityBindingContainerExtension>().Bind<Transform>().ToGameObject("LeftCube").As("LeftCube").Bind<Transform>().ToGameObject(this.rightCube).AsObjectName().Bind<GameObjectRotator>().ToGameObject("Rotator");
		}

		public override void Init()
		{
		}

		[Tooltip("On scene right cube.")]
		public GameObject rightCube;
	}
}
