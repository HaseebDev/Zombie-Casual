using System;
using System.Collections.Generic;
using Framework.Common;
using Framework.Interfaces;
using UnityEngine;

namespace Framework.Managers.Event
{
	public class EventManager : BaseBehavior
	{
		public void AddListener(EVENT_TYPE Event_Type, IEventListener Listener)
		{
			List<IEventListener> list = null;
			if (this.Listeners.TryGetValue(Event_Type, out list))
			{
				list.Add(Listener);
				return;
			}
			list = new List<IEventListener>();
			list.Add(Listener);
			this.Listeners.Add(Event_Type, list);
		}

		public void PostNotification(EVENT_TYPE Event_Type, Component Sender, object Param = null)
		{
			List<IEventListener> list = null;
			if (!this.Listeners.TryGetValue(Event_Type, out list))
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].Equals(null))
				{
					list[i].OnEvent(Event_Type, Sender, Param);
				}
			}
		}

		public void RemoveEvent(EVENT_TYPE Event_Type)
		{
			this.Listeners.Remove(Event_Type);
		}

		public void RemoveRedundancies()
		{
			Dictionary<EVENT_TYPE, List<IEventListener>> dictionary = new Dictionary<EVENT_TYPE, List<IEventListener>>();
			foreach (KeyValuePair<EVENT_TYPE, List<IEventListener>> keyValuePair in this.Listeners)
			{
				for (int i = keyValuePair.Value.Count - 1; i > 0; i--)
				{
					if (keyValuePair.Value[i].Equals(null))
					{
						keyValuePair.Value.RemoveAt(i);
					}
					if (keyValuePair.Value.Count > 0)
					{
						dictionary.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			this.Listeners = dictionary;
		}

		private void OnLevelWasLoaded(int level)
		{
			this.RemoveRedundancies();
		}

		private Dictionary<EVENT_TYPE, List<IEventListener>> Listeners = new Dictionary<EVENT_TYPE, List<IEventListener>>();
	}
}
