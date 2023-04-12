using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("Layout/Extensions/Horizontal Scroll Snap")]
	public class HorizontalScrollSnap : ScrollSnapBase, IEndDragHandler, IEventSystemHandler
	{
		private void Start()
		{
			this._isVertical = false;
			this._childAnchorPoint = new Vector2(0f, 0.5f);
			this._currentPage = this.StartingScreen;
			this.UpdateLayout();
		}

		private void Update()
		{
			if (!this._lerp && this._scroll_rect.velocity == Vector2.zero)
			{
				if (!this._settled && !this._pointerDown && !base.IsRectSettledOnaPage(this._screensContainer.localPosition))
				{
					base.ScrollToClosestElement();
				}
				return;
			}
			if (this._lerp)
			{
				this._screensContainer.localPosition = Vector3.Lerp(this._screensContainer.localPosition, this._lerp_target, this.transitionSpeed * Time.deltaTime);
				if (Vector3.Distance(this._screensContainer.localPosition, this._lerp_target) < 0.1f)
				{
					this._lerp = false;
					base.EndScreenChange();
				}
			}
			base.CurrentPage = base.GetPageforPosition(this._screensContainer.localPosition);
			if (!this._pointerDown && ((double)this._scroll_rect.velocity.x > 0.01 || (double)this._scroll_rect.velocity.x < 0.01) && this.IsRectMovingSlowerThanThreshold(0f))
			{
				base.ScrollToClosestElement();
			}
		}

		private bool IsRectMovingSlowerThanThreshold(float startingSpeed)
		{
			return (this._scroll_rect.velocity.x > startingSpeed && this._scroll_rect.velocity.x < (float)this.SwipeVelocityThreshold) || (this._scroll_rect.velocity.x < startingSpeed && this._scroll_rect.velocity.x > (float)(-(float)this.SwipeVelocityThreshold));
		}

		private void DistributePages()
		{
			this._screens = this._screensContainer.childCount;
			this._scroll_rect.horizontalNormalizedPosition = 0f;
			int num = 0;
			Rect rect = base.gameObject.GetComponent<RectTransform>().rect;
			float num2 = 0f;
			float num3 = this._childSize = (float)((int)rect.width) * ((this.PageStep == 0f) ? 3f : this.PageStep);
			for (int i = 0; i < this._screensContainer.transform.childCount; i++)
			{
				RectTransform component = this._screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
				num2 = (float)(num + (int)((float)i * num3));
				component.sizeDelta = new Vector2(rect.width, rect.height);
				component.anchoredPosition = new Vector2(num2, 0f);
				component.anchorMin = (component.anchorMax = (component.pivot = this._childAnchorPoint));
			}
			float x = num2 + (float)(num * -1);
			this._screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(x, 0f);
		}

		public void AddChild(GameObject GO)
		{
			this.AddChild(GO, false);
		}

		public void AddChild(GameObject GO, bool WorldPositionStays)
		{
			this._scroll_rect.horizontalNormalizedPosition = 0f;
			GO.transform.SetParent(this._screensContainer, WorldPositionStays);
			this.DistributePages();
			if (this.MaskArea)
			{
				base.UpdateVisible();
			}
			this.SetScrollContainerPosition();
		}

		public void RemoveChild(int index, out GameObject ChildRemoved)
		{
			ChildRemoved = null;
			if (index < 0 || index > this._screensContainer.childCount)
			{
				return;
			}
			this._scroll_rect.horizontalNormalizedPosition = 0f;
			Transform child = this._screensContainer.transform.GetChild(index);
			child.SetParent(null);
			ChildRemoved = child.gameObject;
			this.DistributePages();
			if (this.MaskArea)
			{
				base.UpdateVisible();
			}
			if (this._currentPage > this._screens - 1)
			{
				base.CurrentPage = this._screens - 1;
			}
			this.SetScrollContainerPosition();
		}

		public void RemoveAllChildren(out GameObject[] ChildrenRemoved)
		{
			int childCount = this._screensContainer.childCount;
			ChildrenRemoved = new GameObject[childCount];
			for (int i = childCount - 1; i >= 0; i--)
			{
				ChildrenRemoved[i] = this._screensContainer.GetChild(i).gameObject;
				ChildrenRemoved[i].transform.SetParent(null);
			}
			this._scroll_rect.horizontalNormalizedPosition = 0f;
			base.CurrentPage = 0;
			base.InitialiseChildObjectsFromScene();
			this.DistributePages();
			if (this.MaskArea)
			{
				base.UpdateVisible();
			}
		}

		private void SetScrollContainerPosition()
		{
			this._scrollStartPosition = this._screensContainer.localPosition.x;
			this._scroll_rect.horizontalNormalizedPosition = (float)this._currentPage / (float)(this._screens - 1);
		}

		public void UpdateLayout()
		{
			this._lerp = false;
			this.DistributePages();
			if (this.MaskArea)
			{
				base.UpdateVisible();
			}
			this.SetScrollContainerPosition();
			base.ChangeBulletsInfo(this._currentPage);
		}

		private void OnRectTransformDimensionsChange()
		{
			if (this._childAnchorPoint != Vector2.zero)
			{
				this.UpdateLayout();
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			this._pointerDown = false;
			if (this._scroll_rect.horizontal && this.UseFastSwipe)
			{
				if ((this._scroll_rect.velocity.x > 0f && this._scroll_rect.velocity.x > (float)this.FastSwipeThreshold) || (this._scroll_rect.velocity.x < 0f && this._scroll_rect.velocity.x < (float)(-(float)this.FastSwipeThreshold)))
				{
					this._scroll_rect.velocity = Vector3.zero;
					if (this._startPosition.x - this._screensContainer.localPosition.x > 0f)
					{
						base.NextScreen();
						return;
					}
					base.PreviousScreen();
					return;
				}
				else
				{
					base.ScrollToClosestElement();
				}
			}
		}
	}
}
