using MEC;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Health : MonoBehaviour, IHealth
{
    [Header("Markers")]
    public Transform _hitMarker;
    public Transform _fxMarker;


    public bool IsShowFloatingDmg = true;
    public float _effectScaleFactor = 1f;
    public string lastCasterId = null;
    public Action<Collider> _OnTriggerEnter;

    private FloatingTextData _floatingTextData;

    //ZOMBIE VARIABLES
    public bool IsZombieBoss { get; private set; }

    public bool HideFloatingText { get; private set; }

    public object Parent { get; private set; }

    public EffectController EffectController { get; set; }

    public Transform HitMarker => _hitMarker;
    public float EffectScaleFactor { get => _effectScaleFactor; set => _effectScaleFactor = value; }

    public void Revive()
    {
        this.currentHp = this.GetHPWithCoeff();
        this.isDead = false;
        this.UpdateHpView();
    }

    public void SetParentObject(object parent)
    {
        this.Parent = parent;
    }

    public virtual void Initialize(float MaxHp, bool _allowHeadshot = false, EffectController effectController = null, bool IsZombieBoss = false)
    {
        startHp = currentHp = MaxHp;
        healthMultiplier = 1.0f;
        allowHeadshot = _allowHeadshot;
        this.EffectController = effectController;
        this.gameObject.SetActive(true);
        if (this.hpBar != null)
            hpBar.EnableHPBar(false, false);

        this.isDead = false;
        this.HideFloatingText = false;
        if (!HideFloatingText)
            _floatingTextData = new FloatingTextData();

        this.IsZombieBoss = IsZombieBoss;
    }

    private void UpdateHpView()
    {
        if (this.hpBar == null)
        {
            return;
        }

        if (this.currentHp <= 0f)
        {
            this.hpBar.gameObject.SetActive(false);
            return;
        }

        //this.hpBar.gameObject.SetActive(true);
        //this.hpBar.SetHp(this.currentHp, this.GetHPWithCoeff());
    }

    public float GetHPWithCoeff()
    {
        return this.startHp * this.healthMultiplier;
    }

    public void UpdateHealthMultiplier(float _healthMultiplier)
    {
        this.healthMultiplier = _healthMultiplier;
        this.Revive();
    }

    public void DoCastDmg(float dmg)
    {
        if (GamePlayController.instance)
        {
            if (!GamePlayController.instance.CheatNeverEndBattle)
                this.currentHp -= dmg;
        }
        else
        {
            this.currentHp -= dmg;
        }

    }

    private void Start()
    {
        this.currentHp = this.GetHPWithCoeff();
        this.UpdateHpView();
    }

    public virtual void SetDamage(float _dmg, ShotType _type = ShotType.NORMAL, string CasterID = null,
        List<EffectHit> effectHits = null,
        Action<bool, List<EffectArmour>> responseHit = null, float range = 0, Vector3 offset = default(Vector3),
        List<int> _listIgnoreEffectHitsIndex = null)
    {
        lastCasterId = CasterID;

        if (this.isDead)
        {
            return;
        }

        if (effectHits != null && effectHits.Count > 0)
        {
            for (int i = effectHits.Count - 1; i >= 0; i--)
            {
                if (_listIgnoreEffectHitsIndex != null && _listIgnoreEffectHitsIndex.Count > 0)
                {
                    //ignore this effect!
                    if (_listIgnoreEffectHitsIndex.Contains(i))
                        continue;
                }

                var effect = effectHits[i];
                if (effect.Type == EffectType.PASSIVE_MADNESS)
                {
                    if (_type == ShotType.NORMAL)
                    {
                        var remainPercentHP = (this.currentHp * 1.0f) / this.GetHPWithCoeff() * 1.0f;

                        //trigger madness
                        if (remainPercentHP <= effect.Value / 100f)
                        {
                            var rand = UnityEngine.Random.Range(0f, 1.0f);
                            if (rand <= effect.Chance)
                            {
                                //enable crit for madness
                                _dmg *= 2;
                                _type = ShotType.CRIT;
                            }
                        }

                    }
                }
                else if (effect.Type == EffectType.PASSIVE_FURY_SHOT)
                {
                    CalcDmgForFuryShoot(ref _dmg, effect);
                }
                else
                {
                    //effect.Value = _dmg;
                    if (EffectController != null)
                        EffectController.AddEffect(effect);
                }
            }
        }
        float castDmg = 0f;
        if (_type == ShotType.HEADSHOT && allowHeadshot)
        {
            castDmg = GetHPWithCoeff();

            DoCastDmg(castDmg);
        }
        else if (_type == ShotType.DEADSHOT && allowHeadshot)
        {
            castDmg = _dmg;
            if (this.currentHp <= 0.3f * this.startHp)
            {
                var rand = UnityEngine.Random.Range(0, 101);
                if (rand <= 50)
                {
                    castDmg = startHp;

                    DoCastDmg(startHp);
                }
                else
                    _type = ShotType.NORMAL;
            }
            else
            {
                _type = ShotType.NORMAL;
            }

            DoCastDmg(castDmg);
        }
        else
        {
            castDmg = _dmg;


            DoCastDmg(_dmg);
        }

        if (_type != ShotType.NORMAL_SKIP_FX)
            this.OnHit?.Invoke(castDmg);

        if (this.hpBar != null)
            hpBar.EnableHPBar(true, false);

        this.UpdateHpView();
        if (this.currentHp <= 0f)
        {
            this.isDead = true;
            OnDie?.Invoke(this._skipAnimDead);
            SetDie();
        }

        if (InGameCanvas.instance != null && IsShowFloatingDmg)
            ShowFloatingDmg(_type, castDmg, offset);

        this.hpBar?.SetHp(this.currentHp, this.GetHPWithCoeff());

    }

    public virtual void ShowFloatingDmg(ShotType _type, float castDmg, Vector3 offset)
    {
        if (!IsShowFloatingDmg || castDmg <= 0.5f)
            return;

        var hitPos = this.HitMarker != null ? this.HitMarker.position + Vector3.up * 0.1f : transform.position + Vector3.up * 0.8f;
        var screenPos = GamePlayController.instance.GetMainCamera().WorldToScreenPoint(hitPos);
        screenPos.z = 0;
        screenPos += offset;

        if (_type == ShotType.NORMAL || _type == ShotType.FIRE || _type == ShotType.POISON ||
            _type == ShotType.LIGHTNING)
        {
            _floatingTextData.ScreenPos = screenPos;
            _floatingTextData.Content = GamePlayController.instance.FloatingTextQueue.GetDmgText(castDmg);
            _floatingTextData.Size = 75;
            _floatingTextData.MoveHeight = 100;
            _floatingTextData.Color = Color.white;
            _floatingTextData.Duration = 0.5f;
            DoShowFloatingText(_floatingTextData);
        }
        else if (_type == ShotType.CRIT || (_type == ShotType.HEADSHOT && !allowHeadshot))
        {
            _floatingTextData.ScreenPos = screenPos;
            _floatingTextData.Content = GamePlayController.instance.FloatingTextQueue.GetDmgText(castDmg);
            _floatingTextData.Size = 90;
            _floatingTextData.MoveHeight = 110;
            _floatingTextData.Color = Color.red;
            _floatingTextData.Duration = 1f;
            DoShowFloatingText(_floatingTextData);
        }
        else if (_type == ShotType.HEADSHOT && allowHeadshot)
        {
            _floatingTextData.ScreenPos = screenPos;
            _floatingTextData.Content = $"Headshot!";
            _floatingTextData.Size = 90;
            _floatingTextData.MoveHeight = 110;
            _floatingTextData.Color = Color.red;
            _floatingTextData.Duration = 1f;
            DoShowFloatingText(_floatingTextData);
        }
        else if (_type == ShotType.DEADSHOT && AllowHeadshot)
        {
            _floatingTextData.ScreenPos = screenPos;
            _floatingTextData.Content = $"DeadShot!";
            _floatingTextData.Size = 90;
            _floatingTextData.MoveHeight = 110;
            _floatingTextData.Color = Color.red;
            _floatingTextData.Duration = 1f;
            DoShowFloatingText(_floatingTextData);
        }
        else
        {
            _floatingTextData.ScreenPos = screenPos;
            _floatingTextData.Content = GamePlayController.instance.FloatingTextQueue.GetDmgText(castDmg);
            _floatingTextData.Size = 75;
            _floatingTextData.MoveHeight = 100;
            _floatingTextData.Color = Color.white;
            _floatingTextData.Duration = 0.5f;
            DoShowFloatingText(_floatingTextData);
        }
    }

    public virtual void DoShowFloatingText(FloatingTextData data)
    {
        GamePlayController.instance.EnqueueFloatingText(data);
    }

    public virtual void ResetHealth()
    {
        currentHp = startHp;
        isDead = false;
        IsTargetable = true;
    }

    public virtual void RefillHP(float amount, bool withAnim = true)
    {
        if (amount == 0)
            return;

        currentHp += amount;

        if (currentHp >= this.GetHPWithCoeff())
            currentHp = this.GetHPWithCoeff();

        var screenPos = GamePlayController.instance.GetMainCamera().WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        screenPos.z = 0;
        InGameCanvas.instance.ShowFloatingText(screenPos, $"+{Mathf.Round(amount)}", 100, 100, Color.green, 1.0f);

        this.hpBar.PlayAnimSetHP(this.currentHp, this.GetHPWithCoeff(), withAnim);
    }

    public virtual void RefillHPPercent(float percentage)
    {
        var percent = percentage * 1.0f / 100f;
        var hpToFill = this.startHp * percent;

        RefillHP(hpToFill);
    }

    public bool IsDead()
    {
        return this.isDead || currentHp <= 0;
    }

    public void SetDie()
    {
        isDead = true;
        currentHp = 0f;
        IsTargetable = false;
        CountFuryShoot = 0;
        this.gameObject.SetActive(false);
    }

    [SerializeField] public HealthBar hpBar;

    [SerializeField] private float healthMultiplier;

    public float startHp = 100f;

    [SerializeField] protected float currentHp;
    public float CurrentHp => currentHp;
    public float Percent => currentHp / startHp;

    public Action<float> OnHit;

    public Action<bool> OnDie;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(TagConstant.TAG_ATTACKABLE_TRIGGEER))
        {
            IsTargetable = true;
        }
        _OnTriggerEnter?.Invoke(other);
    }

    private bool isDead;

    private bool allowHeadshot;
    protected bool isTargetable;

    public bool AllowHeadshot {
        get => allowHeadshot;
        set => allowHeadshot = value;
    }

    public bool IsTargetable {
        get => isTargetable;
        set => isTargetable = value;
    }

    #region HitEffect

    public AutoDespawnParticles PlayEffectHitVisual(COMMON_FX _fx, float Duration = 1.0f)
    {
        AutoDespawnParticles fx = null;
        fx = GameMaster.PlayEffect(_fx, _fxMarker.position, Quaternion.identity, _fxMarker);
        fx.type = DESPAWN_TYPE.TIMER;
        fx.DelayDespawn = Duration;

        return fx;
    }

    #endregion

    #region Effect FuryShoot

    /// <summary>
    /// The more hit the more dmg
    /// </summary>
    private int CountFuryShoot = 0;

    public void CalcDmgForFuryShoot(ref float NewDmg, EffectHit hitData)
    {
        CountFuryShoot++;
        var Design = DesignHelper.GetSkillDesign(hitData.SkillID);
        NewDmg += NewDmg * CountFuryShoot * (Design.HeroBaseDmgPercent * hitData.Value / 100f);
        if (CountFuryShoot >= hitData.Number)
        {
            CountFuryShoot = 0;
        }
    }

    #endregion

    public bool _skipAnimDead = false;
    public void SkipAnimDead(float inDuration)
    {
        _skipAnimDead = true;
        Timing.CallDelayed(inDuration, () =>
        {
            _skipAnimDead = false;
        });
    }

    public void SetCurrentHp(float _curHp)
    {
        this.currentHp = _curHp;
    }

#if UNITY_EDITOR
    [Button]
    public void FindReference()
    {
        foreach (Transform child in transform)
        {
            var health = child.GetComponent<HealthBar>();
            if (health != null)
            {
                hpBar = health;
                break;
            }
        }
    }
#endif

}