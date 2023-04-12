using System;
using System.Collections;
using System.Collections.Generic;
using Ez.Pooly;
using TMPro;
using UnityEngine;

public class HUDMission : BaseHUD
{
    public MissionDurationItemUI dailyTab;
    public MissionDurationItemUI weeklyTab;

    public static float _lastClaimTime = 0;
    
    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        ShowDaily();
    }

    public override void ResetLayers()
    {
        base.ResetLayers();
        
        dailyTab.Init();
        weeklyTab.Init();
        
        Init();
        dailyTab.ResetLayer();
        weeklyTab.ResetLayer();
    }

    public void ShowDaily()
    {
        dailyTab.Show();
        weeklyTab.Hide();
    }

    public void ShowWeekly()
    {
        dailyTab.Hide();
        weeklyTab.Show();
    }
}