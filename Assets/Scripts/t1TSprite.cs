using System;
using UnityEngine;

public class t1TSprite : MonoBehaviour
{
	private void Start()
	{
		this.Load();
	}

	private void Load()
	{
		this.progress = PlayerPrefs.GetInt("swipeint", 0);
		if (this.progress == 1)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnDrag(Vector3 dir)
	{
		if (this.progress != 1)
		{
			this.progress = 1;
			PlayerPrefs.SetInt("swipeint", 1);
			base.gameObject.SetActive(false);
		}
	}

	private int progress;
}
