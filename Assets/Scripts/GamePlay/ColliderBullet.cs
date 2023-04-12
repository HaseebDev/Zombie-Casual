using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderBullet : Bullet
{
    private bool isHit = false;

    private float _timerDestroy = 0f;
    public override void Launch(float _force, float _damage, ShotType _type, Action onHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        this.MultiplyPerHit = 1.0f;
        this.bulletSpeed = _force;
        this.bulletDamage = _damage;
        this.shotType = _type;
        this.OnHit = onHit;
        this.IsLaunch = true;
        isHit = false;
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

        _timerDestroy = 0f;

        if (shotType == ShotType.PIERCE)
        {
            Timing.CallDelayed(3.0f, () =>
            {
                DestroyBullet();
            });
        }
    }

    public override void Update()
    {
        if (IsLaunch)
        {
            var _deltaTime = Time.deltaTime;
            base.transform.position += base.transform.forward * _deltaTime * this.bulletSpeed;
            _timerDestroy += _deltaTime;

            if (_timerDestroy >= 2.0f)
            {
                DestroyBullet();
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!GameMaster.IsZombieTag(collision.tag) || isHit)
        {
            return;
        }

        isHit = true;
        IHealth component = collision.transform.GetComponent<IHealth>();
        if (component != null)
        {
            var spawnPoint = component.HitMarker != null ? component.HitMarker.position : collision.transform.position;

            var hitPoint = collision.transform.position;
            component.SetDamage(this.bulletDamage, this.shotType, this.OwnerID, this.effectHits);
            if (!component.IsDead() && !GameMaster.IsSpeedUp && GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                GameMaster.instance.PlayEffect(_parImpact, spawnPoint, collision.transform.rotation, collision.transform.parent);
        }

        if (this.shotType != ShotType.PIERCE)
        {
            DestroyBullet();
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!GameMaster.IsZombieTag(collision.tag) || isHit)
        {
            return;
        }

        isHit = true;
        IHealth component = collision.transform.GetComponent<IHealth>();
        if (component != null)
        {
            var spawnPoint = component.HitMarker != null ? component.HitMarker.position : collision.transform.position;

            var hitPoint = collision.transform.position;
            component.SetDamage(this.bulletDamage, this.shotType, this.OwnerID, this.effectHits);
            if (!component.IsDead() && !GameMaster.IsSpeedUp && GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                GameMaster.instance.PlayEffect(_parImpact, spawnPoint, collision.transform.rotation, collision.transform.parent);
        }

        if (this.shotType != ShotType.PIERCE)
        {
            DestroyBullet();
        }
    }
}

