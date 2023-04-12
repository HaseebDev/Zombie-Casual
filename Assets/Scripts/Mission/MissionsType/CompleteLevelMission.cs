using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickEngine.Extensions;
using QuickType;
using UnityEngine;

public class CompleteLevelMission : BaseMission
{
    private GameMode _gameMode = GameMode.NONE;

    public override void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        base.InitWithMissionData(missionData, missionDurationData);
        if (!missionDesignElement.Extend.IsNullOrEmpty())
        {
            _gameMode = missionDesignElement.Extend.ToEnum<GameMode>();
        }
    }

    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.FINISH_LEVEL.ToString(),
            new Action<GameMode>(OnCompleteLevel));
    }


    public void OnCompleteLevel(GameMode gameMode)
    {
        if (_gameMode == GameMode.NONE || _gameMode == gameMode)
        {
            Increase();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.FINISH_LEVEL.ToString(),
            new Action<GameMode>(OnCompleteLevel));
    }
}