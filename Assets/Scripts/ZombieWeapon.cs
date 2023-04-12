using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWeapon : Weapon
{
    //replace later
    private float FindRange = 0.13f;

    [SerializeField]
    private Health target;

    private bool reachedRange = false;

    public override void Initialize(Launcher targetLauncher)
    {
        base.Initialize(targetLauncher);
        reachedRange = false;
    }

    public void UpdateTarget(Health _target)
    {
        target = _target;
    }

    protected override bool CanFire()
    {
        return Time.time >= this.nextFireTime;
    }

    public bool ReachedRange()
    {
        bool valid = false;

        if (this.target != null && !reachedRange)
        {
            if (Vector3.Distance(target.transform.position, transform.position) <= fireRange)
            {
                valid = true;
            }
        }
        else
            valid = reachedRange;

        return valid;
    }
}
