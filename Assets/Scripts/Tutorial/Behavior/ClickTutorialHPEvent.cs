using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTutorialHPEvent : BaseTutorialBehavior
{
    public float percentActive = 0.5f;

    public override void OnEnter()
    {
        Debug.LogError("On enter HP event");
        behaviorType.onExit = OnExit;
        behaviorType.OnEnter();

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_CASTLE_HP,
            new Action<float, float>(OnCastleDamage));
    }

    private void OnCastleDamage(float percent, float currentHP)
    {
        if (percent <= percentActive && !completeShowEvent)
        {
            if (GamePlayController.instance != null && GamePlayController.instance.CurrentLevel == 3)
            {
                completeShowEvent = true;
            }
        }
    }


    // public override void OnUpdate()
    // {
    //     // base.OnUpdate();
    //     if (!showed && completeShowEvent && PreHUDRequirement())
    //     {
    //         Debug.LogError("SHOW " + behaviorType.name);
    //         behaviorType.OnEnter();
    //         showed = true;
    //     }
    //
    //     if (showed)
    //         behaviorType.OnUpdate();
    // }
}