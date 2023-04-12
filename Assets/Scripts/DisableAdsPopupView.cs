using System;
using Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class DisableAdsPopupView : MonoBehaviour, IState
{
	public void Load()
	{
		base.gameObject.SetActive(true);
	}

	public void Unload()
	{
		base.gameObject.SetActive(false);
	}

	public void Loading(bool _isActive)
	{
		UnityEngine.Debug.Log(_isActive);
		this.loadingPanel.SetActive(_isActive);
	}

	public void SetPopupText(string _text)
	{
		this.noAdsText.text = _text;
	}

	public void MyLateUpdate()
	{
		this.disableButtonClicked = false;
		this.restorePurchasesButtonClicked = false;
		this.closeButtonClicked = false;
	}

	private void Start()
	{
		this.disableButton.onClick.AddListener(delegate()
		{
			this.disableButtonClicked = true;
		});
		this.restorePurchasesButton.onClick.AddListener(delegate()
		{
			this.restorePurchasesButtonClicked = true;
		});
		this.closeButton.onClick.AddListener(delegate()
		{
			this.closeButtonClicked = true;
		});
	}

	private void OnDestroy()
	{
		this.disableButton.onClick.RemoveAllListeners();
		this.restorePurchasesButton.onClick.RemoveAllListeners();
		this.closeButton.onClick.RemoveAllListeners();
	}

	[SerializeField]
	private Button disableButton;

	[SerializeField]
	private Button restorePurchasesButton;

	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private Text noAdsText;

	[SerializeField]
	private GameObject loadingPanel;

	public bool disableButtonClicked;

	public bool restorePurchasesButtonClicked;

	public bool closeButtonClicked;
}
