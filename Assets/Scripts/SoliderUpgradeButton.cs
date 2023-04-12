using System;
using System.Collections.Generic;
using System.Linq;
using Adic;
using Coffee.UIEffects;
using com.datld.data;
using DG.Tweening;
using Ez.Pooly;
using Framework.Interfaces;
using Framework.Managers.Event;
using QuickType.Hero;
using QuickType.IdleHero;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IUpgradableController<T>
{
    T Data { get; set; }

    void InitController();
    void UpdateController(float deltaTime);

    int Level { get; }
    int MaxLevel { get; }
    long Price { get; }
    float Value { get; }
    void UpgradeItem();
    void UnlockItem();
    void ResetView();

    bool IsAvailableForBuy();
    bool IsMaxLevel();
    bool IsLocked();
}

public class SoliderUpgradeButton : UpgradeButton, IUpgradableController<HeroData>
{
    public string HeroID;
    [SerializeField] private UIShiny _upgradeSuccessShiny;

    protected HeroData _data;
    public Character _character;
    private int currentLevel;

    public LevelGate levelGate;

    [SerializeField] private Image hidedIcon;

    [SerializeField] private Image activeIcon;

    public Image _iconCurrency;

    [SerializeField] protected Text txtHP;

    [SerializeField] private Text txtName;
    [SerializeField] protected GameObject maxBtn;
    [SerializeField] protected GameObject promoteBtn;

    private List<CharacterUltimateButton> buttonUltimates;

    [Header("DEBUG ONLY!!!")] public RectTransform _rectDebug;
    public RectTransform _debugSkillContents;
    public DebugHeroPassiveRow _debugSkillRowPrefab;
    private bool _isInit = false;

    private void Start()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ADD_SHARD_HERO, new Action<HeroData>(OnAddShard));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ON_PROMOTE_HERO, new Action<HeroData>(OnPromoteHero));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ENABLE_DEBUG_HERO_PANEL, new Action<bool>(SetEnableDebugPanel));
    }

    protected virtual void OnPromoteHero(HeroData heroData)
    {
        if (_data != null && _data.UniqueID == heroData.UniqueID)
        {
            HeroDesign = DesignHelper.GetHeroDesign(HeroID, _data.Rank, _data.GetHeroLevel());
            IdleHeroDesign = DesignHelper.GetIdleHeroDesign(HeroID, _data.IdleLevel);
            ResetView();
        }
    }

    protected virtual void OnAddShard(HeroData heroData)
    {
        if (_data != null && heroData.UniqueID == _data.UniqueID)
        {
            promoteBtn.SetActive(heroData.HasEnoughShardToUpRank());
        }
        else
        {
            promoteBtn.SetActive(false);
        }
    }

    public void OnPromoteButtonClick()
    {
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_SHARD_CHAR, false, null, _data);
    }

    public override void ResetData()
    {
    }

    private void UpdateHidedItem()
    {
        if (this.hidedIcon == null || this.activeIcon == null)
        {
            return;
        }

        this.hidedIcon.gameObject.SetActive(false);
        this.activeIcon.gameObject.SetActive(false);
        if (!IsLocked() && IsInTeamMember())
        {
            this.activeIcon.gameObject.SetActive(true);
            return;
        }

        this.hidedIcon.gameObject.SetActive(true);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(CheckAndUnlockLevelGate));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.RESET_UPGRADE_BUTTON_VIEW,
            new Action<string>(UnlockHero));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ADD_SHARD_HERO, new Action<HeroData>(OnAddShard));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ON_PROMOTE_HERO, new Action<HeroData>(OnPromoteHero));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ENABLE_DEBUG_HERO_PANEL, new Action<bool>(SetEnableDebugPanel));
    }

    protected virtual void PreInit()
    {
        //InitController();
        IsDataDirty = true;
        currentLevel = LevelModel.instance.CurrentLevel;

        this.Inject();

        this.levelGate.CheckAndUnlockLevelGate(currentLevel);
        this.UpdateHidedItem();

        if (!_isInit)
            EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(CheckAndUnlockLevelGate));

        ResourceManager.instance.GetHeroAvatar(HeroID, s =>
        {
            var activeSp = s;
            if (activeSp)
            {
                activeIcon.sprite = activeSp;
                activeIcon.SetNativeSize();
            }
        });

        ResourceManager.instance.GetHeroAvatarInactive(HeroID, s =>
        {
            var InactiveSp = s;
            if (InactiveSp)
            {
                hidedIcon.sprite = InactiveSp;
                // hidedIcon.SetNativeSize();
            }
        });


        txtName.text = ResourceManager.instance._heroResources.GetHeroName(HeroID);
    }

    public override void SetUpgadebleValueTxt(string _upgradableValueText)
    {
        base.SetUpgadebleValueTxt(_upgradableValueText);
        this.txtHP.text = _data != null ? _data.GetPowerDataByGameMode().Hp.ToString() : "0";
        this.UpdateHidedItem();
    }

    public void CheckAndUnlockLevelGate()
    {
        currentLevel = LevelModel.instance.CurrentLevel;
        this.levelGate.CheckAndUnlockLevelGate(currentLevel);
    }

    #region Controller

    public HeroData Data {
        get => _data;
        set => _data = value;
    }

    protected HeroDesign HeroDesign;
    private IdleHeroDesign IdleHeroDesign;
    public bool IsDataDirty { get; private set; }

    protected long _cachedPrice = 0;

    public virtual int Level {
        get => _data == null
            ? 0
            : (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE ? _data.GetHeroLevel() : _data.IdleLevel);
    }

    public int IdleLevel {
        get => _data == null ? 0 : _data.IdleLevel;
    }

    public int MaxLevel {
        get => _data != null ? GetItemMaxLevel() : 0;
    }

    public virtual long Price {
        get {
            if (_data != null)
            {
                if (IsDataDirty)
                {
                    _cachedPrice = GetItemPrice(GameMaster.instance.currentMode == GameMode.CAMPAIGN_MODE
                        ? _data.GetHeroLevel() + 1
                        : _data.IdleLevel + 1);
                }

                return _cachedPrice;
            }
            else
            {
                return 0;
            }
        }
    }

    public float Value {
        get => _data != null ? GetItemValue() : 0;
    }

    protected CurrencyType currencyType = CurrencyType.GOLD;

    public virtual int GetNextLevel()
    {
        if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
            return Data.GetHeroLevel() + 1;
        else
            return Data.IdleLevel + 1;
    }

    public void InitController()
    {
        PreInit();

        IsInitDebug = false;
        _data = SaveManager.Instance.Data.GetHeroData(HeroID);
        if (_data != null)
        {
            HeroDesign = DesignHelper.GetHeroDesign(HeroID, _data.Rank, _data.GetHeroLevel());
            IdleHeroDesign = DesignHelper.GetIdleHeroDesign(HeroID, _data.IdleLevel);
        }

        _character = GamePlayController.instance.GetCharacter(_data.UniqueID);

        if (!_isInit)
            EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_UPGRADE_BUTTON_VIEW,
                new Action<string>(UnlockHero));

        ResetView();

        EnableDebugView(false);
        _isInit = true;
    }

    private void UnlockHero(string heroID)
    {
        if (heroID == _data.UniqueID)
        {
            _character = GamePlayController.instance.GetCharacter(_data.UniqueID);
            ResetView();
        }
    }

    protected virtual int GetItemMaxLevel()
    {
        return GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE
            ? (int)HeroDesign.MaxLevel
            : (int)IdleHeroDesign.MaxLevel;
    }

    protected virtual float GetItemValue()
    {
        var Power = Data.GetPowerDataByGameMode();
        float dps = Power.CalcDPS();
        return dps;
    }

    //Price

    protected virtual long GetItemPrice(int level)
    {
        long price = 0;
        if (IsLocked())
        {
            var unlockHeroElement = DesignHelper.GetUnlockHeroDesignElement(_data.UniqueID);
            if (unlockHeroElement != null)
            {
                var unlocker = unlockHeroElement.GetCurrencyType();
                currencyType = Utils.ConvertToCurrencyType(unlocker.Type);
                price = (long)unlocker.Value;
            }
        }
        else
        {
            if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
            {
                price = GetPriceCampaign(level);
                currencyType = CurrencyType.GOLD;
            }
            else
            {
                price = GetPriceIdle(level);
                currencyType = CurrencyType.TOKEN;
            }
        }


        return price;
    }

    protected virtual long GetPriceCampaign(int level)
    {
        return Data.GetHeroCampaignCost(level);
    }

    protected virtual long GetPriceIdle(int level)
    {
        //var basePrice = IdleHeroDesign.UpgradeToken;
        //return Mathf.RoundToInt(
        //    (float) (basePrice + (IdleHeroDesign.UpgradeTokenStep * (level + 1 - IdleHeroDesign.StartLevel))));
        return (long)Data.GetHeroIdleCost(level);
    }

    //end Price

    public virtual void UpgradeItem()
    {
        int nextLevel = GetNextLevel();
        if (!IsMaxLevelWith(nextLevel))
        {
            CurrencyModels.instance.AddCurrency(currencyType, -1 * Price);

            if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
                Data.LevelUpHero(nextLevel);
            else if (GamePlayController.instance.gameMode == GameMode.IDLE_MODE)
                Data.LevelUpHeroIdleMode(nextLevel);

            SaveManager.Instance.SetDataDirty();

            ResetView();

            EventSystemServiceStatic.DispatchAll(EVENT_NAME.HERO_UPGRADED, HeroID);

            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_UPGRADE);

            _upgradeSuccessShiny.effectPlayer.duration = 0.25f;
            _upgradeSuccessShiny.Play();
            DOVirtual.DelayedCall(0.25f, () => { _upgradeSuccessShiny.Play(); });
        }
    }

    protected virtual void UpdateLevelTxt()
    {
        lvlTxtValue.text = GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE
            ? $"{_data.GetHeroLevel()}"
            : $"{_data.GetHeroLevel()}\n<color=#53FFF0>({_data.IdleLevel})</color>";
    }

    public virtual void ResetView()
    {
        this.maximumIsReached = false;

        IsDataDirty = true;
        var PowerData = Data.GetPowerDataByGameMode();
        UpdateLevelTxt();

        upgradeValueText.text = $"{(int)Value}";

        txtHP.text = $"{PowerData.Hp}";

        if (!IsLocked() && !IsInTeamMember())
        {
            _iconCurrency.gameObject.SetActiveIfNot(false);
            buttonTxtValue.text = $"Waiting...";
        }
        else
        {
            buttonTxtValue.text = $"{FBUtils.CurrencyConvert(this.Price)}";
            _iconCurrency.gameObject.SetActiveIfNot(true);
            ResourceManager.instance.GetCurrencySprite(currencyType, _iconCurrency);
            bool isEnough = CurrencyModels.instance.IsEnough(currencyType, this.Price);
            _shiny.enabled = isEnough;
        }


        UpdateHidedItem();

        // foreach (var btn in buttonUltimates)
        // {
        //     btn.gameObject.SetActiveIfNot(false);
        // }
        //
        // if (_character != null && IsInTeamMember())
        // {
        //     for (int i = 0; i < _data.ListUltimates.Count; i++)
        //     {
        //         buttonUltimates[i].Initialize(this, _data.ListUltimates[i]);
        //         buttonUltimates[i].gameObject.SetActiveIfNot(true);
        //     }
        // }


        //Debug only
        initDebugView();

        OnAddShard(_data);
        IsDataDirty = false;
    }

    public virtual bool IsAvailableForBuy()
    {
        if (IsLocked())
        {
            var unlockHeroElement = DesignHelper.GetUnlockHeroDesignElement(_data.UniqueID);
            var require = DesignHelper.IsRequirementAvailable(_data.UniqueID);

            if (unlockHeroElement != null)
            {
                return require && CurrencyModels.instance.GetValueFromEnum(currencyType) >= Price;
            }
            else
                return false;
        }
        else
        {
            return CurrencyModels.instance.GetValueFromEnum(currencyType) >= Price;
        }
    }

    public virtual bool IsInTeamMember()
    {
        bool result = false;

        result = SaveManager.Instance.Data.GameData.TeamSlots.Contains(_data.UniqueID);

        return result;
    }

    public virtual bool IsMaxLevel()
    {
        return Level >= MaxLevel;
    }

    protected virtual bool IsMaxLevelWith(int level)
    {
        return level > MaxLevel;
    }

    public void UpdateController(float _delaTime)
    {
        if (IsMaxLevel())
        {
            EnableMaximumState();
        }
        else if (IsAvailableForBuy())
        {
            EnableButton();
        }
        else
        {
            DisableButton();
        }

        if (buttonUltimates != null && buttonUltimates.Count != 0)
        {
            for (int i = 0; i < buttonUltimates.Count; i++)
            {
                buttonUltimates[i].UpdateButton(_delaTime);
            }
        }
    }

    public override void EnableButton()
    {
        base.EnableButton();
        maxBtn.SetActive(false);
    }

    public override void EnableMaximumState()
    {
        if (!this.maximumIsReached)
        {
            this.maximumIsReached = true;
            maxBtn.SetActive(true);
        }
    }


    public virtual bool IsLocked()
    {
        return _data != null ? (_data.ItemStatus == ITEM_STATUS.Locked) : true;
    }

    public int GetItemLevel()
    {
        if (_data == null)
            return 0;
        if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
            return _data.GetHeroLevel();
        else
            return _data.IdleLevel;
    }

    #endregion

    public override void ActionOnClicked()
    {
        if (IsMaxLevel())
        {
            MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.PROMO_TO_UP_LV
                .AsLocalizeString());
        }
        else if (IsLocked() && IsAvailableForBuy())
        {
            UnlockItem();
        }
        else if (!IsInTeamMember())
        {
            DeplopyHero();
        }
        else if (!IsMaxLevel() && IsAvailableForBuy())
        {
            UpgradeItem();
        }
        else if (!IsAvailableForBuy())
        {
            ShowNotEnough();
            // MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.NOT_ENOUGH_RESOURCES
            // .AsLocalizeString());
        }
    }

    protected virtual void ShowNotEnough()
    {
        MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(
            GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE ? CurrencyType.GOLD : CurrencyType.TOKEN,
            Price, false);
    }

    public void UnlockItem()
    {
        if (_data != null)
        {
            _data.UnlockHero();
            DeplopyHero();
        }
    }

    public void DeplopyHero()
    {
        SaveManager.Instance.Data.ApplyFreeSlotMember(_data.UniqueID);
        SaveManager.Instance.SetDataDirty();

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_TEAM_SLOTS);
        _character = GamePlayController.instance.GetCharacter(_data.UniqueID);
        _data = SaveManager.Instance.Data.GetHeroData(HeroID);

        ResetView();
    }

    #region DEBUG
    public Transform _rectDebugPanel;

    private bool enableDebugPannel = false;
    private bool IsInitDebug = false;

    public void initDebugView()
    {
        if (_character == null || IsInitDebug)
            return;

        foreach (EffectType effect in Enum.GetValues(typeof(EffectType)))
        {
            DebugHeroPassiveRow row = Pooly.Spawn<DebugHeroPassiveRow>(_debugSkillRowPrefab.transform, Vector3.zero,
                Quaternion.identity, _debugSkillContents);
            row.transform.localScale = Vector3.one;
            row.Initialize(this._character, effect, IsHasThisPassive(effect));
        }

        IsInitDebug = true;
    }

    public void EnableDebugView(bool enable)
    {
        _rectDebug.gameObject.SetActiveIfNot(enable);
        enableDebugPannel = enable;
    }

    public bool IsHasThisPassive(EffectType type)
    {
        bool result = false;
        if (_character != null)
        {
            if (_character.effectHitDatas != null)
            {
                foreach (var item in _character.effectHitDatas)
                {
                    if (item.Type == type)
                    {
                        result = true;
                        break;
                    }
                }
            }
        }

        return result;
    }

    public void OnButtonDebug()
    {
        enableDebugPannel = !enableDebugPannel;
        EnableDebugView(enableDebugPannel);
    }

    public void SetEnableDebugPanel(bool _enable)
    {
        if (this._rectDebugPanel != null)
            this._rectDebugPanel.gameObject.SetActiveIfNot(_enable);
    }

    #endregion

    public void UpdateUltimateBtns(CharacterUltimateButton btn1, CharacterUltimateButton btn2)
    {
        if (buttonUltimates == null)
        {
            buttonUltimates = new List<CharacterUltimateButton>();
            buttonUltimates.Add(btn1);
            buttonUltimates.Add(btn2);
        }
        else
        {
            buttonUltimates[0] = btn1;
            buttonUltimates[1] = btn1;
        }

        foreach (var btn in buttonUltimates)
        {
            btn.gameObject.SetActiveIfNot(false);
        }

        // Skip load ultimate manual hero
        if (_character != null && IsInTeamMember() && _data.UniqueID != GameConstant.MANUAL_HERO)
        {
            for (int i = 0; i < _data.ListUltimates.Count; i++)
            {
                buttonUltimates[i].gameObject.SetActiveIfNot(true);
                buttonUltimates[i].Initialize(this, _data.ListUltimates[i]);
            }
        }
    }
}