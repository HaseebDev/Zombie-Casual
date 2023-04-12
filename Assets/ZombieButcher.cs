using QuickType.Zombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieButcher : Zombie
{
    private Collider[] _colliders;

    private float dmgExplode;

    public override void Start()
    {
        base.Start();
        _colliders = new Collider[40];
    }

    public override void Initialize(ZombieElement data, float hpMultiplier, float dmgMultiplier, int wave, int subWave, int index)
    {
        base.Initialize(data, hpMultiplier, dmgMultiplier, wave, subWave, index);
        this.zmAnimator.SetBool("attack_02", false);
        dmgExplode = this._data.Dmg * this._data.Value * 1.0f / 100f;

        if (this._animationEvents != null)
        {
            this._animationEvents.OnShootEvent02 = CallbackAnimExplode;
        }
    }

    public void CallbackAnimExplode()
    {
        base.Health_OnDie(true);
        GameMaster.PlayEffect(COMMON_FX.FX_EXPLODE_GREEN, transform.position, Quaternion.identity, null, 2.0f);
        var numHit = Physics.OverlapSphereNonAlloc(transform.position, _data.Radius, _colliders, ResourceManager.instance._maskHeroAndZombie);
        if (numHit > 0)
        {
            for (int i = 0; i < numHit; i++)
            {
                var hit = _colliders[i];

                var health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.SetDamage(dmgExplode);
                }
            }
        }
    }


    public override void Health_OnDie(bool skipAnimDead)
    {
        this.zmAnimator.SetBool("attack", false);
        this.zmAnimator.SetBool("attack_02", true);
    }
}
