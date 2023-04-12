using System;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(ToggleGroup))]
	[AddComponentMenu("UI/Extensions/Accordion/Accordion Group")]
	public class Accordion : MonoBehaviour
	{
		public Accordion.Transition transition
		{
			get
			{
				return this.m_Transition;
			}
			set
			{
				this.m_Transition = value;
			}
		}

		public float transitionDuration
		{
			get
			{
				return this.m_TransitionDuration;
			}
			set
			{
				this.m_TransitionDuration = value;
			}
		}

		[SerializeField]
		private Accordion.Transition m_Transition;

		[SerializeField]
		private float m_TransitionDuration = 0.3f;

		public enum Transition
		{
			Instant,
			Tween
		}
	}
}
