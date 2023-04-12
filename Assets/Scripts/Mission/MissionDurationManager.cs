using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using Google.Protobuf.Collections;
using QuickType;
using UnityEngine;

public class MissionDurationManager
{
    public List<BaseMission> missions;

    public void Init(MissionDurationData missionDurationData)
    {
        OnDestroy();
        missions = new List<BaseMission>();

        foreach (var missionData in missionDurationData.MissionDatas)
        {
            MissionDesignElement missionDesignElement = DesignHelper.GetMissionDesign(missionData.Id);
            BaseMission baseMission = null;

            switch (missionDesignElement.Type)
            {
                case MissionType.KILL_ZOMBIE:
                    baseMission = new KillZombieMission();
                    break;
                case MissionType.FINISH_LEVEL:
                    baseMission = new CompleteLevelMission();
                    break;
                case MissionType.UPGRADE_HERO:
                    baseMission = new UpgradeHeroMission();
                    break;
                case MissionType.USE_ULTIMATE:
                    baseMission = new UseUltimateMission();
                    break;
                case MissionType.USE_ADDON:
                    baseMission = new UserAddOnMission();
                    break;
                case MissionType.OPEN_CHEST:
                    baseMission = new OpenChestMission();
                    break;
                case MissionType.UPGRADE_EQUIPMENT:
                    baseMission = new UpgradeEquipMission();
                    break;
                case MissionType.GET_FREE_RESOURCE:
                    baseMission = new GetFreeResourceMission();
                    break;
                case MissionType.LOGIN:
                case MissionType.CLAIM_OFFLINE_REWARD:
                case MissionType.FUSION_EQUIP: 
                case MissionType.WATCH_VIDEO_OPEN_CHEST:
                    baseMission = new TriggerTypeMission(missionDesignElement.Type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (baseMission != null)
            {
                baseMission.InitWithMissionData(missionData, missionDurationData);
                missions.Add(baseMission);
            }
        }
    }

    public void OnDestroy()
    {
        if (missions == null || missions.Count == 0) return;

        foreach (var VARIABLE in missions)
        {
            VARIABLE.OnDestroy();
        }

        missions.Clear();
    }
}