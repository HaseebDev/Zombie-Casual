using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(BoxSlider), typeof(RawImage))]
	[ExecuteInEditMode]
	public class SVBoxSlider : MonoBehaviour
	{
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		private void Awake()
		{
			this.slider = base.GetComponent<BoxSlider>();
			this.image = base.GetComponent<RawImage>();
			this.RegenerateSVTexture();
		}

		private void OnEnable()
		{
			if (Application.isPlaying && this.picker != null)
			{
				this.slider.OnValueChanged.AddListener(new UnityAction<float, float>(this.SliderChanged));
				this.picker.onHSVChanged.AddListener(new UnityAction<float, float, float>(this.HSVChanged));
			}
		}

		private void OnDisable()
		{
			if (this.picker != null)
			{
				this.slider.OnValueChanged.RemoveListener(new UnityAction<float, float>(this.SliderChanged));
				this.picker.onHSVChanged.RemoveListener(new UnityAction<float, float, float>(this.HSVChanged));
			}
		}

		private void OnDestroy()
		{
			if (this.image.texture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.image.texture);
			}
		}

		private void SliderChanged(float saturation, float value)
		{
			if (this.listen)
			{
				this.picker.AssignColor(ColorValues.Saturation, saturation);
				this.picker.AssignColor(ColorValues.Value, value);
			}
			this.listen = true;
		}

		private void HSVChanged(float h, float s, float v)
		{
			if (this.lastH != h)
			{
				this.lastH = h;
				this.RegenerateSVTexture();
			}
			if (s != this.slider.NormalizedValueX)
			{
				this.listen = false;
				this.slider.NormalizedValueX = s;
			}
			if (v != this.slider.NormalizedValueY)
			{
				this.listen = false;
				this.slider.NormalizedValueY = v;
			}
		}

		private void RegenerateSVTexture()
		{
			double h = (double)((this.picker != null) ? (this.picker.H * 360f) : 0f);
			if (this.image.texture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.image.texture);
			}
			Texture2D texture2D = new Texture2D(100, 100)
			{
				hideFlags = HideFlags.DontSave
			};
			for (int i = 0; i < 100; i++)
			{
				Color32[] array = new Color32[100];
				for (int j = 0; j < 100; j++)
				{
					array[j] = HSVUtil.ConvertHsvToRgb(h, (double)((float)i / 100f), (double)((float)j / 100f), 1f);
				}
				texture2D.SetPixels32(i, 0, 1, 100, array);
			}
			texture2D.Apply();
			this.image.texture = texture2D;
		}

		public ColorPickerControl picker;

		private BoxSlider slider;

		private RawImage image;

		private float lastH = -1f;

		private bool listen = true;
	}
}
