using com.datld.data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnAutoManualHero : BaseExpiryUltimate
{
    private Vector3 randWorldPos;
    private int TapRate { get { return Mathf.RoundToInt(_design.Number); } }
    private float timerTapRate = 0f;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        this.randWorldPos = GamePlayController.instance.gameLevel._skillZoneMarker.centerSkillMarker.position;
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        if (base.PointerUpSkill(screenPos, checkValidCast))
        {
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_TOUCH_GROUND, randWorldPos,false);
            timerTapRate = 0f;
            return true;
        }
        return false;
    }
    
    public override void NotifyDontHasAddon()
    {
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);
        if (IsEnableUltimate)
        {
            timerTapRate += deltaTime;
            if (timerTapRate >= 1.0f / TapRate * 1.0f)
            {
                EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_TOUCH_GROUND, randWorldPos,false);
                timerTapRate = 0f;
                _addOnItem.ExpiredDuration = (long)_timerCountDown;
            }
        }


    }

}
