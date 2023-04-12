using System;
using Adic.Examples.BindingsSetup.Data;
using UnityEngine;

namespace Adic.Examples.BindingsSetup.Behaviours
{
	public class CubeRotator : MonoBehaviour
	{
		protected void Start()
		{
			this.cachedTransform = base.GetComponent<Transform>();
			this.Inject();
		}

		protected void Update()
		{
			this.cachedTransform.Rotate(this.speedData.speed, this.speedData.speed, this.speedData.speed);
		}

		[Inject]
		public CubeRotationSpeed speedData;

		protected Transform cachedTransform;
	}
}
