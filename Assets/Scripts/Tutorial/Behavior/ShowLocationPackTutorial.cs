using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLocationPackTutorial : BaseTutorialBehavior
{
    public override void OnEnter()
    {
        OnCompleteShowEvent();
    }

    public override void OnUpdate()
    {
        // base.OnUpdate();

        if (HasEnoughRequire())
        {
            var hudHome = FindObjectOfType<HUDHomeMenu>();
            if (hudHome != null)
            {
                hudHome.ShowPromotionShop();
                OnExit();
            }
        }
    }
}