using System;
using UnityEngine;

public class WaveDifficultController : MonoBehaviour
{
	public float CountOfZombieMultiplier
	{
		get
		{
			return this.countOfZombieMultiplier;
		}
		set
		{
			PlayerPrefs.SetFloat("countOfZombieMultiplier", value);
			this.countOfZombieMultiplier = value;
		}
	}

	public float HpOfZombieMultiplier
	{
		get
		{
			return this.hpOfZombieMultiplier;
		}
		set
		{
			PlayerPrefs.SetFloat("hpOfZombieMultiplier", value);
			this.hpOfZombieMultiplier = value;
		}
	}

	private void Awake()
	{
		this.countOfZombieMultiplier = PlayerPrefs.GetFloat("countOfZombieMultiplier", 1f);
		this.hpOfZombieMultiplier = PlayerPrefs.GetFloat("hpOfZombieMultiplier", 1f);
	}

	public void UpDifficult(int currentLevel)
	{
		if (currentLevel < 5)
		{
			this.CountOfZombieMultiplier += 0.1f;
		}
		if (currentLevel >= 5 && currentLevel <= 10)
		{
			this.CountOfZombieMultiplier += 0.2f;
		}
		this.HpOfZombieMultiplier += 0.2f;
	}

	private float countOfZombieMultiplier;

	private float hpOfZombieMultiplier;
}
