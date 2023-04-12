using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardedVideoButtonView : MonoBehaviour
{
	private void Start()
	{
		this.rewardVideoBtn.onClick.AddListener(delegate()
		{
			this.rewardVideoBtnClicked = true;
		});
	}

	public void Unload()
	{
		this.rewardVideoBtnClicked = false;
	}

	public void MyLateUpdate()
	{
		this.rewardVideoBtnClicked = false;
	}

	private void OnDestroy()
	{
		this.rewardVideoBtn.onClick.RemoveAllListeners();
	}

	[SerializeField]
	private Button rewardVideoBtn;

	public bool rewardVideoBtnClicked;
}
