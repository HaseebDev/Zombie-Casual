using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Extensions/Extensions Toggle Group")]
	[DisallowMultipleComponent]
	public class ExtensionsToggleGroup : UIBehaviour
	{
		public bool AllowSwitchOff
		{
			get
			{
				return this.m_AllowSwitchOff;
			}
			set
			{
				this.m_AllowSwitchOff = value;
			}
		}

		public ExtensionsToggle SelectedToggle { get; private set; }

		protected ExtensionsToggleGroup()
		{
		}

		private void ValidateToggleIsInGroup(ExtensionsToggle toggle)
		{
			if (toggle == null || !this.m_Toggles.Contains(toggle))
			{
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[]
				{
					toggle,
					this
				}));
			}
		}

		public void NotifyToggleOn(ExtensionsToggle toggle)
		{
			this.ValidateToggleIsInGroup(toggle);
			for (int i = 0; i < this.m_Toggles.Count; i++)
			{
				if (this.m_Toggles[i] == toggle)
				{
					this.SelectedToggle = toggle;
				}
				else
				{
					this.m_Toggles[i].IsOn = false;
				}
			}
			this.onToggleGroupChanged.Invoke(this.AnyTogglesOn());
		}

		public void UnregisterToggle(ExtensionsToggle toggle)
		{
			if (this.m_Toggles.Contains(toggle))
			{
				this.m_Toggles.Remove(toggle);
				toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.NotifyToggleChanged));
			}
		}

		private void NotifyToggleChanged(bool isOn)
		{
			this.onToggleGroupToggleChanged.Invoke(isOn);
		}

		public void RegisterToggle(ExtensionsToggle toggle)
		{
			if (!this.m_Toggles.Contains(toggle))
			{
				this.m_Toggles.Add(toggle);
				toggle.onValueChanged.AddListener(new UnityAction<bool>(this.NotifyToggleChanged));
			}
		}

		public bool AnyTogglesOn()
		{
			return this.m_Toggles.Find((ExtensionsToggle x) => x.IsOn) != null;
		}

		public IEnumerable<ExtensionsToggle> ActiveToggles()
		{
			return from x in this.m_Toggles
			where x.IsOn
			select x;
		}

		public void SetAllTogglesOff()
		{
			bool allowSwitchOff = this.m_AllowSwitchOff;
			this.m_AllowSwitchOff = true;
			for (int i = 0; i < this.m_Toggles.Count; i++)
			{
				this.m_Toggles[i].IsOn = false;
			}
			this.m_AllowSwitchOff = allowSwitchOff;
		}

		public void HasTheGroupToggle(bool value)
		{
			UnityEngine.Debug.Log("Testing, the group has toggled [" + value.ToString() + "]");
		}

		public void HasAToggleFlipped(bool value)
		{
			UnityEngine.Debug.Log("Testing, a toggle has toggled [" + value.ToString() + "]");
		}

		[SerializeField]
		private bool m_AllowSwitchOff;

		private List<ExtensionsToggle> m_Toggles = new List<ExtensionsToggle>();

		public ExtensionsToggleGroup.ToggleGroupEvent onToggleGroupChanged = new ExtensionsToggleGroup.ToggleGroupEvent();

		public ExtensionsToggleGroup.ToggleGroupEvent onToggleGroupToggleChanged = new ExtensionsToggleGroup.ToggleGroupEvent();

		[Serializable]
		public class ToggleGroupEvent : UnityEvent<bool>
		{
		}
	}
}
