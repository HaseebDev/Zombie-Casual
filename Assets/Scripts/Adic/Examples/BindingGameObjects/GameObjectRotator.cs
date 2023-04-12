using System;
using UnityEngine;

namespace Adic.Examples.BindingGameObjects
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

		[Inject]
		public Transform objectToRotate;
	}
}
