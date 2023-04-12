using System;
using Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class LevelCheatPanelView : MonoBehaviour, ITickLate, IState
{
	private void Start()
	{
		this.loadPrevLevelBtn.onClick.AddListener(delegate()
		{
			this.onLoadPrevLevelBtnPressed = true;
		});
		this.loadSelectedLevelBtn.onClick.AddListener(delegate()
		{
			this.onLoadSelectedLevelBtnPressed = true;
		});
		this.loadNextLevelBtn.onClick.AddListener(delegate()
		{
			this.onLoadNextLevelBtnPressed = true;
		});
	}

	public string GetLevelInputFieldText()
	{
		return this.levelInputField.text;
	}

	public void TickLate()
	{
		this.onLoadPrevLevelBtnPressed = false;
		this.onLoadSelectedLevelBtnPressed = false;
		this.onLoadNextLevelBtnPressed = false;
	}

	private void OnDestroy()
	{
		this.loadPrevLevelBtn.onClick.RemoveAllListeners();
		this.loadSelectedLevelBtn.onClick.RemoveAllListeners();
		this.loadNextLevelBtn.onClick.RemoveAllListeners();
	}

	public void Load()
	{
		base.gameObject.SetActive(true);
	}

	public void Unload()
	{
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	private Button loadPrevLevelBtn;

	[SerializeField]
	private Button loadSelectedLevelBtn;

	[SerializeField]
	private Button loadNextLevelBtn;

	[SerializeField]
	private InputField levelInputField;

	public bool onLoadPrevLevelBtnPressed;

	public bool onLoadSelectedLevelBtnPressed;

	public bool onLoadNextLevelBtnPressed;
}
