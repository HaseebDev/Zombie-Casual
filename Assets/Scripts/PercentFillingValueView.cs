using System;
using Framework.Views;
using UnityEngine;
using UnityEngine.UI;

public class PercentFillingValueView : BaseFloatValueView
{
	public override void SetNormalizedValue(float value)
	{
		this.textField.text = string.Format("{0:0}%", value * 100f);
	}

	[SerializeField]
	private Text textField;
}
