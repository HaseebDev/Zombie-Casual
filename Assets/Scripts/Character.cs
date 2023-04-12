using com.datld.data;
using System;
using UnityEngine;
using Ez.Pooly;
using UnityEngine.EventSystems;
using HighlightPlus;
using DG.Tweening;
using MEC;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using com.datld.talent;
using UnityEngine.AddressableAssets;
using QuickType.Hero;

public enum CHARACTER_TYPE
{
    AUTOMATIC = 0,
    MANUAL
}

public enum CHARACTER_STATE
{
    NONE,
    NORMAL,
    DIE,
    REVIVE
}

public enum CHARACTER_BOOST_TYPE
{
    NONE = 0,
    INCREASE_HEADSHOT
}

[Serializable]
public class CharacterBoostData
{
    public CHARACTER_BOOST_TYPE type;
    public BoostItemView iconBoost;
    public float timerDuration;
    public float Duration;

    public bool IsFinished = false;
    private bool isInited = false;

    public CharacterBoostData(CHARACTER_BOOST_TYPE _type, BoostItemView _icon, float _Duration)
    {
        type = _type;
        iconBoost = _icon;
        Duration = _Duration;
        timerDuration = 0f;
        IsFinished = false;
        isInited = true;
    }

    public void RemoveThisBoost()
    {
        if (iconBoost != null)
        {
            iconBoost._canvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear)
                .OnComplete(() => { Pooly.Despawn(iconBoost.transform); });
        }
    }

    public void UpdateBoost(float _deltaTime)
    {
        if (!isInited || IsFinished)
            return;

        timerDuration += _deltaTime;
        if (timerDuration >= this.Duration)
        {
            timerDuration = 0f;
            IsFinished = true;
        }
    }
}


[Serializable]
public class EffectHit
{
    public EffectType Type;
    public float BaseDmg;
    public string SkillID;
    public float Duration;
    public float Value;
    public string OwnerID;
    public int Number;
    public float Chance;
}

[Serializable]
public struct EffectArmour
{
    public ARMOUR_EFFECT_TYPE Type;
    public float BaseDmg;
    public string SkillID;
    public float Value;
}

public class Character : MonoBehaviour, IBoostDamageEffect
{
    public static float REVIVE_DURATION = 10f;
    public static float CALC_SHOOT_SPEED_DURATION = 2.0f;
    //public variables
    public CHARACTER_TYPE type = CHARACTER_TYPE.AUTOMATIC;

    //private variables
    public HeroData Data { get; private set; }
    public WeaponData WeaponData { get; private set; }

    public HeroAnimMachine _animationController;

    private float _timerRevive = 0f;

    public List<EffectHit> effectHitDatas { get; private set; }
    public List<EffectArmour> effectArmours { get; private set; }

    public HeroDesign HeroDesign { get; private set; }

    public SkinnedMeshRenderer _meshCharacter;

    /// <summary>
    /// to Keep current launcher
    /// </summary>
    public bool KeepCurrentLauncher = false;

    [Header("Hightlight Effect")] public HighlightPlus.HighlightEffect _highlightFx;

    //[Header("Character ultimate")] public BaseCharacterUltimate CharacterUltimate;
    public UltimateBar UltimateBar;

    [Header("Boost Canvas")] private HeroBoostCanvas heroBoostCanvas;

    private Dictionary<string, BaseCharacterUltimate> _dictCharUltimate;

    #region State Machine

    private CHARACTER_STATE _state = CHARACTER_STATE.NONE;

    public void SetState(CHARACTER_STATE state)
    {
        _state = state;
        switch (state)
        {
            case CHARACTER_STATE.NONE:
                break;
            case CHARACTER_STATE.NORMAL:
                SetPlayerNormal();
                break;
            case CHARACTER_STATE.DIE:
                SetPlayerDie();
                break;
            case CHARACTER_STATE.REVIVE:
                SetPlayerRevive();
                break;
            default:
                break;
        }
    }

    public void SetPlayerNormal()
    {
        _timerRevive = 0f;
        Weapon weapon = this.weapon;
        weapon.OnLaunch = (Action<float, float, ShotType>)Delegate.Combine(weapon.OnLaunch,
            new Action<float, float, ShotType>(this.Weapon_OnLaunch));
        this.weapon.enabled = false;
        // this.towerBehavior.ApplyBehavior();
        TowerBehavior towerBehavior = this.towerBehavior;
        towerBehavior.OnTargetFound =
            (Action<Transform>)Delegate.Combine(towerBehavior.OnTargetFound,
                new Action<Transform>(this.TowerBehavior_OnTargetFound));
        TowerBehavior towerBehavior2 = this.towerBehavior;
        towerBehavior2.OnTargetSearch = (Action)Delegate.Combine(towerBehavior2.OnTargetSearch,
            new Action(this.TowerBehavior_OnTargetSearch));
    }

    public void SetPlayerDie()
    {
        towerBehavior.StopBehavior();
        weapon.StopBehaviour();
        // _animationController.SetBool("shoot", false);
        _animationController?.PlayAnimIdle();
    }

    public void SetPlayerRevive()
    {
        Initialize(Data);
        if (_health)
        {
            var Power = Data.GetPowerDataByGameMode();
            _health.SetParentObject(this);
            _health.Initialize(Power.Hp);
            _health.Revive();
        }
    }

    #endregion

    public float WeaponDamage {
        get { return this.weapon.damage; }
        set { this.weapon.damage = value; }
    }

    public int Level {
        get { return this.level; }
        set { this.level = value; }
    }

    public string HeroID { get; private set; }

    private void Start()
    {
        //_animationController = this.GetComponentInChildren<HeroAnimMachine>();
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.HERO_UPGRADED, new Action<string>(UpradeHero));

    }

    private void OnParashuteFallComplete()
    {
        this.weapon._launcher.ResetMountPoint();
        this.towerBehavior.ApplyBehavior();


    }

    // private void OnDestroy()
    // {
    //     DestroyHero();
    // }

    public void UpradeHero(string heroID)
    {
        if (heroID == HeroID)
        {
            Data = SaveManager.Instance.Data.GetHeroData(HeroID);
            ResetHeroData();
        }
    }

    public void ResetHeroData()
    {
        var PowerData = Data.GetPowerDataByGameMode();

        if (effectHitDatas == null)
            effectHitDatas = new List<EffectHit>();
        else
            effectHitDatas.Clear();

        //init passive
        var effectHitData = Data.GetInitEffectHit();
        if (effectHitData != null)
            effectHitDatas.Add(effectHitData);

        //weapon attributes
        var listWeponSpecial = Data.GetListEquippedWeaponEffectHit();
        if (listWeponSpecial != null && listWeponSpecial.Count > 0)
        {
            effectHitDatas.AddRange(listWeponSpecial);
        }

        weapon.ResetData(PowerData.Dmg, PowerData.Firerate, PowerData.HeadshotPercent, PowerData.CritPercent,
            effectHitDatas, Data.UniqueID, PowerData.Range);

        towerBehavior.ResetData(PowerData.Range, HeroDesign.TimeSwitchTarget);
        _timerRecalcSpeed = 0f;


    }

    public void ApplyCustomBullet(ref Launcher launcher, Transform gunMount, OverWriteBullet overwriteBullet)
    {
        if (overwriteBullet != null && overwriteBullet.gunBullet != null)
        {
            launcher.bulletPrefab = overwriteBullet.gunBullet.gameObject;

            if (overwriteBullet.FireForce > 0)
                launcher.FireForce = overwriteBullet.FireForce;

            if (overwriteBullet.muzzlePar != null)
            {
                launcher.shootParticlesEffects = new AutoDespawnParticles[1];
                launcher.shootParticlesEffects[0] = overwriteBullet.muzzlePar;
            }

            if (overwriteBullet.sfxShoot != SFX_ENUM.NONE)
            {
                launcher._soundFire = overwriteBullet.sfxShoot;
            }

            if (overwriteBullet.offsetGunMount > 0)
            {
                gunMount.position += transform.forward * overwriteBullet.offsetGunMount;
            }
        }
    }

    public void Initialize(HeroData data, bool skipParashute = false)
    {
        Data = data;
        HeroDesign = DesignHelper.GetHeroDesign(data);

        WeaponData = SaveManager.Instance.Data.GetWeaponData(Data.EquippedWeapon);
        if (!KeepCurrentLauncher)
        {
            if (this.weapon._launcher != null)
                GameObject.DestroyImmediate(this.weapon._launcher.gameObject);

            if (_animationController != null)
            {
                _animationController.OnLaunchBullet = LaunchBullet;
                _animationController.Initialize(WeaponData.Type, WeaponData.WeaponID);
                _animationController.LoadCurrentShootMarker(transform);
                _animationController.CalcShootSpeed(Data.FinalPowerData.Firerate);

                var launcherPref = ResourceManager.instance.getLauncherByType(WeaponData.Type);
                var overwriteBullet = ResourceManager.instance.GetOverwriteBullet(WeaponData.WeaponID);

                if (launcherPref != null)
                {
                    var launcher = GameObject.Instantiate(launcherPref, this.weapon.transform);
                    launcher.transform.localPosition = Vector3.zero;
                    launcher.transform.localScale = Vector3.one;
                    launcher.transform.localRotation = Quaternion.Euler(0, -90, 0);
                    ApplyCustomBullet(ref launcher, _animationController.CurrentShootMarker, overwriteBullet);

                    this.weapon.Initialize(launcher);
                }

                this.weapon._launcher.Initialize(_animationController.CurrentShootMarker, this.weapon);
                this.towerBehavior.SetGunMount(this.weapon._launcher.weaponMountPoint);
            }
            else
            {
                var launcherPref = ResourceManager.instance.getLauncherByType(WeaponData.Type);
                var overwriteBullet = ResourceManager.instance.GetOverwriteBullet(WeaponData.WeaponID);

                var fakeMountPoint = new GameObject("fakeMountPoint");
                fakeMountPoint.transform.SetParent(transform);
                fakeMountPoint.transform.localRotation = Quaternion.Euler(Vector3.zero);
                fakeMountPoint.transform.localPosition = new Vector3(0, 0.5f, 0.6f);

                if (launcherPref != null)
                {
                    var launcher = GameObject.Instantiate(launcherPref, this.weapon.transform);
                    launcher.transform.localPosition = Vector3.zero;
                    launcher.transform.localScale = Vector3.one;
                    launcher.transform.localRotation = Quaternion.Euler(0, -90, 0);
                    ApplyCustomBullet(ref launcher, fakeMountPoint.transform, overwriteBullet);

                    this.weapon.Initialize(launcher);
                }



                this.weapon._launcher.Initialize(fakeMountPoint.transform, this.weapon);
                this.towerBehavior.SetGunMount(this.weapon._launcher.weaponMountPoint);
            }
        }


        HeroID = Data.UniqueID;
        effectHitDatas = new List<EffectHit>();
        effectArmours = new List<EffectArmour>();

        _dictCharUltimate = new Dictionary<string, BaseCharacterUltimate>();

        ResetHeroData();

        EnableHero(true);

        SetState(CHARACTER_STATE.NORMAL);

        if (type == CHARACTER_TYPE.MANUAL)
        {
            skipParashute = true;
            InitForManualHero();
        }

        //test only must replace later
        if (data.ListUltimates.Count > 0)
        {
            for (int i = 0; i < data.ListUltimates.Count; i++)
            {
                var skill = data.ListUltimates[i];
                // var path = $"GameData/Ultimate/{DesignHelper.ConvertSkillID(skill)}";
                //var skillRes = Resources.Load<BaseCharacterUltimate>(path);

                Timing.RunCoroutine(SpawnSkillInstance(DesignHelper.ConvertSkillID(skill), (skillRes) =>
                 {
                     if (skillRes != null)
                     {
                         BaseCharacterUltimate skillInstance = GameObject.Instantiate(skillRes, transform);
                         skillInstance.SetReduceCountDown(this.Data.FinalPowerData.ReduceSkillCountDownPercent);
                         skillInstance.PreInit(skill, false, this);
                         if (!_dictCharUltimate.ContainsKey(skill))
                         {
                             _dictCharUltimate.Add(skill, skillInstance);
                         }
                     }
                     else
                     {
                         Debug.Log($"Cant not find skill path:");
                     }
                 }));
            }
        }

        //boost

        AddressableManager.instance.SpawnObjectAsync<HeroBoostCanvas>(POOLY_PREF.HERO_BOOST_CANVAS, (boostCanvas) =>
         {
             heroBoostCanvas = boostCanvas;
             heroBoostCanvas.transform.SetParent(transform);
             heroBoostCanvas.transform.localScale = Vector3.one;
             heroBoostCanvas.transform.localPosition = Vector3.zero;
         });


        parashuteFallBehavior.Initialize(skipParashute);
        parashuteFallBehavior.OnBehaviourComplete = OnParashuteFallComplete;
        parashuteFallBehavior.PreInit();
        Timing.CallDelayed(0.5f, () =>
        {
            this.parashuteFallBehavior.ApplyBehavior();
        });

        _timerRecalcSpeed = 0f;
    }

    IEnumerator<float> SpawnSkillInstance(string SkillID, Action<BaseCharacterUltimate> callback)
    {
        BaseCharacterUltimate result = null;
        var op = Addressables.LoadAssetAsync<GameObject>(DesignHelper.ConvertSkillID(SkillID));
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        if (op.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"SpawnSkillInstance error {SkillID}");
            callback?.Invoke(result);
            yield break;
        }

        result = op.Result.GetComponent<BaseCharacterUltimate>();
        callback?.Invoke(result);
    }

    private Launcher _launcher;
    private float _timerRecalcSpeed;
    public void UpdateWeaponData(WeaponData weaponData)
    {
        WeaponData = weaponData;
        WeaponData.ResetPowerData();

        if (_animationController != null)
        {
            _animationController.OnLaunchBullet = LaunchBullet;
            _animationController.Initialize(WeaponData.Type, WeaponData.WeaponID);
            _animationController.LoadCurrentShootMarker(transform);
            _animationController.CalcShootSpeed(Data.FinalPowerData.Firerate);


            var launcherPref = ResourceManager.instance.getLauncherByType(WeaponData.Type);
            var overwriteBullet = ResourceManager.instance.GetOverwriteBullet(WeaponData.WeaponID);
            if (launcherPref != null)
            {
                if (_launcher != null)
                    DestroyImmediate(_launcher.gameObject);

                this.weapon.transform.DestroyAllChildImmediate();

                _launcher = GameObject.Instantiate(launcherPref, this.weapon.transform);
                _launcher.transform.localPosition = Vector3.zero;
                _launcher.transform.localScale = Vector3.one;
                _launcher.transform.localRotation = Quaternion.Euler(0, -90, 0);
                ApplyCustomBullet(ref _launcher, _animationController.CurrentShootMarker, overwriteBullet);
                this.weapon.Initialize(_launcher);
            }

            this.weapon._launcher.Initialize(_animationController.CurrentShootMarker, this.weapon);
            this.towerBehavior.SetGunMount(this.weapon._launcher.weaponMountPoint);

            _animationController.CalcShootSpeed(Data.FinalPowerData.Firerate);
            //force calc one more time!

            //Timing.CallDelayed(0.5f, () =>
            //{
            //    _animationController.CalcShootSpeed(Data.FinalPowerData.Firerate);
            //});
        }

        ResetHeroData();
    }

    public virtual void TowerBehavior_OnTargetSearch()
    {
        //_animationController.SetBool("shoot", false);
        _animationController?.PlayAnimIdle();
        this.weapon.enabled = false;
    }

    public virtual void TowerBehavior_OnTargetFound(Transform target)
    {
        //_animationController.SetBool("shoot", true);

        this.firstLaunch = true;
        this.weapon.enabled = true;
    }

    public void BoostDamageEffect(bool _status)
    {
        this.weapon.DoubleDamage(_status);
    }

    public void UpdateCharacter(float _deltaTime)
    {
        this.towerBehavior.UpdateBehaviour(_deltaTime);
        this.weapon.UpdateBehaviour(_deltaTime);
        switch (type)
        {
            case CHARACTER_TYPE.AUTOMATIC:
                UpdateCharacterAutomatic(_deltaTime);
                break;
            case CHARACTER_TYPE.MANUAL:
                UpdateCharacterManual(_deltaTime);
                break;
            default:
                break;
        }

        if (_dictCharUltimate != null)
        {
            foreach (var item in _dictCharUltimate)
            {
                item.Value.UpdateSkill(_deltaTime);
            }
        }

        UpdateBoostCharacter(_deltaTime);

        _timerRecalcSpeed += _deltaTime;
        if (_timerRecalcSpeed >= CALC_SHOOT_SPEED_DURATION)
        {
            _timerRecalcSpeed = 0f;
            if (_animationController != null)
                _animationController.CalcShootSpeed(Data.FinalPowerData.Firerate);
        }

        //DebugLog();
    }

    private void UpdateCharacterAutomatic(float deltaTime)
    {
        if (_state == CHARACTER_STATE.DIE)
        {
            _timerRevive += deltaTime;
            if (_timerRevive >= REVIVE_DURATION)
            {
                SetState(CHARACTER_STATE.REVIVE);
            }
        }
    }

    public void DebugLog()
    {
        var Category = this.Data.UniqueID;

        foreach (var item in effectHitDatas)
        {
            Logwin.Log($"EffectHits_{item.SkillID}", item, Category);
        }
    }

    [SerializeField] public Weapon weapon;

    [SerializeField] private TowerBehavior towerBehavior;

    [SerializeField] public ParashuteFallBehavior parashuteFallBehavior;

    //[SerializeField] private Animator animator;
    private int level;

    public Health _health;

    public void ApplyHealth(Health health)
    {
        this._health = health;
        _health.OnHit = (Action<float>)Delegate.Combine(_health.OnHit, new Action<float>(OnHeroHit));
        _health.OnDie = (Action<bool>)Delegate.Combine(_health.OnDie, new Action<bool>(OnHeroDie));
    }

    private void OnHeroDie(bool skipAnimDead)
    {
        Debug.Log("On Hero Die!!!");
        SetState(CHARACTER_STATE.DIE);
    }

    private void OnHeroHit(float dmg)
    {
    }

    #region Manual Character

    public static float TIME_AUTO_DISABLE_HERO = 1.0f;
    public int MAX_MANUAL_TOUCH_ULTIMATE = 20;
    public float UlitimateDmgPercent = 200f;
    public bool IsEnableHero = false;

    private PireceBullet pireceBulletPrefab;

    private float timerDisableHero = 0f;
    public float CountUltimateShoot;

    public void InitForManualHero()
    {
        var des = DesignHelper.GetSkillDesign("SKILL_MANUAL_HERO");
        if (des != null)
        {
            MAX_MANUAL_TOUCH_ULTIMATE = (int)des.Number;
            UlitimateDmgPercent = (float)des.HeroBaseDmgPercent;
        }

        if (UltimateBar)
        {
            UltimateBar.Initialize(MAX_MANUAL_TOUCH_ULTIMATE);
        }

        AddressableManager.instance.LoadObjectAsync<PireceBullet>(POOLY_PREF.PIERCE_BULLET, (bullet) =>
          {
              pireceBulletPrefab = bullet;
          });
    }

    public void IncreaseUltimateShoot()
    {
        CountUltimateShoot += 1 + ModelTalent.angleRageIncreasePercent / 100f;
        // CountUltimateShoot += 1;
        UltimateBar?.SetPowerValue(CountUltimateShoot);
    }

    public void ResetTimeDisableHero()
    {
        timerDisableHero = TIME_AUTO_DISABLE_HERO;
    }

    public void SetTimeDisableHero(float timerDisable)
    {
        timerDisableHero = timerDisable;
    }

    public void EnableHero(bool enable)
    {
        IsEnableHero = enable;
        if (enable)
            gameObject.SetActiveIfNot(true);

        DOFade(enable ? 1 : 0, 0.3f, () => { gameObject.SetActiveIfNot(enable); });

        if (enable && _highlightFx != null)
        {
            CountUltimateShoot = 0;
            Highlight(enable);
        }

        UltimateBar?.SetEnable(enable);
        if (enable)
            UltimateBar?.SetPowerValue(CountUltimateShoot);
    }

    public void SetPosition(Vector3 _pos)
    {
        transform.position = _pos;
    }

    public void FindTargetThenShoot(Vector3 worldPos, bool forceShoot)
    {
        if (towerBehavior.ManualFindTarget(worldPos))
        {
            IncreaseUltimateShoot();
            if (IsUltimateShoot(ref CountUltimateShoot))
            {
                UltimateBar?.SetPowerValue(CountUltimateShoot);
                shootUltimate();
            }
            else
            {
                weapon?.FireWeapon();
            }
        }
        else if (forceShoot)
        {
            UltimateBar?.SetPowerValue(CountUltimateShoot);
            weapon.enabled = true;
            weapon?.FireWeapon();
        }
    }

    public void ShootAtTarget(Health target)
    {
        if (towerBehavior.ManualLockTarget(target))
        {
            IncreaseUltimateShoot();
            if (IsUltimateShoot(ref CountUltimateShoot))
            {
                UltimateBar?.SetPowerValue(CountUltimateShoot);
                shootUltimate();
            }
            else
            {
                weapon?.FireWeapon();
            }
        }
        else
        {
            UltimateBar?.SetPowerValue(CountUltimateShoot);
            weapon.enabled = true;
            weapon?.FireWeapon();
        }
    }

    private void shootUltimate()
    {
        if (!pireceBulletPrefab)
            return;
        var _bullet =
            Pooly.Spawn<PireceBullet>(pireceBulletPrefab.transform, transform.position, Quaternion.identity, null);
        _bullet.Initialize(null, Data.UniqueID, ResourceManager.instance._maskZombieOnly);
        _bullet.gameObject.SetActiveIfNot(true);
        _bullet.transform.position = transform.position + Vector3.up;
        _bullet.transform.position += transform.forward * 0.3f;
        _bullet.transform.rotation = transform.rotation;
        _bullet.Launch(20, weapon.damage * UlitimateDmgPercent / 100f, ShotType.NORMAL);
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_CAST_SKILL_MANUAL_HERO);
    }

    public bool IsUltimateShoot(ref float touchCount)
    {
        bool result = false;
        if (touchCount >= MAX_MANUAL_TOUCH_ULTIMATE)
        {
            result = true;
            touchCount = 0;
        }

        return result;
    }

    public void UpdateCharacterManual(float _deltaTime)
    {
        if (IsEnableHero)
        {
            timerDisableHero -= _deltaTime;

            if (timerDisableHero <= 0)
            {
                EnableHero(false);
            }
            else if (timerDisableHero <= TIME_AUTO_DISABLE_HERO - 0.5f)
            {
                //_animationController.SetBool("shoot", false);
                _animationController?.PlayAnimIdle();
            }
        }
    }

    void Highlight(bool state)
    {
        if (_highlightFx == null && state)
        {
            return;
        }

        if (_highlightFx != null)
        {
            _highlightFx.SetHighlighted(state);
        }
    }

    #endregion

    public void ResetHero()
    {
        this.towerBehavior.CleanUp();
    }

    public void RestartLevel()
    {
        if (_dictCharUltimate != null)
        {
            foreach (var skill in _dictCharUltimate)
            {
                skill.Value.ResetSkill();
            }
        }
    }

    public void DestroyHero()
    {
        TowerBehavior towerBehavior = this.towerBehavior;
        towerBehavior.OnTargetFound =
            (Action<Transform>)Delegate.Remove(towerBehavior.OnTargetFound,
                new Action<Transform>(this.TowerBehavior_OnTargetFound));
        TowerBehavior towerBehavior2 = this.towerBehavior;
        towerBehavior2.OnTargetSearch = (Action)Delegate.Remove(towerBehavior2.OnTargetSearch,
            new Action(this.TowerBehavior_OnTargetSearch));
        Weapon weapon = this.weapon;
        weapon.OnLaunch = (Action<float, float, ShotType>)Delegate.Remove(weapon.OnLaunch,
            new Action<float, float, ShotType>(this.Weapon_OnLaunch));

        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.HERO_UPGRADED, new Action<string>(UpradeHero));


        if (heroBoostCanvas != null)
            Addressables.ReleaseInstance(heroBoostCanvas.gameObject);

        this.towerBehavior.CleanUp();
        Destroy(gameObject);

        if (_dictCharUltimate != null)
        {
            foreach (var skill in _dictCharUltimate)
            {
                skill.Value.CleanUp();
            }
        }


        // Pooly.Despawn(transform);
    }

    #region Ultimate

    public void PointerDownUltimate(string skillID, Vector2 pos)
    {
        BaseCharacterUltimate skillInstance = null;
        _dictCharUltimate.TryGetValue(skillID, out skillInstance);
        if (skillInstance)
            skillInstance?.PointerDownSkill(pos);
    }

    public void PointerUpUltimate(string skillID, Vector2 pos)
    {
        BaseCharacterUltimate skillInstance = null;
        _dictCharUltimate.TryGetValue(skillID, out skillInstance);
        if (skillInstance)
            skillInstance?.PointerUpSkill(pos);
    }

    public void BeginDragUltimate(string skillID, Vector2 pos)
    {
        BaseCharacterUltimate skillInstance = null;
        _dictCharUltimate.TryGetValue(skillID, out skillInstance);
        if (skillInstance)
            skillInstance?.BeginDragSkill(pos);
    }

    #endregion

    #region Utils

    public void DOFade(float alpha, float duration = 0.3f, Action OnComplete = null)
    {
        if (_meshCharacter == null)
            return;

        if (_meshCharacter.material.HasProperty("_Color"))
        {
            _meshCharacter.material.DOFade(alpha, duration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        else
        {
            OnComplete?.Invoke();
        }
    }

    #endregion

    #region Boost

    /// <summary>
    /// Must replace Boost system later!!!
    /// </summary>
    private bool IsBoostedHeadShot = false;

    private List<CharacterBoostData> _listCurrentBoosts = new List<CharacterBoostData>();

    public void SetBoostCharacter(CHARACTER_BOOST_TYPE _type, float value, float Duration, Sprite iconSprite)
    {
        var exists = _listCurrentBoosts.FirstOrDefault(x => x.type == _type);
        if (exists == null)
        {
            switch (_type)
            {
                case CHARACTER_BOOST_TYPE.NONE:
                    break;
                case CHARACTER_BOOST_TYPE.INCREASE_HEADSHOT:
                    this.weapon.SetAdditionalHeadshotPercent(value);
                    heroBoostCanvas?.SetIconBoost(iconSprite, (boostView) =>
                     {
                         _listCurrentBoosts.Add(new CharacterBoostData(_type, boostView, Duration));
                     });
                    break;
                default:
                    break;
            }
        }
    }

    public void RemoveBoostCharacter(CHARACTER_BOOST_TYPE type)
    {
        switch (type)
        {
            case CHARACTER_BOOST_TYPE.NONE:
                break;
            case CHARACTER_BOOST_TYPE.INCREASE_HEADSHOT:
                this.weapon.SetAdditionalHeadshotPercent(0);
                break;
            default:
                break;
        }
    }

    public void UpdateBoostCharacter(float _deltaTime)
    {
        if (_listCurrentBoosts == null || _listCurrentBoosts.Count <= 0)
            return;

        for (int i = _listCurrentBoosts.Count - 1; i >= 0; i--)
        {
            _listCurrentBoosts[i].UpdateBoost(_deltaTime);
            if (_listCurrentBoosts[i].IsFinished)
            {
                _listCurrentBoosts[i].RemoveThisBoost();
                RemoveBoostCharacter(_listCurrentBoosts[i].type);
                _listCurrentBoosts.RemoveAt(i);
            }
        }
    }

    #endregion

    #region Effect hits Data

    public void AddEffectHit(EffectHit newEffect)
    {
        var exists = effectHitDatas.Find(x => x.Type == newEffect.Type);
        if (exists == null)
        {
            effectHitDatas.Add(newEffect);
        }
    }

    public void RemoveEffectHit(EffectType type)
    {
        var exists = effectHitDatas.Find(x => x.Type == type);
        if (exists != null)
        {
            effectHitDatas.Remove(exists);
        }
    }

    #endregion

    #region Weapon Shoot

    private float _force;
    private float _dmg;
    private ShotType _shotType;

    private ShotType _lastShotType;
    private bool firstLaunch = false;

    public virtual void Weapon_OnLaunch(float force, float dmg, ShotType type)
    {
        _force = force;
        _dmg = dmg;
        _shotType = type;
        if (this._animationController != null)
        {
            this._animationController.PlayAnimShoot((complete) =>
            {
                //this.weapon._launcher.Launch(force, dmg, type);
                //this.weapon.SetPerfomShoot(false);
            });
        }
        else
        {
            LaunchBullet();
        }
    }

    public void LaunchBullet()
    {
        if (GamePlayController.instance != null && GamePlayController.instance.IsPausedGame)
            return;

        if (firstLaunch)
        {
            this.weapon._launcher.ResetMountPoint();
            firstLaunch = false;
        }

        if (_lastShotType == _shotType && _shotType == ShotType.HEADSHOT)
            _shotType = ShotType.NORMAL;

        this.weapon._launcher.Launch(_force, _dmg, _shotType);
        this.weapon.SetPerfomShoot(false);

        _lastShotType = _shotType;
    }

    #endregion

}