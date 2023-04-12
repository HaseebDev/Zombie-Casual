using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.Tweens
{
	public struct FloatTween : ITweenValue
	{
		public float startFloat
		{
			get
			{
				return this.m_StartFloat;
			}
			set
			{
				this.m_StartFloat = value;
			}
		}

		public float targetFloat
		{
			get
			{
				return this.m_TargetFloat;
			}
			set
			{
				this.m_TargetFloat = value;
			}
		}

		public float duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				this.m_Duration = value;
			}
		}

		public bool ignoreTimeScale
		{
			get
			{
				return this.m_IgnoreTimeScale;
			}
			set
			{
				this.m_IgnoreTimeScale = value;
			}
		}

		public void TweenValue(float floatPercentage)
		{
			if (!this.ValidTarget())
			{
				return;
			}
			this.m_Target.Invoke(Mathf.Lerp(this.m_StartFloat, this.m_TargetFloat, floatPercentage));
		}

		public void AddOnChangedCallback(UnityAction<float> callback)
		{
			if (this.m_Target == null)
			{
				this.m_Target = new FloatTween.FloatTweenCallback();
			}
			this.m_Target.AddListener(callback);
		}

		public void AddOnFinishCallback(UnityAction callback)
		{
			if (this.m_Finish == null)
			{
				this.m_Finish = new FloatTween.FloatFinishCallback();
			}
			this.m_Finish.AddListener(callback);
		}

		public bool GetIgnoreTimescale()
		{
			return this.m_IgnoreTimeScale;
		}

		public float GetDuration()
		{
			return this.m_Duration;
		}

		public bool ValidTarget()
		{
			return this.m_Target != null;
		}

		public void Finished()
		{
			if (this.m_Finish != null)
			{
				this.m_Finish.Invoke();
			}
		}

		private float m_StartFloat;

		private float m_TargetFloat;

		private float m_Duration;

		private bool m_IgnoreTimeScale;

		private FloatTween.FloatTweenCallback m_Target;

		private FloatTween.FloatFinishCallback m_Finish;

		public class FloatTweenCallback : UnityEvent<float>
		{
		}

		public class FloatFinishCallback : UnityEvent
		{
		}
	}
}
