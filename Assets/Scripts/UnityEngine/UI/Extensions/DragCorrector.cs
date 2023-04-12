using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(EventSystem))]
	[AddComponentMenu("UI/Extensions/DragCorrector")]
	public class DragCorrector : MonoBehaviour
	{
		private void Start()
		{
			this.dragTH = this.baseTH * (int)Screen.dpi / this.basePPI;
			EventSystem component = base.GetComponent<EventSystem>();
			if (component)
			{
				component.pixelDragThreshold = this.dragTH;
			}
		}

		public int baseTH = 6;

		public int basePPI = 210;

		public int dragTH;
	}
}
