using System;
using System.Globalization;
using Adic;
using UnityEngine;

public class OfflineIncomePanelModel
{
	public void CountMinutesSinceLastExit()
	{
		this.lastExitDate = this.LoadAndGetLastExitDate();
		this.minutesPassed = this.lastExitMinutesPassed();
	}

	public void SetLikesPerSecValue(long _likesPerSec)
	{
		this.likesPerSec = _likesPerSec;
	}

	public long GetRewardValue()
	{
		if (LevelModel.instance.CurrentLevel == 1)
		{
			return 0L;
		}
		if (this.minutesPassed >= 90)
		{
			this.minutesPassed = 90;
		}
		return (long)(30 + this.minutesPassed + LevelModel.instance.CurrentLevel * this.minutesPassed / 60);
	}

	public long GetExtraRewardValue()
	{
		return this.GetRewardValue() * 2L;
	}

	private int lastExitMinutesPassed()
	{
		DateTime now = TimeService.instance.GetCurrentDateTime();
		DateTime d = this.lastExitDate;
		return (int)(now - d).TotalMinutes;
	}

	private DateTime LoadAndGetLastExitDate()
	{
		if (PlayerPrefs.HasKey("exitDate"))
		{
			return DateTime.Parse(PlayerPrefs.GetString("exitDate"), CultureInfo.InvariantCulture, DateTimeStyles.None);
		}
		return TimeService.instance.GetCurrentDateTime();;
	}

	public void SaveExitDate()
	{
		PlayerPrefs.SetString("exitDate", TimeService.instance.GetCurrentDateTime().ToString(CultureInfo.InvariantCulture));
	}

	private long likesPerSec;

	private int minutesPassed;
    

	private DateTime lastExitDate;
}
