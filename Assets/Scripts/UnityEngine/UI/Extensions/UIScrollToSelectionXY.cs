using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	public class UIScrollToSelectionXY : MonoBehaviour
	{
		private void Start()
		{
			this.targetScrollRect = base.GetComponent<ScrollRect>();
			this.scrollWindow = this.targetScrollRect.GetComponent<RectTransform>();
		}

		private void Update()
		{
			this.ScrollRectToLevelSelection();
		}

		private void ScrollRectToLevelSelection()
		{
			EventSystem current = EventSystem.current;
			if (this.targetScrollRect == null || this.layoutListGroup == null || this.scrollWindow == null)
			{
				return;
			}
			RectTransform rectTransform = (current.currentSelectedGameObject != null) ? current.currentSelectedGameObject.GetComponent<RectTransform>() : null;
			if (rectTransform != this.targetScrollObject)
			{
				this.scrollToSelection = true;
			}
			if (rectTransform == null || !this.scrollToSelection || rectTransform.transform.parent != this.layoutListGroup.transform)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (this.targetScrollRect.vertical)
			{
				float num = -rectTransform.anchoredPosition.y;
				float num2 = this.layoutListGroup.anchoredPosition.y - num;
				this.targetScrollRect.verticalNormalizedPosition += num2 / this.layoutListGroup.sizeDelta.y * Time.deltaTime * this.scrollSpeed;
				flag2 = (Mathf.Abs(num2) < 2f);
			}
			if (this.targetScrollRect.horizontal)
			{
				float num3 = -rectTransform.anchoredPosition.x;
				float num4 = this.layoutListGroup.anchoredPosition.x - num3;
				this.targetScrollRect.horizontalNormalizedPosition += num4 / this.layoutListGroup.sizeDelta.x * Time.deltaTime * this.scrollSpeed;
				flag = (Mathf.Abs(num4) < 2f);
			}
			if (flag && flag2)
			{
				this.scrollToSelection = false;
			}
			this.targetScrollObject = rectTransform;
		}

		public float scrollSpeed = 10f;

		[SerializeField]
		private RectTransform layoutListGroup;

		private RectTransform targetScrollObject;

		private bool scrollToSelection = true;

		private RectTransform scrollWindow;

		private RectTransform currentCanvas;

		private ScrollRect targetScrollRect;
	}
}
