using System;
using UnityEngine;

public class AutomaticWeapon : Weapon
{
    private bool IsPause = false;
    private float timerPerform = 0f;
    public bool DisableAutomatic = false;

    private void OnEnable()
    {
        nextFireTime = -1;
    }

    public override void UpdateBehaviour(float _deltaTime)
    {
        if (IsPause || !enabled || DisableAutomatic)
            return;

        if (this.CanFire())
        {
            this.FireWeapon();
        }
    }

    public override void FireWeapon()
    {
        //this.nextFireTime = Time.time + ratePerSec;
        this.IsPerformingShoot = true;
        timerPerform = 0f;
        base.FireWeapon();
    }

    public override void SetPerfomShoot(bool value)
    {
        float ratePerSec = 1.0f / (this.fireRate * 1.0f + this.attrFireRate * 1.0f);
        this.nextFireTime = Time.time + ratePerSec * (1.0f + this._ReduceSpeedMultiplier * 1.0f / 100f);
        base.SetPerfomShoot(value);
    }

    protected override bool CanFire()
    {
        if (this.IsPerformingShoot)
        {
            timerPerform += Time.deltaTime;
            if (timerPerform >= 2.0f)
            {
                IsPerformingShoot = false;
            }
            return false;
        }
        else
        {
            return Time.time >= this.nextFireTime;
        }

    }

    public override void SetPauseBehaviour(bool _value)
    {
        base.SetPauseBehaviour(_value);
        IsPause = _value;
    }

}
