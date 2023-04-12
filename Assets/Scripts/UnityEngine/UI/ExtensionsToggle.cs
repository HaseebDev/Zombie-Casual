using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Extensions/Extensions Toggle", 31)]
	[RequireComponent(typeof(RectTransform))]
	public class ExtensionsToggle : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICanvasElement
	{
		public ExtensionsToggleGroup Group
		{
			get
			{
				return this.m_Group;
			}
			set
			{
				this.m_Group = value;
				this.SetToggleGroup(this.m_Group, true);
				this.PlayEffect(true);
			}
		}

		protected ExtensionsToggle()
		{
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetToggleGroup(this.m_Group, false);
			this.PlayEffect(true);
		}

		protected override void OnDisable()
		{
			this.SetToggleGroup(null, false);
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			if (this.graphic != null)
			{
				bool flag = !Mathf.Approximately(this.graphic.canvasRenderer.GetColor().a, 0f);
				if (this.m_IsOn != flag)
				{
					this.m_IsOn = flag;
					this.Set(!flag);
				}
			}
			base.OnDidApplyAnimationProperties();
		}

		private void SetToggleGroup(ExtensionsToggleGroup newGroup, bool setMemberValue)
		{
			ExtensionsToggleGroup group = this.m_Group;
			if (this.m_Group != null)
			{
				this.m_Group.UnregisterToggle(this);
			}
			if (setMemberValue)
			{
				this.m_Group = newGroup;
			}
			if (this.m_Group != null && this.IsActive())
			{
				this.m_Group.RegisterToggle(this);
			}
			if (newGroup != null && newGroup != group && this.IsOn && this.IsActive())
			{
				this.m_Group.NotifyToggleOn(this);
			}
		}

		public bool IsOn
		{
			get
			{
				return this.m_IsOn;
			}
			set
			{
				this.Set(value);
			}
		}

		private void Set(bool value)
		{
			this.Set(value, true);
		}

		private void Set(bool value, bool sendCallback)
		{
			if (this.m_IsOn == value)
			{
				return;
			}
			this.m_IsOn = value;
			if (this.m_Group != null && this.IsActive() && (this.m_IsOn || (!this.m_Group.AnyTogglesOn() && !this.m_Group.AllowSwitchOff)))
			{
				this.m_IsOn = true;
				this.m_Group.NotifyToggleOn(this);
			}
			this.PlayEffect(this.toggleTransition == ExtensionsToggle.ToggleTransition.None);
			if (sendCallback)
			{
				this.onValueChanged.Invoke(this.m_IsOn);
				this.onToggleChanged.Invoke(this);
			}
		}

		private void PlayEffect(bool instant)
		{
			if (this.graphic == null)
			{
				return;
			}
			this.graphic.CrossFadeAlpha(this.m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
		}

		protected override void Start()
		{
			this.PlayEffect(true);
		}

		private void InternalToggle()
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this.IsOn = !this.IsOn;
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			this.InternalToggle();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.InternalToggle();
		}

	
		public string UniqueID;

		public ExtensionsToggle.ToggleTransition toggleTransition = ExtensionsToggle.ToggleTransition.Fade;

		public Graphic graphic;

		[SerializeField]
		private ExtensionsToggleGroup m_Group;

		[Tooltip("Use this event if you only need the bool state of the toggle that was changed")]
		public ExtensionsToggle.ToggleEvent onValueChanged = new ExtensionsToggle.ToggleEvent();

		[Tooltip("Use this event if you need access to the toggle that was changed")]
		public ExtensionsToggle.ToggleEventObject onToggleChanged = new ExtensionsToggle.ToggleEventObject();

		[FormerlySerializedAs("m_IsActive")]
		[Tooltip("Is the toggle currently on or off?")]
		[SerializeField]
		private bool m_IsOn;

		public enum ToggleTransition
		{
			None,
			Fade
		}

		[Serializable]
		public class ToggleEvent : UnityEvent<bool>
		{
		}

		[Serializable]
		public class ToggleEventObject : UnityEvent<ExtensionsToggle>
		{
		}
	}
}
