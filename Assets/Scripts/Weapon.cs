using System;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using QuickType.SkillDesign;
using Random = System.Random;

public enum ShotType
{
    NORMAL,
    CRIT,
    HEADSHOT,
    POISON,
    FIRE,
    LIGHTNING,
    PIERCE,
    AOE,
    DEADSHOT,
    NORMAL_SKIP_FX
}

public abstract class Weapon : MonoBehaviour
{
    public Launcher _launcher;
    public bool MuteSFXShoot = false;

    //for random
    public Random _randomSeed;

    protected float _ReduceSpeedMultiplier;

    private void Awake()
    {
        _launcher = gameObject.GetComponentInChildrenRecursively<Launcher>();

        _randomSeed = new Random();
    }

    public virtual void SetReduceSpeedMultiplier(float _reduce)
    {
        _ReduceSpeedMultiplier = _reduce;
    }

    public virtual void Initialize(Launcher targetLauncher)
    {
        _launcher = targetLauncher;
        _launcher.ResetLauncher(this.MuteSFXShoot);
        IsPerformingShoot = false;
    }

    public void ResetData(float dmg, float fireRate, float headshotpercent, float critpercent, List<EffectHit> _effectHits, string OwnerID, float FireRange)
    {
        _launcher = gameObject.GetComponentInChildrenRecursively<Launcher>();
        this.damage = dmg;
        this.damageK = 1;
        this.fireRate = fireRate;
        this.critPercent = critpercent;
        this.headshotPercent = headshotpercent;
        this.IsPause = false;
        this.effectHits = _effectHits;
        ResetWeaponAttribute();
        ApplyAttributeWeapon();
        //this.enabled = false;
        this.OwnerID = OwnerID;
        this.fireRange = FireRange;
        SetReduceSpeedMultiplier(1.0f);
    }

    public virtual void UpdateBehaviour(float _deltaTime)
    {
        if (IsPause)
            return;

        if (this.CanFire())
        {
            this.FireWeapon();
        }
    }

    public void DoubleDamage(bool status)
    {
        if (status)
        {
            this.damageK = 2;
            return;
        }
        this.damageK = 1;
    }

    public virtual void FireWeapon()
    {
        DebugLog();

        ShotType type = ShotType.NORMAL;
        float dmg = this.damage + this.damage * (this.attrDmg * 1.0f / 100f);
        float rand = (float)(_randomSeed.NextDouble() * 100f);
        if (rand < this.headshotPercent + additionalHeadshotPercent + attrHeadShot)
        {
            type = ShotType.HEADSHOT;
            dmg = 2 * dmg;
            //Debug.Log("[Headshot]!!!");
        }
        else
        {
            rand = (float)(_randomSeed.NextDouble() * 100f);
            if (rand <= this.critPercent)
            {
                type = ShotType.CRIT;
                dmg = (2.0f + (attrCritDmg * 1.0f / 100f)) * dmg;
            }
        }

        OnLaunch?.Invoke(this.fireForce, dmg, type);
    }

    public virtual void ResetRandomSeed()
    {
        //get a randomizer
        var randomizer = new System.Random();

        //get a random int seed
        int seed = randomizer.Next(int.MinValue, int.MaxValue);

        //set Unity's randomizer to that seed
        UnityEngine.Random.InitState(seed);
    }

    protected abstract bool CanFire();

    [SerializeField]
    public float fireForce = 80f;
    public float damage;
    private int damageK;
    private bool IsPause = false;

    [SerializeField]
    protected float fireRate = 1f;
    protected float nextFireTime;
    protected float critPercent = 0f;
    protected float headshotPercent = 0f;
    protected float additionalHeadshotPercent = 0f;
    protected float fireRange = 0f;

    //weapon attribute add

    public List<EffectHit> effectHits { get; private set; }
    public string OwnerID { get; private set; }
    public Action<float, float, ShotType> OnLaunch;

    public bool IsPerformingShoot { get; protected set; }

    public virtual void SetPerfomShoot(bool value)
    {
        IsPerformingShoot = value;
    }

    public void StopBehaviour()
    {
        base.enabled = false;
    }
    public virtual void SetPauseBehaviour(bool _value)
    {
        IsPause = _value;
    }
    public virtual void SetAdditionalHeadshotPercent(float value)
    {
        this.additionalHeadshotPercent = value;
    }

    #region Weapon Attribute
    protected float attrHeadShot = 0f;
    protected float attrCritDmg = 0f;
    protected float attrFireRate = 0f;
    protected float attrDmg = 0f;

    public void ResetWeaponAttribute()
    {
        attrHeadShot = 0f;
        attrCritDmg = 0f;
        attrFireRate = 0f;
        attrDmg = 0f;
    }

    public virtual void ApplyAttributeWeapon()
    {
        if (effectHits != null && effectHits.Count > 0)
        {
            foreach (var effect in effectHits)
            {
                SkillDesignElement Design = null;
                if (effect != null && effect.Type != EffectType.NONE)
                {
                    Design = DesignHelper.GetSkillDesign(effect.SkillID);
                }

            }
        }

    }

    #endregion

    public void DebugLog()
    {
        //Logwin.Log($"weapon {OwnerID}", $"HS: {this.headshotPercent + additionalHeadshotPercent + attrHeadShot}", "[WEAPON]");
    }
}
