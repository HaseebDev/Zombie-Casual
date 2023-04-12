using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	public class ScrollSnapBase : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
	{
		public int CurrentPage
		{
			get
			{
				return this._currentPage;
			}
			internal set
			{
				if ((value != this._currentPage && value >= 0 && value < this._screensContainer.childCount) || (value == 0 && this._screensContainer.childCount == 0))
				{
					this._previousPage = this._currentPage;
					this._currentPage = value;
					if (this.MaskArea)
					{
						this.UpdateVisible();
					}
					if (!this._lerp)
					{
						this.ScreenChange();
					}
					this.ChangeBulletsInfo(this._currentPage);
				}
			}
		}

		public ScrollSnapBase.SelectionChangeStartEvent OnSelectionChangeStartEvent
		{
			get
			{
				return this.m_OnSelectionChangeStartEvent;
			}
			set
			{
				this.m_OnSelectionChangeStartEvent = value;
			}
		}

		public ScrollSnapBase.SelectionPageChangedEvent OnSelectionPageChangedEvent
		{
			get
			{
				return this.m_OnSelectionPageChangedEvent;
			}
			set
			{
				this.m_OnSelectionPageChangedEvent = value;
			}
		}

		public ScrollSnapBase.SelectionChangeEndEvent OnSelectionChangeEndEvent
		{
			get
			{
				return this.m_OnSelectionChangeEndEvent;
			}
			set
			{
				this.m_OnSelectionChangeEndEvent = value;
			}
		}

		private void Awake()
		{
			this._scroll_rect = base.gameObject.GetComponent<ScrollRect>();
			if (this._scroll_rect.horizontalScrollbar || this._scroll_rect.verticalScrollbar)
			{
				UnityEngine.Debug.LogWarning("Warning, using scrollbars with the Scroll Snap controls is not advised as it causes unpredictable results");
			}
			if (this.StartingScreen < 0)
			{
				this.StartingScreen = 0;
			}
			this._screensContainer = this._scroll_rect.content;
			if (this.ChildObjects != null && this.ChildObjects.Length != 0)
			{
				if (this._screensContainer.transform.childCount > 0)
				{
					UnityEngine.Debug.LogError("ScrollRect Content has children, this is not supported when using managed Child Objects\n Either remove the ScrollRect Content children or clear the ChildObjects array");
					return;
				}
				this.InitialiseChildObjectsFromArray();
			}
			else
			{
				this.InitialiseChildObjectsFromScene();
			}
			if (this.NextButton)
			{
				this.NextButton.GetComponent<Button>().onClick.AddListener(delegate()
				{
					this.NextScreen();
				});
			}
			if (this.PrevButton)
			{
				this.PrevButton.GetComponent<Button>().onClick.AddListener(delegate()
				{
					this.PreviousScreen();
				});
			}
		}

		internal void InitialiseChildObjectsFromScene()
		{
			int childCount = this._screensContainer.childCount;
			this.ChildObjects = new GameObject[childCount];
			for (int i = 0; i < childCount; i++)
			{
				this.ChildObjects[i] = this._screensContainer.transform.GetChild(i).gameObject;
				if (this.MaskArea && this.ChildObjects[i].activeSelf)
				{
					this.ChildObjects[i].SetActive(false);
				}
			}
		}

		internal void InitialiseChildObjectsFromArray()
		{
			int num = this.ChildObjects.Length;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ChildObjects[i]);
				if (this.UseParentTransform)
				{
					RectTransform component = gameObject.GetComponent<RectTransform>();
					component.rotation = this._screensContainer.rotation;
					component.localScale = this._screensContainer.localScale;
					component.position = this._screensContainer.position;
				}
				gameObject.transform.SetParent(this._screensContainer.transform);
				this.ChildObjects[i] = gameObject;
				if (this.MaskArea && this.ChildObjects[i].activeSelf)
				{
					this.ChildObjects[i].SetActive(false);
				}
			}
		}

		internal void UpdateVisible()
		{
			if (!this.MaskArea || this.ChildObjects == null || this.ChildObjects.Length < 1 || this._screensContainer.childCount < 1)
			{
				return;
			}
			this._maskSize = (this._isVertical ? this.MaskArea.rect.height : this.MaskArea.rect.width);
			this._halfNoVisibleItems = (int)Math.Round((double)(this._maskSize / (this._childSize * this.MaskBuffer)), MidpointRounding.AwayFromZero) / 2;
			this._bottomItem = (this._topItem = 0);
			for (int i = this._halfNoVisibleItems + 1; i > 0; i--)
			{
				this._bottomItem = ((this._currentPage - i < 0) ? 0 : i);
				if (this._bottomItem > 0)
				{
					break;
				}
			}
			for (int j = this._halfNoVisibleItems + 1; j > 0; j--)
			{
				this._topItem = ((this._screensContainer.childCount - this._currentPage - j < 0) ? 0 : j);
				if (this._topItem > 0)
				{
					break;
				}
			}
			for (int k = this.CurrentPage - this._bottomItem; k < this.CurrentPage + this._topItem; k++)
			{
				try
				{
					this.ChildObjects[k].SetActive(true);
				}
				catch
				{
					UnityEngine.Debug.Log("Failed to setactive child [" + k + "]");
				}
			}
			if (this._currentPage > this._halfNoVisibleItems)
			{
				this.ChildObjects[this.CurrentPage - this._bottomItem].SetActive(false);
			}
			if (this._screensContainer.childCount - this._currentPage > this._topItem)
			{
				this.ChildObjects[this.CurrentPage + this._topItem].SetActive(false);
			}
		}

		public void NextScreen()
		{
			if (this._currentPage < this._screens - 1)
			{
				if (!this._lerp)
				{
					this.StartScreenChange();
				}
				this._lerp = true;
				this.CurrentPage = this._currentPage + 1;
				this.GetPositionforPage(this._currentPage, ref this._lerp_target);
				this.ScreenChange();
			}
		}

		public void PreviousScreen()
		{
			if (this._currentPage > 0)
			{
				if (!this._lerp)
				{
					this.StartScreenChange();
				}
				this._lerp = true;
				this.CurrentPage = this._currentPage - 1;
				this.GetPositionforPage(this._currentPage, ref this._lerp_target);
				this.ScreenChange();
			}
		}

		public void GoToScreen(int screenIndex)
		{
			if (screenIndex <= this._screens - 1 && screenIndex >= 0)
			{
				if (!this._lerp)
				{
					this.StartScreenChange();
				}
				this._lerp = true;
				this.CurrentPage = screenIndex;
				this.GetPositionforPage(this._currentPage, ref this._lerp_target);
				this.ScreenChange();
			}
		}

		internal int GetPageforPosition(Vector3 pos)
		{
			if (!this._isVertical)
			{
				return -(int)Math.Round((double)((pos.x - this._scrollStartPosition) / this._childSize));
			}
			return -(int)Math.Round((double)((pos.y - this._scrollStartPosition) / this._childSize));
		}

		internal bool IsRectSettledOnaPage(Vector3 pos)
		{
			if (!this._isVertical)
			{
				return -((pos.x - this._scrollStartPosition) / this._childSize) == (float)(-(float)((int)Math.Round((double)((pos.x - this._scrollStartPosition) / this._childSize))));
			}
			return -((pos.y - this._scrollStartPosition) / this._childSize) == (float)(-(float)((int)Math.Round((double)((pos.y - this._scrollStartPosition) / this._childSize))));
		}

		internal void GetPositionforPage(int page, ref Vector3 target)
		{
			this._childPos = -this._childSize * (float)page;
			if (this._isVertical)
			{
				target.y = this._childPos + this._scrollStartPosition;
				return;
			}
			target.x = this._childPos + this._scrollStartPosition;
		}

		internal void ScrollToClosestElement()
		{
			this._lerp = true;
			this.CurrentPage = this.GetPageforPosition(this._screensContainer.localPosition);
			this.GetPositionforPage(this._currentPage, ref this._lerp_target);
			this.ChangeBulletsInfo(this._currentPage);
		}

		internal void ChangeBulletsInfo(int targetScreen)
		{
			if (this.Pagination)
			{
				for (int i = 0; i < this.Pagination.transform.childCount; i++)
				{
					this.Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (targetScreen == i);
				}
			}
		}

		private void OnValidate()
		{
			int childCount = base.gameObject.GetComponent<ScrollRect>().content.childCount;
			if (childCount != 0 || this.ChildObjects != null)
			{
				int num = (this.ChildObjects == null || this.ChildObjects.Length == 0) ? childCount : this.ChildObjects.Length;
				if (this.StartingScreen > num - 1)
				{
					this.StartingScreen = num - 1;
				}
				if (this.StartingScreen < 0)
				{
					this.StartingScreen = 0;
				}
			}
			if (this.MaskBuffer <= 0f)
			{
				this.MaskBuffer = 1f;
			}
			if (this.PageStep < 0f)
			{
				this.PageStep = 0f;
			}
			if (this.PageStep > 8f)
			{
				this.PageStep = 9f;
			}
		}

		internal void StartScreenChange()
		{
			this.OnSelectionChangeStartEvent.Invoke();
		}

		internal void ScreenChange()
		{
			this.OnSelectionPageChangedEvent.Invoke(this._currentPage);
		}

		internal void EndScreenChange()
		{
			this.OnSelectionChangeEndEvent.Invoke(this._currentPage);
			this._settled = true;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			this._pointerDown = true;
			this._settled = false;
			this.StartScreenChange();
			this._startPosition = this._screensContainer.localPosition;
		}

		public void OnDrag(PointerEventData eventData)
		{
			this._lerp = false;
		}

		internal RectTransform _screensContainer;

		internal bool _isVertical;

		internal int _screens = 1;

		internal float _scrollStartPosition;

		internal float _childSize;

		private float _childPos;

		private float _maskSize;

		internal Vector2 _childAnchorPoint;

		internal ScrollRect _scroll_rect;

		internal Vector3 _lerp_target;

		internal bool _lerp;

		internal bool _pointerDown;

		internal bool _settled = true;

		internal Vector3 _startPosition;

		[Tooltip("The currently active page")]
		internal int _currentPage;

		internal int _previousPage;

		internal int _halfNoVisibleItems;

		private int _bottomItem;

		private int _topItem;

		[Tooltip("The screen / page to start the control on\n*Note, this is a 0 indexed array")]
		[SerializeField]
		public int StartingScreen;

		[Tooltip("The distance between two pages based on page height, by default pages are next to each other")]
		[SerializeField]
		[Range(1f, 8f)]
		public float PageStep = 1f;

		[Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
		public GameObject Pagination;

		[Tooltip("Button to go to the next page. (optional)")]
		public GameObject NextButton;

		[Tooltip("Button to go to the previous page. (optional)")]
		public GameObject PrevButton;

		[Tooltip("Transition speed between pages. (optional)")]
		public float transitionSpeed = 7.5f;

		[Tooltip("Fast Swipe makes swiping page next / previous (optional)")]
		public bool UseFastSwipe;

		[Tooltip("How far swipe has to travel to initiate a page change (optional)")]
		public int FastSwipeThreshold = 100;

		[Tooltip("Speed at which the ScrollRect will keep scrolling before slowing down and stopping (optional)")]
		public int SwipeVelocityThreshold = 200;

		[Tooltip("The visible bounds area, controls which items are visible/enabled. *Note Should use a RectMask. (optional)")]
		public RectTransform MaskArea;

		[Tooltip("Pixel size to buffer arround Mask Area. (optional)")]
		public float MaskBuffer = 1f;

		[Tooltip("(Experimental)\nBy default, child array objects will use the parent transform\nHowever you can disable this for some interesting effects")]
		public bool UseParentTransform = true;

		[Tooltip("Scroll Snap children. (optional)\nEither place objects in the scene as children OR\nPrefabs in this array, NOT BOTH")]
		public GameObject[] ChildObjects;

		[SerializeField]
		[Tooltip("Event fires when a user starts to change the selection")]
		private ScrollSnapBase.SelectionChangeStartEvent m_OnSelectionChangeStartEvent = new ScrollSnapBase.SelectionChangeStartEvent();

		[SerializeField]
		[Tooltip("Event fires as the page changes, while dragging or jumping")]
		private ScrollSnapBase.SelectionPageChangedEvent m_OnSelectionPageChangedEvent = new ScrollSnapBase.SelectionPageChangedEvent();

		[SerializeField]
		[Tooltip("Event fires when the page settles after a user has dragged")]
		private ScrollSnapBase.SelectionChangeEndEvent m_OnSelectionChangeEndEvent = new ScrollSnapBase.SelectionChangeEndEvent();

		[Serializable]
		public class SelectionChangeStartEvent : UnityEvent
		{
		}

		[Serializable]
		public class SelectionPageChangedEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public class SelectionChangeEndEvent : UnityEvent<int>
		{
		}
	}
}
