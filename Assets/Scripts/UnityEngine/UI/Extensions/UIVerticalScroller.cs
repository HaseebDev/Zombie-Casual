using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("Layout/Extensions/Vertical Scroller")]
	public class UIVerticalScroller : MonoBehaviour
	{
		public UIVerticalScroller()
		{
		}

		public UIVerticalScroller(RectTransform scrollingPanel, GameObject[] arrayOfElements, RectTransform center)
		{
			this._scrollingPanel = scrollingPanel;
			this._arrayOfElements = arrayOfElements;
			this._center = center;
		}

		public void Awake()
		{
			ScrollRect component = base.GetComponent<ScrollRect>();
			if (!this._scrollingPanel)
			{
				this._scrollingPanel = component.content;
			}
			if (!this._center)
			{
				UnityEngine.Debug.LogError("Please define the RectTransform for the Center viewport of the scrollable area");
			}
			if (this._arrayOfElements == null || this._arrayOfElements.Length == 0)
			{
				int childCount = component.content.childCount;
				if (childCount > 0)
				{
					this._arrayOfElements = new GameObject[childCount];
					for (int i = 0; i < childCount; i++)
					{
						this._arrayOfElements[i] = component.content.GetChild(i).gameObject;
					}
				}
			}
		}

		public void Start()
		{
			if (this._arrayOfElements.Length < 1)
			{
				UnityEngine.Debug.Log("No child content found, exiting..");
				return;
			}
			this.elementLength = this._arrayOfElements.Length;
			this.distance = new float[this.elementLength];
			this.distReposition = new float[this.elementLength];
			this.deltaY = this._arrayOfElements[0].GetComponent<RectTransform>().rect.height * (float)this.elementLength / 3f * 2f;
			Vector2 anchoredPosition = new Vector2(this._scrollingPanel.anchoredPosition.x, -this.deltaY);
			this._scrollingPanel.anchoredPosition = anchoredPosition;
			for (int i = 0; i < this._arrayOfElements.Length; i++)
			{
				this.AddListener(this._arrayOfElements[i], i);
			}
			if (this.ScrollUpButton)
			{
				this.ScrollUpButton.GetComponent<Button>().onClick.AddListener(delegate()
				{
					this.ScrollUp();
				});
			}
			if (this.ScrollDownButton)
			{
				this.ScrollDownButton.GetComponent<Button>().onClick.AddListener(delegate()
				{
					this.ScrollDown();
				});
			}
			if (this.StartingIndex > -1)
			{
				this.StartingIndex = ((this.StartingIndex > this._arrayOfElements.Length) ? (this._arrayOfElements.Length - 1) : this.StartingIndex);
				this.SnapToElement(this.StartingIndex);
			}
		}

		private void AddListener(GameObject button, int index)
		{
			button.GetComponent<Button>().onClick.AddListener(delegate()
			{
				this.DoSomething(index);
			});
		}

		private void DoSomething(int index)
		{
			if (this.ButtonClicked != null)
			{
				this.ButtonClicked.Invoke(index);
			}
		}

		public void Update()
		{
			if (this._arrayOfElements.Length < 1)
			{
				return;
			}
			for (int i = 0; i < this.elementLength; i++)
			{
				this.distReposition[i] = this._center.GetComponent<RectTransform>().position.y - this._arrayOfElements[i].GetComponent<RectTransform>().position.y;
				this.distance[i] = Mathf.Abs(this.distReposition[i]);
				float num = Mathf.Max(0.7f, 1f / (1f + this.distance[i] / 200f));
				this._arrayOfElements[i].GetComponent<RectTransform>().transform.localScale = new Vector3(num, num, 1f);
			}
			float num2 = Mathf.Min(this.distance);
			for (int j = 0; j < this.elementLength; j++)
			{
				this._arrayOfElements[j].GetComponent<CanvasGroup>().interactable = false;
				if (num2 == this.distance[j])
				{
					this.minElementsNum = j;
					this._arrayOfElements[j].GetComponent<CanvasGroup>().interactable = true;
					this.result = this._arrayOfElements[j].GetComponentInChildren<Text>().text;
				}
			}
			this.ScrollingElements(-this._arrayOfElements[this.minElementsNum].GetComponent<RectTransform>().anchoredPosition.y);
		}

		private void ScrollingElements(float position)
		{
			float y = Mathf.Lerp(this._scrollingPanel.anchoredPosition.y, position, Time.deltaTime * 1f);
			Vector2 anchoredPosition = new Vector2(this._scrollingPanel.anchoredPosition.x, y);
			this._scrollingPanel.anchoredPosition = anchoredPosition;
		}

		public string GetResults()
		{
			return this.result;
		}

		public void SnapToElement(int element)
		{
			float num = this._arrayOfElements[0].GetComponent<RectTransform>().rect.height * (float)element;
			Vector2 anchoredPosition = new Vector2(this._scrollingPanel.anchoredPosition.x, -num);
			this._scrollingPanel.anchoredPosition = anchoredPosition;
		}

		public void ScrollUp()
		{
			float num = this._arrayOfElements[0].GetComponent<RectTransform>().rect.height / 1.2f;
			Vector2 b = new Vector2(this._scrollingPanel.anchoredPosition.x, this._scrollingPanel.anchoredPosition.y - num);
			this._scrollingPanel.anchoredPosition = Vector2.Lerp(this._scrollingPanel.anchoredPosition, b, 1f);
		}

		public void ScrollDown()
		{
			float num = this._arrayOfElements[0].GetComponent<RectTransform>().rect.height / 1.2f;
			Vector2 anchoredPosition = new Vector2(this._scrollingPanel.anchoredPosition.x, this._scrollingPanel.anchoredPosition.y + num);
			this._scrollingPanel.anchoredPosition = anchoredPosition;
		}

		[Tooltip("Scrollable area (content of desired ScrollRect)")]
		public RectTransform _scrollingPanel;

		[Tooltip("Elements to populate inside the scroller")]
		public GameObject[] _arrayOfElements;

		[Tooltip("Center display area (position of zoomed content)")]
		public RectTransform _center;

		[Tooltip("Select the item to be in center on start. (optional)")]
		public int StartingIndex = -1;

		[Tooltip("Button to go to the next page. (optional)")]
		public GameObject ScrollUpButton;

		[Tooltip("Button to go to the previous page. (optional)")]
		public GameObject ScrollDownButton;

		[Tooltip("Event fired when a specific item is clicked, exposes index number of item. (optional)")]
		public UnityEvent<int> ButtonClicked;

		private float[] distReposition;

		private float[] distance;

		private int minElementsNum;

		private int elementLength;

		private float deltaY;

		private string result;
	}
}
