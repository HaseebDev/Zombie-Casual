using System;
using UnityEngine;

namespace Adic.Examples.Prefabs
{
	public class GameRoot : ContextRoot
	{
		public override void SetupContainers()
		{
			base.AddContainer<InjectionContainer>().RegisterExtension<UnityBindingContainerExtension>().Bind<Transform>().ToPrefab(this.cubePrefab).As("cube").Bind<GameObject>().ToPrefabSingleton(this.planePrefab);
		}

		public override void Init()
		{
		}

		[Tooltip("Prefab for the cube.")]
		public GameObject cubePrefab;

		[Tooltip("Prefab for the plane.")]
		public GameObject planePrefab;
	}
}
