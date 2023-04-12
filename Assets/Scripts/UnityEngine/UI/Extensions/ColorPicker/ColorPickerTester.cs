using System;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerTester : MonoBehaviour
	{
		private void Awake()
		{
			this.pickerRenderer = base.GetComponent<Renderer>();
		}

		private void Start()
		{
			this.picker.onValueChanged.AddListener(delegate(Color color)
			{
				this.pickerRenderer.material.color = color;
			});
		}

		public Renderer pickerRenderer;

		public ColorPickerControl picker;
	}
}
