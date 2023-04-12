using System;
using Framework.Views;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitcherView : BaseGoSwitcherView
{
	private void Start()
	{
		this.changeStateBtn.onClick.AddListener(delegate()
		{
			this.state = base.Switch();
			this.onStateChanged = true;
		});
	}

	public void MyLateUpdate()
	{
		this.onStateChanged = false;
	}

	private void OnDestroy()
	{
		this.changeStateBtn.onClick.RemoveAllListeners();
	}

	[SerializeField]
	private Button changeStateBtn;

	public bool onStateChanged;

	public bool state;
}
