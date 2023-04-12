using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType;
using UnityEngine;

public class BaseMission
{
    public MissionDurationData missionDurationData;
    public MissionData missionData;
    public MissionDesignElement missionDesignElement;
    private Action _onComplete;

    public virtual void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        this.missionDurationData = missionDurationData;
        this.missionData = missionData;
        missionDesignElement = DesignHelper.GetMissionDesign(missionData.Id);
        InitCallback();
    }

    public virtual void InitCallback()
    {
    }

    public virtual void OnDestroy()
    {
    }

    public virtual void Increase()
    {
        if(!missionData.IsComplete)
            missionData.CurrentStep++;
        
        if (!missionData.IsComplete && missionData.CurrentStep >= missionDesignElement.Total)
        {
            OnComplete();
        }
        
        // Debug.LogError($"Increase step mission {missionData.Id}, current step {missionData.CurrentStep}, total {missionDesignElement.Total}");
    }

    public virtual void OnComplete()
    {
        Debug.LogError($"Complete mission {missionData.Id}");

        missionData.IsComplete = true;
        _onComplete?.Invoke();

        bool isCompleteAll = true;
        
        foreach (var VARIABLE in missionDurationData.MissionDatas)
        {
            if (!VARIABLE.IsComplete)
            {
                isCompleteAll = false;
                break;
            }
        }

        missionDurationData.IsComplete = isCompleteAll;
    }
    
}