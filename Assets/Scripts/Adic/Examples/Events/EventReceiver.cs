using System;
using UnityEngine;

namespace Adic.Examples.Events
{
	public class EventReceiver : IUpdatable, ILateUpdatable, IFixedUpdatable, IFocusable, IPausable, IQuitable
	{
		public void Update()
		{
			UnityEngine.Debug.Log("Updating...");
		}

		public void LateUpdate()
		{
			UnityEngine.Debug.Log("Late updating...");
		}

		public void FixedUpdate()
		{
			UnityEngine.Debug.Log("Fixed updating...");
		}

		public void OnApplicationFocus(bool hasFocus)
		{
			UnityEngine.Debug.LogFormat("Has focus? {0}", new object[]
			{
				hasFocus
			});
		}

		public void OnApplicationPause(bool isPaused)
		{
			UnityEngine.Debug.LogFormat("Is paused? {0}", new object[]
			{
				isPaused
			});
		}

		public void OnApplicationQuit()
		{
			UnityEngine.Debug.Log("Game quit");
		}
	}
}
