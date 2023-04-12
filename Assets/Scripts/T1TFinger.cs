using System;
using UnityEngine;

public class T1TFinger : MonoBehaviour
{
	private void Start()
	{
		this.Load();
	}

	private void Load()
	{
		this.progress = PlayerPrefs.GetInt("buyint", 0);
		if (this.progress == 1)
		{
			base.gameObject.SetActive(false);
		}
        base.gameObject.SetActive(false);
    }

	public void OnBuy()
	{
		if (this.progress != 1)
		{
			this.progress = 1;
			PlayerPrefs.SetInt("buyint", 1);
			base.gameObject.SetActive(false);
		}
	}

	private int progress;
}
