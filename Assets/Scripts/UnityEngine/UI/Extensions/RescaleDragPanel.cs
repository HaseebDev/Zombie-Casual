using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/RescalePanels/RescaleDragPanel")]
	public class RescaleDragPanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
	{
		private void Awake()
		{
			Canvas componentInParent = base.GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				this.canvasRectTransform = (componentInParent.transform as RectTransform);
				this.panelRectTransform = (base.transform.parent as RectTransform);
				this.goTransform = base.transform.parent;
			}
		}

		public void OnPointerDown(PointerEventData data)
		{
			this.panelRectTransform.SetAsLastSibling();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.panelRectTransform, data.position, data.pressEventCamera, out this.pointerOffset);
		}

		public void OnDrag(PointerEventData data)
		{
			if (this.panelRectTransform == null)
			{
				return;
			}
			Vector2 screenPoint = this.ClampToWindow(data);
			Vector2 a;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.canvasRectTransform, screenPoint, data.pressEventCamera, out a))
			{
				this.panelRectTransform.localPosition = a - new Vector2(this.pointerOffset.x * this.goTransform.localScale.x, this.pointerOffset.y * this.goTransform.localScale.y);
			}
		}

		private Vector2 ClampToWindow(PointerEventData data)
		{
			Vector2 position = data.position;
			Vector3[] array = new Vector3[4];
			this.canvasRectTransform.GetWorldCorners(array);
			float x = Mathf.Clamp(position.x, array[0].x, array[2].x);
			float y = Mathf.Clamp(position.y, array[0].y, array[2].y);
			return new Vector2(x, y);
		}

		private Vector2 pointerOffset;

		private RectTransform canvasRectTransform;

		private RectTransform panelRectTransform;

		private Transform goTransform;
	}
}
