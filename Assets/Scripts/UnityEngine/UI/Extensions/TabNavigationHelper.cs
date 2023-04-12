using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(EventSystem))]
	[AddComponentMenu("Event/Extensions/Tab Navigation Helper")]
	public class TabNavigationHelper : MonoBehaviour
	{
		private void Start()
		{
			this._system = base.GetComponent<EventSystem>();
			if (this._system == null)
			{
				UnityEngine.Debug.LogError("Needs to be attached to the Event System component in the scene");
			}
		}

		public void Update()
		{
			Selectable selectable = null;
			if (UnityEngine.Input.GetKeyDown(KeyCode.Tab) && UnityEngine.Input.GetKey(KeyCode.LeftShift))
			{
				if (this._system.currentSelectedGameObject != null)
				{
					selectable = this._system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
				}
				else
				{
					selectable = this._system.firstSelectedGameObject.GetComponent<Selectable>();
				}
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
			{
				if (this._system.currentSelectedGameObject != null)
				{
					selectable = this._system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
				}
				else
				{
					selectable = this._system.firstSelectedGameObject.GetComponent<Selectable>();
				}
			}
			else if (this.NavigationMode == NavigationMode.Manual)
			{
				for (int i = 0; i < this.NavigationPath.Length; i++)
				{
					if (!(this._system.currentSelectedGameObject != this.NavigationPath[i].gameObject))
					{
						selectable = ((i == this.NavigationPath.Length - 1) ? this.NavigationPath[0] : this.NavigationPath[i + 1]);
						break;
					}
				}
			}
			else if (this._system.currentSelectedGameObject == null)
			{
				selectable = this._system.firstSelectedGameObject.GetComponent<Selectable>();
			}
			this.selectGameObject(selectable);
		}

		private void selectGameObject(Selectable selectable)
		{
			if (selectable != null)
			{
				InputField component = selectable.GetComponent<InputField>();
				if (component != null)
				{
					component.OnPointerClick(new PointerEventData(this._system));
				}
				this._system.SetSelectedGameObject(selectable.gameObject, new BaseEventData(this._system));
			}
		}

		private EventSystem _system;

		[Tooltip("The path to take when user is tabbing through ui components.")]
		public Selectable[] NavigationPath;

		[Tooltip("Use the default Unity navigation system or a manual fixed order using Navigation Path")]
		public NavigationMode NavigationMode;
	}
}
