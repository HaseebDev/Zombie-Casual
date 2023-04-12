using System;
using UnityEngine;

namespace Adic.Examples.Prefabs
{
	public class CubeRotator : MonoBehaviour
	{
		[Inject]
		protected void MethodInjection()
		{
			this.messages = this.messages + "MethodInjection called." + Environment.NewLine;
			string str = (this.cube == null) ? "No..." : "Yes!";
			this.messages = this.messages + ("Cube injected? " + str) + Environment.NewLine;
		}

		protected void Start()
		{
			this.Inject();
		}

		protected void Update()
		{
			if (this.cube != null)
			{
				this.cube.Rotate(1f, 1f, 1f);
			}
		}

		protected void OnGUI()
		{
			GUI.Label(new Rect(10f, 10f, 300f, 100f), this.messages);
		}

		[Inject("cube")]
		public Transform cube;

		private string messages;
	}
}
