using System;
using System.Collections;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Tween Scale")]
	public class UI_TweenScale : MonoBehaviour
	{
		private void Awake()
		{
			this.myTransform = base.GetComponent<Transform>();
			this.initScale = this.myTransform.localScale;
			if (this.playAtAwake)
			{
				this.Play();
			}
		}

		public void Play()
		{
			base.StartCoroutine("Tween");
		}

		private IEnumerator Tween()
		{
			this.myTransform.localScale = this.initScale;
			float t = 0f;
			float maxT = this.animCurve.keys[this.animCurve.length - 1].time;
			while (t < maxT || this.isLoop)
			{
				t += this.speed * Time.deltaTime;
				if (!this.isUniform)
				{
					this.newScale.x = 1f * this.animCurve.Evaluate(t);
					this.newScale.y = 1f * this.animCurveY.Evaluate(t);
					this.myTransform.localScale = this.newScale;
				}
				else
				{
					this.myTransform.localScale = Vector3.one * this.animCurve.Evaluate(t);
				}
				yield return null;
			}
			yield break;
		}

		public void ResetTween()
		{
			base.StopCoroutine("Tween");
			this.myTransform.localScale = this.initScale;
		}

		public AnimationCurve animCurve;

		[Tooltip("Animation speed multiplier")]
		public float speed = 1f;

		[Tooltip("If true animation will loop, for best effect set animation curve to loop on start and end point")]
		public bool isLoop;

		[Tooltip("If true animation will start automatically, otherwise you need to call Play() method to start the animation")]
		public bool playAtAwake;

		[Space(10f)]
		[Header("Non uniform scale")]
		[Tooltip("If true component will scale by the same amount in X and Y axis, otherwise use animCurve for X scale and animCurveY for Y scale")]
		public bool isUniform = true;

		public AnimationCurve animCurveY;

		private Vector3 initScale;

		private Transform myTransform;

		private Vector3 newScale = Vector3.one;
	}
}
