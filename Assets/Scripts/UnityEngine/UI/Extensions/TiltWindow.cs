using System;

namespace UnityEngine.UI.Extensions
{
	public class TiltWindow : MonoBehaviour
	{
		private void Start()
		{
			this.mTrans = base.transform;
			this.mStart = this.mTrans.localRotation;
		}

		private void Update()
		{
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			float num = (float)Screen.width * 0.5f;
			float num2 = (float)Screen.height * 0.5f;
			float x = Mathf.Clamp((mousePosition.x - num) / num, -1f, 1f);
			float y = Mathf.Clamp((mousePosition.y - num2) / num2, -1f, 1f);
			this.mRot = Vector2.Lerp(this.mRot, new Vector2(x, y), Time.deltaTime * 5f);
			this.mTrans.localRotation = this.mStart * Quaternion.Euler(-this.mRot.y * this.range.y, this.mRot.x * this.range.x, 0f);
		}

		public Vector2 range = new Vector2(5f, 3f);

		private Transform mTrans;

		private Quaternion mStart;

		private Vector2 mRot = Vector2.zero;
	}
}
