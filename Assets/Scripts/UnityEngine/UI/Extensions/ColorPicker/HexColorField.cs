using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(InputField))]
	public class HexColorField : MonoBehaviour
	{
		private void Awake()
		{
			this.hexInputField = base.GetComponent<InputField>();
			this.hexInputField.onEndEdit.AddListener(new UnityAction<string>(this.UpdateColor));
			this.ColorPicker.onValueChanged.AddListener(new UnityAction<Color>(this.UpdateHex));
		}

		private void OnDestroy()
		{
			this.hexInputField.onValueChanged.RemoveListener(new UnityAction<string>(this.UpdateColor));
			this.ColorPicker.onValueChanged.RemoveListener(new UnityAction<Color>(this.UpdateHex));
		}

		private void UpdateHex(Color newColor)
		{
			this.hexInputField.text = this.ColorToHex(newColor);
		}

		private void UpdateColor(string newHex)
		{
			Color32 c;
			if (HexColorField.HexToColor(newHex, out c))
			{
				this.ColorPicker.CurrentColor = c;
				return;
			}
			UnityEngine.Debug.Log("hex value is in the wrong format, valid formats are: #RGB, #RGBA, #RRGGBB and #RRGGBBAA (# is optional)");
		}

		private string ColorToHex(Color32 color)
		{
			if (this.displayAlpha)
			{
				return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[]
				{
					color.r,
					color.g,
					color.b,
					color.a
				});
			}
			return string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
		}

		public static bool HexToColor(string hex, out Color32 color)
		{
			if (Regex.IsMatch(hex, "^#?(?:[0-9a-fA-F]{3,4}){1,2}$"))
			{
				int num = hex.StartsWith("#") ? 1 : 0;
				if (hex.Length == num + 8)
				{
					color = new Color32(byte.Parse(hex.Substring(num, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 2, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 4, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 6, 2), NumberStyles.AllowHexSpecifier));
				}
				else if (hex.Length == num + 6)
				{
					color = new Color32(byte.Parse(hex.Substring(num, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 2, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 4, 2), NumberStyles.AllowHexSpecifier), byte.MaxValue);
				}
				else if (hex.Length == num + 4)
				{
					color = new Color32(byte.Parse(hex[num].ToString() + hex[num].ToString(), NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 1].ToString() + hex[num + 1].ToString(), NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 2].ToString() + hex[num + 2].ToString(), NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 3].ToString() + hex[num + 3].ToString(), NumberStyles.AllowHexSpecifier));
				}
				else
				{
					color = new Color32(byte.Parse(hex[num].ToString() + hex[num].ToString(), NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 1].ToString() + hex[num + 1].ToString(), NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 2].ToString() + hex[num + 2].ToString(), NumberStyles.AllowHexSpecifier), byte.MaxValue);
				}
				return true;
			}
			color = default(Color32);
			return false;
		}

		public ColorPickerControl ColorPicker;

		public bool displayAlpha;

		private InputField hexInputField;

		private const string hexRegex = "^#?(?:[0-9a-fA-F]{3,4}){1,2}$";
	}
}
