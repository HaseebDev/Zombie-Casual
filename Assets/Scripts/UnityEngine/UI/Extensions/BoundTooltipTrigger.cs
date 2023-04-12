using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Trigger")]
	public class BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.useMousePosition)
			{
				this.StartHover(new Vector3(eventData.position.x, eventData.position.y, 0f));
				return;
			}
			this.StartHover(base.transform.position + this.offset);
		}

		public void OnSelect(BaseEventData eventData)
		{
			this.StartHover(base.transform.position);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			this.StopHover();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			this.StopHover();
		}

		private void StartHover(Vector3 position)
		{
			BoundTooltipItem.Instance.ShowTooltip(this.text, position);
		}

		private void StopHover()
		{
			BoundTooltipItem.Instance.HideTooltip();
		}

		[TextArea]
		public string text;

		public bool useMousePosition;

		public Vector3 offset;
	}
}
