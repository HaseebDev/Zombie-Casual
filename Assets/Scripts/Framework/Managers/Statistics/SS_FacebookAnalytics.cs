using System;
// using Facebook.Unity;

namespace Framework.Managers.Statistics
{
	public class SS_FacebookAnalytics : IStatisticSystem
	{
		public void Init()
		{
			/*if (FB.IsInitialized)
			{
				FB.ActivateApp();
				return;
			}
			FB.Init(delegate()
			{
				FB.ActivateApp();
			}, null, null);*/
		}

		public void OnApplicationFocus(bool hasFocus)
		{
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			if (!pauseStatus)
			{
				this.Init();
			}
		}

		public void SendEvent(string eventName)
		{
			// FB.LogAppEvent(eventName, null, null);
		}

		public void SendLevelLooseEvent(int levelNumb)
		{
		}

		public void SendLevelWinEvent(int levelNumb)
		{
		}
	}
}
