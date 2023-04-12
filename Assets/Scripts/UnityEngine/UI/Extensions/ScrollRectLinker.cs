using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/ScrollRectLinker")]
	public class ScrollRectLinker : MonoBehaviour
	{
		private void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			if (this.controllingScrollRect != null)
			{
				this.controllingScrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.MirrorPos));
			}
		}

		private void MirrorPos(Vector2 scrollPos)
		{
			if (this.clamp)
			{
				this.scrollRect.normalizedPosition = new Vector2(Mathf.Clamp01(scrollPos.x), Mathf.Clamp01(scrollPos.y));
				return;
			}
			this.scrollRect.normalizedPosition = scrollPos;
		}

		public bool clamp = true;

		[SerializeField]
		private ScrollRect controllingScrollRect;

		private ScrollRect scrollRect;
	}
}
