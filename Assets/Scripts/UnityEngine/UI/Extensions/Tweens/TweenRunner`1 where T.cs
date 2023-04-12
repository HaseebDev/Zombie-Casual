using System;
using System.Collections;

namespace UnityEngine.UI.Extensions.Tweens
{
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		private static IEnumerator Start(T tweenInfo)
		{
			if (!tweenInfo.ValidTarget())
			{
				yield break;
			}
			float elapsedTime = 0f;
			while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += (tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
				float floatPercentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
				tweenInfo.TweenValue(floatPercentage);
				yield return null;
			}
			tweenInfo.TweenValue(1f);
			tweenInfo.Finished();
			yield break;
		}

		public void Init(MonoBehaviour coroutineContainer)
		{
			this.m_CoroutineContainer = coroutineContainer;
		}

		public void StartTween(T info)
		{
			if (this.m_CoroutineContainer == null)
			{
				UnityEngine.Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
				return;
			}
			if (this.m_Tween != null)
			{
				this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
				this.m_Tween = null;
			}
			if (!this.m_CoroutineContainer.gameObject.activeInHierarchy)
			{
				info.TweenValue(1f);
				return;
			}
			this.m_Tween = TweenRunner<T>.Start(info);
			this.m_CoroutineContainer.StartCoroutine(this.m_Tween);
		}

		protected MonoBehaviour m_CoroutineContainer;

		protected IEnumerator m_Tween;
	}
}
