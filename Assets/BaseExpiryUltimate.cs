using com.datld.data;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExpiryUltimate : BaseAddOnUltimate
{
    protected long Duration { get { return (long)_design.Duration; } }
    protected bool IsEnableUltimate { get; set; }

    protected float _timerCountDown;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);

        //force disable first time
        //if (_addOnItem.Status == ITEM_STATUS.Choosing)
        //    _addOnItem.Status = ITEM_STATUS.Available;

        //GamePlayController.instance.OnStartGame += OnStartGame;

        this.EnableUltimate(_addOnItem.Status == ITEM_STATUS.Choosing);

        //_addOnItem.ExpiredDuration = (long)_design.Value;
        _timerCountDown = _addOnItem.ExpiredDuration;

    }

    private void OnStartGame()
    {
        //this.EnableUltimate(_addOnItem.Status == ITEM_STATUS.Choosing);
    }

    public virtual void EnableUltimate(bool enable)
    {
        if (enable)
        {
            IsEnableUltimate = true;
            if (_addOnItem.ExpiredDuration <= 0)
                _addOnItem.ExpiredDuration = (long)DesignSkill.Duration;
            _addOnItem.Status = ITEM_STATUS.Choosing;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_TIMER, _design.SkillId, true, (long)_addOnItem.ExpiredDuration, (long)_design.Duration);
            _timerCountDown = _addOnItem.ExpiredDuration;
        }
        else
        {
            //pause this
            IsEnableUltimate = false;
            //_addOnItem.Status = ITEM_STATUS.Available;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_TIMER, _design.SkillId, false, (long)_addOnItem.ExpiredDuration, (long)_design.Duration);
            _timerCountDown = _addOnItem.ExpiredDuration;
        }
    }

    public override void UpdateSkill(float deltaTime)
    {
        //Logwin.Log($"[skill debug - {SkillID}]", $"skill: {SkillID} - isEnable: {IsEnableUltimate} remain: {_addOnItem.ExpiredDuration}");
        base.UpdateSkill(deltaTime);

        if (IsEnableUltimate && !this._addOnItem.IsUnlimitedItem)
        {
            _timerCountDown -= Time.unscaledDeltaTime;
            _addOnItem.ExpiredDuration = (long)_timerCountDown;
            if (_timerCountDown <= 0)
            {
                _timerCountDown = 0f;
                IsEnableUltimate = false;

                _addOnItem.ExpiredDuration = (long)_timerCountDown;
                _addOnItem.Status = ITEM_STATUS.Available;
            }
        }
    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        base.PointerDownSkill(screenPos);
        IsEnableUltimate = false;
        _timerCountDown = 0f;
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        bool baseCheck = base.PointerUpSkill(screenPos, checkValidCast);

        if (_addOnItem.Status == ITEM_STATUS.Choosing)
        {
            EnableUltimate(false);
            _addOnItem.Status = ITEM_STATUS.Locked;
            return false;
        }
        else if (_addOnItem.Status == ITEM_STATUS.Locked || _addOnItem.IsUnlimitedItem)
            baseCheck = true;


        if (baseCheck)
        {
            EnableUltimate(true);
            return true;
        }
        return false;
    }

    public override void OnSaveGame()
    {
        base.OnSaveGame();
        SaveManager.Instance.SetDataDirty();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        //if (_addOnItem.Status == com.datld.data.ITEM_STATUS.Choosing)
        //    _addOnItem.Status = com.datld.data.ITEM_STATUS.Locked;

        // GamePlayController.instance.OnStartGame -= OnStartGame;
    }

}
