using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/TextPic")]
	[ExecuteInEditMode]
	public class TextPic : Text, IPointerClickHandler, IEventSystemHandler, IPointerExitHandler, IPointerEnterHandler, ISelectHandler
	{
		public override void SetVerticesDirty()
		{
			base.SetVerticesDirty();
			this.UpdateQuadImage();
		}

		private new void Start()
		{
			this.button = base.GetComponent<Button>();
			if (this.inspectorIconList != null && this.inspectorIconList.Length != 0)
			{
				foreach (TextPic.IconName iconName in this.inspectorIconList)
				{
					this.iconList.Add(iconName.name, iconName.sprite);
				}
			}
			this.Reset_m_HrefInfos();
		}

		protected void UpdateQuadImage()
		{
			this.m_OutputText = this.GetOutputText();
			this.m_ImagesVertexIndex.Clear();
			foreach (object obj in TextPic.s_Regex.Matches(this.m_OutputText))
			{
				Match match = (Match)obj;
				int item = match.Index * 4 + 3;
				this.m_ImagesVertexIndex.Add(item);
				this.m_ImagesPool.RemoveAll((Image image) => image == null);
				if (this.m_ImagesPool.Count == 0)
				{
					base.GetComponentsInChildren<Image>(this.m_ImagesPool);
				}
				if (this.m_ImagesVertexIndex.Count > this.m_ImagesPool.Count)
				{
					GameObject gameObject = DefaultControls.CreateImage(default(DefaultControls.Resources));
					gameObject.layer = base.gameObject.layer;
					RectTransform rectTransform = gameObject.transform as RectTransform;
					if (rectTransform)
					{
						rectTransform.SetParent(base.rectTransform);
						rectTransform.localPosition = Vector3.zero;
						rectTransform.localRotation = Quaternion.identity;
						rectTransform.localScale = Vector3.one;
					}
					this.m_ImagesPool.Add(gameObject.GetComponent<Image>());
				}
				string value = match.Groups[1].Value;
				Image image2 = this.m_ImagesPool[this.m_ImagesVertexIndex.Count - 1];
				if ((image2.sprite == null || image2.sprite.name != value) && this.inspectorIconList != null && this.inspectorIconList.Length != 0)
				{
					foreach (TextPic.IconName iconName in this.inspectorIconList)
					{
						if (iconName.name == value)
						{
							image2.sprite = iconName.sprite;
							break;
						}
					}
				}
				image2.rectTransform.sizeDelta = new Vector2((float)base.fontSize * this.ImageScalingFactor, (float)base.fontSize * this.ImageScalingFactor);
				image2.enabled = true;
				if (this.positions.Count == this.m_ImagesPool.Count)
				{
					image2.rectTransform.anchoredPosition = this.positions[this.m_ImagesVertexIndex.Count - 1];
				}
			}
			for (int j = this.m_ImagesVertexIndex.Count; j < this.m_ImagesPool.Count; j++)
			{
				if (this.m_ImagesPool[j])
				{
					this.m_ImagesPool[j].gameObject.SetActive(false);
					this.m_ImagesPool[j].gameObject.hideFlags = HideFlags.HideAndDontSave;
					this.culled_ImagesPool.Add(this.m_ImagesPool[j].gameObject);
					this.m_ImagesPool.Remove(this.m_ImagesPool[j]);
				}
			}
			if (this.culled_ImagesPool.Count > 1)
			{
				this.clearImages = true;
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			string text = this.m_Text;
			this.m_Text = this.GetOutputText();
			base.OnPopulateMesh(toFill);
			this.m_Text = text;
			this.positions.Clear();
			UIVertex uivertex = default(UIVertex);
			for (int i = 0; i < this.m_ImagesVertexIndex.Count; i++)
			{
				int num = this.m_ImagesVertexIndex[i];
				Vector2 sizeDelta = this.m_ImagesPool[i].rectTransform.sizeDelta;
				if (num < toFill.currentVertCount)
				{
					toFill.PopulateUIVertex(ref uivertex, num);
					this.positions.Add(new Vector2(uivertex.position.x + sizeDelta.x / 2f, uivertex.position.y + sizeDelta.y / 2f) + this.imageOffset);
					toFill.PopulateUIVertex(ref uivertex, num - 3);
					Vector3 position = uivertex.position;
					int j = num;
					int num2 = num - 3;
					while (j > num2)
					{
						toFill.PopulateUIVertex(ref uivertex, num);
						uivertex.position = position;
						toFill.SetUIVertex(uivertex, j);
						j--;
					}
				}
			}
			if (this.m_ImagesVertexIndex.Count != 0)
			{
				this.m_ImagesVertexIndex.Clear();
			}
			foreach (TextPic.HrefInfo hrefInfo in this.m_HrefInfos)
			{
				hrefInfo.boxes.Clear();
				if (hrefInfo.startIndex < toFill.currentVertCount)
				{
					toFill.PopulateUIVertex(ref uivertex, hrefInfo.startIndex);
					Vector3 position2 = uivertex.position;
					Bounds bounds = new Bounds(position2, Vector3.zero);
					int num3 = hrefInfo.startIndex;
					int endIndex = hrefInfo.endIndex;
					while (num3 < endIndex && num3 < toFill.currentVertCount)
					{
						toFill.PopulateUIVertex(ref uivertex, num3);
						position2 = uivertex.position;
						if (position2.x < bounds.min.x)
						{
							hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
							bounds = new Bounds(position2, Vector3.zero);
						}
						else
						{
							bounds.Encapsulate(position2);
						}
						num3++;
					}
					hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
				}
			}
			this.UpdateQuadImage();
		}

		public TextPic.HrefClickEvent onHrefClick
		{
			get
			{
				return this.m_OnHrefClick;
			}
			set
			{
				this.m_OnHrefClick = value;
			}
		}

		protected string GetOutputText()
		{
			TextPic.s_TextBuilder.Length = 0;
			int num = 0;
			this.fixedString = this.text;
			if (this.inspectorIconList != null && this.inspectorIconList.Length != 0)
			{
				foreach (TextPic.IconName iconName in this.inspectorIconList)
				{
					if (iconName.name != null && iconName.name != "")
					{
						this.fixedString = this.fixedString.Replace(iconName.name, string.Concat(new object[]
						{
							"<quad name=",
							iconName.name,
							" size=",
							base.fontSize,
							" width=1 />"
						}));
					}
				}
			}
			int num2 = 0;
			foreach (object obj in TextPic.s_HrefRegex.Matches(this.fixedString))
			{
				Match match = (Match)obj;
				TextPic.s_TextBuilder.Append(this.fixedString.Substring(num, match.Index - num));
				TextPic.s_TextBuilder.Append("<color=" + this.hyperlinkColor + ">");
				Group group = match.Groups[1];
				if (this.isCreating_m_HrefInfos)
				{
					TextPic.HrefInfo item = new TextPic.HrefInfo
					{
						startIndex = TextPic.s_TextBuilder.Length * 4,
						endIndex = (TextPic.s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
						name = group.Value
					};
					this.m_HrefInfos.Add(item);
				}
				else if (this.m_HrefInfos.Count > 0)
				{
					this.m_HrefInfos[num2].startIndex = TextPic.s_TextBuilder.Length * 4;
					this.m_HrefInfos[num2].endIndex = (TextPic.s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3;
					num2++;
				}
				TextPic.s_TextBuilder.Append(match.Groups[2].Value);
				TextPic.s_TextBuilder.Append("</color>");
				num = match.Index + match.Length;
			}
			if (this.isCreating_m_HrefInfos)
			{
				this.isCreating_m_HrefInfos = false;
			}
			TextPic.s_TextBuilder.Append(this.fixedString.Substring(num, this.fixedString.Length - num));
			return TextPic.s_TextBuilder.ToString();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Vector2 point;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, eventData.position, eventData.pressEventCamera, out point);
			foreach (TextPic.HrefInfo hrefInfo in this.m_HrefInfos)
			{
				List<Rect> boxes = hrefInfo.boxes;
				for (int i = 0; i < boxes.Count; i++)
				{
					if (boxes[i].Contains(point))
					{
						this.m_OnHrefClick.Invoke(hrefInfo.name);
						return;
					}
				}
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_ImagesPool.Count >= 1)
			{
				foreach (Image image in this.m_ImagesPool)
				{
					if (this.button != null && this.button.isActiveAndEnabled)
					{
						image.color = this.button.colors.highlightedColor;
					}
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.m_ImagesPool.Count >= 1)
			{
				foreach (Image image in this.m_ImagesPool)
				{
					if (this.button != null && this.button.isActiveAndEnabled)
					{
						image.color = this.button.colors.normalColor;
					}
					else
					{
						image.color = this.color;
					}
				}
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (this.m_ImagesPool.Count >= 1)
			{
				foreach (Image image in this.m_ImagesPool)
				{
					if (this.button != null && this.button.isActiveAndEnabled)
					{
						image.color = this.button.colors.highlightedColor;
					}
				}
			}
		}

		private void Update()
		{
			Object obj = this.thisLock;
			lock (obj)
			{
				if (this.clearImages)
				{
					for (int i = 0; i < this.culled_ImagesPool.Count; i++)
					{
						UnityEngine.Object.DestroyImmediate(this.culled_ImagesPool[i]);
					}
					this.culled_ImagesPool.Clear();
					this.clearImages = false;
				}
			}
			if (this.previousText != this.text)
			{
				this.Reset_m_HrefInfos();
			}
		}

		private void Reset_m_HrefInfos()
		{
			this.previousText = this.text;
			this.m_HrefInfos.Clear();
			this.isCreating_m_HrefInfos = true;
		}

		private readonly List<Image> m_ImagesPool = new List<Image>();

		private readonly List<GameObject> culled_ImagesPool = new List<GameObject>();

		private bool clearImages;

		private Object thisLock = new Object();

		private readonly List<int> m_ImagesVertexIndex = new List<int>();

		private static readonly Regex s_Regex = new Regex("<quad name=(.+?) size=(\\d*\\.?\\d+%?) width=(\\d*\\.?\\d+%?) />", RegexOptions.Singleline);

		private string fixedString;

		private string m_OutputText;

		public TextPic.IconName[] inspectorIconList;

		private Dictionary<string, Sprite> iconList = new Dictionary<string, Sprite>();

		public float ImageScalingFactor = 1f;

		public string hyperlinkColor = "blue";

		[SerializeField]
		public Vector2 imageOffset = Vector2.zero;

		private Button button;

		private List<Vector2> positions = new List<Vector2>();

		private string previousText = "";

		public bool isCreating_m_HrefInfos = true;

		private readonly List<TextPic.HrefInfo> m_HrefInfos = new List<TextPic.HrefInfo>();

		private static readonly StringBuilder s_TextBuilder = new StringBuilder();

		private static readonly Regex s_HrefRegex = new Regex("<a href=([^>\\n\\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

		[SerializeField]
		private TextPic.HrefClickEvent m_OnHrefClick = new TextPic.HrefClickEvent();

		[Serializable]
		public struct IconName
		{
			public string name;

			public Sprite sprite;
		}

		[Serializable]
		public class HrefClickEvent : UnityEvent<string>
		{
		}

		private class HrefInfo
		{
			public int startIndex;

			public int endIndex;

			public string name;

			public readonly List<Rect> boxes = new List<Rect>();
		}
	}
}
