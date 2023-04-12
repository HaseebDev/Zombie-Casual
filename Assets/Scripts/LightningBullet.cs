using DigitalRuby.LightningBolt;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBullet : Bullet
{
    public LightningBoltScript light;

    public Transform _markerLight;

    public override void Update()
    {
        //do nothing
    }

    public override void Launch(float _force, float _damage, ShotType _type, Action onHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        this.MultiplyPerHit = 1.0f;

        if (this._overwriteForce > 0)
            this.bulletSpeed = _overwriteForce;
        else
            this.bulletSpeed = _force;

        this.bulletDamage = _damage;
        this.shotType = _type;
        this.OnHit = onHit;
        this.IsLaunch = true;
        //rb.velocity = Vector3.zero;
        //rb.isKinematic = true;
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
        if (this.shotType == ShotType.PIERCE)
        {
            hitCount = Physics.RaycastNonAlloc(_ray, raycastHits, float.PositiveInfinity, _maskHit);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var hit = raycastHits[i];
                    CastDmg(hit, this.bulletDamage);
                    if(i==0)
                    {
                        light.Initialize();
                        light.PlayAnim(_markerLight.transform.position, singleHit.transform.position, 1.0f,
                            () => { });
                    }
                }
            }
        }
        else
        {
            var isHit = Physics.Raycast(_ray, out singleHit, float.PositiveInfinity, _maskHit);
            if (isHit)
            {
                CastDmg(singleHit,this.bulletDamage, _type);
                light.Initialize();
                light.PlayAnim(_markerLight.transform.position, singleHit.transform.position, 1.0f,
                    () => { });
            }

        }

        if (singleHit.transform != null)
        {
            targetPos = singleHit.transform.position;
            targetPos.y = transform.position.y;
        }
    }
}
