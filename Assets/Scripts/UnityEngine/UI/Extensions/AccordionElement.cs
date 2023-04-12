using System;
using UnityEngine.Events;
using UnityEngine.UI.Extensions.Tweens;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
	[AddComponentMenu("UI/Extensions/Accordion/Accordion Element")]
	public class AccordionElement : Toggle
	{
		protected AccordionElement()
		{
			if (this.m_FloatTweenRunner == null)
			{
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			}
			this.m_FloatTweenRunner.Init(this);
		}

		protected override void Awake()
		{
			base.Awake();
			base.transition = Selectable.Transition.None;
			this.toggleTransition = Toggle.ToggleTransition.None;
			this.m_Accordion = base.gameObject.GetComponentInParent<Accordion>();
			this.m_RectTransform = (base.transform as RectTransform);
			this.m_LayoutElement = base.gameObject.GetComponent<LayoutElement>();
			this.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		public void OnValueChanged(bool state)
		{
			if (this.m_LayoutElement == null)
			{
				return;
			}
			Accordion.Transition transition = (this.m_Accordion != null) ? this.m_Accordion.transition : Accordion.Transition.Instant;
			if (transition != Accordion.Transition.Instant)
			{
				if (transition == Accordion.Transition.Tween)
				{
					if (state)
					{
						this.StartTween(this.m_MinHeight, this.GetExpandedHeight());
						return;
					}
					this.StartTween(this.m_RectTransform.rect.height, this.m_MinHeight);
				}
				return;
			}
			if (state)
			{
				this.m_LayoutElement.preferredHeight = -1f;
				return;
			}
			this.m_LayoutElement.preferredHeight = this.m_MinHeight;
		}

		protected float GetExpandedHeight()
		{
			if (this.m_LayoutElement == null)
			{
				return this.m_MinHeight;
			}
			float preferredHeight = this.m_LayoutElement.preferredHeight;
			this.m_LayoutElement.preferredHeight = -1f;
			float preferredHeight2 = LayoutUtility.GetPreferredHeight(this.m_RectTransform);
			this.m_LayoutElement.preferredHeight = preferredHeight;
			return preferredHeight2;
		}

		protected void StartTween(float startFloat, float targetFloat)
		{
			float duration = (this.m_Accordion != null) ? this.m_Accordion.transitionDuration : 0.3f;
			FloatTween info = new FloatTween
			{
				duration = duration,
				startFloat = startFloat,
				targetFloat = targetFloat
			};
			info.AddOnChangedCallback(new UnityAction<float>(this.SetHeight));
			info.ignoreTimeScale = true;
			this.m_FloatTweenRunner.StartTween(info);
		}

		protected void SetHeight(float height)
		{
			if (this.m_LayoutElement == null)
			{
				return;
			}
			this.m_LayoutElement.preferredHeight = height;
		}

		[SerializeField]
		private float m_MinHeight = 18f;

		private Accordion m_Accordion;

		private RectTransform m_RectTransform;

		private LayoutElement m_LayoutElement;

		[NonSerialized]
		private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
	}
}
