using System;
using UnityEngine;

namespace Framework.Common.Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class Camera3DFollow : MonoBehaviour
	{
		private void Start()
		{
		}

		public void SetTarget(Transform newTarget)
		{
			this.target = newTarget;
			this.offset = base.transform.position - this.target.position;
		}

		private void LateUpdate()
		{
			if (this.target != null)
			{
				Vector3 b = this.target.position + this.offset;
				base.transform.position = Vector3.Slerp(base.transform.position, b, this.Smooth);
			}
		}

		private Transform target;

		[Range(0.01f, 1f)]
		public float Smooth = 0.5f;

		private Vector3 offset;
	}
}
