using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeBullet : Bullet
{
    public Collider collider;
    public Transform _rotator;
    public float rotSpeed = 500;

    private bool isHit = false;

    public override void Initialize(List<EffectHit> listEffects, string _OwnerID, LayerMask targetMask, float _rangeBullet = float.PositiveInfinity)
    {
        base.Initialize(listEffects, _OwnerID, targetMask, _rangeBullet);
        gameObject.SetActiveIfNot(false);

    }

    public override void Update()
    {
        base.Update();
        _rotator.Rotate(new Vector3(1, 0, 0) * rotSpeed * Time.deltaTime);
    }

    public override void Launch(float _force, float _damage, ShotType _type, Action onHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        // base.Launch(_force, _damage, _type, onHit);

        this.MultiplyPerHit = 1.0f;
        this.bulletSpeed = _force;
        this.bulletDamage = _damage;
        this.shotType = _type;
        this.OnHit = onHit;
        this.IsLaunch = true;
        this.targetPos = new Vector3(targetPos.x, transform.position.y, transform.position.z);
        isHit = false;
        gameObject.SetActiveIfNot(true);
        collider.enabled = true;
        //RaycastHit hitInfo;
        //var hit = Physics.Raycast(transform.position, transform.forward, out hitInfo, 0.5f, this._maskHit);
        //if (hit)
        //{
        //    IHealth component = hitInfo.transform.GetComponent<IHealth>();
        //    if (component != null)
        //    {
        //        var hitPoint = hitInfo.transform.position;
        //        component.SetDamage(this.bulletDamage, this.shotType, this.OwnerID, this.effectHits);
        //        DestroyBullet();
        //    }
        //}
    }

    private void OnTriggerEnter(Collider collision)
    {
        //if (collision.tag != "zombie" || isHit)
        //{
        //    return;
        //}

        isHit = true;
        IHealth component = collision.transform.GetComponent<IHealth>();
        if (component != null)
        {
            var hitPoint = collision.transform.position;
            component.SetDamage(this.bulletDamage, this.shotType, this.OwnerID, this.effectHits);
            DestroyBullet();
        }

    }
}
