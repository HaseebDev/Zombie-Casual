using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/Scrollrect Conflict Manager")]
	public class ScrollConflictManager : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
	{
		private void Awake()
		{
			this._myScrollRect = base.GetComponent<ScrollRect>();
			this.scrollOtherHorizontally = this._myScrollRect.vertical;
			if (this.scrollOtherHorizontally)
			{
				if (this._myScrollRect.horizontal)
				{
					UnityEngine.Debug.Log("You have added the SecondScrollRect to a scroll view that already has both directions selected");
				}
				if (!this.ParentScrollRect.horizontal)
				{
					UnityEngine.Debug.Log("The other scroll rect doesnt support scrolling horizontally");
					return;
				}
			}
			else if (!this.ParentScrollRect.vertical)
			{
				UnityEngine.Debug.Log("The other scroll rect doesnt support scrolling vertically");
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			float num = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
			float num2 = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
			if (this.scrollOtherHorizontally)
			{
				if (num > num2)
				{
					this.scrollOther = true;
					this._myScrollRect.enabled = false;
					this.ParentScrollRect.OnBeginDrag(eventData);
					return;
				}
			}
			else if (num2 > num)
			{
				this.scrollOther = true;
				this._myScrollRect.enabled = false;
				this.ParentScrollRect.OnBeginDrag(eventData);
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (this.scrollOther)
			{
				this.scrollOther = false;
				this._myScrollRect.enabled = true;
				this.ParentScrollRect.OnEndDrag(eventData);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (this.scrollOther)
			{
				this.ParentScrollRect.OnDrag(eventData);
			}
		}

		public ScrollRect ParentScrollRect;

		private ScrollRect _myScrollRect;

		private bool scrollOther;

		private bool scrollOtherHorizontally;
	}
}
