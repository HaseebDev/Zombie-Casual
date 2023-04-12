using System.Collections;
using System.Collections.Generic;
using MEC;
using QuickType.Zombie;
using UnityEngine;

public class ZombieAxe : Zombie
{
    public GameObject axeGO;

    private float _shootSpeed;

    private float _animShootSpeed;

    public override void Initialize(ZombieElement data, float hpMultiplier, float dmgMultiplier, int wave, int subwave, int index )
    {
        base.Initialize(data, hpMultiplier, dmgMultiplier, wave, subwave, index);
        axeGO.gameObject.SetActiveIfNot(true);

        var animShoot = this.zmAnimator.GetAnimationInfo("Attack");
        if (animShoot != null)
        {
            _animShootSpeed = animShoot.length;
            float numInSec = 1.0f / animShoot.length * 1.0f;
            //_shootSpeed = numInSec / FireRate;
            _shootSpeed = data.FireRate / numInSec;
        }



    }

    public override void Behavior_OnWallReached(bool isReachedWall)
    {
        if (isReachedWall)
        {
            this.isAttacked = true;
            this.zmAnimator.SetBool("attack", true);
            weapon.enabled = true;
        }

    }

    protected override void Weapon_OnLaunch(float force, float damage, ShotType type)
    {
        base.Weapon_OnLaunch(force, damage, type);
        // this.zmAnimator.speed *= _shootSpeed;
        axeGO.gameObject.SetActiveIfNot(true);

    }

    public override void AnimationCallbackShoot()
    {
        axeGO.gameObject.SetActiveIfNot(false);
        this.isAttackDuration = true;
        base.AnimationCallbackShoot();

        if (this._animShootSpeed <= 1.0f / this._data.FireRate)
        {
            isAttackDuration = false;
            SetState(ZOM_STATE.WALK);
            delayMove = 1.0f / this._data.FireRate;
            this.zmAnimator.speed = 1.0f;

        }

    }

    public override void AnimationCallbackFinishShoot()
    {
        axeGO.gameObject.SetActiveIfNot(true);

    }

    private float delayMove = 0f;
    private bool isAttackDuration = false;

    [Header("Tune Zombie AXE")]
    [SerializeField] private float AttackDuration = 0.1f;
    [SerializeField] private float MoveDuration = 2.0f;

    private bool pendingAttack = true;

    public override void UpdateZombie(float _deltaTime)
    {
        base.UpdateZombie(_deltaTime);

        if (this == null || this._state == ZOM_STATE.IDLE)
            return;

        if (_state == ZOM_STATE.WALK && _target && delayMove <= 0f && !isAttackDuration && Mathf.Abs(transform.position.x - _target.transform.position.x) <= _data.MaxRange)
        {
            SetState(ZOM_STATE.ATTACK);
            isAttackDuration = true;
        }

        delayMove -= _deltaTime;
    }

}
