using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickEngine.Extensions;
using QuickType;
using UnityEngine;

public class KillZombieMission : BaseMission
{
    private string _zombieId;

    public override void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        base.InitWithMissionData(missionData, missionDurationData);
        _zombieId = missionDesignElement.Extend;
    }

    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.KILL_ZOMBIE.ToString(),new Action<string>(OnZombieKilled));
    }
    

    public void OnZombieKilled(string zombieId)
    {
        if (_zombieId.IsNullOrEmpty() || zombieId == _zombieId )
        {
            Increase();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.KILL_ZOMBIE.ToString(),new Action<string>(OnZombieKilled));
    }
}
