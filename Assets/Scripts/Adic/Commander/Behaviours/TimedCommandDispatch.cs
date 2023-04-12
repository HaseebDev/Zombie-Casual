using System;
using UnityEngine;

namespace Adic.Commander.Behaviours
{
	[AddComponentMenu("Adic/Commander/Timed command dispatch")]
	public class TimedCommandDispatch : CommandDispatch
	{
		protected void OnEnable()
		{
			base.Invoke("DispatchCommand", this.timer);
		}

		public float timer;
	}
}
