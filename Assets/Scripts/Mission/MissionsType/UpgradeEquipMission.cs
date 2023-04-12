using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickEngine.Extensions;
using QuickType;
using UnityEngine;

public class UpgradeEquipMission : BaseMission
{
    private string _equipmentID;
    
    public override void InitWithMissionData(MissionData missionData, MissionDurationData missionDurationData)
    {
        base.InitWithMissionData(missionData, missionDurationData);
        _equipmentID = missionDesignElement.Extend;
    }

    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.UPGRADE_EQUIPMENT.ToString(),
            new Action<string>(OnUpgradeEquipment));
    }

    public void OnUpgradeEquipment(string equipmentID)
    {
        if (_equipmentID.IsNullOrEmpty() || equipmentID.Contains(_equipmentID))
        {
            Increase();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.UPGRADE_EQUIPMENT.ToString(),
            new Action<string>(OnUpgradeEquipment));
    }
}
