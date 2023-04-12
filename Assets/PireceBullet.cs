using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;
using System;

public class PireceBullet : Bullet
{
    float bulletSpeed = 0f;
    float bulletDamage = 0f;
    ShotType shotType = ShotType.NORMAL;

    float LifeTime = 2f;
    private float timerLifeTime = 0f;
    private bool shooted = false;
    private float MultiplyPerHit = 1.0f;

    private List<EffectHit> _listEffectHits;
    private string OwnerID;

    public override void Launch(float _force, float _damage, ShotType _type, Action OnHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        this.bulletSpeed = _force;
        this.bulletDamage = _damage;
        this.shotType = _type;
        timerLifeTime = 0f;
        shooted = true;
    }

    public override void DestroyBullet()
    {
        shooted = false;
        timerLifeTime = 0f;
        Pooly.Despawn(this.transform);
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

    private void OnTriggerEnter(Collider collision)
    {
        if (!GameMaster.IsZombieTag(collision.tag))
            return;

        IHealth component = collision.transform.GetComponent<IHealth>();
        if (component != null)
        {
            component.SetDamage(this.bulletDamage, this.shotType, this.OwnerID, _listEffectHits);
        }
    }


}
