using System;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerControl : MonoBehaviour
	{
		public Color CurrentColor
		{
			get
			{
				return new Color(this._red, this._green, this._blue, this._alpha);
			}
			set
			{
				if (this.CurrentColor == value)
				{
					return;
				}
				this._red = value.r;
				this._green = value.g;
				this._blue = value.b;
				this._alpha = value.a;
				this.RGBChanged();
				this.SendChangedEvent();
			}
		}

		private void Start()
		{
			this.SendChangedEvent();
		}

		public float H
		{
			get
			{
				return this._hue;
			}
			set
			{
				if (this._hue == value)
				{
					return;
				}
				this._hue = value;
				this.HSVChanged();
				this.SendChangedEvent();
			}
		}

		public float S
		{
			get
			{
				return this._saturation;
			}
			set
			{
				if (this._saturation == value)
				{
					return;
				}
				this._saturation = value;
				this.HSVChanged();
				this.SendChangedEvent();
			}
		}

		public float V
		{
			get
			{
				return this._brightness;
			}
			set
			{
				if (this._brightness == value)
				{
					return;
				}
				this._brightness = value;
				this.HSVChanged();
				this.SendChangedEvent();
			}
		}

		public float R
		{
			get
			{
				return this._red;
			}
			set
			{
				if (this._red == value)
				{
					return;
				}
				this._red = value;
				this.RGBChanged();
				this.SendChangedEvent();
			}
		}

		public float G
		{
			get
			{
				return this._green;
			}
			set
			{
				if (this._green == value)
				{
					return;
				}
				this._green = value;
				this.RGBChanged();
				this.SendChangedEvent();
			}
		}

		public float B
		{
			get
			{
				return this._blue;
			}
			set
			{
				if (this._blue == value)
				{
					return;
				}
				this._blue = value;
				this.RGBChanged();
				this.SendChangedEvent();
			}
		}

		private float A
		{
			get
			{
				return this._alpha;
			}
			set
			{
				if (this._alpha == value)
				{
					return;
				}
				this._alpha = value;
				this.SendChangedEvent();
			}
		}

		private void RGBChanged()
		{
			HsvColor hsvColor = HSVUtil.ConvertRgbToHsv(this.CurrentColor);
			this._hue = hsvColor.NormalizedH;
			this._saturation = hsvColor.NormalizedS;
			this._brightness = hsvColor.NormalizedV;
		}

		private void HSVChanged()
		{
			Color color = HSVUtil.ConvertHsvToRgb((double)(this._hue * 360f), (double)this._saturation, (double)this._brightness, this._alpha);
			this._red = color.r;
			this._green = color.g;
			this._blue = color.b;
		}

		private void SendChangedEvent()
		{
			this.onValueChanged.Invoke(this.CurrentColor);
			this.onHSVChanged.Invoke(this._hue, this._saturation, this._brightness);
		}

		public void AssignColor(ColorValues type, float value)
		{
			switch (type)
			{
			case ColorValues.R:
				this.R = value;
				return;
			case ColorValues.G:
				this.G = value;
				return;
			case ColorValues.B:
				this.B = value;
				return;
			case ColorValues.A:
				this.A = value;
				return;
			case ColorValues.Hue:
				this.H = value;
				return;
			case ColorValues.Saturation:
				this.S = value;
				return;
			case ColorValues.Value:
				this.V = value;
				return;
			default:
				return;
			}
		}

		public float GetValue(ColorValues type)
		{
			switch (type)
			{
			case ColorValues.R:
				return this.R;
			case ColorValues.G:
				return this.G;
			case ColorValues.B:
				return this.B;
			case ColorValues.A:
				return this.A;
			case ColorValues.Hue:
				return this.H;
			case ColorValues.Saturation:
				return this.S;
			case ColorValues.Value:
				return this.V;
			default:
				throw new NotImplementedException("");
			}
		}

		private float _hue;

		private float _saturation;

		private float _brightness;

		private float _red;

		private float _green;

		private float _blue;

		private float _alpha = 1f;

		public ColorChangedEvent onValueChanged = new ColorChangedEvent();

		public HSVChangedEvent onHSVChanged = new HSVChangedEvent();
	}
}
