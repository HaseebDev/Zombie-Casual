using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Ez.Pooly;
using UnityEngine;

public class ObstacleExplode : ObstacleProtectGate
{
    private Collider[] _hitColliders;

    private float _damage = 10000f;
    private float _radius = 3;
    
    private void Start()
    {
        _hitColliders = new Collider[30];
    }

    public void SetDamage(float dmg)
    {
        _damage = dmg;
    }

    public override void OnDie(bool skipAnimDead)
    {
        if (skipAnimDead)
        {
            Pooly.Despawn(this.transform);
            return;
        }
        
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_BOMB_EXPLODE_01);

        int numCollide = Physics.OverlapSphereNonAlloc(transform.position, _radius, _hitColliders,
            ResourceManager.instance._maskZombieOnly);

        if (numCollide > 0)
        {
            int num = numCollide; // >= _hitColliders.Length ? _hitColliders.Length : numCollide;
            for (int i = num - 1; i >= 0; i--)
            {
                var col = _hitColliders[i];
                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null)
                {
                    component.SetDamage(_damage, ShotType.AOE);
                }
            }
        }
        
        GameMaster.PlayEffect(COMMON_FX.FX_EXPLODE_BOOM_01,transform.position,Quaternion.identity,scale:1.5f);
        Pooly.Despawn(this.transform);
    }

    private void OnDestroy()
    {
        _hitColliders = null;
    }
}