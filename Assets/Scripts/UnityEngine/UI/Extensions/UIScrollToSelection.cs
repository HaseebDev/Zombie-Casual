using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/UIScrollToSelection")]
	public class UIScrollToSelection : MonoBehaviour
	{
		protected RectTransform LayoutListGroup
		{
			get
			{
				if (!(this.TargetScrollRect != null))
				{
					return null;
				}
				return this.TargetScrollRect.content;
			}
		}

		protected UIScrollToSelection.ScrollType ScrollDirection
		{
			get
			{
				return this.scrollDirection;
			}
		}

		protected float ScrollSpeed
		{
			get
			{
				return this.scrollSpeed;
			}
		}

		protected bool CancelScrollOnInput
		{
			get
			{
				return this.cancelScrollOnInput;
			}
		}

		protected List<KeyCode> CancelScrollKeycodes
		{
			get
			{
				return this.cancelScrollKeycodes;
			}
		}

		protected RectTransform ScrollWindow { get; set; }

		protected ScrollRect TargetScrollRect { get; set; }

		protected EventSystem CurrentEventSystem
		{
			get
			{
				return EventSystem.current;
			}
		}

		protected GameObject LastCheckedGameObject { get; set; }

		protected GameObject CurrentSelectedGameObject
		{
			get
			{
				return EventSystem.current.currentSelectedGameObject;
			}
		}

		protected RectTransform CurrentTargetRectTransform { get; set; }

		protected bool IsManualScrollingAvailable { get; set; }

		protected virtual void Awake()
		{
			this.TargetScrollRect = base.GetComponent<ScrollRect>();
			this.ScrollWindow = this.TargetScrollRect.GetComponent<RectTransform>();
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			this.UpdateReferences();
			this.CheckIfScrollingShouldBeLocked();
			this.ScrollRectToLevelSelection();
		}

		private void UpdateReferences()
		{
			if (this.CurrentSelectedGameObject != this.LastCheckedGameObject)
			{
				this.CurrentTargetRectTransform = ((this.CurrentSelectedGameObject != null) ? this.CurrentSelectedGameObject.GetComponent<RectTransform>() : null);
				if (this.CurrentSelectedGameObject != null && this.CurrentSelectedGameObject.transform.parent == this.LayoutListGroup.transform)
				{
					this.IsManualScrollingAvailable = false;
				}
			}
			this.LastCheckedGameObject = this.CurrentSelectedGameObject;
		}

		private void CheckIfScrollingShouldBeLocked()
		{
			if (!this.CancelScrollOnInput || this.IsManualScrollingAvailable)
			{
				return;
			}
			for (int i = 0; i < this.CancelScrollKeycodes.Count; i++)
			{
				if (UnityEngine.Input.GetKeyDown(this.CancelScrollKeycodes[i]))
				{
					this.IsManualScrollingAvailable = true;
					return;
				}
			}
		}

		private void ScrollRectToLevelSelection()
		{
			if (this.TargetScrollRect == null || this.LayoutListGroup == null || this.ScrollWindow == null || this.IsManualScrollingAvailable)
			{
				return;
			}
			RectTransform currentTargetRectTransform = this.CurrentTargetRectTransform;
			if (currentTargetRectTransform == null || currentTargetRectTransform.transform.parent != this.LayoutListGroup.transform)
			{
				return;
			}
			switch (this.ScrollDirection)
			{
			case UIScrollToSelection.ScrollType.VERTICAL:
				this.UpdateVerticalScrollPosition(currentTargetRectTransform);
				return;
			case UIScrollToSelection.ScrollType.HORIZONTAL:
				this.UpdateHorizontalScrollPosition(currentTargetRectTransform);
				return;
			case UIScrollToSelection.ScrollType.BOTH:
				this.UpdateVerticalScrollPosition(currentTargetRectTransform);
				this.UpdateHorizontalScrollPosition(currentTargetRectTransform);
				return;
			default:
				return;
			}
		}

		private void UpdateVerticalScrollPosition(RectTransform selection)
		{
			float position = -selection.anchoredPosition.y;
			float height = selection.rect.height;
			float height2 = this.ScrollWindow.rect.height;
			float y = this.LayoutListGroup.anchoredPosition.y;
			float scrollOffset = this.GetScrollOffset(position, y, height, height2);
			this.TargetScrollRect.verticalNormalizedPosition += scrollOffset / this.LayoutListGroup.rect.height * Time.deltaTime * this.scrollSpeed;
		}

		private void UpdateHorizontalScrollPosition(RectTransform selection)
		{
			float x = selection.anchoredPosition.x;
			float width = selection.rect.width;
			float width2 = this.ScrollWindow.rect.width;
			float listAnchorPosition = -this.LayoutListGroup.anchoredPosition.x;
			float num = -this.GetScrollOffset(x, listAnchorPosition, width, width2);
			this.TargetScrollRect.horizontalNormalizedPosition += num / this.LayoutListGroup.rect.width * Time.deltaTime * this.scrollSpeed;
		}

		private float GetScrollOffset(float position, float listAnchorPosition, float targetLength, float maskLength)
		{
			if (position < listAnchorPosition)
			{
				return listAnchorPosition - position;
			}
			if (position + targetLength > listAnchorPosition + maskLength)
			{
				return listAnchorPosition + maskLength - (position + targetLength);
			}
			return 0f;
		}

		[Header("[ Settings ]")]
		[SerializeField]
		private UIScrollToSelection.ScrollType scrollDirection;

		[SerializeField]
		private float scrollSpeed = 10f;

		[Header("[ Input ]")]
		[SerializeField]
		private bool cancelScrollOnInput;

		[SerializeField]
		private List<KeyCode> cancelScrollKeycodes = new List<KeyCode>();

		public enum ScrollType
		{
			VERTICAL,
			HORIZONTAL,
			BOTH
		}
	}
}
