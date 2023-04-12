using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Scrollrect Occlusion")]
	public class UI_ScrollRectOcclusion : MonoBehaviour
	{
		private void Awake()
		{
			if (this.InitByUser)
			{
				return;
			}
			this.Init();
		}

		public void Init()
		{
			if (base.GetComponent<ScrollRect>() != null)
			{
				this._scrollRect = base.GetComponent<ScrollRect>();
				this._scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnScroll));
				this._isHorizontal = this._scrollRect.horizontal;
				this._isVertical = this._scrollRect.vertical;
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
					return;
				}
			}
			else
			{
				UnityEngine.Debug.LogError("UI_ScrollRectOcclusion => No ScrollRect component found");
			}
		}

		private void DisableGridComponents()
		{
			if (this._isVertical)
			{
				this._disableMarginY = this._scrollRect.GetComponent<RectTransform>().rect.height / 2f + this.items[0].sizeDelta.y;
			}
			if (this._isHorizontal)
			{
				this._disableMarginX = this._scrollRect.GetComponent<RectTransform>().rect.width / 2f + this.items[0].sizeDelta.x;
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
			this.hasDisabledGridComponents = true;
		}

		public void OnScroll(Vector2 pos)
		{
			if (!this.hasDisabledGridComponents)
			{
				this.DisableGridComponents();
			}
			for (int i = 0; i < this.items.Count; i++)
			{
				if (this._isVertical && this._isHorizontal)
				{
					if (this._scrollRect.transform.InverseTransformPoint(this.items[i].position).y < -this._disableMarginY || this._scrollRect.transform.InverseTransformPoint(this.items[i].position).y > this._disableMarginY || this._scrollRect.transform.InverseTransformPoint(this.items[i].position).x < -this._disableMarginX || this._scrollRect.transform.InverseTransformPoint(this.items[i].position).x > this._disableMarginX)
					{
						this.items[i].gameObject.SetActive(false);
					}
					else
					{
						this.items[i].gameObject.SetActive(true);
					}
				}
				else
				{
					if (this._isVertical)
					{
						if (this._scrollRect.transform.InverseTransformPoint(this.items[i].position).y < -this._disableMarginY || this._scrollRect.transform.InverseTransformPoint(this.items[i].position).y > this._disableMarginY)
						{
							this.items[i].gameObject.SetActive(false);
						}
						else
						{
							this.items[i].gameObject.SetActive(true);
						}
					}
					if (this._isHorizontal)
					{
						if (this._scrollRect.transform.InverseTransformPoint(this.items[i].position).x < -this._disableMarginX || this._scrollRect.transform.InverseTransformPoint(this.items[i].position).x > this._disableMarginX)
						{
							this.items[i].gameObject.SetActive(false);
						}
						else
						{
							this.items[i].gameObject.SetActive(true);
						}
					}
				}
			}
		}

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

		private bool hasDisabledGridComponents;

		private List<RectTransform> items = new List<RectTransform>();
	}
}
