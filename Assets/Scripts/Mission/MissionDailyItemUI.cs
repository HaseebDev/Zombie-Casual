using System;
using System.Collections;
using System.Collections.Generic;
using Ez.Pooly;
using UnityEngine;

public class MissionDailyItemUI : MissionDurationItemUI
{
    public override void Init()
    {
        base.Init();
        _missionDurationData = SaveManager.Instance.Data.MissionManagerData.Daily;
    }

    public override void UpdateTimer()
    {
        base.UpdateTimer();
        var now = TimeService.instance.GetCurrentDateTime();
        double dailyTimeSeconds = (now.AddDays(1).Date - now).TotalSeconds;
        resetTimeText.text = $"{TimeService.FormatTimeSpan(dailyTimeSeconds,false)}";
    }
}
