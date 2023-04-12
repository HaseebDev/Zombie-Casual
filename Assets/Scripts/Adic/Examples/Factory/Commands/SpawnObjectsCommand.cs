using System;
using Adic.Container;
using UnityEngine;

namespace Adic.Examples.Factory.Commands
{
	public class SpawnObjectsCommand : Command
	{
		public override void Execute(params object[] parameters)
		{
			for (int i = 0; i < 36; i++)
			{
				this.container.Resolve<GameObject>().name = string.Format("Cube {0:00}", i);
			}
		}

		[Inject]
		public IInjectionContainer container;
	}
}
