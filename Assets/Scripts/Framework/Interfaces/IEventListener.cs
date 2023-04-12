using System;
using UnityEngine;

namespace Framework.Interfaces
{
	public interface IEventListener
	{
		void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null);
	}
}
