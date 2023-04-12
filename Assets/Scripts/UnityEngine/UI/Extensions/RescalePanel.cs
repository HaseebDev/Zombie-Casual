using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/RescalePanels/RescalePanel")]
	public class RescalePanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
	{
		private void Awake()
		{
			this.rectTransform = base.transform.parent.GetComponent<RectTransform>();
			this.goTransform = base.transform.parent;
			this.thisRectTransform = base.GetComponent<RectTransform>();
			this.sizeDelta = this.thisRectTransform.sizeDelta;
		}

		public void OnPointerDown(PointerEventData data)
		{
			this.rectTransform.SetAsLastSibling();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, data.position, data.pressEventCamera, out this.previousPointerPosition);
		}

		public void OnDrag(PointerEventData data)
		{
			if (this.rectTransform == null)
			{
				return;
			}
			Vector3 vector = this.goTransform.localScale;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, data.position, data.pressEventCamera, out this.currentPointerPosition);
			Vector2 vector2 = this.currentPointerPosition - this.previousPointerPosition;
			vector += new Vector3(-vector2.y * 0.001f, -vector2.y * 0.001f, 0f);
			vector = new Vector3(Mathf.Clamp(vector.x, this.minSize.x, this.maxSize.x), Mathf.Clamp(vector.y, this.minSize.y, this.maxSize.y), 1f);
			this.goTransform.localScale = vector;
			this.previousPointerPosition = this.currentPointerPosition;
			float num = this.sizeDelta.x / this.goTransform.localScale.x;
			Vector2 vector3 = new Vector2(num, num);
			this.thisRectTransform.sizeDelta = vector3;
		}

		public Vector2 minSize;

		public Vector2 maxSize;

		private RectTransform rectTransform;

		private Transform goTransform;

		private Vector2 currentPointerPosition;

		private Vector2 previousPointerPosition;

		private RectTransform thisRectTransform;

		private Vector2 sizeDelta;
	}
}
