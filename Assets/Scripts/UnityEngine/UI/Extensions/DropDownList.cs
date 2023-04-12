using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/Dropdown List")]
	public class DropDownList : MonoBehaviour
	{
		public DropDownListItem SelectedItem { get; private set; }

		public float ScrollBarWidth
		{
			get
			{
				return this._scrollBarWidth;
			}
			set
			{
				this._scrollBarWidth = value;
				this.RedrawPanel();
			}
		}

		public int ItemsToDisplay
		{
			get
			{
				return this._itemsToDisplay;
			}
			set
			{
				this._itemsToDisplay = value;
				this.RedrawPanel();
			}
		}

		public void Start()
		{
			this.Initialize();
		}

		private bool Initialize()
		{
			bool result = true;
			try
			{
				this._rectTransform = base.GetComponent<RectTransform>();
				this._mainButton = new DropDownListButton(this._rectTransform.Find("MainButton").gameObject);
				this._overlayRT = this._rectTransform.Find("Overlay").GetComponent<RectTransform>();
				this._overlayRT.gameObject.SetActive(false);
				this._scrollPanelRT = this._overlayRT.Find("ScrollPanel").GetComponent<RectTransform>();
				this._scrollBarRT = this._scrollPanelRT.Find("Scrollbar").GetComponent<RectTransform>();
				this._slidingAreaRT = this._scrollBarRT.Find("SlidingArea").GetComponent<RectTransform>();
				this._itemsPanelRT = this._scrollPanelRT.Find("Items").GetComponent<RectTransform>();
				this._canvas = base.GetComponentInParent<Canvas>();
				this._canvasRT = this._canvas.GetComponent<RectTransform>();
				this._scrollRect = this._scrollPanelRT.GetComponent<ScrollRect>();
				this._scrollRect.scrollSensitivity = this._rectTransform.sizeDelta.y / 2f;
				this._scrollRect.movementType = ScrollRect.MovementType.Clamped;
				this._scrollRect.content = this._itemsPanelRT;
				this._itemTemplate = this._rectTransform.Find("ItemTemplate").gameObject;
				this._itemTemplate.SetActive(false);
			}
			catch (NullReferenceException exception)
			{
				UnityEngine.Debug.LogException(exception);
				UnityEngine.Debug.LogError("Something is setup incorrectly with the dropdownlist component causing a Null Reference Exception");
				result = false;
			}
			this._panelItems = new List<DropDownListButton>();
			this.RebuildPanel();
			this.RedrawPanel();
			return result;
		}

		private void RebuildPanel()
		{
			if (this.Items.Count == 0)
			{
				return;
			}
			int num = this._panelItems.Count;
			while (this._panelItems.Count < this.Items.Count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._itemTemplate);
				gameObject.name = "Item " + num;
				gameObject.transform.SetParent(this._itemsPanelRT, false);
				this._panelItems.Add(new DropDownListButton(gameObject));
				num++;
			}
			for (int i = 0; i < this._panelItems.Count; i++)
			{
				if (i < this.Items.Count)
				{
					DropDownListItem item = this.Items[i];
					this._panelItems[i].txt.text = item.Caption;
					if (item.IsDisabled)
					{
						this._panelItems[i].txt.color = this.disabledTextColor;
					}
					if (this._panelItems[i].btnImg != null)
					{
						this._panelItems[i].btnImg.sprite = null;
					}
					this._panelItems[i].img.sprite = item.Image;
					this._panelItems[i].img.color = ((item.Image == null) ? new Color(1f, 1f, 1f, 0f) : (item.IsDisabled ? new Color(1f, 1f, 1f, 0.5f) : Color.white));
					int ii = i;
					this._panelItems[i].btn.onClick.RemoveAllListeners();
					this._panelItems[i].btn.onClick.AddListener(delegate()
					{
						this.OnItemClicked(ii);
						if (item.OnSelect != null)
						{
							item.OnSelect();
						}
					});
				}
				this._panelItems[i].gameobject.SetActive(i < this.Items.Count);
			}
		}

		private void OnItemClicked(int indx)
		{
			UnityEngine.Debug.Log("item " + indx + " clicked");
			if (indx != this._selectedIndex && this.OnSelectionChanged != null)
			{
				this.OnSelectionChanged(indx);
			}
			this._selectedIndex = indx;
			this.ToggleDropdownPanel(true);
			this.UpdateSelected();
		}

		private void UpdateSelected()
		{
			this.SelectedItem = ((this._selectedIndex > -1 && this._selectedIndex < this.Items.Count) ? this.Items[this._selectedIndex] : null);
			if (this.SelectedItem == null)
			{
				return;
			}
			if (this.SelectedItem.Image != null)
			{
				this._mainButton.img.sprite = this.SelectedItem.Image;
				this._mainButton.img.color = Color.white;
			}
			else
			{
				this._mainButton.img.sprite = null;
			}
			this._mainButton.txt.text = this.SelectedItem.Caption;
			if (this.OverrideHighlighted)
			{
				for (int i = 0; i < this._itemsPanelRT.childCount; i++)
				{
					this._panelItems[i].btnImg.color = ((this._selectedIndex == i) ? this._mainButton.btn.colors.highlightedColor : new Color(0f, 0f, 0f, 0f));
				}
			}
		}

		private void RedrawPanel()
		{
			float num = (this.Items.Count > this.ItemsToDisplay) ? this._scrollBarWidth : 0f;
			if (!this._hasDrawnOnce || this._rectTransform.sizeDelta != this._mainButton.rectTransform.sizeDelta)
			{
				this._hasDrawnOnce = true;
				this._mainButton.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this._rectTransform.sizeDelta.x);
				this._mainButton.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this._rectTransform.sizeDelta.y);
				this._mainButton.txt.rectTransform.offsetMax = new Vector2(4f, 0f);
				this._scrollPanelRT.SetParent(base.transform, true);
				this._scrollPanelRT.anchoredPosition = new Vector2(0f, -this._rectTransform.sizeDelta.y);
				this._overlayRT.SetParent(this._canvas.transform, false);
				this._overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this._canvasRT.sizeDelta.x);
				this._overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this._canvasRT.sizeDelta.y);
				this._overlayRT.SetParent(base.transform, true);
				this._scrollPanelRT.SetParent(this._overlayRT, true);
			}
			if (this.Items.Count < 1)
			{
				return;
			}
			float num2 = this._rectTransform.sizeDelta.y * (float)Mathf.Min(this._itemsToDisplay, this.Items.Count);
			this._scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
			this._scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this._rectTransform.sizeDelta.x);
			this._itemsPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this._scrollPanelRT.sizeDelta.x - num - 5f);
			this._itemsPanelRT.anchoredPosition = new Vector2(5f, 0f);
			this._scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
			this._scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
			this._slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			this._slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2 - this._scrollBarRT.sizeDelta.x);
		}

		public void ToggleDropdownPanel(bool directClick)
		{
			this._overlayRT.transform.localScale = new Vector3(1f, 1f, 1f);
			this._scrollBarRT.transform.localScale = new Vector3(1f, 1f, 1f);
			this._isPanelActive = !this._isPanelActive;
			this._overlayRT.gameObject.SetActive(this._isPanelActive);
			if (this._isPanelActive)
			{
				base.transform.SetAsLastSibling();
				return;
			}
		}

		public Color disabledTextColor;

		public List<DropDownListItem> Items;

		public Action<int> OnSelectionChanged;

		public bool OverrideHighlighted = true;

		private bool _isPanelActive;

		private bool _hasDrawnOnce;

		private DropDownListButton _mainButton;

		private RectTransform _rectTransform;

		private RectTransform _overlayRT;

		private RectTransform _scrollPanelRT;

		private RectTransform _scrollBarRT;

		private RectTransform _slidingAreaRT;

		private RectTransform _itemsPanelRT;

		private Canvas _canvas;

		private RectTransform _canvasRT;

		private ScrollRect _scrollRect;

		private List<DropDownListButton> _panelItems;

		private GameObject _itemTemplate;

		[SerializeField]
		private float _scrollBarWidth = 20f;

		private int _selectedIndex;

		[SerializeField]
		private int _itemsToDisplay;
	}
}
