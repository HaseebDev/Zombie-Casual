using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(InputField))]
	[AddComponentMenu("UI/Extensions/Return Key Trigger")]
	public class ReturnKeyTriggersButton : MonoBehaviour, ISubmitHandler, IEventSystemHandler
	{
		private void Start()
		{
			this._system = EventSystem.current;
		}

		private void RemoveHighlight()
		{
			this.button.OnPointerExit(new PointerEventData(this._system));
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (this.highlight)
			{
				this.button.OnPointerEnter(new PointerEventData(this._system));
			}
			this.button.OnPointerClick(new PointerEventData(this._system));
			if (this.highlight)
			{
				base.Invoke("RemoveHighlight", this.highlightDuration);
			}
		}

		private EventSystem _system;

		public Button button;

		private bool highlight = true;

		public float highlightDuration = 0.2f;
	}
}
