using System;
using System.Collections;
using System.Collections.Generic;
using QuickType;
using UnityEngine;

public class OpenChestMission : BaseMission
{
    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, MissionType.OPEN_CHEST.ToString(),
            new Action<int>(OnOpenChest));
    }

    public void OnOpenChest(int times)
    {
        for (int i = 0; i < times; i++)
            Increase();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, MissionType.OPEN_CHEST.ToString(),
            new Action<int>(OnOpenChest));
    }
}