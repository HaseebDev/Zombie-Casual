using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(Text))]
	public class ColorLabel : MonoBehaviour
	{
		private void Awake()
		{
			this.label = base.GetComponent<Text>();
		}

		private void OnEnable()
		{
			if (Application.isPlaying && this.picker != null)
			{
				this.picker.onValueChanged.AddListener(new UnityAction<Color>(this.ColorChanged));
				this.picker.onHSVChanged.AddListener(new UnityAction<float, float, float>(this.HSVChanged));
			}
		}

		private void OnDestroy()
		{
			if (this.picker != null)
			{
				this.picker.onValueChanged.RemoveListener(new UnityAction<Color>(this.ColorChanged));
				this.picker.onHSVChanged.RemoveListener(new UnityAction<float, float, float>(this.HSVChanged));
			}
		}

		private void ColorChanged(Color color)
		{
			this.UpdateValue();
		}

		private void HSVChanged(float hue, float sateration, float value)
		{
			this.UpdateValue();
		}

		private void UpdateValue()
		{
			if (this.picker == null)
			{
				this.label.text = this.prefix + "-";
				return;
			}
			float value = this.minValue + this.picker.GetValue(this.type) * (this.maxValue - this.minValue);
			this.label.text = this.prefix + this.ConvertToDisplayString(value);
		}

		private string ConvertToDisplayString(float value)
		{
			if (this.precision > 0)
			{
				return value.ToString("f " + this.precision);
			}
			return Mathf.FloorToInt(value).ToString();
		}

		public ColorPickerControl picker;

		public ColorValues type;

		public string prefix = "R: ";

		public float minValue;

		public float maxValue = 255f;

		public int precision;

		private Text label;
	}
}
