using System;
using UnityEngine;

namespace Adic
{
	public class EventCallerBehaviour : MonoBehaviour
	{
		public EventCallerContainerExtension extension { get; set; }

		protected void Update()
		{
			if (Mathf.Approximately(Time.deltaTime, 0f))
			{
				return;
			}
			for (int i = 0; i < this.extension.updateable.Count; i++)
			{
				this.extension.updateable[i].Update();
			}
		}

		protected void LateUpdate()
		{
			if (Mathf.Approximately(Time.deltaTime, 0f))
			{
				return;
			}
			for (int i = 0; i < this.extension.lateUpdateable.Count; i++)
			{
				this.extension.lateUpdateable[i].LateUpdate();
			}
		}

		protected void FixedUpdate()
		{
			for (int i = 0; i < this.extension.fixedUpdateable.Count; i++)
			{
				this.extension.fixedUpdateable[i].FixedUpdate();
			}
		}

		protected void OnApplicationFocus(bool hasFocus)
		{
			for (int i = 0; i < this.extension.focusable.Count; i++)
			{
				this.extension.focusable[i].OnApplicationFocus(hasFocus);
			}
		}

		protected void OnApplicationPause(bool isPaused)
		{
			for (int i = 0; i < this.extension.pausable.Count; i++)
			{
				this.extension.pausable[i].OnApplicationPause(isPaused);
			}
		}

		protected void OnApplicationQuit()
		{
			for (int i = 0; i < this.extension.quitable.Count; i++)
			{
				this.extension.quitable[i].OnApplicationQuit();
			}
		}

		protected void OnDestroy()
		{
			foreach (IDisposable disposable in this.extension.disposable)
			{
				disposable.Dispose();
			}
		}
	}
}
