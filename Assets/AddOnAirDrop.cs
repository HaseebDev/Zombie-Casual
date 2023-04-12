using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnAirDrop : BaseAddOnUltimate
{
    private bool IsReadyToUse = false;
    private float _timerReadyUse = 0f;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);

        if (this._addOnItem.ReadyTS <= 0)
            this._addOnItem.ReadyTS = TimeService.instance.GetCurrentTimeStamp(true);

        //IsReadyToUse = this._addOnItem.ExpiredDuration < 0;
        //if (IsReadyToUse)
        //{
        //    this._addOnItem.Status = com.datld.data.ITEM_STATUS.Available;
        //}
        ResetState();

    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        base.PointerDownSkill(screenPos);

    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        bool result = false;
        if (IsReadyToUse && base.PointerUpSkill(screenPos, checkValidCast))
        {
            this._addOnItem.ExpiredDuration = (long)_design.Duration;
            this._addOnItem.ReadyTS = (TimeService.instance.GetCurrentTimeStamp(true) + (long)_design.Duration);
            result = true;
        }
        else
        {
            //if (_timerReadyUse > 0)
            //    InGameCanvas.instance.ShowFloatingTextNotify($"Remaining time: {TimeService.FormatTimeSpan(_timerReadyUse)}");
        }

        ResetState();
        return result;
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);

        if (!IsReadyToUse)
        {
            this._addOnItem.ExpiredDuration = (long)_timerReadyUse;
            _timerReadyUse -= Time.unscaledDeltaTime;
            if (_timerReadyUse <= 0)
            {
                _addOnItem.ExpiredDuration = 0;
                IsReadyToUse = true;
                _timerReadyUse = 0f;
                this._addOnItem.Status = com.datld.data.ITEM_STATUS.Available;
                EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_READY_USE, _design.SkillId, true, 0, (long)_design.Duration);
            }
        }
    }

    public void ResetState()
    {
        _timerReadyUse = _addOnItem.ReadyTS - TimeService.instance.GetCurrentTimeStamp(true);
        // Debug.Log($"TimeLeft: {_timerReadyUse}");
        if (IsReachedLimit())
        {
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_REACHED_LIMIT, _design.SkillId);
        }

        if (_timerReadyUse > 0)
        {
            _addOnItem.ExpiredDuration = (long)_timerReadyUse;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_READY_USE, _design.SkillId, true, _timerReadyUse, (long)_design.Duration);
            IsReadyToUse = false;
            this._addOnItem.Status = com.datld.data.ITEM_STATUS.Locked;
        }
        else
        {
            IsReadyToUse = true;
            this._addOnItem.ExpiredDuration = 0;
            this._addOnItem.Status = com.datld.data.ITEM_STATUS.Available;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_READY_USE, _design.SkillId, true, 0, (long)_design.Duration);
        }
    }

    public override void NotifyDontHasAddon()
    {
    }

    private bool IsReachedLimit()
    {
        bool result = false;

        var airdropDesign = DesignHelper.GetSkillDesign(this._addOnItem.ItemID);
        //check max earn today
        if (SaveManager.Instance.Data.DayTrackingData.TodayEarnAddonAirDrop >= airdropDesign.Number)
        {
            result = true;
        }
        return result;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            Debug.Log("[AddOnAirDrop] OnApplicationPause = " + pause);
            ResetState();
        }
    }
}
