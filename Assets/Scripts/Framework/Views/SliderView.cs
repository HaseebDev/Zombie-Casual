using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Views
{
	public class SliderView : BaseFloatValueView
	{
		public override void SetNormalizedValue(float value)
		{
			this._slider.value = (this._slider.maxValue - this._slider.minValue) * value;
		}

		[SerializeField]
		private Slider _slider;
	}
}
