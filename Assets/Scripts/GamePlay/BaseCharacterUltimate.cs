using QuickType.SkillDesign;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MEC;
using com.datld.data;
using System;
using System.Linq;
using QuickType;

public interface ICharacterUltimate
{
    string SkillID { get; }
    bool IsUnlocked { get; set; }
    bool CanUse { get; set; }
    SkillDesignElement DesignSkill { get; set; }

    void PreInit(string skillID, bool isUnlocked, params object[] args);
    void PointerDownSkill(Vector2 screenPos);
    void BeginDragSkill(Vector2 screenPos);
    bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true);
    void PostSkill();
    void UpdateSkill(float deltaTime);
}

[Serializable]
public enum UltimateType
{
    NONE,
    CHAR_ULTIMATE,
    ADD_ON
}

public class BaseCharacterUltimate : MonoBehaviour, ICharacterUltimate
{
    public UltimateType type = UltimateType.CHAR_ULTIMATE;

    [Header("SFX")]
    [Space(10)]
    public SFX_ENUM SoundCastSkill = SFX_ENUM.NONE;

    protected string _skillID;
    protected bool _isUnlocked;
    protected bool _canUse;
    protected SkillDesignElement _design;
    protected float _timerCountDown = 0f;
    protected Vector3 _startSpawnPos;
    protected Vector3 _endSpanwPos;
    protected Vector3 _centerPos;
    protected string _OwnerID;
    protected Vector2 _pointerDownPos = Vector2.zero;

    protected float ReduceCountDownPercent = 0f;

    public AddOnItem _addOnItem;

    private bool IsInited = false;

    public string SkillID => _skillID;

    private float ReadyTime = 0f;
    public bool IsUnlocked { get => _isUnlocked; set => _isUnlocked = value; }

    public float GetCountDownTime()
    {
        return _design.CountDown * (100 - this.ReduceCountDownPercent) / 100f;
    }

    public bool CanUse {
        get => _canUse;
        set {
            _canUse = value;
            if (!_canUse)
            {
                //ReadyTime = Time.time + _design.CountDown;
                EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN, _skillID, Time.time, GetCountDownTime());
                Timing.CallDelayed(GetCountDownTime(), () =>
                {
                    _canUse = true;
                });
            }
        }
    }

    public SkillDesignElement DesignSkill { get => _design; set => _design = value; }
    public Character _parent { get; private set; }

    private Collider[] _hitDetectZombies;

    private void Awake()
    {
        IsInited = false;
        _timerCountDown = 0f;
    }

    public virtual void PointerDownSkill(Vector2 screenPos)
    {
        GamePlayController.instance.touchDetector.SetEnableTouch(false);
        if (!CanUse)
            return;
        _pointerDownPos = screenPos;
    }

    public virtual void NotifyDontHasAddon()
    {
        MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.COLLECT_MORE_TO_USE.AsLocalizeString());
    }

    public virtual void BeginDragSkill(Vector2 screenPos)
    {

    }

    /// <summary>
    /// Check pointer inside button ==> valid cast!!!!
    /// </summary>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    public virtual bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        Timing.CallDelayed(0.2f, () =>
        {
            GamePlayController.instance.touchDetector.SetEnableTouch(true);
        });

        var result = false;
        if (!CanUse)
            return false;


        if (checkValidCast)
            result = PointerInsideButton(screenPos);
        else
            result = true;
        if (result)
        {
            PostSkill();
        }

        return result;

    }

    public virtual void PostSkill()
    {
        CanUse = false;
        _timerCountDown = 0f;
        AudioSystem.instance.PlaySFX(this.SoundCastSkill);
        MissionManager.Instance.TriggerMission(MissionType.USE_ULTIMATE, _design.SkillId);
    }

    public virtual void SetReduceCountDown(float _value)
    {
        this.ReduceCountDownPercent = _value;
    }

    public virtual void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        _skillID = skillID;
        _isUnlocked = isUnlocked;
        _design = DesignHelper.GetSkillDesign(_skillID);

        if (args != null && args.Length > 0)
        {
            this._parent = (Character)args[0];
            if (this._parent != null)
                this._OwnerID = this._parent.Data.UniqueID;
        }

        if (type == UltimateType.ADD_ON)
        {
            _addOnItem = SaveManager.Instance.Data.GetAddOnItem(skillID);
        }

        CanUse = true;
        _startSpawnPos = GamePlayController.instance.gameLevel._skillZoneMarker.startSkillMarker.position;
        _endSpanwPos = GamePlayController.instance.gameLevel._skillZoneMarker.endSkillMarker.position;
        _centerPos = GamePlayController.instance.gameLevel._skillZoneMarker.centerSkillMarker.position;

        _hitDetectZombies = new Collider[100];
        IsInited = true;

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.REPLAY_CAMPAIGN_BATTLE, new Action(OnReplayCampaignBattle));
    }

    public virtual void SetParent(Character hero)
    {
        this._parent = hero;
    }

    private float _subTimer = 0f;
    public virtual void UpdateSkill(float deltaTime)
    {
        if (GamePlayController.instance != null && GamePlayController.instance.IsPausedGame)
            return;

        if (!CanUse && IsInited)
        {
            _subTimer += deltaTime;
            _timerCountDown += deltaTime;
            // EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN, _skillID, _timerCountDown, _design.CountDown, ReadyTime);

            if (_timerCountDown >= GetCountDownTime())
            {
                //EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN, _skillID, _design.CountDown + 10, _design.CountDown, ReadyTime);
                CanUse = true;
                _timerCountDown = 0f;
            }
        }
    }

    public virtual bool PointerInsideButton(Vector2 releasePos)
    {
        if (Vector2.Distance(_pointerDownPos, releasePos) <= 100f)
            return true;
        else
            return false;
    }

    /// <summary>
    /// To Save Duration for special boost skills
    /// </summary>
    public virtual void OnSaveGame()
    {

    }

    public virtual Vector3 DetectHugeZombieZone(Vector2 offsetDetect, float radius)
    {
        Vector3 worldPos = _centerPos;
        Vector3 travelPos = _endSpanwPos;

        var countZom = 0;
        while (travelPos.x > _endSpanwPos.x || travelPos.z < _endSpanwPos.z)
        {
            Collider[] result = new Collider[20];
            int counter = 0;
            counter = Physics.OverlapSphereNonAlloc(travelPos, radius, result, ResourceManager.instance._maskZombieOnly, QueryTriggerInteraction.Collide);
            if (result != null && result.Length > 0)
            {
                if (counter > countZom)
                {
                    countZom = counter;
                    worldPos = travelPos;
                }
            }
            travelPos.z += offsetDetect.y;
            if (travelPos.z >= _endSpanwPos.z)
            {
                if (travelPos.x >= _endSpanwPos.x)
                    break;

                travelPos.z = _startSpawnPos.z;
                travelPos.x += offsetDetect.x;
            }
        }

        return worldPos;
    }

    public virtual Vector3 DetectHugeZombieZone02()
    {
        Vector3 result = _centerPos;
        var radius = Vector3.Distance(_centerPos, _endSpanwPos);
        var numZombie = Physics.OverlapSphereNonAlloc(_centerPos, radius, _hitDetectZombies, ResourceManager.instance._maskZombieOnly);

        if (numZombie > 0)
        {
            float minDist = float.MaxValue;
            for (int i = 0; i < numZombie; i++)
            {
                var col = _hitDetectZombies[i];

                var dist = Vector3.Distance(col.transform.position, GamePlayController.instance.gameLevel._castleTrf.position);
                if (dist <= minDist)
                {
                    minDist = dist;
                    result = col.transform.position;
                }
            }

        }

        return result;
    }

    public virtual Zombie DetectNearestZombie()
    {
        Zombie result = null;
        var numZombie = Physics.OverlapSphereNonAlloc(_centerPos, int.MaxValue, _hitDetectZombies, ResourceManager.instance._maskZombieOnly);
        if (numZombie > 0)
        {
            float minDist = float.MaxValue;
            for (int i = 0; i < numZombie; i++)
            {
                var col = _hitDetectZombies[i];

                var dist = Vector3.Distance(col.transform.position, GamePlayController.instance.gameLevel._castleTrf.position);
                if (dist <= minDist)
                {
                    minDist = dist;
                    result = col.GetComponentInParent<Zombie>();
                }
            }
        }

        return result;

    }

    public virtual List<Health> GetRandomZombies()
    {
        List<Health> result = new List<Health>();
        var numZombie = Physics.OverlapSphereNonAlloc(_centerPos, int.MaxValue, _hitDetectZombies, ResourceManager.instance._maskZombieOnly);
        if (numZombie > 0)
        {
            for (int i = 0; i < numZombie; i++)
            {
                var col = _hitDetectZombies[i];

                result.Add(col.GetComponent<Health>());
            }
        }

        return result;
    }

    public virtual Health FindZombieBoss()
    {
        Health result = null;
        var numZombie = Physics.OverlapSphereNonAlloc(_centerPos, int.MaxValue, _hitDetectZombies, ResourceManager.instance._maskZombieOnly);
        if (numZombie > 0)
        {
            for (int i = 0; i < numZombie; i++)
            {
                var col = _hitDetectZombies[i];
                var health = col.GetComponent<Health>();
                if (health.IsZombieBoss)
                {
                    result = health;
                    break;
                }

            }
        }

        return result;
    }

    public virtual float GetUltimateDmg()
    {
        if (this._parent != null)
            return _design.Dmg + this._parent.Data.BaseHeroPower.Dmg * _design.HeroBaseDmgPercent / 100f;
        else
            return _design.Dmg + SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel * _design.HeroBaseDmgPercent;
    }

    public virtual void ResetSkill(bool hardReset = false)
    {

    }


    public virtual void CleanUp()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.REPLAY_CAMPAIGN_BATTLE, new Action(OnReplayCampaignBattle));
    }

    public virtual void OnReplayCampaignBattle()
    {
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN, _skillID, Time.time, 0);
        this.CanUse = true;
        this._timerCountDown = 0f;
    }
}



