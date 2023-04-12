using System;
using UnityEngine;

namespace Adic.Examples.Commander.Commands
{
	public class RotateGameObjectCommand : Command, IUpdatable
	{
		public override void Execute(params object[] parameters)
		{
			this.objectToRotate = (Transform)parameters[0];
			this.Retain();
		}

		public void Update()
		{
			if (this.objectToRotate != null)
			{
				this.objectToRotate.Rotate(1f, 1f, 1f);
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			UnityEngine.Debug.Log("RotateGameObjectCommand released");
		}

		protected Transform objectToRotate;
	}
}
