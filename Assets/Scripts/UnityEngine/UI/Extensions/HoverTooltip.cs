using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/HoverTooltip")]
	public class HoverTooltip : MonoBehaviour
	{
		private void Start()
		{
			this.GUICamera = GameObject.Find("GUICamera").GetComponent<Camera>();
			this.GUIMode = base.transform.parent.parent.GetComponent<Canvas>().renderMode;
			this.bgImageSource = this.bgImage.GetComponent<Image>();
			this.inside = false;
			this.HideTooltipVisibility();
			base.transform.parent.gameObject.SetActive(false);
		}

		public void SetTooltip(string text)
		{
			this.NewTooltip();
			this.thisText.text = text;
			this.OnScreenSpaceCamera();
		}

		public void SetTooltip(string[] texts)
		{
			this.NewTooltip();
			string text = "";
			int num = 0;
			foreach (string text2 in texts)
			{
				if (num == 0)
				{
					text += text2;
				}
				else
				{
					text = text + "\n" + text2;
				}
				num++;
			}
			this.thisText.text = text;
			this.OnScreenSpaceCamera();
		}

		public void SetTooltip(string text, bool test)
		{
			this.NewTooltip();
			this.thisText.text = text;
			this.OnScreenSpaceCamera();
		}

		public void OnScreenSpaceCamera()
		{
			Vector3 vector = this.GUICamera.ScreenToViewportPoint(UnityEngine.Input.mousePosition);
			float num = this.GUICamera.ViewportToScreenPoint(vector).x + this.tooltipRealWidth * this.bgImage.pivot.x;
			if (num > this.upperRight.x)
			{
				float num2 = this.upperRight.x - num;
				float num3;
				if ((double)num2 > (double)this.defaultXOffset * 0.75)
				{
					num3 = num2;
				}
				else
				{
					num3 = this.defaultXOffset - this.tooltipRealWidth * 2f;
				}
				Vector3 position = new Vector3(this.GUICamera.ViewportToScreenPoint(vector).x + num3, 0f, 0f);
				vector.x = this.GUICamera.ScreenToViewportPoint(position).x;
			}
			num = this.GUICamera.ViewportToScreenPoint(vector).x - this.tooltipRealWidth * this.bgImage.pivot.x;
			if (num < this.lowerLeft.x)
			{
				float num4 = this.lowerLeft.x - num;
				float num3;
				if ((double)num4 < (double)this.defaultXOffset * 0.75 - (double)this.tooltipRealWidth)
				{
					num3 = -num4;
				}
				else
				{
					num3 = this.tooltipRealWidth * 2f;
				}
				Vector3 position2 = new Vector3(this.GUICamera.ViewportToScreenPoint(vector).x - num3, 0f, 0f);
				vector.x = this.GUICamera.ScreenToViewportPoint(position2).x;
			}
			num = this.GUICamera.ViewportToScreenPoint(vector).y - (this.bgImage.sizeDelta.y * this.currentYScaleFactor * this.bgImage.pivot.y - this.tooltipRealHeight);
			if (num > this.upperRight.y)
			{
				float num5 = this.upperRight.y - num;
				float num6 = this.bgImage.sizeDelta.y * this.currentYScaleFactor * this.bgImage.pivot.y;
				if ((double)num5 > (double)this.defaultYOffset * 0.75)
				{
					num6 = num5;
				}
				else
				{
					num6 = this.defaultYOffset - this.tooltipRealHeight * 2f;
				}
				Vector3 position3 = new Vector3(vector.x, this.GUICamera.ViewportToScreenPoint(vector).y + num6, 0f);
				vector.y = this.GUICamera.ScreenToViewportPoint(position3).y;
			}
			num = this.GUICamera.ViewportToScreenPoint(vector).y - this.bgImage.sizeDelta.y * this.currentYScaleFactor * this.bgImage.pivot.y;
			if (num < this.lowerLeft.y)
			{
				float num7 = this.lowerLeft.y - num;
				float num6 = this.bgImage.sizeDelta.y * this.currentYScaleFactor * this.bgImage.pivot.y;
				if ((double)num7 < (double)this.defaultYOffset * 0.75 - (double)this.tooltipRealHeight)
				{
					num6 = num7;
				}
				else
				{
					num6 = this.tooltipRealHeight * 2f;
				}
				Vector3 position4 = new Vector3(vector.x, this.GUICamera.ViewportToScreenPoint(vector).y + num6, 0f);
				vector.y = this.GUICamera.ScreenToViewportPoint(position4).y;
			}
			base.transform.parent.transform.position = new Vector3(this.GUICamera.ViewportToWorldPoint(vector).x, this.GUICamera.ViewportToWorldPoint(vector).y, 0f);
			base.transform.parent.gameObject.SetActive(true);
			this.inside = true;
		}

		public void HideTooltip()
		{
			if (this.GUIMode == RenderMode.ScreenSpaceCamera && this != null)
			{
				base.transform.parent.gameObject.SetActive(false);
				this.inside = false;
				this.HideTooltipVisibility();
			}
		}

		private void Update()
		{
			this.LayoutInit();
			if (this.inside && this.GUIMode == RenderMode.ScreenSpaceCamera)
			{
				this.OnScreenSpaceCamera();
			}
		}

		private void LayoutInit()
		{
			if (this.firstUpdate)
			{
				this.firstUpdate = false;
				this.bgImage.sizeDelta = new Vector2(this.hlG.preferredWidth + (float)this.horizontalPadding, this.hlG.preferredHeight + (float)this.verticalPadding);
				this.defaultYOffset = this.bgImage.sizeDelta.y * this.currentYScaleFactor * this.bgImage.pivot.y;
				this.defaultXOffset = this.bgImage.sizeDelta.x * this.currentXScaleFactor * this.bgImage.pivot.x;
				this.tooltipRealHeight = this.bgImage.sizeDelta.y * this.currentYScaleFactor;
				this.tooltipRealWidth = this.bgImage.sizeDelta.x * this.currentXScaleFactor;
				this.ActivateTooltipVisibility();
			}
		}

		private void NewTooltip()
		{
			this.firstUpdate = true;
			this.lowerLeft = this.GUICamera.ViewportToScreenPoint(new Vector3(0f, 0f, 0f));
			this.upperRight = this.GUICamera.ViewportToScreenPoint(new Vector3(1f, 1f, 0f));
			this.currentYScaleFactor = (float)Screen.height / base.transform.root.GetComponent<CanvasScaler>().referenceResolution.y;
			this.currentXScaleFactor = (float)Screen.width / base.transform.root.GetComponent<CanvasScaler>().referenceResolution.x;
		}

		public void ActivateTooltipVisibility()
		{
			Color color = this.thisText.color;
			this.thisText.color = new Color(color.r, color.g, color.b, 1f);
			this.bgImageSource.color = new Color(this.bgImageSource.color.r, this.bgImageSource.color.g, this.bgImageSource.color.b, 0.8f);
		}

		public void HideTooltipVisibility()
		{
			Color color = this.thisText.color;
			this.thisText.color = new Color(color.r, color.g, color.b, 0f);
			this.bgImageSource.color = new Color(this.bgImageSource.color.r, this.bgImageSource.color.g, this.bgImageSource.color.b, 0f);
		}

		public int horizontalPadding;

		public int verticalPadding;

		public Text thisText;

		public HorizontalLayoutGroup hlG;

		public RectTransform bgImage;

		private Image bgImageSource;

		private bool firstUpdate;

		private bool inside;

		private RenderMode GUIMode;

		private Camera GUICamera;

		private Vector3 lowerLeft;

		private Vector3 upperRight;

		private float currentYScaleFactor;

		private float currentXScaleFactor;

		private float defaultYOffset;

		private float defaultXOffset;

		private float tooltipRealHeight;

		private float tooltipRealWidth;
	}
}
