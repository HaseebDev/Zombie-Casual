using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWeeklyItemUI : MissionDurationItemUI
{
    public override void Init()
    {
        base.Init();
        _missionDurationData = SaveManager.Instance.Data.MissionManagerData.Weekly;
    }

    public override void UpdateTimer()
    {
        base.UpdateTimer();

        long dayOfYearWeekly = SaveManager.Instance.Data.MissionManagerData.Weekly.StartDate;
        int year = TimeService.instance.GetCurrentDateTime().Year; //Or any year you want
        DateTime theDate = new DateTime(year, 1, 1).AddDays(dayOfYearWeekly - 1);

        double weeklyTimeSeconds = (theDate.AddDays(7).Date - TimeService.instance.GetCurrentDateTime()).TotalSeconds;
        resetTimeText.text = $"{TimeService.FormatTimeSpan(weeklyTimeSeconds)}";
    }
}