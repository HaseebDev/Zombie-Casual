using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBoomBullet : BoomBullet
{
    private float SeekingDelay = 0.1f;
    public float RotSpeed = 20f;

    private Health _target;
    private float _timerSeeking = 0f;
    private float _additionSpeed = 1.0f;
    private float _targetSpeed = 0f;


    public override void Initialize(List<EffectHit> listEffects, string _OwnerID, LayerMask maskHit, float rangeBullet = float.PositiveInfinity)
    {
        base.Initialize(listEffects, _OwnerID, maskHit, rangeBullet);
        _additionSpeed = 1.0f;
    }

    public void SetTarget(Health target)
    {
        this._target = target;
    }

    public void SetAdditionSpeed(float additionSpeed = 0.1f)
    {
        _additionSpeed = additionSpeed;
    }

    public override void Launch(float _force, float _damage, ShotType _type, Action OnHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        base.Launch(_force, _damage, _type, OnHit);
        _timerSeeking = 0f;
       
    }



    public override void Update()
    {
        base.Update();
        _timerSeeking += Time.deltaTime;
        if (_timerSeeking >= SeekingDelay && this._target != null)
        {
            var targetRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotSpeed * Time.deltaTime);
            this.bulletSpeed += _timerSeeking * _additionSpeed;
        }




    }
}
