using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Item")]
	public class BoundTooltipItem : MonoBehaviour
	{
		public bool IsActive
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		private void Awake()
		{
			BoundTooltipItem.instance = this;
			if (!this.TooltipText)
			{
				this.TooltipText = base.GetComponentInChildren<Text>();
			}
			this.HideTooltip();
		}

		public void ShowTooltip(string text, Vector3 pos)
		{
			if (this.TooltipText.text != text)
			{
				this.TooltipText.text = text;
			}
			base.transform.position = pos + this.ToolTipOffset;
			base.gameObject.SetActive(true);
		}

		public void HideTooltip()
		{
			base.gameObject.SetActive(false);
		}

		public static BoundTooltipItem Instance
		{
			get
			{
				if (BoundTooltipItem.instance == null)
				{
					BoundTooltipItem.instance = UnityEngine.Object.FindObjectOfType<BoundTooltipItem>();
				}
				return BoundTooltipItem.instance;
			}
		}

		public Text TooltipText;

		public Vector3 ToolTipOffset;

		private static BoundTooltipItem instance;
	}
}
