using QuickType.Zombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieNurse : Zombie
{
    private float delayMove = 0f;
    private bool isAttackDuration = false;
    private Collider[] _hits;

    public ParticleSystem _parHealing;

    private Health _targetStragthForward;

    public override void Start()
    {
        base.Start();
        _hits = new Collider[30];
    }

    public override void Initialize(ZombieElement data, float hpMultiplier, float dmgMultiplier, int wave, int subWave, int index)
    {
        base.Initialize(data, hpMultiplier, dmgMultiplier, wave, subWave, index);
        delayMove = this._data.Duration;
        isAttackDuration = false;
        _parHealing.transform.localScale = Vector3.one * _data.Radius;
        _parHealing.Stop(true);

        _targetStragthForward = FindTargetStragthforward();
    }

    public override void Behavior_OnWallReached(bool isReachedWall)
    {
        //if (isReachedWall)
        //{
        //    this.isAttacked = true;
        //    this.zmAnimator.SetBool("attack", true);
        //    weapon.enabled = true;
        //}
    }

    public override void UpdateZombie(float _deltaTime)
    {
        base.UpdateZombie(_deltaTime);

        if (this._state == ZOM_STATE.IDLE)
            return;

        if (_state == ZOM_STATE.WALK)
        {
            if (_target && delayMove <= 0f && Mathf.Abs(transform.position.x - _target.transform.position.x) <= _data.MaxRange)
            {
                SetState(ZOM_STATE.ATTACK);
                //isAttackDuration = true;
                delayMove = this._data.Duration;
            }

            //if (this._targetStragthForward != null && this._targetStragthForward.transform.position.x - this.transform.position.x <= this._data.MinRange)
            //{
            //    this.goToWallBeh.enabled = false;
            //    if (this._targetStragthForward.IsDead() || this._targetStragthForward == null)
            //    {
            //        this._targetStragthForward = FindTargetStragthforward();
            //        this.SetState(ZOM_STATE.WALK);
            //    }
            //}
        }

        delayMove -= _deltaTime;
    }

    public override void AnimationCallbackShoot()
    {
        base.AnimationCallbackShoot();
        DoHealing();
    }

    public void DoHealing()
    {
        var hitCount = Physics.OverlapSphereNonAlloc(transform.position, this._data.Radius, _hits, ResourceManager.instance._maskZombieOnly);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hits[i];
                var health = hit.GetComponent<Health>();
                if (health != null)
                {
                    if (health.CurrentHp < health.GetHPWithCoeff())
                    {
                        var hp = health.IsZombieBoss ? health.GetHPWithCoeff() * _data.Dmg / 100f : health.GetHPWithCoeff() * _data.Value / 100f;
                        health.RefillHP(hp, false);
                    }

                }
            }
        }
    }

    public override void AnimationCallbackFinishShoot()
    {
        base.AnimationCallbackFinishShoot();
        isAttackDuration = false;
        SetState(ZOM_STATE.WALK);
        delayMove = this._data.Duration;
        this.zmAnimator.speed = 1.0f;
    }

    public override void SetStateAttack()
    {
        this.zmAnimator.SetBool("attack", true);
        this.zmAnimator.speed = _animAttackSpeed;
        this.weapon.SetPauseBehaviour(false);
        this.goToWallBeh.SetPauseBehaviour(true);
        weapon.enabled = false;
        _parHealing.time = 0;
        _parHealing.ResetParticle();
        _parHealing.Play(true);
        isAttackDuration = false;

        //nurse heal do not deal dmg

    }
}
