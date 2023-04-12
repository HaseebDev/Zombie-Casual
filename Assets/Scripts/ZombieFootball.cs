using DG.Tweening;
using MEC;
using QuickType.Zombie;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZombieFootball : Zombie
{
    public override void Start()
    {
        base.Start();

    }

    private AutoDespawnParticles _fxStunned = null;
    private Health _targetStragthforward;

    private float _tunningDuration = 0f;

    private bool isReachedWall = false;

    private Collider[] _sphereCastHit = new Collider[5];

    public override void Initialize(ZombieElement data, float hpMultiplier, float dmgMultiplier, int wave, int subWave, int index)
    {
        base.Initialize(data, hpMultiplier, dmgMultiplier, wave, subWave, index);
        if (_fxStunned != null)
        {
            _fxStunned.CleanUp();
            _fxStunned = null;
        }

        this.goToWallBeh.FindWall();
        _targetStragthforward = FindTargetStragthforward();

        CalcAnimWalkSpeed((float)_data.DefSpeed);
        _animAttackSpeed = _animWalkSpeed;
        isReachedWall = false;
    }

    public override void MultiplyZombieSpeed(float multiplier = 1, bool withAnimTransition = false, float FXDuration = 3.0f, Action callback = null)
    {
        //ZOMBIE FOOTBALL CANT NOT MULTIPLY SPEED!!!
        this.IsMuliplyingSpawnSpeed = false;
    }

    public override void SetStateWalk()
    {
        _targetStragthforward = FindTargetStragthforward();
        base.SetStateWalk();

        if (_fxStunned != null)
        {
            _fxStunned.CleanUp();
            _fxStunned = null;
        }
    }

    public override void AnimationCallbackShoot()
    {
        if (GamePlayController.instance.IsPausedGame)
            return;

        //_targetStragthforward = FindTargetStragthforward();
        _targetStragthforward = FindTargetAround();
        float dmgHit = GamePlayController.instance.gameLevel.castleHealth.GetHPWithCoeff() * _data.Value / 100f;

        if (_targetStragthforward != null)
        {

            _targetStragthforward?.SetDamage(dmgHit, _shotType, $"Zombie{gameObject.GetInstanceID()}", null, (responseHit, armoursResponses) =>
            {
            }, _data.MinRange);
        }
        else
        {
            this.weapon._launcher.Launch(_force, dmgHit, _shotType);
            this.weapon.SetPerfomShoot(false);

        }
        this.SetState(ZOM_STATE.IDLE);
        AudioSystem.instance.PlaySFX(this.SoundZomAttack);
        _fxStunned = health.PlayEffectHitVisual(COMMON_FX.FX_STUNNED, -1);
        _tunningDuration = 5.0f;

    }

    public Health FindTargetAround()
    {
        Health result = null;

        int numHit = Physics.OverlapSphereNonAlloc(transform.position, 0.5f, _sphereCastHit);
        if (numHit > 0)
        {
            for (int i = 0; i < numHit; i++)
            {
                var Health = _sphereCastHit[i].GetComponent<Health>();
                if (Health != null && Health.gameObject.CompareTag(TagConstant.TAG_PLAYER))
                {
                    result = Health;
                    break;
                }
            }
        }

        return result;
    }

    public override void SetStateIdle()
    {
        //DO NOTHING
    }

    public override void Behavior_OnWallReached(bool isReached)
    {
        if (this._state == ZOM_STATE.WALK || this._state == ZOM_STATE.RUN)
        {
            this.SetState(ZOM_STATE.ATTACK);
            _targetStragthforward = FindTargetStragthforward();
            if (_targetStragthforward != null && _targetStragthforward.GetType() == typeof(CastleHealth))
            {
                isReachedWall = true;
            }
        }

    }

    public override void DestroyZombie(float delay = 0)
    {
        base.DestroyZombie(delay);
        if (_fxStunned != null)
        {
            _fxStunned.CleanUp();
            _fxStunned = null;
        }
    }

    public override void SetStateAttack()
    {
        base.SetStateAttack();
        GamePlayController.instance.DoCameraShake();
    }

    public override void UpdateZombie(float _deltaTime)
    {
        if (this._state == ZOM_STATE.IDLE && !isReachedWall)
        {
            if (_tunningDuration > 0)
            {
                _tunningDuration -= _deltaTime;
                if (_tunningDuration <= 0f)
                {
                    this.SetState(ZOM_STATE.WALK);
                }
            }
            return;
        }
        base.UpdateZombie(_deltaTime);

        if (_targetStragthforward != null && _state == ZOM_STATE.WALK)
        {
            if (Mathf.Abs(transform.position.x - _targetStragthforward.transform.position.x) <= this._data.MaxRange)
            {
                if (_animMultiplySpeedHandler != null)
                {
                    _animMultiplySpeedHandler.Kill();
                }
            }
        }
    }

}
