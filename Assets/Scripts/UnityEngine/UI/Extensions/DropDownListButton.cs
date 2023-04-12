using System;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform), typeof(Button))]
	public class DropDownListButton
	{
		public DropDownListButton(GameObject btnObj)
		{
			this.gameobject = btnObj;
			this.rectTransform = btnObj.GetComponent<RectTransform>();
			this.btnImg = btnObj.GetComponent<Image>();
			this.btn = btnObj.GetComponent<Button>();
			this.txt = this.rectTransform.Find("Text").GetComponent<Text>();
			this.img = this.rectTransform.Find("Image").GetComponent<Image>();
		}

		public RectTransform rectTransform;

		public Button btn;

		public Text txt;

		public Image btnImg;

		public Image img;

		public GameObject gameobject;
	}
}
