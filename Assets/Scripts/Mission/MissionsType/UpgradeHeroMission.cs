using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickEngine.Extensions;
using QuickType;
using UnityEngine;

public class UpgradeHeroMission : BaseMission
{
    private string _heroID;

    public override void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        base.InitWithMissionData(missionData, missionDurationData);
        _heroID = missionDesignElement.Extend;
    }

    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.UPGRADE_HERO.ToString(),
            new Action<string>(OnUpgradeHero));
    }


    public void OnUpgradeHero(string heroID)
    {
        if (_heroID.IsNullOrEmpty() || _heroID == heroID)
        {
            Increase();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.UPGRADE_HERO.ToString(),
            new Action<string>(OnUpgradeHero));
    }
}
