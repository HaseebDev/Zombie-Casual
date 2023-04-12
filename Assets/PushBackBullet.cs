using Ez.Pooly;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBackBullet : Bullet
{
    private bool shooted = false;
    float bulletSpeed = 0f;
    float bulletDamage = 0f;
    ShotType shotType = ShotType.NORMAL;
    private float timerLifeTime = 0f;
    private float LifeTime = 2.0f;

    public SphereCollider _spherCollider;

    private float PushBackTime = 0.35f;
    private float PushBackSpeed = 1.0f;

    public override void Initialize(List<EffectHit> listEffects, string _OwnerID, LayerMask targetMask, float _rangeBullet = float.PositiveInfinity)
    {
        base.Initialize(listEffects, _OwnerID, targetMask, _rangeBullet);

    }

    public void ResetValue(float _value, float _speed)
    {
        PushBackTime = _value;
        PushBackSpeed = _speed;
    }

    public override void DestroyBullet()
    {
        base.DestroyBullet();
        this.timerLifeTime = 0f;
        Pooly.Despawn(this.transform);
        shooted = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameMaster.IsZombieTag(other.tag))
        {
            var zom = other.GetComponentInParent<Zombie>();
            if (zom != null)
            {
                zom.health.SetDamage(this.bulletDamage, ShotType.NORMAL);
                zom.PushBack(PushBackSpeed, PushBackTime);
            }
         
        }
    }

    public override void Launch(float _force, float _damage, ShotType _type, Action OnHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        this.bulletSpeed = _force;
        this.bulletDamage = _damage;
        this.shotType = _type;
        timerLifeTime = 0f;
        shooted = true;
    }

    public override void Update()
    {
        if (!shooted)
            return;

        base.transform.position += base.transform.forward * Time.deltaTime * this.bulletSpeed;
        timerLifeTime += Time.deltaTime;
        if (timerLifeTime >= LifeTime)
        {
            DestroyBullet();
        }
    }
}
