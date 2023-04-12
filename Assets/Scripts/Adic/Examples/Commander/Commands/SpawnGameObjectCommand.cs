using System;
using Adic.Container;
using UnityEngine;

namespace Adic.Examples.Commander.Commands
{
	public class SpawnGameObjectCommand : Command
	{
		public override void Execute(params object[] parameters)
		{
			Transform transform = this.container.Resolve<Transform>();
			base.dispatcher.Dispatch<RotateGameObjectCommand>(new object[]
			{
				transform
			}).Tag("Rotator");
		}

		[Inject]
		public IInjectionContainer container;
	}
}
