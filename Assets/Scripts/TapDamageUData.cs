using System;
using UnityEngine;

public class TapDamageUData : CountdownEffect, IUpgradableFloatValue
{

    public UpgradeableObject upgradeData;

    public float GetUpgradableDmg {
        get {
            return upgradeData.Value * this.upgradableDmg;
        }
    }

    public override void ApplyEffect()
    {
        this.doubleReward = 2;
    }

    public override void ResetEffect()
    {
        this.doubleReward = 1;
    }

    public void SetUpgradableFloatValue(float _value)
    {
        this.upgradableDmg = 1f;
    }

    [SerializeField]
    private float upgradableDmg = 1f;

    private int doubleReward = 1;
}
