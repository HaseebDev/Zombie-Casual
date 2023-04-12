using System;
using UnityEngine;

namespace Framework.Views
{
	public class BaseGoSwitcherView : MonoBehaviour
	{
		public bool Switch()
		{
			if (this.state)
			{
				this.TurnOFff();
			}
			else
			{
				this.TurnOn();
			}
			return this.state;
		}

		public bool Switch(bool state)
		{
			if (state)
			{
				this.TurnOn();
			}
			else
			{
				this.TurnOFff();
			}
			return state;
		}

		private void TurnOn()
		{
			this.state = true;
			this.StateChanged();
		}

		private void TurnOFff()
		{
			this.state = false;
			this.StateChanged();
		}

		private void StateChanged()
		{
			this.gameObjectOn.SetActive(false);
			this.gameObjectOff.SetActive(false);
			if (this.state)
			{
				this.gameObjectOn.SetActive(true);
				return;
			}
			this.gameObjectOff.SetActive(true);
		}

		private bool state;

		[SerializeField]
		private GameObject gameObjectOn;

		[SerializeField]
		private GameObject gameObjectOff;
	}
}
