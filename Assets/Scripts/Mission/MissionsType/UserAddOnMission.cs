using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickEngine.Extensions;
using QuickType;
using UnityEngine;

public class UserAddOnMission : BaseMission
{
    private string _addonID;
    private string _lastAddonID;
    private float _lastTime;
    
    public override void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        base.InitWithMissionData(missionData, missionDurationData);
        _addonID =  missionDesignElement.Extend; 
    }

    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.USE_ADDON.ToString(),
            new Action<string>(OnUseAddon));
    }

    public void OnUseAddon(string addonId)
    {
        if (_addonID.IsNullOrEmpty() || addonId.Contains(_addonID))
        {
            if (addonId == _lastAddonID)
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

            _lastAddonID = addonId;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.USE_ADDON.ToString(),
            new Action<string>(OnUseAddon));
    }
}
