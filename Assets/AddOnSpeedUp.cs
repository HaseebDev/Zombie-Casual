using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnSpeedUp : BaseExpiryUltimate
{
    private bool IsTriggerTimeScaleUp = false;
    private float oldTimeScale = 1.0f;

    private float TimeScaleUp { get { return _design.Number; } }
    private float timerTimeScale = 0f;

    public override void NotifyDontHasAddon()
    {
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        if (base.PointerUpSkill(screenPos, checkValidCast))
        {
            if (IsEnableUltimate)
            {
                oldTimeScale = 1.0f;
                Time.timeScale = 2.0f;
                timerTimeScale = Duration;
            }
            return true;
        }
        else
        {
            Time.timeScale = oldTimeScale;
            return false;
        }

    }

    public override void EnableUltimate(bool enable)
    {
        base.EnableUltimate(enable);
        if (enable)
        {
            oldTimeScale = 1.0f;
            Time.timeScale = 2.0f;
            timerTimeScale = Duration;
        }
        else
        {
            Time.timeScale = oldTimeScale;
        }
    }

    public override void UpdateSkill(float deltaTime)
    {
        if (GamePlayController.instance.IsPausedGame)
            return;
        
        if (IsEnableUltimate && !_addOnItem.IsUnlimitedItem)
        {
            timerTimeScale -= Time.unscaledDeltaTime;
            _addOnItem.ExpiredDuration = (long)timerTimeScale;
            if (timerTimeScale <= 0)
            {
                Time.timeScale = oldTimeScale;
                IsTriggerTimeScaleUp = false;
                timerTimeScale = 0f;
                IsEnableUltimate = false;
            }
        }
        base.UpdateSkill(deltaTime);

        //else if (Time.timeScale != oldTimeScale)
        //{
        //    Time.timeScale = oldTimeScale;
        //}
    }

    public override void CleanUp()
    {
        base.CleanUp();

        Time.timeScale = oldTimeScale;
        IsEnableUltimate = false;
    }


}
