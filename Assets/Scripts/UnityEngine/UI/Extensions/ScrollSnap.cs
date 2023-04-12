using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/Scroll Snap")]
	public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
	{
		public event ScrollSnap.PageSnapChange onPageChange;

		private void Awake()
		{
			this.lerp = false;
			this.scrollRect = base.gameObject.GetComponent<ScrollRect>();
			this.scrollRectTransform = base.gameObject.GetComponent<RectTransform>();
			this.listContainerTransform = this.scrollRect.content;
			this.listContainerRectTransform = this.listContainerTransform.GetComponent<RectTransform>();
			this.rectTransform = this.listContainerTransform.gameObject.GetComponent<RectTransform>();
			this.UpdateListItemsSize();
			this.UpdateListItemPositions();
			this.PageChanged(this.CurrentPage());
			if (this.nextButton)
			{
				this.nextButton.GetComponent<Button>().onClick.AddListener(delegate()
				{
					this.NextScreen();
				});
			}
			if (this.prevButton)
			{
				this.prevButton.GetComponent<Button>().onClick.AddListener(delegate()
				{
					this.PreviousScreen();
				});
			}
		}

		private void Start()
		{
			this.Awake();
		}

		public void UpdateListItemsSize()
		{
			float num;
			float num2;
			if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
			{
				num = this.scrollRectTransform.rect.width / (float)this.itemsVisibleAtOnce;
				num2 = this.listContainerRectTransform.rect.width / (float)this.itemsCount;
			}
			else
			{
				num = this.scrollRectTransform.rect.height / (float)this.itemsVisibleAtOnce;
				num2 = this.listContainerRectTransform.rect.height / (float)this.itemsCount;
			}
			this.itemSize = num;
			if (this.linkScrolrectScrollSensitivity)
			{
				this.scrollRect.scrollSensitivity = this.itemSize;
			}
			if (this.autoLayoutItems && num2 != num && this.itemsCount > 0)
			{
				if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
				{
					IEnumerator enumerator = this.listContainerTransform.GetEnumerator();
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						GameObject gameObject = ((Transform)obj).gameObject;
						if (gameObject.activeInHierarchy)
						{
							LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();
							if (layoutElement == null)
							{
								layoutElement = gameObject.AddComponent<LayoutElement>();
							}
							layoutElement.minWidth = this.itemSize;
						}
					}
					return;
				}
				foreach (object obj2 in this.listContainerTransform)
				{
					GameObject gameObject2 = ((Transform)obj2).gameObject;
					if (gameObject2.activeInHierarchy)
					{
						LayoutElement layoutElement2 = gameObject2.GetComponent<LayoutElement>();
						if (layoutElement2 == null)
						{
							layoutElement2 = gameObject2.AddComponent<LayoutElement>();
						}
						layoutElement2.minHeight = this.itemSize;
					}
				}
			}
		}

		public void UpdateListItemPositions()
		{
			if (!this.listContainerRectTransform.rect.size.Equals(this.listContainerCachedSize))
			{
				int num = 0;
				IEnumerator enumerator = this.listContainerTransform.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (((Transform)enumerator.Current).gameObject.activeInHierarchy)
					{
						num++;
					}
				}
				this.itemsCount = 0;
				Array.Resize<Vector3>(ref this.pageAnchorPositions, num);
				if (num > 0)
				{
					this.pages = Mathf.Max(num - this.itemsVisibleAtOnce + 1, 1);
					if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
					{
						this.scrollRect.horizontalNormalizedPosition = 0f;
						this.listContainerMaxPosition = this.listContainerTransform.localPosition.x;
						this.scrollRect.horizontalNormalizedPosition = 1f;
						this.listContainerMinPosition = this.listContainerTransform.localPosition.x;
						this.listContainerSize = this.listContainerMaxPosition - this.listContainerMinPosition;
						for (int i = 0; i < this.pages; i++)
						{
							this.pageAnchorPositions[i] = new Vector3(this.listContainerMaxPosition - this.itemSize * (float)i, this.listContainerTransform.localPosition.y, this.listContainerTransform.localPosition.z);
						}
					}
					else
					{
						this.scrollRect.verticalNormalizedPosition = 1f;
						this.listContainerMinPosition = this.listContainerTransform.localPosition.y;
						this.scrollRect.verticalNormalizedPosition = 0f;
						this.listContainerMaxPosition = this.listContainerTransform.localPosition.y;
						this.listContainerSize = this.listContainerMaxPosition - this.listContainerMinPosition;
						for (int j = 0; j < this.pages; j++)
						{
							this.pageAnchorPositions[j] = new Vector3(this.listContainerTransform.localPosition.x, this.listContainerMinPosition + this.itemSize * (float)j, this.listContainerTransform.localPosition.z);
						}
					}
					this.UpdateScrollbar(this.linkScrolbarSteps);
					this.startingPage = Mathf.Min(this.startingPage, this.pages);
					this.ResetPage();
				}
				if (this.itemsCount != num)
				{
					this.PageChanged(this.CurrentPage());
				}
				this.itemsCount = num;
				this.listContainerCachedSize.Set(this.listContainerRectTransform.rect.size.x, this.listContainerRectTransform.rect.size.y);
			}
		}

		public void ResetPage()
		{
			if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
			{
				this.scrollRect.horizontalNormalizedPosition = ((this.pages > 1) ? ((float)this.startingPage / (float)(this.pages - 1)) : 0f);
				return;
			}
			this.scrollRect.verticalNormalizedPosition = ((this.pages > 1) ? ((float)(this.pages - this.startingPage - 1) / (float)(this.pages - 1)) : 0f);
		}

		protected void UpdateScrollbar(bool linkSteps)
		{
			if (linkSteps)
			{
				if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
				{
					if (this.scrollRect.horizontalScrollbar != null)
					{
						this.scrollRect.horizontalScrollbar.numberOfSteps = this.pages;
						return;
					}
				}
				else if (this.scrollRect.verticalScrollbar != null)
				{
					this.scrollRect.verticalScrollbar.numberOfSteps = this.pages;
					return;
				}
			}
			else if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
			{
				if (this.scrollRect.horizontalScrollbar != null)
				{
					this.scrollRect.horizontalScrollbar.numberOfSteps = 0;
					return;
				}
			}
			else if (this.scrollRect.verticalScrollbar != null)
			{
				this.scrollRect.verticalScrollbar.numberOfSteps = 0;
			}
		}

		private void LateUpdate()
		{
			this.UpdateListItemsSize();
			this.UpdateListItemPositions();
			if (this.lerp)
			{
				this.UpdateScrollbar(false);
				this.listContainerTransform.localPosition = Vector3.Lerp(this.listContainerTransform.localPosition, this.lerpTarget, 7.5f * Time.deltaTime);
				if (Vector3.Distance(this.listContainerTransform.localPosition, this.lerpTarget) < 0.001f)
				{
					this.listContainerTransform.localPosition = this.lerpTarget;
					this.lerp = false;
					this.UpdateScrollbar(this.linkScrolbarSteps);
				}
				if (Vector3.Distance(this.listContainerTransform.localPosition, this.lerpTarget) < 10f)
				{
					this.PageChanged(this.CurrentPage());
				}
			}
			if (this.fastSwipeTimer)
			{
				this.fastSwipeCounter++;
			}
		}

		public void NextScreen()
		{
			this.UpdateListItemPositions();
			if (this.CurrentPage() < this.pages - 1)
			{
				this.lerp = true;
				this.lerpTarget = this.pageAnchorPositions[this.CurrentPage() + 1];
				this.PageChanged(this.CurrentPage() + 1);
			}
		}

		public void PreviousScreen()
		{
			this.UpdateListItemPositions();
			if (this.CurrentPage() > 0)
			{
				this.lerp = true;
				this.lerpTarget = this.pageAnchorPositions[this.CurrentPage() - 1];
				this.PageChanged(this.CurrentPage() - 1);
			}
		}

		private void NextScreenCommand()
		{
			if (this.pageOnDragStart < this.pages - 1)
			{
				int num = Mathf.Min(this.pages - 1, this.pageOnDragStart + this.itemsVisibleAtOnce);
				this.lerp = true;
				this.lerpTarget = this.pageAnchorPositions[num];
				this.PageChanged(num);
			}
		}

		private void PrevScreenCommand()
		{
			if (this.pageOnDragStart > 0)
			{
				int num = Mathf.Max(0, this.pageOnDragStart - this.itemsVisibleAtOnce);
				this.lerp = true;
				this.lerpTarget = this.pageAnchorPositions[num];
				this.PageChanged(num);
			}
		}

		public int CurrentPage()
		{
			float num;
			if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
			{
				num = this.listContainerMaxPosition - this.listContainerTransform.localPosition.x;
				num = Mathf.Clamp(num, 0f, this.listContainerSize);
			}
			else
			{
				num = this.listContainerTransform.localPosition.y - this.listContainerMinPosition;
				num = Mathf.Clamp(num, 0f, this.listContainerSize);
			}
			return Mathf.Clamp(Mathf.RoundToInt(num / this.itemSize), 0, this.pages);
		}

		public void ChangePage(int page)
		{
			if (0 <= page && page < this.pages)
			{
				this.lerp = true;
				this.lerpTarget = this.pageAnchorPositions[page];
				this.PageChanged(page);
			}
		}

		private void PageChanged(int currentPage)
		{
			this.startingPage = currentPage;
			if (this.nextButton)
			{
				this.nextButton.interactable = (currentPage < this.pages - 1);
			}
			if (this.prevButton)
			{
				this.prevButton.interactable = (currentPage > 0);
			}
			if (this.onPageChange != null)
			{
				this.onPageChange(currentPage);
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			this.UpdateScrollbar(false);
			this.fastSwipeCounter = 0;
			this.fastSwipeTimer = true;
			this.positionOnDragStart = eventData.position;
			this.pageOnDragStart = this.CurrentPage();
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			this.startDrag = true;
			float num;
			if (this.direction == ScrollSnap.ScrollDirection.Horizontal)
			{
				num = this.positionOnDragStart.x - eventData.position.x;
			}
			else
			{
				num = -this.positionOnDragStart.y + eventData.position.y;
			}
			if (!this.useFastSwipe)
			{
				this.lerp = true;
				this.lerpTarget = this.pageAnchorPositions[this.CurrentPage()];
				return;
			}
			this.fastSwipe = false;
			this.fastSwipeTimer = false;
			if (this.fastSwipeCounter <= this.fastSwipeTarget && Math.Abs(num) > (float)this.fastSwipeThreshold)
			{
				this.fastSwipe = true;
			}
			if (!this.fastSwipe)
			{
				this.lerp = true;
				this.lerpTarget = this.pageAnchorPositions[this.CurrentPage()];
				return;
			}
			if (num > 0f)
			{
				this.NextScreenCommand();
				return;
			}
			this.PrevScreenCommand();
		}

		public void OnDrag(PointerEventData eventData)
		{
			this.lerp = false;
			if (this.startDrag)
			{
				this.OnBeginDrag(eventData);
				this.startDrag = false;
			}
		}

		public ScrollSnap.ScrollDirection direction;

		protected ScrollRect scrollRect;

		protected RectTransform scrollRectTransform;

		protected Transform listContainerTransform;

		protected RectTransform rectTransform;

		private int pages;

		protected int startingPage;

		protected Vector3[] pageAnchorPositions;

		protected Vector3 lerpTarget;

		protected bool lerp;

		protected float listContainerMinPosition;

		protected float listContainerMaxPosition;

		protected float listContainerSize;

		protected RectTransform listContainerRectTransform;

		protected Vector2 listContainerCachedSize;

		protected float itemSize;

		protected int itemsCount;

		[Tooltip("Button to go to the next page. (optional)")]
		public Button nextButton;

		[Tooltip("Button to go to the previous page. (optional)")]
		public Button prevButton;

		[Tooltip("Number of items visible in one page of scroll frame.")]
		[Range(1f, 100f)]
		public int itemsVisibleAtOnce = 1;

		[Tooltip("Sets minimum width of list items to 1/itemsVisibleAtOnce.")]
		public bool autoLayoutItems = true;

		[Tooltip("If you wish to update scrollbar numberOfSteps to number of active children on list.")]
		public bool linkScrolbarSteps;

		[Tooltip("If you wish to update scrollrect sensitivity to size of list element.")]
		public bool linkScrolrectScrollSensitivity;

		public bool useFastSwipe = true;

		public int fastSwipeThreshold = 100;

		protected bool startDrag = true;

		protected Vector3 positionOnDragStart;

		protected int pageOnDragStart;

		protected bool fastSwipeTimer;

		protected int fastSwipeCounter;

		protected int fastSwipeTarget = 10;

		private bool fastSwipe;

		public enum ScrollDirection
		{
			Horizontal,
			Vertical
		}

		public delegate void PageSnapChange(int page);
	}
}
