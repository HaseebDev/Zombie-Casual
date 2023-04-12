using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualWeapon : Weapon
{
    public override void UpdateBehaviour(float _deltaTime)
    {
    }

    protected override bool CanFire()
    {
        return true;
    }
    
}
