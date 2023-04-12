using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(Image))]
	public class ColorImage : MonoBehaviour
	{
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
			this.picker.onValueChanged.AddListener(new UnityAction<Color>(this.ColorChanged));
		}

		private void OnDestroy()
		{
			this.picker.onValueChanged.RemoveListener(new UnityAction<Color>(this.ColorChanged));
		}

		private void ColorChanged(Color newColor)
		{
			this.image.color = newColor;
		}

		public ColorPickerControl picker;

		private Image image;
	}
}
