using System;
using UnityEngine;

namespace Framework.Views
{
	public abstract class BaseFloatValueView : MonoBehaviour
	{
		public abstract void SetNormalizedValue(float value);
	}
}
