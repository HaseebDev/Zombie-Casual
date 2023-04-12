using System;
using Framework.Utility;
using UnityEngine;

namespace Framework.Common.Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class Camera2DFollowTDS : MonoSingleton<Camera2DFollowTDS>
	{
		private void OnEnable()
		{
			this.cam = base.GetComponent<UnityEngine.Camera>();
			this.cam.orthographic = true;
			this.CalculateBounds();
		}

		public override void Init()
		{
		}

		public void UseCameraBounds(bool value)
		{
			this.useBounds = value;
		}

		public void SetTarget(Transform newTarget)
		{
			this._target = newTarget;
			if (this._target)
			{
				base.transform.position = new Vector3(this._target.position.x, this._target.position.y, base.transform.position.z);
			}
		}

		public void CalculateBounds()
		{
			if (this.boundsMap == null)
			{
				return;
			}
			Bounds bounds = this.Camera2DBounds();
			this.min = bounds.max + this.boundsMap.bounds.min;
			this.max = bounds.min + this.boundsMap.bounds.max;
		}

		private Bounds Camera2DBounds()
		{
			float num = this.cam.orthographicSize * 2f;
			return new Bounds(Vector3.zero, new Vector3(num * this.cam.aspect, num, 0f));
		}

		private Vector3 MoveInside(Vector3 current, Vector3 pMin, Vector3 pMax)
		{
			if (!this.useBounds || this.boundsMap == null)
			{
				return current;
			}
			current = Vector3.Max(current, pMin);
			current = Vector3.Min(current, pMax);
			return current;
		}

		private Vector3 Mouse()
		{
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			mousePosition.z = -base.transform.position.z;
			return this.cam.ScreenToWorldPoint(mousePosition);
		}

		private void Follow()
		{
			if (this.face == Camera2DFollowTDS.Mode.Player)
			{
				this.direction = this._target.right;
			}
			else
			{
				this.direction = (this.Mouse() - this._target.position).normalized;
			}
			Vector3 vector = this._target.position + this.direction * this.offset;
			vector.z = base.transform.position.z;
			vector = this.MoveInside(vector, new Vector3(this.min.x, this.min.y, vector.z), new Vector3(this.max.x, this.max.y, vector.z));
			base.transform.position = Vector3.Lerp(base.transform.position, vector, this.smooth * Time.deltaTime);
		}

		private void LateUpdate()
		{
			if (this._target)
			{
				this.Follow();
			}
		}

		[SerializeField]
		private Camera2DFollowTDS.Mode face;

		[SerializeField]
		private SpriteRenderer boundsMap;

		[SerializeField]
		private bool useBounds = true;

		[SerializeField]
		private float smooth = 2.5f;

		[SerializeField]
		private float offset;

		private Transform _target;

		private Vector3 min;

		private Vector3 max;

		private Vector3 direction;

		private UnityEngine.Camera cam;

		private enum Mode
		{
			Player,
			Cursor
		}
	}
}
