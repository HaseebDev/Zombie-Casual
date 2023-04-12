using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/VR Cursor")]
	public class VRCursor : MonoBehaviour
	{
		private void Update()
		{
			Vector3 position;
			position.x = UnityEngine.Input.mousePosition.x * this.xSens;
			position.y = UnityEngine.Input.mousePosition.y * this.ySens - 1f;
			position.z = base.transform.position.z;
			base.transform.position = position;
			VRInputModule.cursorPosition = base.transform.position;
			if (Input.GetMouseButtonDown(0) && this.currentCollider)
			{
				VRInputModule.PointerSubmit(this.currentCollider.gameObject);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			VRInputModule.PointerEnter(other.gameObject);
			this.currentCollider = other;
		}

		private void OnTriggerExit(Collider other)
		{
			VRInputModule.PointerExit(other.gameObject);
			this.currentCollider = null;
		}

		public float xSens;

		public float ySens;

		private Collider currentCollider;
	}
}
