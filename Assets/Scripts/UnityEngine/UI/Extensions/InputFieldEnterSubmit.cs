using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(InputField))]
	[AddComponentMenu("UI/Extensions/Input Field Submit")]
	public class InputFieldEnterSubmit : MonoBehaviour
	{
		private void Awake()
		{
			this._input = base.GetComponent<InputField>();
			this._input.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
		}

		public void OnEndEdit(string txt)
		{
			if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				return;
			}
			this.EnterSubmit.Invoke(txt);
		}

		public InputFieldEnterSubmit.EnterSubmitEvent EnterSubmit;

		private InputField _input;

		[Serializable]
		public class EnterSubmitEvent : UnityEvent<string>
		{
		}
	}
}
