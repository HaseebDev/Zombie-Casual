using QuickType.Zombie;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDrum : Zombie
{
    public static float BUFF_DURATION = 0.5f;
    public ParticleSystem _parBuff;
    public float RadiusBuffSpeed { get; private set; }

    private float _timerBuff = 0f;
    private Collider[] _hits;

    private Health _targetStragthForward;

    public override void Start()
    {
        base.Start();
        _hits = new Collider[30];
    }

    public override void Initialize(ZombieElement data, float hpMultiplier, float dmgMultiplier, int wave, int subWave, int index)
    {
        base.Initialize(data, hpMultiplier, dmgMultiplier, wave, subWave, index);
        this.RadiusBuffSpeed = this._data.Radius;
        _parBuff.transform.localScale = Vector3.one * _data.Radius;
        _parBuff.Stop(true);

        _targetStragthForward = FindTargetStragthforward();

    }

    public override void SetStateAttack()
    {
        //DO NOTHING ZOMBIE DRUM!!!!
    }

    public override void SetStateWalk()
    {
        base.SetStateWalk();
    }

    public override void DestroyZombie(float delay = 0)
    {
        base.DestroyZombie(delay);
        _parBuff.Stop(true);
    }

    public override void UpdateZombie(float _deltaTime)
    {
        base.UpdateZombie(_deltaTime);

        if (!_parBuff.isPlaying)
            _parBuff.Play(true);

        _timerBuff += BUFF_DURATION;
        if (_timerBuff >= BUFF_DURATION)
        {
            _timerBuff = 0f;
            BuffSpeedZombie();
        }

        if (this._state == ZOM_STATE.WALK)
        {
            if (this._targetStragthForward != null && this._targetStragthForward.transform.position.x - this.transform.position.x <= this._data.MinRange)
            {
                this.goToWallBeh.enabled = false;
                if (this._targetStragthForward.IsDead() || this._targetStragthForward == null)
                {
                    this._targetStragthForward = FindTargetStragthforward();
                    this.SetState(ZOM_STATE.WALK);

                   
                }
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.tag.Equals(TagConstant.TAG_ZOMBIE))
    //        return;

    //    var zom = other.GetComponentInParent<Zombie>();
    //    BuffSpeedForZom(zom);

    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!other.tag.Equals(TagConstant.TAG_ZOMBIE))
    //        return;

    //    var zom = other.GetComponentInParent<Zombie>();
    //    BuffSpeedForZom(zom);
    //}

    public void BuffSpeedZombie()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * _data.Radius, Color.red);
        var hitCount = Physics.OverlapSphereNonAlloc(transform.position, this._data.Radius, _hits, ResourceManager.instance._maskZombieOnly);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hits[i];
                var zom = hit.GetComponentInParent<Zombie>();
                if (zom != null && zom.EffectController != null)
                {
                    if (!zom.EffectController.HasEffect(EffectType.PASSIVE_MULTIPLY_SPEED))
                    {
                        zom.EffectController.AddEffect(new EffectHit()
                        {
                            Type = EffectType.PASSIVE_MULTIPLY_SPEED,
                            Value = _data.Value,
                            Duration = _data.Duration,
                            OwnerID = "ZOMBIE_DRUM",
                            SkillID = EffectType.PASSIVE_MULTIPLY_SPEED.ToString()
                        });
                    }

                }
            }
        }
    }

    public void BuffSpeedForZom(Zombie zom)
    {
        if (zom != null && zom.EffectController != null)
        {
            if (!zom.EffectController.HasEffect(EffectType.PASSIVE_MULTIPLY_SPEED))
            {
                zom.EffectController.AddEffect(new EffectHit()
                {
                    Type = EffectType.PASSIVE_MULTIPLY_SPEED,
                    Value = _data.Value,
                    Duration = _data.Duration,
                    OwnerID = "ZOMBIE_DRUM",
                    SkillID = EffectType.PASSIVE_MULTIPLY_SPEED.ToString()
                });
            }

        }
    }

}
