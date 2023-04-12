using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Infinite Scroll")]
	public class UI_InfiniteScroll : MonoBehaviour
	{
		private void Awake()
		{
			if (!this.InitByUser)
			{
				this.Init();
			}
		}

		public void Init()
		{
			if (base.GetComponent<ScrollRect>() != null)
			{
				this._scrollRect = base.GetComponent<ScrollRect>();
				this._scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScroll));
				this._scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
				for (int i = 0; i < this._scrollRect.content.childCount; i++)
				{
					this.items.Add(this._scrollRect.content.GetChild(i).GetComponent<RectTransform>());
				}
				if (this._scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
				{
					this._verticalLayoutGroup = this._scrollRect.content.GetComponent<VerticalLayoutGroup>();
				}
				if (this._scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
				{
					this._horizontalLayoutGroup = this._scrollRect.content.GetComponent<HorizontalLayoutGroup>();
				}
				if (this._scrollRect.content.GetComponent<GridLayoutGroup>() != null)
				{
					this._gridLayoutGroup = this._scrollRect.content.GetComponent<GridLayoutGroup>();
				}
				if (this._scrollRect.content.GetComponent<ContentSizeFitter>() != null)
				{
					this._contentSizeFitter = this._scrollRect.content.GetComponent<ContentSizeFitter>();
				}
				this._isHorizontal = this._scrollRect.horizontal;
				this._isVertical = this._scrollRect.vertical;
				if (this._isHorizontal && this._isVertical)
				{
					UnityEngine.Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, plase choose one direction (horizontal or vertical)");
				}
				this._itemCount = this._scrollRect.content.childCount;
				return;
			}
			UnityEngine.Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
		}

		private void DisableGridComponents()
		{
			if (this._isVertical)
			{
				this._recordOffsetY = this.items[0].GetComponent<RectTransform>().anchoredPosition.y - this.items[1].GetComponent<RectTransform>().anchoredPosition.y;
				this._disableMarginY = this._recordOffsetY * (float)this._itemCount / 2f;
			}
			if (this._isHorizontal)
			{
				this._recordOffsetX = this.items[1].GetComponent<RectTransform>().anchoredPosition.x - this.items[0].GetComponent<RectTransform>().anchoredPosition.x;
				this._disableMarginX = this._recordOffsetX * (float)this._itemCount / 2f;
			}
			if (this._verticalLayoutGroup)
			{
				this._verticalLayoutGroup.enabled = false;
			}
			if (this._horizontalLayoutGroup)
			{
				this._horizontalLayoutGroup.enabled = false;
			}
			if (this._contentSizeFitter)
			{
				this._contentSizeFitter.enabled = false;
			}
			if (this._gridLayoutGroup)
			{
				this._gridLayoutGroup.enabled = false;
			}
			this._hasDisabledGridComponents = true;
		}

		public void OnScroll(Vector2 pos)
		{
			if (!this._hasDisabledGridComponents)
			{
				this.DisableGridComponents();
			}
			for (int i = 0; i < this.items.Count; i++)
			{
				if (this._isHorizontal)
				{
					if (this._scrollRect.transform.InverseTransformPoint(this.items[i].gameObject.transform.position).x > this._disableMarginX + this._treshold)
					{
						this._newAnchoredPosition = this.items[i].anchoredPosition;
						this._newAnchoredPosition.x = this._newAnchoredPosition.x - (float)this._itemCount * this._recordOffsetX;
						this.items[i].anchoredPosition = this._newAnchoredPosition;
						this._scrollRect.content.GetChild(this._itemCount - 1).transform.SetAsFirstSibling();
					}
					else if (this._scrollRect.transform.InverseTransformPoint(this.items[i].gameObject.transform.position).x < -this._disableMarginX)
					{
						this._newAnchoredPosition = this.items[i].anchoredPosition;
						this._newAnchoredPosition.x = this._newAnchoredPosition.x + (float)this._itemCount * this._recordOffsetX;
						this.items[i].anchoredPosition = this._newAnchoredPosition;
						this._scrollRect.content.GetChild(0).transform.SetAsLastSibling();
					}
				}
				if (this._isVertical)
				{
					if (this._scrollRect.transform.InverseTransformPoint(this.items[i].gameObject.transform.position).y > this._disableMarginY + this._treshold)
					{
						this._newAnchoredPosition = this.items[i].anchoredPosition;
						this._newAnchoredPosition.y = this._newAnchoredPosition.y - (float)this._itemCount * this._recordOffsetY;
						this.items[i].anchoredPosition = this._newAnchoredPosition;
						this._scrollRect.content.GetChild(this._itemCount - 1).transform.SetAsFirstSibling();
					}
					else if (this._scrollRect.transform.InverseTransformPoint(this.items[i].gameObject.transform.position).y < -this._disableMarginY)
					{
						this._newAnchoredPosition = this.items[i].anchoredPosition;
						this._newAnchoredPosition.y = this._newAnchoredPosition.y + (float)this._itemCount * this._recordOffsetY;
						this.items[i].anchoredPosition = this._newAnchoredPosition;
						this._scrollRect.content.GetChild(0).transform.SetAsLastSibling();
					}
				}
			}
		}

		[Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
		public bool InitByUser;

		private ScrollRect _scrollRect;

		private ContentSizeFitter _contentSizeFitter;

		private VerticalLayoutGroup _verticalLayoutGroup;

		private HorizontalLayoutGroup _horizontalLayoutGroup;

		private GridLayoutGroup _gridLayoutGroup;

		private bool _isVertical;

		private bool _isHorizontal;

		private float _disableMarginX;

		private float _disableMarginY;

		private bool _hasDisabledGridComponents;

		private List<RectTransform> items = new List<RectTransform>();

		private Vector2 _newAnchoredPosition = Vector2.zero;

		private float _treshold = 100f;

		private int _itemCount;

		private float _recordOffsetX;

		private float _recordOffsetY;
	}
}
