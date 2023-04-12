using System;

namespace Framework.Managers.Statistics
{
	public interface IStatisticSystem
	{
		void Init();

		void SendEvent(string eventName);

		void SendLevelWinEvent(int levelNumb);

		void SendLevelLooseEvent(int levelNumb);

		void OnApplicationPause(bool pauseStatus);

		void OnApplicationFocus(bool hasFocus);
	}
}
