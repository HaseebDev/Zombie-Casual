using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickEngine.Extensions;
using QuickType;
using UnityEngine;

public class GetFreeResourceMission : BaseMission
{
    private REWARD_TYPE _rewardType = REWARD_TYPE.NONE;

    public override void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        base.InitWithMissionData(missionData, missionDurationData);
        if (!missionDesignElement.Extend.IsNullOrEmpty())
        {
            _rewardType = missionDesignElement.Extend.ToEnum<REWARD_TYPE>();
        }
    }

    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.GET_FREE_RESOURCE.ToString(),new Action<REWARD_TYPE>(OnGetFreeResource));
    }
    
    public void OnGetFreeResource(REWARD_TYPE type)
    {
        if (_rewardType == REWARD_TYPE.NONE || _rewardType == type)
        {
            Increase();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.GET_FREE_RESOURCE.ToString(),new Action<REWARD_TYPE>(OnGetFreeResource));
    }
}
