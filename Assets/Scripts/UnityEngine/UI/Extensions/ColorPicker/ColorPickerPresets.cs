using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerPresets : MonoBehaviour
	{
		private void Awake()
		{
			this.picker.onValueChanged.AddListener(new UnityAction<Color>(this.ColorChanged));
		}

		public void CreatePresetButton()
		{
			for (int i = 0; i < this.presets.Length; i++)
			{
				if (!this.presets[i].activeSelf)
				{
					this.presets[i].SetActive(true);
					this.presets[i].GetComponent<Image>().color = this.picker.CurrentColor;
					return;
				}
			}
		}

		public void PresetSelect(Image sender)
		{
			this.picker.CurrentColor = sender.color;
		}

		private void ColorChanged(Color color)
		{
			this.createPresetImage.color = color;
		}

		public ColorPickerControl picker;

		public GameObject[] presets;

		public Image createPresetImage;
	}
}
