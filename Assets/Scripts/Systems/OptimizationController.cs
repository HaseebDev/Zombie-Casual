using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class OptimizeData
{
    public bool EnableTextDmg;
    public bool EnableImpactHit;
    public bool EnableBlood;
    public bool EnableFlashWhenHit;

    public void DisableAll()
    {
        EnableTextDmg = false;
        EnableFlashWhenHit = false;
        EnableBlood = false;
        EnableImpactHit = false;
    }

    public void EnableAll()
    {
        EnableTextDmg = true;
        EnableFlashWhenHit = true;
        EnableBlood = true;
        EnableImpactHit = true;
    }
}

public class OptimizationController : BaseSystem<OptimizationController>
{
    public OptimizeData Data { get; private set; }

    private OptimizeData _lastProfile;
    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);
        Data = new OptimizeData()
        {
            EnableTextDmg = true,
            EnableImpactHit = true,
            EnableBlood = true,
            EnableFlashWhenHit = true
        };
        _lastProfile = Data;
    }


    #region GET/ SET
    public void SetEnableText(bool _value)
    {
        Data.EnableTextDmg = _value;
    }

    public void SetEnableImpactHit(bool _value)
    {
        Data.EnableImpactHit = _value;
    }

    public void SetEnableBlood(bool _value)
    {
        Data.EnableBlood = _value;
    }

    public void SetEnableFlashWhenHit(bool _value)
    {
        Data.EnableFlashWhenHit = _value;
    }
    #endregion

    public void DisableAllFX()
    {
        _lastProfile = Data;
        Data.DisableAll();
    }

    public void ResumeLastProfile()
    {
        Data.EnableAll();
    }

}
