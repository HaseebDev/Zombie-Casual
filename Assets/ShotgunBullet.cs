using Ez.Pooly;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBullet : Bullet
{
    public static int pellets = 6;
    private Ray _ray;
    private Collider[] _hits;

    private void Start()
    {
        _ray = new Ray();
        _hits = new Collider[10];
    }

    public override void Initialize(List<EffectHit> listEffects, string _OwnerID, LayerMask targetMask, float rangeBullet = float.PositiveInfinity)
    {
        base.Initialize(listEffects, _OwnerID, targetMask);

    }

    public override void Launch(float _force, float _damage, ShotType _type, Action onHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        this.MultiplyPerHit = 1.0f;
        this.bulletSpeed = _force;
        this.bulletDamage = _damage;
        this.shotType = _type;
        this.OnHit = onHit;
        this.IsLaunch = true;

        if (_bulletTrail)
        {
            _bulletTrail.Clear();
            _bulletTrail.enabled = true;
            _bulletTrail.emitting = true;
        }

        if (_parProjectile != null)
        {
            _parProjectile.Clear(true);
            _parProjectile.time = 0;
            _parProjectile.Play(true);
        }

        if (shotType == ShotType.PIERCE)
        {
            Timing.CallDelayed(5.0f, () =>
            {
                DestroyBullet();
            });
        }

        _ray.origin = transform.position;
        _ray.direction = transform.forward;
        var isHit = Physics.Raycast(_ray, out singleHit, float.PositiveInfinity, _maskHit);
        if (isHit)
        {
            CastDmg(singleHit, this.bulletDamage);
            if (!GameMaster.IsSpeedUp && GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                GameMaster.instance.PlayEffect(_parImpact, singleHit.transform.position, Quaternion.identity, singleHit.transform);
            OnDealAoeDmg(singleHit.transform.position, 0.5f, singleHit.transform.gameObject.GetInstanceID());
        }


    }

    public override void OnDealAoeDmg(Vector3 startPos, float radius, int firstHitZombieID)
    {
        int hits = Physics.OverlapSphereNonAlloc(startPos, radius, _hits, _maskHit);
        if (hits > 0)
        {

            for (int i = 0; i < hits; i++)
            {
                var col = _hits[i];
                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null && col.gameObject.GetInstanceID() != firstHitZombieID)
                {
                    component.SetDamage(this.bulletDamage * 0.5f, ShotType.FIRE, this.OwnerID);
                    if (!component.IsDead() && !GameMaster.IsSpeedUp && GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                        GameMaster.instance.PlayEffect(_parImpact, col.transform.position, Quaternion.identity, col.transform);
                }

            }


        }
    }

}
