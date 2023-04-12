using System;
using System.Collections;
using System.Collections.Generic;
using QuickType;
using UnityEngine;

public class TriggerTypeMission : BaseMission
{
    private MissionType _type;
    
    public TriggerTypeMission(MissionType type)
    {
        _type = type;
    }
    
    public override void InitCallback()
    {
        base.InitCallback();
        EventSystemServiceStatic.AddListener(this, _type.ToString(),new Action(Increase));
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, _type.ToString(),new Action(Increase));
    }
}
