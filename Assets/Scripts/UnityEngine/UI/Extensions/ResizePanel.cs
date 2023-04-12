using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/RescalePanels/ResizePanel")]
	public class ResizePanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
	{
		private void Awake()
		{
			this.rectTransform = base.transform.parent.GetComponent<RectTransform>();
			float width = this.rectTransform.rect.width;
			float height = this.rectTransform.rect.height;
			this.ratio = height / width;
			this.minSize = new Vector2(0.1f * width, 0.1f * height);
			this.maxSize = new Vector2(10f * width, 10f * height);
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
			Vector2 vector = this.rectTransform.sizeDelta;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, data.position, data.pressEventCamera, out this.currentPointerPosition);
			Vector2 vector2 = this.currentPointerPosition - this.previousPointerPosition;
			vector += new Vector2(vector2.x, this.ratio * vector2.x);
			vector = new Vector2(Mathf.Clamp(vector.x, this.minSize.x, this.maxSize.x), Mathf.Clamp(vector.y, this.minSize.y, this.maxSize.y));
			this.rectTransform.sizeDelta = vector;
			this.previousPointerPosition = this.currentPointerPosition;
		}

		public Vector2 minSize;

		public Vector2 maxSize;

		private RectTransform rectTransform;

		private Vector2 currentPointerPosition;

		private Vector2 previousPointerPosition;

		private float ratio;
	}
}
