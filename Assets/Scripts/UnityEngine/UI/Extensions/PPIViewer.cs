using System;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Text))]
	[AddComponentMenu("UI/Extensions/PPIViewer")]
	public class PPIViewer : MonoBehaviour
	{
		private void Awake()
		{
			this.label = base.GetComponent<Text>();
		}

		private void Start()
		{
			if (this.label != null)
			{
				this.label.text = "PPI: " + Screen.dpi.ToString();
			}
		}

		private Text label;
	}
}
