using System;
using UnityEngine;

namespace Adic.Examples.UsingConditions
{
	public class GameObjectRotator : MonoBehaviour
	{
		protected void Start()
		{
			this.Inject();
		}

		protected void Update()
		{
			if (this.objectToRotate != null)
			{
				this.objectToRotate.Rotate(1f, 1f, 1f);
			}
		}

		[Inject("RightCube")]
		public Transform objectToRotate;
	}
}
