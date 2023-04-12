using System;
using Adic;
using UnityEngine;

public class WeaponSoundPlayer : MonoBehaviour
{
    private void Start()
    {
        this.Inject();
        this.weapon = base.GetComponentInParent<Weapon>();
        Weapon weapon = this.weapon;
        weapon.OnLaunch = (Action<float, float, ShotType>)Delegate.Combine(weapon.OnLaunch, new Action<float, float, ShotType>(this.Weapon_OnLaunch));
    }

    private void Weapon_OnLaunch(float arg1, float arg2, ShotType type)
    {
        //AudioSystem.instance.PlaySFX(this.soundEnum);
    }

    private void OnDestroy()
    {
        Weapon weapon = this.weapon;
        weapon.OnLaunch = (Action<float, float, ShotType>)Delegate.Remove(weapon.OnLaunch, new Action<float, float, ShotType>(this.Weapon_OnLaunch));
    }

    private Weapon weapon;

    [SerializeField]
    private SFX_ENUM soundEnum;
}
