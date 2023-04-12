using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickEngine.Extensions;
using QuickType;
using UnityEngine;

public class UseUltimateMission : BaseMission 
{
    private string _ultimateID;
    private string _lastUltimateID;
    private float _lastTime;
    
    public override void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        base.InitWithMissionData(missionData, missionDurationData);
        _ultimateID =  missionDesignElement.Extend; 
    }

    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.USE_ULTIMATE.ToString(),
            new Action<string>(OnUseUltimate));
    }


    public void OnUseUltimate(string ultimateID)
    {
        if (_ultimateID.IsNullOrEmpty() || ultimateID.Contains(_ultimateID))
        {
            if (ultimateID == _lastUltimateID)
            {
                if (Time.time - _lastTime > 1f)
                {
                    Increase();
                    _lastTime = Time.time;
                }
            }
            else
            {
                Increase();
                _lastTime = Time.time;
            }

            _lastUltimateID = ultimateID;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.USE_ULTIMATE.ToString(),
            new Action<string>(OnUseUltimate));
    }
}
