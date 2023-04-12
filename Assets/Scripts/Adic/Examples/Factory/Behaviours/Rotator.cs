using System;
using UnityEngine;

namespace Adic.Examples.Factory.Behaviours
{
	public class Rotator : MonoBehaviour
	{
		protected void Start()
		{
			this.cachedTransform = base.GetComponent<Transform>();
		}

		protected void Update()
		{
			this.cachedTransform.Rotate(this.speed, this.speed, this.speed);
		}

		public float speed;

		protected Transform cachedTransform;
	}
}
