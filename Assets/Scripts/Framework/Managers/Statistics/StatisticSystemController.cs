using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Managers.Statistics
{
	public class StatisticSystemController : MonoBehaviour
	{
		public void AddStatisticSystem(IStatisticSystem statisticSystem)
		{
			if (!this.statisticSystemList.Contains(statisticSystem))
			{
				this.statisticSystemList.Add(statisticSystem);
			}
		}

		public void Init()
		{
			for (int i = 0; i < this.statisticSystemList.Count; i++)
			{
				this.statisticSystemList[i].Init();
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			for (int i = 0; i < this.statisticSystemList.Count; i++)
			{
				this.statisticSystemList[i].OnApplicationPause(pauseStatus);
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			for (int i = 0; i < this.statisticSystemList.Count; i++)
			{
				this.statisticSystemList[i].OnApplicationFocus(hasFocus);
			}
		}

		public void SendEvent(string eventName)
		{
			for (int i = 0; i < this.statisticSystemList.Count; i++)
			{
				this.statisticSystemList[i].SendEvent(eventName);
			}
		}

		public void SendLevelWinEvent(int levelNumb)
		{
			for (int i = 0; i < this.statisticSystemList.Count; i++)
			{
				this.statisticSystemList[i].SendLevelWinEvent(levelNumb);
			}
		}

		public void SendLevelLooseEvent(int levelNumb)
		{
			for (int i = 0; i < this.statisticSystemList.Count; i++)
			{
				this.statisticSystemList[i].SendLevelLooseEvent(levelNumb);
			}
		}

		private List<IStatisticSystem> statisticSystemList = new List<IStatisticSystem>();
	}
}
