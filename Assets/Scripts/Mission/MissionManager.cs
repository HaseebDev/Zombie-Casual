using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using Google.Protobuf.Collections;
using QuickType;
using UnityEngine;

public class MissionManager
{
    private static MissionManager _instance = null;

    public static MissionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MissionManager();
            }

            return _instance;
        }
    }


    private MissionDurationData _dailyMissionManager;
    private MissionDurationData _weeklyMissionManager;

    private UserData _userData;

    public MissionDurationManager dailyMissionManager;
    public MissionDurationManager weeklyMissionManager;

    public void Init()
    {
        _userData = SaveManager.Instance.Data;
        // _userData.MissionManagerData ??= new MissionManagerData();
        if (_userData.MissionManagerData == null)
        {
            _userData.MissionManagerData = new MissionManagerData();
        }

        if (_userData.MissionManagerData.Daily == null)
        {
            ResetDailyMission();
        }
        else
        {
            _dailyMissionManager = _userData.MissionManagerData.Daily;
            dailyMissionManager = new MissionDurationManager();
            dailyMissionManager.Init(_dailyMissionManager);
        }

        if (_userData.MissionManagerData.Weekly == null)
        {
            ResetWeeklyMission();
        }
        else
        {
            _weeklyMissionManager = _userData.MissionManagerData.Weekly;
            weeklyMissionManager = new MissionDurationManager();
            weeklyMissionManager.Init(_weeklyMissionManager);
        }

        CheckToReset();
    }

    public void ResetDailyMission()
    {
        Debug.LogError("ResetDailyMission");
        dailyMissionManager?.OnDestroy();
        dailyMissionManager = new MissionDurationManager();

        _userData.MissionManagerData.Daily = new MissionDurationData();
        _dailyMissionManager = _userData.MissionManagerData.Daily;
        _dailyMissionManager.Duration = 1;
        _dailyMissionManager.StartDate = TimeService.instance.GetCurrentDateTime().DayOfYear;
        var dailyDesign =
            DesignHelper.GetMissionDurationDesign(_dailyMissionManager.Duration, SaveGameHelper.GetMaxCampaignLevel());

        var missionDesigns = dailyDesign.GetMissionsDesign();
        foreach (var VARIABLE in missionDesigns)
        {
            MissionData missionData = new MissionData();
            missionData.Id = VARIABLE.Id;
            missionData.CurrentStep = 0;
            missionData.IsComplete = false;
            _dailyMissionManager.MissionDatas.Add(missionData);
            ButtonMission.newMission++;
        }

        dailyMissionManager.Init(_dailyMissionManager);
    }

    public void ResetWeeklyMission()
    {
        Debug.LogError("ResetWeeklyMission");

        weeklyMissionManager?.OnDestroy();
        weeklyMissionManager = new MissionDurationManager();

        _userData.MissionManagerData.Weekly = new MissionDurationData();
        _weeklyMissionManager = _userData.MissionManagerData.Weekly;
        _weeklyMissionManager.Duration = 7;

        var startDay = TimeService.instance.GetCurrentDateTime().StartOfWeek(DayOfWeek.Monday);
        _weeklyMissionManager.StartDate = startDay.DayOfYear;

        // Debug.LogError(startDay);
        var weeklyDesign =
            DesignHelper.GetMissionDurationDesign(_weeklyMissionManager.Duration, SaveGameHelper.GetMaxCampaignLevel());

        var missionDesigns = weeklyDesign.GetMissionsDesign();
        foreach (var VARIABLE in missionDesigns)
        {
            MissionData missionData = new MissionData {Id = VARIABLE.Id, CurrentStep = 0, IsComplete = false};
            _weeklyMissionManager.MissionDatas.Add(missionData);
            ButtonMission.newMission++;
        }

        weeklyMissionManager.Init(_weeklyMissionManager);
    }

    public void CheckToReset()
    {
        int currentDate = TimeService.instance.GetCurrentDateTime().DayOfYear;

        if (currentDate != _dailyMissionManager.StartDate)
        {
            ResetDailyMission();
        }

        if (currentDate < _weeklyMissionManager.StartDate || (currentDate - _weeklyMissionManager.StartDate) >= 7)
        {
            ResetWeeklyMission();
        }
    }

    public void TriggerMission(MissionType missionType, params object[] pr)
    {
        EventSystemServiceStatic.DispatchAll(missionType.ToString(), pr);
    }

    public void CheckLoginMission()
    {
        if (TimeService.instance.GetCurrentDateTime().DayOfYear !=
            SaveManager.Instance.Data.MissionManagerData.LastCheckLoginMission)
        {
            TriggerMission(MissionType.LOGIN);
            SaveManager.Instance.Data.MissionManagerData.LastCheckLoginMission =
                TimeService.instance.GetCurrentDateTime().DayOfYear;
        }
    }
}