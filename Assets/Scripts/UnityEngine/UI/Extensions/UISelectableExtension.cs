using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Selectable Extension")]
	[RequireComponent(typeof(Selectable))]
	public class UISelectableExtension : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (this.OnButtonPress != null)
			{
				this.OnButtonPress.Invoke(eventData.button);
			}
			this._pressed = true;
			this._heldEventData = eventData;
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.OnButtonRelease != null)
			{
				this.OnButtonRelease.Invoke(eventData.button);
			}
			this._pressed = false;
			this._heldEventData = null;
		}

		private void Update()
		{
			if (!this._pressed)
			{
				return;
			}
			if (this.OnButtonHeld != null)
			{
				this.OnButtonHeld.Invoke(this._heldEventData.button);
			}
		}

		public void TestClicked()
		{
		}

		public void TestPressed()
		{
		}

		public void TestReleased()
		{
		}

		public void TestHold()
		{
		}

		private void OnDisable()
		{
			this._pressed = false;
		}

		[Tooltip("Event that fires when a button is initially pressed down")]
		public UISelectableExtension.UIButtonEvent OnButtonPress;

		[Tooltip("Event that fires when a button is released")]
		public UISelectableExtension.UIButtonEvent OnButtonRelease;

		[Tooltip("Event that continually fires while a button is held down")]
		public UISelectableExtension.UIButtonEvent OnButtonHeld;

		private bool _pressed;

		private PointerEventData _heldEventData;

		[Serializable]
		public class UIButtonEvent : UnityEvent<PointerEventData.InputButton>
		{
		}
	}
}
