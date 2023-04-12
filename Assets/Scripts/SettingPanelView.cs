using System;
using Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelView : MonoBehaviour, IState, ITickLate
{
	private void Start()
	{
		this.settingsBtn.onClick.AddListener(delegate()
		{
			this.slidePanel.SwitchState();
		});
	}

	public void Load()
	{
		base.gameObject.SetActive(true);
	}

	public void Unload()
	{
		base.gameObject.SetActive(false);
	}

	public ButtonSwitcherView GetSoundBtnView()
	{
		return this.soundBtn;
	}

	public ButtonSwitcherView GetVibroBtnView()
	{
		return this.vibroBtn;
	}

	public void TickLate()
	{
		this.onCloseBtnPressed = false;
	}

	private void OnDestroy()
	{
		this.settingsBtn.onClick.RemoveAllListeners();
	}

	[SerializeField]
	private Button settingsBtn;

	[SerializeField]
	private ButtonSwitcherView soundBtn;

	[SerializeField]
	private ButtonSwitcherView vibroBtn;

	[SerializeField]
	private SlideToPointUIPanelComponent slidePanel;

	public bool onCloseBtnPressed;
}
