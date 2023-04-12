using System;
using UnityEngine;

public class LevelUpPanelModel
{
	public void SetCurrentLevelAndStarsValues(int _currentLevel, long _currentStars)
	{
		this.currentLevel = _currentLevel;
		this.currentStars = _currentStars;
	}

	public long GetRewardValue()
	{
		if (this.currentLevel <= 10)
		{
			return (long)Mathf.CeilToInt(160f + 25f * Mathf.Pow(1.8f, (float)this.currentLevel));
		}
		return (long)Mathf.CeilToInt(160f + 25f * Mathf.Pow(1.8f, 10f) * Mathf.Pow(1.3f, (float)(this.currentLevel - 10)));
	}

	public long GetExtraRewardValue()
	{
		return this.GetRewardValue() * 2L;
	}

	private int currentLevel;

	private long currentStars;
}
