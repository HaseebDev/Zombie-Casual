using System;
using UnityEngine;

public class UpgradebleObjectView : MonoBehaviour
{
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	public string upgardebleValueGameObjectId;
}
