using System;
using System.Collections;
using Adic;
using Framework.Managers.Event;
using UnityEngine;
using DG.Tweening;
using Ez.Pooly;
using QuickType.Zombie;
using DigitalRuby.LightningBolt;
using MEC;
using System.Collections.Generic;
using com.datld.talent;
using System.Linq;
using DG.Tweening.Core;
using QuickType;

public enum ZOM_STATE
{
    NONE,
    IDLE,
    WALK,
    RUN,
    ATTACK,
    DIE
}

public class Zombie : MonoBehaviour
{
    [Header("Effect Scale")]
    public float FooterEffectScale = 1.0f;
    public float BodyEffectScale = 1.0f;

    [Header("IS BOSS ZOMBIE?")]
    public bool IsBoss;
    public EffectController EffectController { get; private set; }
    public ZombieElement _data;
    private float _hpMultiplier;
    protected Health _target;
    public ZOM_STATE _state { get; private set; }
    public bool IsAngry { get; private set; }

    //To Determine wave/ subwave index
    private int WaveOwner = -1;
    private int SubWaveOwner = -1;
    private int SubWaveIndex = -1;

    protected float _animWalkSpeed = 1.0f;
    protected float _animAttackSpeed = 1.0f;
    private float CurrentSpeedMultiplier = 1.0f;

    private bool firstHit = false;

    public bool IsMuliplyingSpawnSpeed = false;

    [Header("Attack Delay")]
    [Range(0f, 1f)]
    public float AttackKeyFrame = 0.5f;
    private float attackDelay = 0.5f;

    private bool shouldUpdateZombie = false;

    private float _ReduceSpeedMultiplier = 1.0f;
    private SkinnedMeshRenderer _skinMesh;

    public void Awake()
    {
    }

    private void OnEnable()
    {
        _timerCheckStuck = 0f;
    }

    #region zombie stuck
    private bool IsZombieStucked = false;
    private float _timerCheckStuck = 0f;
    private void CheckZombieStuck(float _deltaTime)
    {
        if (transform.position.x < GamePlayController.instance.gameLevel._attackableLine.position.x)
        {
            _timerCheckStuck += _deltaTime;
            if (_timerCheckStuck >= 15f)
            {
                IsZombieStucked = true;
                Debug.LogError($"ZOMBIE GOT STUCK!!!! {this.gameObject.name}");
            }
        }

    }

    #endregion

    private void Update()
    {
        //float _deltaTime = Time.deltaTime;
        //if (!shouldUpdateZombie || this == null)
        //    return;

        //CheckZombieStuck(_deltaTime);
        //if (IsZombieStucked)
        //{
        //    this.health.SetDamage(this.health.GetHPWithCoeff() + 1000);
        //    IsZombieStucked = false;
        //}

    }

    public virtual void UpdateZombie(float _deltaTime)
    {
        if (!shouldUpdateZombie || this == null)
            return;


        this.goToWallBeh.UpdateBehaviour(_deltaTime);
        this.weapon.UpdateBehaviour(_deltaTime);
        this.runBeh.UpdateBehaviour(_deltaTime);

        EffectController?.UpdateEffectController(_deltaTime);
        if (GamePlayController.instance)
        {
            if (transform.position.x >= GamePlayController.instance.gameLevel._attackableLine.position.x)
                health.IsTargetable = true;
            else
                health.IsTargetable = false;

            //clamp zombie speed line


            //this is first 

            if (CurrentSpeedMultiplier >= GameConstant.SPAWN_ZOM_SPEED_MULTIPLIER && transform.position.x >= GamePlayController.instance.gameLevel._clampZombieSpeedLine.position.x)
            {
                //reset speed asdefault
                //MultiplyZombieSpeed(1.0f, true);
                GamePlayController.instance.SetMultiplierSpeedForSubwave(this.WaveOwner, this.SubWaveOwner, 1.0f);
            }

            if (_state == ZOM_STATE.ATTACK)
            {
                if (_target.IsDead() || _target == null)
                {
                    this.SetState(ZOM_STATE.WALK);
                    this.goToWallBeh.FindTargetPlayer();
                }
            }

        }
        else
            health.IsTargetable = true;

    }

    public virtual void Initialize(ZombieElement data, float hpMultiplier, float dmgMultiplier, int wave, int subWave, int index)
    {
        _data = data;
        _hpMultiplier = hpMultiplier;
        WaveOwner = wave;
        SubWaveOwner = subWave;
        SubWaveIndex = index;

        if (!IsBoss)
            IsBoss = _data.ZombieId.ToLower().Contains("boss");

        if (EffectController == null)
        {
            EffectController = new EffectController();
            this.EffectController.OnAppliedEffect = OnAppliedEffect;
            this.EffectController.OnDestroyedEffect = OnDestroyEffect;
            this.EffectController.OnEffectTicked = OnEffectTicked;
        }

        this.health.Revive();
        this.motor.isMove = true;
        this.isAttacked = false;

        this.gotoHell.StopBehavior();
        this.SetRandomVisual();
        //this.zmAnimator.SetBool("dying", false);
        SetAnimDie(false);

        CalcAnimWalkSpeed((float)_data.DefSpeed);
        this.health.IsTargetable = false;
        this.motor.Initialize((float)_data.DefSpeed, (float)_data.TurningSpeed);
        this.health.Initialize(_data.HP, !_data.ZombieId.ToLower().Contains("boss"), this.EffectController, IsBoss);
        this.weapon.ResetData(_data.Dmg * dmgMultiplier, _data.FireRate, 0, 0, null, null, _data.MinRange);


        float attackableLineX = GamePlayController.instance.gameLevel._attackableLine.position.x;

        goToWallBeh.Initialize(_data.MinTurningTime, _data.MaxTurningTime,
            (_data.MinTurningTime != 0 || _data.MaxTurningTime != 0), _data.MinRange, attackableLineX, _data.TurningAngle);
        goToWallBeh.OnFoundTarget = OnTargetFound;
        goToWallBeh.OnAttackRangeRached = OnAttackRangeReached;
        goToWallBeh.ApplyBehavior();

        defKillReward = Mathf.RoundToInt(_data.KillRewardGold);
        SetHpMultiplier(hpMultiplier);

        weapon.enabled = false;
        _target = null;

        health.IsTargetable = false;

        if (_animationEvents != null)
        {
            _animationEvents.OnShootEvent = AnimationCallbackShoot;
            _animationEvents.OnFinishShoot = AnimationCallbackFinishShoot;
        }

        IsAngry = false;
        firstHit = false;

        MultiplyZombieSpeed(GameConstant.SPAWN_ZOM_SPEED_MULTIPLIER);
        IsMuliplyingSpawnSpeed = true;
        ResetZomMat();
        flashColourBusy = false;
        SetState(ZOM_STATE.WALK);
        shouldUpdateZombie = true;
        _firstWeaponLaunch = true;


        IsZombieStucked = false;
        _timerCheckStuck = 0f;
        _ReduceSpeedMultiplier = 1.0f;

        if (GamePlayController.instance != null)
        {
            GamePlayController.instance.OnFinishLevel += OnFinishedLevel;
        }

        SetMaterial(ZOMBIE_MAT.NORMAL);

    }

    private void OnFinishedLevel()
    {

    }

    public void SetZombieSpeedToNormal()
    {
        MultiplyZombieSpeed(1.0f, true, 3.0f, () =>
        {
            IsMuliplyingSpawnSpeed = false;
        });
    }

    public TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _animMultiplySpeedHandler;

    public virtual void MultiplyZombieSpeed(float multiplier = 1.0f, bool withAnimTransition = false, float durationFX = 3.0f, Action callback = null)
    {
        if (!withAnimTransition)
        {
            CalcAnimWalkSpeed((float)_data.DefSpeed * multiplier + motor.AdditionalSpeed);
            this.motor.SetSpeed((float)_data.DefSpeed * multiplier);
            if (this._state == ZOM_STATE.WALK || this._state == ZOM_STATE.RUN)
                this.zmAnimator.speed = _animWalkSpeed;
        }
        else
        {
            var cachedMultiplier = CurrentSpeedMultiplier;
            _animMultiplySpeedHandler = DOTween.To(() => cachedMultiplier, x =>
                {
                    CalcAnimWalkSpeed((float)_data.DefSpeed * x + motor.AdditionalSpeed);
                    this.motor.SetSpeed((float)_data.DefSpeed * x);
                    if (this._state == ZOM_STATE.WALK || this._state == ZOM_STATE.RUN)
                        this.zmAnimator.speed = _animWalkSpeed;
                }, multiplier, durationFX).SetEase(Ease.Linear).OnComplete(() =>
                {
                    CalcAnimWalkSpeed((float)_data.DefSpeed * multiplier);
                    this.motor.SetSpeed((float)_data.DefSpeed * multiplier);
                    if (this._state == ZOM_STATE.WALK || this._state == ZOM_STATE.RUN)
                        this.zmAnimator.speed = _animWalkSpeed;
                    callback?.Invoke();
                });

        }
        CurrentSpeedMultiplier = multiplier;

    }

    public virtual void ReduceZombieSpeed(float _reduce)
    {
        _ReduceSpeedMultiplier = _reduce;
        this.motor.SetReduceSpeedMultiplier(_ReduceSpeedMultiplier);
        this.weapon.SetReduceSpeedMultiplier(_ReduceSpeedMultiplier);
        if (_animMultiplySpeedHandler != null)
        {
            _animMultiplySpeedHandler.Kill(false);
        }
        ResetAnimSpeed(true);
    }

    public virtual void SetAdditionalSpeed(float speedAdd)
    {
        this.motor.SetAdditionalSpeed(speedAdd);
        ResetAnimSpeed();

    }

    public void ResetAnimSpeed(bool forceReset = false)
    {
        if (forceReset || this._state == ZOM_STATE.WALK || this._state == ZOM_STATE.RUN)
        {
            CalcAnimWalkSpeed(this.motor.GetFinalSpeed());
            this.zmAnimator.speed = _animWalkSpeed;
        }
    }

    protected void CalcAnimWalkSpeed(float motorSpeed)
    {
        _animWalkSpeed = (motorSpeed / 0.35f) * _AnimWalkMultiplier;
    }

    public void CalcAnimAttackSpeed()
    {
        var _animAttack = this.zmAnimator.GetAnimationInfo("Attack");
        float numInSec = 1.0f / _animAttack.length * 1.0f;
        _animAttackSpeed = (this._data.FireRate / numInSec) * _AnimAttackMultiplier;
        attackDelay = _animAttack.length * _animAttackSpeed * AttackKeyFrame;
    }

    private void OnAttackRangeReached()
    {
        //this.isAttacked = true;
        //this.zmAnimator.SetBool("attack", true);
        //weapon.enabled = true;
        SetState(ZOM_STATE.ATTACK);
    }

    private void OnGoToHellComplete()
    {
        DestroyZombie();
    }

    private void OnTargetFound(Health target)
    {
        if (target != null)
        {
            _target = target;

        }
    }

    public virtual void Behavior_OnWallReached(bool isReached)
    {
        if (isReached)
            SetState(ZOM_STATE.ATTACK);
        else
            SetState(ZOM_STATE.WALK);
    }

    private void SetRandomVisual()
    {
        for (int i = 0; i < this.visualVariants.Length; i++)
        {
            this.visualVariants[i].SetActive(false);
        }

        GameObject gameObject = null;
        if (this.debugVariant == null)
        {
            gameObject = this.visualVariants[UnityEngine.Random.Range(0, this.visualVariants.Length - 1)];
            gameObject.SetActive(true);
        }
        else
        {
            gameObject = this.debugVariant;
            gameObject.SetActive(true);
        }

        if (this._skinMesh == null)
        {
            _skinMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        this.zmAnimator = gameObject.GetComponent<Animator>();
        this.zmNormalMat = _skinMesh.material;
    }

    public float Speed {
        get { return this.motor.GetDefSpeed(); }
        set { this.motor.SetSpeed(value); }
    }

    public bool IsDead {
        get { return this.health.IsDead(); }
    }

    public void SetHpMultiplier(float hpMultiplierValue)
    {
        this.health.UpdateHealthMultiplier(hpMultiplierValue);
    }

    public virtual void Start()
    {
        this.Inject();
        Health health = this.health;
        health.OnHit = (Action<float>)Delegate.Combine(health.OnHit, new Action<float>(this.Health_OnHit));
        Health health2 = this.health;
        health2.OnDie = (Action<bool>)Delegate.Combine(health2.OnDie, new Action<bool>(this.Health_OnDie));
        GoToWallBehavior goToWallBehavior = this.goToWallBeh;
        goToWallBehavior.OnWallReached =
            (Action<bool>)Delegate.Combine(goToWallBehavior.OnWallReached, new Action<bool>(this.Behavior_OnWallReached));
        weapon.OnLaunch = (Action<float, float, ShotType>)Delegate.Combine(weapon.OnLaunch,
            new Action<float, float, ShotType>(this.Weapon_OnLaunch));
    }

    public virtual void Health_OnDie(bool skipAnimDead)
    {
        Timing.RunCoroutine(HealthOnDieCoroutine(skipAnimDead));
    }

    IEnumerator<float> HealthOnDieCoroutine(bool skipAnimDead)
    {
        yield return Timing.WaitForOneFrame;
        this._skipAnimDead = skipAnimDead;
        this.motor.isMove = false;
        KilledZombieInfoStruct killedZombieInfoStruct = default(KilledZombieInfoStruct);
        var mode = GamePlayController.instance.gameMode;
        killedZombieInfoStruct.killerID = health.lastCasterId;
        killedZombieInfoStruct.gameMode = mode;
        killedZombieInfoStruct.isAttacked = this.isAttacked;
        killedZombieInfoStruct.killReward = (mode == GameMode.CAMPAIGN_MODE)
            ? Mathf.RoundToInt(_data.KillRewardGold)
            : Mathf.RoundToInt(_data.KillRewardToken);
        killedZombieInfoStruct.isBoss = this.isBoss;
        killedZombieInfoStruct.worldPosition = this.transform.position;

        yield return Timing.WaitForOneFrame;
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.ZOMBIE_KILLED, killedZombieInfoStruct);
        MissionManager.Instance.TriggerMission(MissionType.KILL_ZOMBIE, _data.ZombieId);

        this.runBeh.StopBehavior();
        this.goToWallBeh.StopBehavior();
        //this.zmAnimator.SetBool("dying", true);

        SetAnimDie(true);

        InGameCanvas.instance._gamePannelView.DecreaseZomHpBar(health.GetHPWithCoeff(), WaveOwner);

        StartAnimDead();

        AudioSystem.instance.PlaySFX(this.SoundDead);
        SetState(ZOM_STATE.DIE);
    }

    public virtual void DestroyZombie(float delay = 0f)
    {
        shouldUpdateZombie = false;
        if (delay > 0)
        {
            Timing.CallDelayed(delay, () =>
            {
                DoDestroyZombie();
            });
        }
        else
        {
            DoDestroyZombie();
        }

    }

    public void DoDestroyZombie()
    {
        health.SetDie();
        this.weapon.StopBehaviour();
        this.goToWallBeh.StopBehavior();
        this.gotoHell.StopBehavior();

        DestroyAllEffects();
        if (this.EffectController != null)
            this.EffectController.ClearAllEffects();

        GamePlayController.instance.OnFinishLevel -= OnFinishedLevel;
        Pooly.Despawn(transform);
    }

    private void StartAnimDead()
    {
        gotoHell.OnGoToHellComplete = OnGoToHellComplete;
        gotoHell.SetHitPoint(this.health._hitMarker != null ? this.health._hitMarker.position : transform.position);
        this.gotoHell.ApplyBehavior(_skipAnimDead);
    }

    private void Health_OnHit(float dmg)
    {
        if (!firstHit && _data != null)
        {
            firstHit = true;
            if (GamePlayController.instance.CheckIsAngryZombie())
            {
                var multiplier = _data.AngrySpeed * 1.0f / _data.DefSpeed;
                if (this.EffectController != null && !this.EffectController.HasEffect(EffectType.PASSIVE_HIT_NOVA))
                    MultiplyZombieSpeed((float)multiplier, false);
            }
        }

        FlashColourWhenHit();
    }

    public void PushBack(float pushBackSpeed = 0.35f, float pushBackTime = 0.2f)
    {
        this.motor.PushBack(pushBackSpeed, pushBackTime);
    }

    [SerializeField] public Health health;

    [SerializeField] public Weapon weapon;

    [SerializeField] public Motor motor;

    [SerializeField] public GoToWallBehavior goToWallBeh;

    [SerializeField] public GoToHellBehavior gotoHell;

    [SerializeField] public RunBehavior runBeh;

    [SerializeField] private bool isBoss;

    [SerializeField] private int defKillReward;

    [SerializeField] private GameObject[] visualVariants;

    [Header("Debug only variant")]
    [SerializeField] private GameObject debugVariant;

    [Header("Markers")]
    public Transform hitMarker;

    public Animator zmAnimator;


    [SerializeField] protected AnimationEventHandlers _animationEvents;

    protected bool isAttacked;

    [SerializeField] private float deadLifetime;

    [Range(0f, 1f)] [SerializeField] private float runChance = 0.1f;

    // vars to Flash a colour when hit
    [Header("Colors")]
    public Color NormalColour = Color.white;
    public Color FlashColour = Color.red;
    public Color StunColor = Color.yellow;


    private bool flashColourBusy = false;

    public void ResetZomMat()
    {
        zmNormalMat.color = NormalColour;
        zmNormalMat.SetFloat("_Blend", 1.0f);

    }

    public void FlashColourWhenHit()
    {
        if (!shouldDoFlashAnim() || flashColourBusy)
            return;

        Timing.RunCoroutine(FlashColourWhenHitCoroutine());
    }

    private bool shouldDoFlashAnim()
    {
        bool result = false;

        //result = GameMaster.instance.OptmizationController.Data.EnableFlashWhenHit && (IsBoss || !GameMaster.IsSpeedUp);
        result = GameMaster.instance.OptmizationController.Data.EnableFlashWhenHit && IsBoss;
        return result;
    }

    IEnumerator<float> FlashColourWhenHitCoroutine()
    {
        if (zmNormalMat == null)
            yield break;

        flashColourBusy = true;
        zmNormalMat.SetFloat("_Blend", 0.3f);
        yield return Timing.WaitForSeconds(0.05f);
        zmNormalMat.color = NormalColour;
        zmNormalMat.SetFloat("_Blend", 1f);
        flashColourBusy = false;
    }

    #region Apply Effect

    private Dictionary<EffectType, BaseEffectObject<Zombie>> _dictEffectObjects =
        new Dictionary<EffectType, BaseEffectObject<Zombie>>();

    //private EffectType currentEffect = EffectType.NONE;
    //private float timerEffect = 0f;
    //private float CurrentEffectDuration = 0f;
    //private float CurrentEffectValue = 0f;

    //public void ApplyEffect(EffectType type, float Duration, float value)
    //{
    //    RemoveAllEffect(currentEffect);
    //    switch (type)
    //    {
    //        case EffectType.NONE:
    //            break;
    //        case EffectType.Fire:
    //            break;
    //        case EffectType.Poison:
    //            ApplyPoisonEffect();
    //            break;
    //        case EffectType.Stun:
    //            ApplyStunEffect();
    //            break;
    //        default:
    //            break;
    //    }
    //    timerEffect = 0f;
    //    CurrentEffectDuration = Duration;
    //    CurrentEffectValue = value;
    //    currentEffect = type;
    //}

    //public void UpdateEffect(float deltaTime)
    //{
    //    switch (currentEffect)
    //    {
    //        case EffectType.NONE:
    //            break;
    //        case EffectType.Fire:
    //            break;
    //        case EffectType.Poison:
    //            UpdatePoisonEffect(deltaTime);
    //            break;
    //        case EffectType.Stun:
    //            break;
    //        default:
    //            break;
    //    }

    //    timerEffect += deltaTime;
    //    if (timerEffect >= CurrentEffectDuration)
    //    {
    //        RemoveAllEffect(currentEffect);
    //        timerEffect = 0f;

    //    }
    //}

    //public void RemoveAllEffect(EffectType lastEffect)
    //{
    //    switch (lastEffect)
    //    {
    //        case EffectType.NONE:
    //            break;
    //        case EffectType.Fire:
    //            break;
    //        case EffectType.Poison:
    //            DestroyPoisonEffect();
    //            break;
    //        case EffectType.Stun:
    //            DestroyStunEffect();
    //            break;
    //        default:
    //            break;
    //    }
    //    currentEffect = EffectType.NONE;
    //}

    public void OnAppliedEffect(EffectType type, EffectHit data)
    {
        BaseEffectObject<Zombie> effectObj = null;
        _dictEffectObjects.TryGetValue(type, out effectObj);
        if (effectObj == null)
        {
            switch (type)
            {
                case EffectType.NONE:
                    break;
                case EffectType.PASSIVE_HIT_FIRE:
                    effectObj = new ZomFireEffect();
                    break;
                case EffectType.PASSIVE_HIT_POISON:
                    effectObj = new ZomPoisonEffect();
                    break;
                case EffectType.PASSIVE_HIT_FROZEN:
                    effectObj = new ZomFrozenEffect();
                    break;
                case EffectType.PASSIVE_HIT_STUN:
                    effectObj = new ZomStunEffect();
                    break;
                case EffectType.PASSIVE_HIT_LIGHTING:
                    effectObj = new ZomLightningEffect();
                    break;
                case EffectType.PASSIVE_HIT_BOMB:
                    effectObj = new ZomBombAfterDeathEffect();
                    break;
                case EffectType.PASSIVE_RICOCHET:
                    effectObj = new ZomRicochet();
                    break;
                case EffectType.PASSIVE_PUSHBACK:
                    effectObj = new ZomPushBack();
                    break;
                case EffectType.PASSIVE_MULTIPLY_SPEED:
                    effectObj = new ZomMultiplySpeed();
                    break;
                case EffectType.PASSIVE_HIT_NOVA:
                    effectObj = new ZomSlowNovaEffect();
                    break;
                case EffectType.PASSIVE_HIT_BURNING_AREA:
                    effectObj = new ZomBurningAreaEffect();
                    break;
                default:
                    effectObj = null;
                    break;
            }

            if (effectObj != null)
                _dictEffectObjects.Add(type, effectObj);
        }

        if (effectObj != null)
            effectObj.OnAppliedEffect(this, data);
    }

    private void OnEffectTicked(EffectType type)
    {
        BaseEffectObject<Zombie> effectObj = null;
        _dictEffectObjects.TryGetValue(type, out effectObj);
        if (effectObj != null)
        {
            effectObj.OnEffectTicked();
        }
        else
        {
            Debug.LogError($"Sth went wrong with effect: {type}");
        }
    }

    private void OnDestroyEffect(EffectType type)
    {
        BaseEffectObject<Zombie> effectObj = null;
        _dictEffectObjects.TryGetValue(type, out effectObj);
        if (effectObj != null)
        {
            effectObj.OnDestroyEffect();
        }
        else
        {
            //Debug.LogError($"Sth went wrong with effect: {type}");
        }
    }

    public void DestroyAllEffects()
    {
        if (_dictEffectObjects != null)
        {
            foreach (var item in _dictEffectObjects)
            {
                item.Value.OnDestroyEffect();
            }
        }
    }

    #endregion

    #region Animation
    public void SetAnimDie(bool enable)
    {
        //this.zmAnimator.SetBool("dying", enable);
    }

    public virtual void AnimationCallbackFinishShoot()
    {

    }
    #endregion

    #region Shoot Event
    protected float _force;
    protected float _dmg;
    protected ShotType _shotType;

    private bool _firstWeaponLaunch = false;


    protected virtual void Weapon_OnLaunch(float force, float damage, ShotType type)
    {
        if (_target == null || _target.IsDead())
        {
            SetState(ZOM_STATE.WALK);
            this.goToWallBeh.FindTargetPlayer();
            return;
        }

        _force = force;
        _dmg = damage;
        _shotType = type;

        if (this.weapon._launcher != null)
        {
            //destroy this zombie anyway
            if (this.weapon._launcher.GetType().Equals(typeof(SuicideLauncher)))
            {
                this.weapon._launcher.Launch(_force, _dmg, _shotType);
                this.weapon.SetPerfomShoot(false);
                DestroyZombie(0.2f);

            }
            else
            {
                //wait for anim call back
                //DoProjectileShoot(force, damage, type);
            }
            AudioSystem.instance.PlaySFX(this.SoundZomAttack);
        }
        else if (!_animationEvents)
        {

            if (_firstWeaponLaunch)
            {
                Timing.CallDelayed(attackDelay, () =>
                 {
                     _target?.SetDamage(_dmg, _shotType, $"Zombie{gameObject.GetInstanceID()}", null, (responseHit, armoursResponses) =>
                     {
                     }, _data.MinRange);
                     AudioSystem.instance.PlaySFX(this.SoundZomAttack);
                 }, this.gameObject);

                _firstWeaponLaunch = false;
            }
            else
            {
                _target?.SetDamage(_dmg, _shotType, $"Zombie{gameObject.GetInstanceID()}", null, (responseHit, armoursResponses) =>
                {
                }, _data.MinRange);
                AudioSystem.instance.PlaySFX(this.SoundZomAttack);
            }

        }



    }

    public void DoProjectileShoot(float force, float dmg, ShotType type)
    {
    }

    public virtual void AnimationCallbackShoot()
    {
        if (GamePlayController.instance.IsPausedGame)
            return;

        if (this.weapon._launcher != null)
        {
            this.weapon._launcher.Launch(_force, _dmg, _shotType);
            this.weapon.SetPerfomShoot(false);

        }
        else
        {

            _target?.SetDamage(_dmg, _shotType, $"Zombie{gameObject.GetInstanceID()}", null, (responseHit, armoursResponses) =>
            {
            }, _data.MinRange);
        }

        AudioSystem.instance.PlaySFX(this.SoundZomAttack);

    }

    #endregion

    #region State Machine
    private ZOM_STATE _previousState = ZOM_STATE.WALK;

    public void SetState(ZOM_STATE state)
    {
        //if (this._state == state)
        //    return;
        _previousState = this._state;
        this._state = state;
        switch (_state)
        {
            case ZOM_STATE.IDLE:
                SetStateIdle();
                break;
            case ZOM_STATE.NONE:
                break;
            case ZOM_STATE.RUN:
            case ZOM_STATE.WALK:
                SetStateWalk();
                break;
            case ZOM_STATE.ATTACK:
                SetStateAttack();
                break;
            case ZOM_STATE.DIE:
                break;
            default:
                break;
        }


    }

    public virtual void SetStateIdle()
    {
        this.zmAnimator.SetBool("attack", false);
        this.weapon.SetPauseBehaviour(true);
        this.goToWallBeh.SetPauseBehaviour(true);
        weapon.enabled = false;
        goToWallBeh.enabled = false;
    }

    public virtual void SetStateWalk()
    {
        this.zmAnimator.SetBool("attack", false);
        this.weapon.SetPauseBehaviour(true);
        this.goToWallBeh.SetPauseBehaviour(false);
        weapon.enabled = false;
        goToWallBeh.enabled = true;
        this.zmAnimator.speed = _animWalkSpeed;
    }

    public virtual void SetStateAttack()
    {
        this.zmAnimator.SetBool("attack", true);
        this.zmAnimator.speed = _animAttackSpeed;
        this.weapon.SetPauseBehaviour(false);
        this.goToWallBeh.SetPauseBehaviour(true);
        weapon.enabled = true;

    }

    public void RollbackState()
    {
        if (_previousState == ZOM_STATE.IDLE)
        {
            _previousState = ZOM_STATE.WALK;
            CalcAnimWalkSpeed((float)_data.DefSpeed);
            _oldAnimSpeed = _animWalkSpeed;
        }

        SetState(_previousState);
    }

    private float _oldAnimSpeed = 1.0f;
    public void SetCurrentAnimSpeed(float _newSpeed)
    {
        _oldAnimSpeed = this.zmAnimator.speed;
        this.zmAnimator.speed = _newSpeed;
    }

    public void ReverseAnimSpeed()
    {
        this.zmAnimator.speed = _oldAnimSpeed;
    }


    #endregion

    #region Zombie Utils
    public virtual Health FindTargetStragthforward()
    {
        Health result = null;
        RaycastHit hitInfo;
        bool isHit = Physics.Raycast(transform.position, Vector3.right, out hitInfo, 100f, ResourceManager.instance._maskHero);
        bool isHitL = Physics.Raycast(transform.position - 0.5f * transform.forward, Vector3.right, out hitInfo, 100f, ResourceManager.instance._maskHero);
        bool isHitR = Physics.Raycast(transform.position + 0.5f * transform.forward, Vector3.right, out hitInfo, 100f, ResourceManager.instance._maskHero);


        Debug.DrawLine(transform.position, transform.position + Vector3.right * 100f, Color.red);
        Debug.DrawLine(transform.position - 0.5f * Vector3.forward, transform.position + Vector3.right * 100f, Color.red);
        Debug.DrawLine(transform.position + 0.5f * Vector3.forward, transform.position + Vector3.right * 100f, Color.red);

        if (isHit || isHitL || isHitR)
        {
            result = hitInfo.transform.GetComponent<Health>();
        }
        else
        {
            if (this.goToWallBeh != null && this.goToWallBeh.wallTransform != null)
                result = this.goToWallBeh.wallTransform.GetComponent<Health>();
        }

        return result;
    }

    #endregion

    #region Materials
    [Header("Zombie Mats")]
    public Material sharedSpecialMat;


    private ZOMBIE_MAT _currentMat;

    public enum ZOMBIE_MAT
    {
        NORMAL = 0,
        FORZEN,
        FIRE
    }


    public Material zmNormalMat { get; private set; }
    public Material zmSpecialMat { get; private set; }

    public void SetMaterial(ZOMBIE_MAT _mat)
    {
        _currentMat = _mat;
        switch (_currentMat)
        {
            case ZOMBIE_MAT.NORMAL:
                this._skinMesh.material = zmNormalMat;
                break;
            case ZOMBIE_MAT.FORZEN:
                if (this.zmSpecialMat == null)
                {
                    this.zmSpecialMat = new Material(sharedSpecialMat);
                }
                this._skinMesh.material = zmSpecialMat;
                zmSpecialMat.SetColor("_Emission", FBUtils.HexToColor("#32A9C8"));
                zmSpecialMat.SetColor("_FresnelColor", FBUtils.HexToColor("#FFFFFF"));
                zmSpecialMat.SetFloat("_FresnelExponent", 2.455f);
                zmSpecialMat.SetFloat("_Metallic", 0.291f);
                break;
            case ZOMBIE_MAT.FIRE:
                if (this.zmSpecialMat == null)
                {
                    this.zmSpecialMat = new Material(sharedSpecialMat);
                }
                this._skinMesh.material = zmSpecialMat;
                zmSpecialMat.SetColor("_Emission", FBUtils.HexToColor("#FF0D00"));
                zmSpecialMat.SetColor("_FresnelColor", FBUtils.HexToColor("#FF9500"));
                zmSpecialMat.SetFloat("_FresnelExponent", 2.455f);
                zmSpecialMat.SetFloat("_Metallic", 0.291f);

                break;
            default:
                break;
        }
    }

    #endregion

    private bool _skipAnimDead = false;

    [Header("SFX")]
    [Space(10)]
    public SFX_ENUM SoundZomAttack = SFX_ENUM.SFX_ZOMBIE_ATTACK_01;
    //public SFX_ENUM SoundZomHit = SFX_ENUM.SFX_ZOMBIE_HIT_01;
    public SFX_ENUM SoundDead = SFX_ENUM.SFX_ZOMBIE_DEAD_01;


    [Header("Zombie Anim Speed")]
    [Range(0f, 10f)]
    public float _AnimWalkMultiplier = 1.0f;

    [Range(0f, 10f)]
    public float _AnimAttackMultiplier = 1.0f;

}

public class ZomStunEffect : BaseEffectObject<Zombie>
{
    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        parent.weapon.SetPauseBehaviour(true);
        parent.goToWallBeh.SetPauseBehaviour(true);
        parent.health.PlayEffectHitVisual(COMMON_FX.FX_STUNNED, data.Duration);
        parent.SetCurrentAnimSpeed(0f);

        // parent.zmNormalMat.color = Color.yellow;
    }

    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        _parent.weapon.SetPauseBehaviour(false);
        _parent.goToWallBeh.SetPauseBehaviour(false);
        // _parent.zmNormalMat.color = _parent.NormalColour;
    }
}

public class ZomPoisonEffect : BaseEffectObject<Zombie>
{
    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        parent.zmNormalMat.color = Color.green;
    }

    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        _parent.zmNormalMat.color = _parent.NormalColour;
    }

    public override void OnEffectTicked()
    {
        base.OnEffectTicked();
        var dmg = data.BaseDmg * _Design.Value * 1.0f / 100f / 2.0f;
        _parent.health.SetDamage(dmg, ShotType.POISON, _data.OwnerID);
    }
}

/// <summary>
/// Fire Effect tick percent dmg of zobie per seconds
/// </summary>
public class ZomFireEffect : BaseEffectObject<Zombie>
{
    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        //parent.zmMat.color = FBUtils.HexToColor("#F76C00");
    }

    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        // _parent.zmMat.color = _parent.NormalColour;
    }

    public override void OnEffectTicked()
    {
        base.OnEffectTicked();
        //var dmg = data.Value * _parent.health.GetHPWithCoeff() / 100f;
        var dmg = data.BaseDmg * data.Value / 100f / 2.0f;
        _parent.health.SetDamage(dmg, ShotType.FIRE, _data.OwnerID);
    }
}

/// <summary>
/// Value: time stunr
/// dmg = dmg + hero_dmg* percentdmg
/// </summary>
public class ZomLightningEffect : BaseEffectObject<Zombie>
{
    private bool isApplyingEffect = false;
    private LightningBoltScript lightningPrefab;

    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        if (isApplyingEffect)
            return;

        base.OnAppliedEffect(parent, data);
        isApplyingEffect = true;

        if (lightningPrefab == null)
        {
            AddressableManager.instance.LoadObjectAsync<LightningBoltScript>(POOLY_PREF.LIGHTING_LINE, (line) =>
            {
                lightningPrefab = line;
                DoCastLightning(parent);
            });
        }
        else
            DoCastLightning(parent);

        //Timing.CallDelayed(_Design.Duration, () =>
        //{
        //    isApplyingEffect = false;
        //});
    }

    public void DoCastLightning(Zombie parent)
    {
        Collider[] cols =
    Physics.OverlapSphere(parent.transform.position, _Design.Radius, ResourceManager.instance._maskZombieOnly);
        Vector3 lastStartPos = parent.transform.position;
        int counter = 0;
        if (cols != null && cols.Length > 0)
        {
            cols = cols.OrderBy(x => Mathf.Abs(parent.transform.position.x - x.transform.position.x)).ToArray();
            Timing.RunCoroutine(OnAppliedEffectCoroutine(cols.ToList(), parent));
        }

    }


    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        isApplyingEffect = false;
    }

    IEnumerator<float> OnAppliedEffectCoroutine(List<Collider> cols, Zombie parent)
    {
        Vector3 lastStartPos = parent.transform.position;
        ApplyStun(parent, data.Duration);

        int counter = 0;
        for (int i = cols.Count - 1; i >= 0; i--)
        {
            if (counter > data.Number)
                yield break;

            var randIndex = UnityEngine.Random.Range(0, cols.Count);
            var col = cols[randIndex];
            Zombie zom = col.GetComponentInParent<Zombie>();
            IHealth component = col.transform.GetComponent<IHealth>();
            if (component != null && col.gameObject.GetInstanceID() != parent.health.gameObject.GetInstanceID())
            {
                component.SetDamage(data.BaseDmg, ShotType.LIGHTNING, _data.OwnerID);

                var targetPos = component.HitMarker ? component.HitMarker.position : col.transform.position + Vector3.up * 0.2f;

                LightningBoltScript light = Pooly.Spawn<LightningBoltScript>(lightningPrefab.transform,
                    col.transform.position, Quaternion.identity, null);
                light.Initialize();
                light.PlayAnim(lastStartPos + Vector3.up * 0.2f, targetPos, 1f,
                    () => { Pooly.Despawn(light.transform); });
                lastStartPos = col.transform.position;

                // for stun effect
                ApplyStun(zom, data.Duration);
                //
                yield return Timing.WaitForSeconds(0.1f);
            }

            counter++;

            cols.RemoveAt(randIndex);
        }


        isApplyingEffect = false;
    }

    private void ApplyStun(Zombie zom, float duration)
    {
        if (zom != null)
        {
            //if (!zom.health.IsTargetable)
            //    return;

            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_CAST_SKILL_FORKED_LIGHTNING);

            if (GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                GameMaster.PlayEffect(POOLY_PREF.FX_LIGHTNING_IMPACT, zom.transform.position, Quaternion.identity, zom.transform);
            //zom.weapon.SetPauseBehaviour(true);
            //zom.goToWallBeh.SetPauseBehaviour(true);
            //zom.zmMat.color = zom.StunColor;

            if (zom._animMultiplySpeedHandler != null)
            {
                zom._animMultiplySpeedHandler.Kill();
            }

            zom.SetState(ZOM_STATE.IDLE);
            zom.health.PlayEffectHitVisual(COMMON_FX.FX_STUNNED, duration);
            zom.SetCurrentAnimSpeed(0f);
            Timing.CallDelayed(duration, () =>
            {
                //zom.weapon.SetPauseBehaviour(false);
                //zom.goToWallBeh.SetPauseBehaviour(false);
                //zom.zmMat.color = zom.NormalColour;
                zom.RollbackState();
                zom.ReverseAnimSpeed();
            });
        }
    }

}

public class ZomBurningAreaEffect : BaseEffectObject<Zombie>
{
    private AutoDespawnParticles fireFX;
    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        var rand = UnityEngine.Random.Range(0, 101f);
        if (rand <= data.Chance)
        {
            //fire effect
            if (fireFX == null)
            {
                fireFX = GameMaster.PlayEffect(COMMON_FX.FX_BURNING_AREA_MINI, parent.transform.position, Quaternion.identity, null, _Design.Radius);
            }
            else
            {
                if (!fireFX.IsPlayingEffect())
                {
                    fireFX.PlayEffect();
                }
            }


            fireFX.transform.SetParent(parent.transform);
            fireFX.transform.localPosition = Vector3.zero;
            fireFX.transform.localScale = Vector3.one * _parent.FooterEffectScale;
            fireFX.DelayDespawn = _Design.Duration;

        }

        //_parent.SetMaterial(Zombie.ZOMBIE_MAT.FIRE);
    }

    public override void OnEffectTicked()
    {
        base.OnEffectTicked();
        var dmg = data.BaseDmg * data.Value / 100f / 2.0f;
        _parent.health.SetDamage(dmg, ShotType.FIRE, _data.OwnerID, null, null, 0, new Vector3(0, 50, 0));
    }

    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        if (fireFX != null)
        {
            fireFX.transform.SetParent(null);
            fireFX.CleanUp();
        }
        _parent.SetMaterial(Zombie.ZOMBIE_MAT.NORMAL);

    }
}


public class ZomBombAfterDeathEffect : BaseEffectObject<Zombie>
{
    public static string EXPLODE_FX = "FX_Explode";

    private Health parentHealth;

    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        var rand = UnityEngine.Random.Range(0, 101);
        if (rand <= _Design.Value)
        {
            parentHealth = parent.health;
            parentHealth.OnDie = (Action<bool>)Delegate.Combine(parentHealth.OnDie, new Action<bool>(this.OnZombieDie));
        }
    }

    public void OnZombieDie(bool skipAnimDead)
    {
        Collider[] cols = Physics.OverlapSphere(_parent.transform.position, 1.0f,
            ResourceManager.instance._maskZombieOnly);
        Vector3 lastStartPos = _parent.transform.position;
        if (cols != null && cols.Length > 0)
        {
            var explodeFX =
                Pooly.Spawn<AutoDespawnParticles>(EXPLODE_FX, _parent.transform.position, Quaternion.identity, null);
            explodeFX.PlayEffect();

            var screenPos = GamePlayController.instance.GetMainCamera().WorldToScreenPoint(_parent.transform.position + Vector3.up * 0.3f);
            screenPos.z = 0;
            InGameCanvas.instance.ShowFloatingText(screenPos, $"BOOM!!!", 50, 20, FBUtils.HexToColor("#F76C00"), 1f);

            for (int i = cols.Length - 1; i >= 0; i--)
            {
                var col = cols[i];
                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null && col.gameObject.GetInstanceID() != _parent.health.gameObject.GetInstanceID())
                {
                    var dmg = data.BaseDmg * _Design.HeroBaseDmgPercent * 1.0f / 100f;
                    component.SetDamage(dmg, ShotType.FIRE, null);
                }
            }

            parentHealth.OnDie = (Action<bool>)Delegate.Remove(parentHealth.OnDie, new Action<bool>(this.OnZombieDie));
        }
    }
}


public class ZomFrozenEffect : BaseEffectObject<Zombie>
{
    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        var rand = UnityEngine.Random.Range(0, 101);
        if (rand <= _Design.Value)
        {
            parent.motor.SetSpeed(0.5f);
            parent.zmNormalMat.color = Color.cyan;
            parent.NormalColour = Color.cyan;
        }
    }

    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        _parent.motor.ResetSpeed();
        _parent.NormalColour = Color.white;
        _parent.zmNormalMat.color = _parent.NormalColour;
    }
}

public class ZomSlowNovaEffect : BaseEffectObject<Zombie>
{
    private Zombie _zom;
    private bool isApplied = false;

    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        _zom = parent;
        if (_zom.sharedSpecialMat == null)
        {
            _zom.zmNormalMat.color = Color.blue;
            _zom.NormalColour = Color.blue;
        }
        else
        {
            _zom.SetMaterial(Zombie.ZOMBIE_MAT.FORZEN);
        }

        _zom.ReduceZombieSpeed(this.data.Value / 100f);

        isApplied = true;
    }

    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        if (isApplied)
        {
            _zom.zmNormalMat.color = Color.white;
            _zom.NormalColour = Color.white;
            _zom.SetMaterial(Zombie.ZOMBIE_MAT.NORMAL);

            _zom.ReduceZombieSpeed(1.0f);

            isApplied = false;

        }

    }
}


public class ZomRicochet : BaseEffectObject<Zombie>
{
    private Transform lastTrf;
    private float DmgBullet;
    private Collider[] _listCollider;
    private int numHits = 0;

    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        if (_listCollider == null)
        {
            _listCollider = new Collider[5];
        }

        base.OnAppliedEffect(parent, data);

        DmgBullet = data.BaseDmg * _Design.Value / 100f;
        float counter = _Design.Number;
        numHits = Physics.OverlapSphereNonAlloc(parent.transform.position, 4.0f, _listCollider, ResourceManager.instance._maskZombieOnly);
        Vector3 lastStartPos = parent.transform.position;
        lastTrf = this._parent.transform;
        Timing.RunCoroutine(PlayRicochetCoroutine(_listCollider, numHits, DmgBullet, counter));
    }

    IEnumerator<float> PlayRicochetCoroutine(Collider[] cols, int numHit, float _dmgBullet, float counter)
    {
        if (cols != null && cols.Length > 0)
        {
            bool waitingBullet = false;
            for (int i = 0; i < numHit; i++)
            {
                if (counter < 0 || _dmgBullet <= 0f)
                    yield break;

                var col = cols[i];
                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null &&
                    col.gameObject.GetInstanceID() != this._parent.health.gameObject.GetInstanceID())
                {
                    var spawnPos = lastTrf.position + Vector3.up * 0.5f;
                    Bullet bullet = Pooly.Spawn<Bullet>(POOLY_PREF.DEFAULT_BULLET, spawnPos,
                        Quaternion.identity, null);
                    bullet.Initialize(null, null, ResourceManager.instance._maskZombieOnly);
                    bullet.transform.LookAtTarget(col.transform);
                    waitingBullet = true;
                    // bullet.Launch(10, remainDmg, ShotType.NORMAL, () => { waitingBullet = false; });
                    var endPos = col.transform.position + Vector3.up * 0.5f;
                    bullet.transform.DOMove(endPos, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        bullet.CastDmg(col.transform, _dmgBullet);
                        waitingBullet = false;
                        Pooly.Despawn(bullet.transform);
                    });

                    lastTrf = col.transform;
                    while (waitingBullet)
                    {
                        yield return Timing.WaitForOneFrame;
                    }

                    yield return Timing.WaitForOneFrame;
                }

                counter--;
            }
        }
    }
}

public class ZomPushBack : BaseEffectObject<Zombie>
{
    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        base.OnAppliedEffect(parent, data);
        parent.PushBack(_Design.Value, _Design.Duration);
    }
}

public class ZomMultiplySpeed : BaseEffectObject<Zombie>
{
    private Zombie _zom;
    private float _speedAdd;
    public override void OnAppliedEffect(Zombie parent, EffectHit data)
    {
        _zom = parent;
        base.OnAppliedEffect(parent, data);
        _speedAdd = (float)(parent._data.DefSpeed * (data.Value / 100f));
        parent.SetAdditionalSpeed(_speedAdd);
        parent.health.PlayEffectHitVisual(COMMON_FX.FX_MUSIC_NOTE, data.Duration);
    }

    public override void OnDestroyEffect()
    {
        base.OnDestroyEffect();
        _zom.SetAdditionalSpeed(0);
        _zom.EffectController.RemoveEffect(EffectType.PASSIVE_MULTIPLY_SPEED);
    }
}
