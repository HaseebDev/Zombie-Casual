using System;

namespace UnityEngine.UI.Extensions
{
	public class ExampleSelectable : MonoBehaviour, IBoxSelectable
	{
		public bool selected
		{
			get
			{
				return this._selected;
			}
			set
			{
				this._selected = value;
			}
		}

		public bool preSelected
		{
			get
			{
				return this._preSelected;
			}
			set
			{
				this._preSelected = value;
			}
		}

		private void Start()
		{
			this.spriteRenderer = base.transform.GetComponent<SpriteRenderer>();
			this.image = base.transform.GetComponent<Image>();
			this.text = base.transform.GetComponent<Text>();
		}

		private void Update()
		{
			Color color = Color.white;
			if (this.preSelected)
			{
				color = Color.yellow;
			}
			if (this.selected)
			{
				color = Color.green;
			}
			if (this.spriteRenderer)
			{
				this.spriteRenderer.color = color;
				return;
			}
			if (this.text)
			{
				this.text.color = color;
				return;
			}
			if (this.image)
			{
				this.image.color = color;
				return;
			}
			if (base.GetComponent<Renderer>())
			{
				base.GetComponent<Renderer>().material.color = color;
			}
		}



		private bool _selected;

		private bool _preSelected;

		private SpriteRenderer spriteRenderer;

		private Image image;

		private Text text;
	}
}
