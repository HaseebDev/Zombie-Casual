using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Views
{
	public class FillingImageView : BaseFloatValueView
	{
		public override void SetNormalizedValue(float value)
		{
			this._image.fillAmount = value;
		}

		[SerializeField]
		private Image _image;
	}
}
