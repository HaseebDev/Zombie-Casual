using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/ScrollRectTweener")]
	public class ScrollRectTweener : MonoBehaviour, IDragHandler, IEventSystemHandler
	{
		private void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			this.wasHorizontal = this.scrollRect.horizontal;
			this.wasVertical = this.scrollRect.vertical;
		}

		public void ScrollHorizontal(float normalizedX)
		{
			this.Scroll(new Vector2(normalizedX, this.scrollRect.verticalNormalizedPosition));
		}

		public void ScrollHorizontal(float normalizedX, float duration)
		{
			this.Scroll(new Vector2(normalizedX, this.scrollRect.verticalNormalizedPosition), duration);
		}

		public void ScrollVertical(float normalizedY)
		{
			this.Scroll(new Vector2(this.scrollRect.horizontalNormalizedPosition, normalizedY));
		}

		public void ScrollVertical(float normalizedY, float duration)
		{
			this.Scroll(new Vector2(this.scrollRect.horizontalNormalizedPosition, normalizedY), duration);
		}

		public void Scroll(Vector2 normalizedPos)
		{
			this.Scroll(normalizedPos, this.GetScrollDuration(normalizedPos));
		}

		private float GetScrollDuration(Vector2 normalizedPos)
		{
			Vector2 currentPos = this.GetCurrentPos();
			return Vector2.Distance(this.DeNormalize(currentPos), this.DeNormalize(normalizedPos)) / this.moveSpeed;
		}

		private Vector2 DeNormalize(Vector2 normalizedPos)
		{
			return new Vector2(normalizedPos.x * this.scrollRect.content.rect.width, normalizedPos.y * this.scrollRect.content.rect.height);
		}

		private Vector2 GetCurrentPos()
		{
			return new Vector2(this.scrollRect.horizontalNormalizedPosition, this.scrollRect.verticalNormalizedPosition);
		}

		public void Scroll(Vector2 normalizedPos, float duration)
		{
			this.startPos = this.GetCurrentPos();
			this.targetPos = normalizedPos;
			if (this.disableDragWhileTweening)
			{
				this.LockScrollability();
			}
			base.StopAllCoroutines();
			base.StartCoroutine(this.DoMove(duration));
		}

		private IEnumerator DoMove(float duration)
		{
			if (duration < 0.05f)
			{
				yield break;
			}
			Vector2 posOffset = this.targetPos - this.startPos;
			float currentTime = 0f;
			while (currentTime < duration)
			{
				currentTime += Time.deltaTime;
				this.scrollRect.normalizedPosition = this.EaseVector(currentTime, this.startPos, posOffset, duration);
				yield return null;
			}
			this.scrollRect.normalizedPosition = this.targetPos;
			if (this.disableDragWhileTweening)
			{
				this.RestoreScrollability();
			}
			yield break;
		}

		public Vector2 EaseVector(float currentTime, Vector2 startValue, Vector2 changeInValue, float duration)
		{
			return new Vector2(changeInValue.x * Mathf.Sin(currentTime / duration * 1.57079637f) + startValue.x, changeInValue.y * Mathf.Sin(currentTime / duration * 1.57079637f) + startValue.y);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!this.disableDragWhileTweening)
			{
				this.StopScroll();
			}
		}

		private void StopScroll()
		{
			base.StopAllCoroutines();
			if (this.disableDragWhileTweening)
			{
				this.RestoreScrollability();
			}
		}

		private void LockScrollability()
		{
			this.scrollRect.horizontal = false;
			this.scrollRect.vertical = false;
		}

		private void RestoreScrollability()
		{
			this.scrollRect.horizontal = this.wasHorizontal;
			this.scrollRect.vertical = this.wasVertical;
		}

		private ScrollRect scrollRect;

		private Vector2 startPos;

		private Vector2 targetPos;

		private bool wasHorizontal;

		private bool wasVertical;

		public float moveSpeed = 5000f;

		public bool disableDragWhileTweening;
	}
}
