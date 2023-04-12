using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPurchaseHeroTutorial : BaseTutorialBehavior
{
    public string heroId;

    public override void OnEnter()
    {
        OnCompleteShowEvent();
    }

    public override void OnUpdate()
    {
        // base.OnUpdate();

        if (HasEnoughRequire())
        {
            // var _heroData = SaveManager.Instance.Data.GetHeroData(heroId);
            // MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_PURCHASE_HERO_INGAME, false, null, _heroData, null);
            OnExit();
        }
    }
}