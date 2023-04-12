using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using com.datld.data;
using Ez.Pooly;
using QuickEngine.Extensions;
using QuickType.Attribute;
using QuickType.Hero;
using QuickType.Shop;
using QuickType.SkillDesign;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;

public class HeroInfoUIHelper : MonoBehaviour
{
    [SerializeField] private RawImage _heroPreview;
    [SerializeField] private LocalizedTMPTextUI _heroNameText;
    [SerializeField] private LocalizedTMPTextUI _levelText;
    [SerializeField] private LocalizedTMPTextUI _maxLevelText;

    [SerializeField] private Transform _skillUiHolder;

    [SerializeField] private AttributeUI _damageAttributeUi;
    [SerializeField] private AttributeUI _headShotAttributeUi;
    [SerializeField] private AttributeUI _hpAttributeUi;

    [Header("Upgrade")] [SerializeField] private LocalizedTMPTextUI _upgradeCost;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private UIShiny _upgradeButtonShiny;
    [SerializeField] private GameObject _maxLevelNotify;

    [Header("Unlock")] [SerializeField] private Button _unlockButton;
    [SerializeField] private LocalizedTMPTextUI _unlockCost;
    [SerializeField] private Image _unlockCurrencyIcon;
    [SerializeField] private GameObject _unlockText;
    [SerializeField] private GameObject _cantUnlockPanel;
    [SerializeField] private LocalizedTMPTextUI _cantUnlockDetailText;
    

    //[Header("Reference")] [SerializeField] private StarHelper _starHelper;
    [SerializeField] private StarsGroup _starsGroup;

    [Header("Tooltip")] [SerializeField] private TalentTooltip _tooltip;
    [SerializeField] private Transform _tooltipArrow;
    [SerializeField] private Transform _tooltipBox;

    [Header("AskPopup")] [SerializeField] private PopupAskPurchaseHero _popupAskPurchaseHero;
    [Header("Shard")] [SerializeField] private Button _btnShard;
    [SerializeField] private Button _btnPromote;
    
    public Action<HeroData> onUpgrade;
    public Action<HeroData> onUnlock;
    private HeroData _heroData;

    private ShopHeroPreview _shopHeroPreview;
    private int MaxRank = 0;


    private LoadIAPButton _loadIapButton;

    private LoadIAPButton loadIapButton
    {
        get
        {
            if (_loadIapButton == null)
                _loadIapButton = _unlockButton.GetComponent<LoadIAPButton>();
            return _loadIapButton;
        }
    }

    private long GetUpgradeCost()
    {
        return _heroData.GetHeroCampaignCost(_heroData.GetHeroLevel() + 1);
    }

    private void Awake()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_HERO_INFO_VIEW, new Action(ResetView));
    }

    private void UpdateRenderTexture()
    {
        if (_shopHeroPreview == null)
        {
            _shopHeroPreview = FindObjectOfType<ShopHeroPreview>();
            _shopHeroPreview.ResetRenderTexture();
        }

        _heroPreview.texture = _shopHeroPreview._renderTexture;
        _heroPreview.SetAllDirty();
    }

    public void Load(HeroData heroData)
    {
        UpdateRenderTexture();

        _heroData = heroData;
        var heroDesign = DesignHelper.GetHeroDesign(heroData);
        _heroNameText.textName = heroDesign.Name;
        _heroNameText.text = heroDesign.GetLocalizeName();

        _upgradeCost.text = $"x{FBUtils.CurrencyConvert((long) GetUpgradeCost())}";
        _levelText.text = $"{heroData.GetHeroLevel():00}";
        _maxLevelText.text = $"/{heroDesign.MaxLevel:00}";

        LoadSkill(heroDesign);
        //_starHelper.Load(heroData.Rank);

        MaxRank = DesignHelper.GetHeroMaxRank(_heroData.UniqueID);
        _starsGroup.Initialize(MaxRank);
        _starsGroup.EnableStars(_heroData.Rank);

        var heroPower = heroData.CalculatePowerBaseAndTalentHeroData(); // Base  + talent,  without equip

        _damageAttributeUi.LoadWithBonusTotal(EffectType.PASSIVE_INCREASE_DMG.ToString(), heroPower.Dmg,
            heroDesign.DmgUprade, false);
        _headShotAttributeUi.LoadWithBonusTotal(EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT.ToString(),
            heroPower.HeadshotPercent,
            heroDesign.HeadshotPercentUpgrade, false);
        _hpAttributeUi.LoadWithBonusTotal(EffectType.PASSIVE_INCREASE_HP.ToString(), heroPower.Hp, heroDesign.HpUprade,
            false);

        // _damageAttributeUi.Load(EffectType.DAMAGE.ToString(), heroPower.Dmg);
        // _headShotAttributeUi.Load(EffectType.FIRE_RATE.ToString(), heroPower.Firerate);
        // _hpAttributeUi.Load(EffectType.HP.ToString(), heroPower.Hp);

        _btnShard.gameObject.SetActiveIfNot(false);
        _cantUnlockPanel.SetActive(false);
        _maxLevelNotify.SetActive(false);
        if(_btnPromote != null)
            _btnPromote.gameObject.SetActive(false);

        if (heroData.IsUnlocked())
        {
            _unlockButton.gameObject.SetActive(false);
            //_unlockedMask.gameObject.SetActive(false);
            _btnShard.gameObject.SetActiveIfNot(true);

            if (heroData.HasEnoughShardToUpRank() && _btnPromote != null)
            {
                _btnPromote.gameObject.SetActive(true);                
            }
            else
            {
                if (heroData.GetHeroLevel() < heroDesign.MaxLevel)
                {
                    _upgradeButton.gameObject.SetActive(true);
                    bool isEnough = CurrencyModels.instance.IsEnough(CurrencyType.GOLD, (long) GetUpgradeCost());
                    _upgradeButtonShiny.enabled = isEnough;
                }
                else
                {
                    _upgradeButton.gameObject.SetActive(false);
                    _maxLevelNotify.SetActive(true);

                    // Skip load bonus
                    _damageAttributeUi.Load(EffectType.PASSIVE_INCREASE_DMG.ToString(), heroPower.Dmg, false);
                    _headShotAttributeUi.Load(EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT.ToString(),
                        heroPower.HeadshotPercent, false);
                    _hpAttributeUi.Load(EffectType.PASSIVE_INCREASE_HP.ToString(), heroPower.Hp, false);
                } 
            }
        }
        else
        {
            LoadState(heroData);
        }
    }

    public void HideBonus()
    {
        var heroPower = _heroData.CalculatePowerBaseAndTalentHeroData(); // Base  + talent,  without equip

        _damageAttributeUi.Load(EffectType.PASSIVE_INCREASE_DMG.ToString(), heroPower.Dmg, false);
        _headShotAttributeUi.Load(EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT.ToString(), heroPower.HeadshotPercent,
            false);
        _hpAttributeUi.Load(EffectType.PASSIVE_INCREASE_HP.ToString(), heroPower.Hp, false);
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.RESET_HERO_INFO_VIEW, new Action(ResetView));
    }

    private void LoadState(HeroData heroData)
    {
        //_unlockedMask.gameObject.SetActive(true);
        _upgradeButton.gameObject.SetActive(false);

        var unlockHeroElement = DesignHelper.GetUnlockHeroDesignElement(heroData.UniqueID);
        var costType = unlockHeroElement.CostType.ToEnum<CostType>();

        // var unlockRequire = DesignHelper.IsRequirementAvailable(heroData.UniqueID);
        // Unlock by campaign level
        if (costType == CostType.FREE)
        {
            _unlockButton.gameObject.SetActive(false);
            _cantUnlockPanel.SetActive(true);
            _cantUnlockDetailText.text =
                LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV_HERO,
                    unlockHeroElement.UnlockLevel); // $"Unlock in level {unlockHeroElement.UnlockLevel}";
        }
        else
        {
            _unlockButton.gameObject.SetActive(true);
            var unlockCurrency = unlockHeroElement.GetCurrencyType();
            _unlockCost.text = unlockCurrency.PriceStr;

            bool isIap = unlockCurrency.IsIAP();
            _unlockCurrencyIcon.gameObject.SetActive(!isIap);
            _unlockText.gameObject.SetActive(!isIap);
            ResourceManager.instance.GetCostTypeSprite(unlockCurrency.Type, s => { _unlockCurrencyIcon.sprite = s; });

            if (isIap)
            {
                loadIapButton.StartLoadCost(unlockCurrency.PriceStr, unlockHeroElement.getIAPProductID(),
                    _unlockCost.targetTMPText);
            }
        }
    }

    private void LoadSkill(HeroDesign heroDesign)
    {
        _skillUiHolder.DespawnAllChild();
        // foreach (Transform child in _skillUiHolder.transform)
        // {
            // Destroy(child.gameObject);
        // }

        var unlockedSkills = heroDesign.GetAllSkills();
        var notUnlockedSkills = heroDesign.GetNextLockedSkills();
        foreach (var skill in unlockedSkills)
        {
            var skillUI = Pooly.Spawn<SkillUI>(POOLY_PREF.SKILL_UI, Vector3.zero, Quaternion.identity, _skillUiHolder); //Instantiate(_skillUiPrefab, _skillUiHolder);
            skillUI.transform.localScale = Vector3.one;
            skillUI.Load(skill);
            skillUI.SetLock(false);
            skillUI.SetOnClickCallback(OnSkillClick);
        }

        foreach (var skill in notUnlockedSkills)
        {
            var skillUI = Pooly.Spawn<SkillUI>(POOLY_PREF.SKILL_UI, Vector3.zero, Quaternion.identity, _skillUiHolder); //Instantiate(_skillUiPrefab, _skillUiHolder);
            skillUI.transform.localScale = Vector3.one;

            skillUI.Load(skill.Item1);
            skillUI.SetLock(true, skill.Item2);
            skillUI.SetOnClickCallback(OnSkillClick);
        }
    }

    private void OnSkillClick(SkillUI skillUi)
    {
        SkillDesignElement skillDesignElement = DesignHelper.GetSkillDesign(skillUi.SkillID);
        if (skillUi.IsLock)
            _tooltip.UpdateText(skillDesignElement.GetName(),
                LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_RANK,
                    skillUi.UnlockRank)); // $"Unlock in rank {skillUi.UnlockRank}");
        else
        {
            _tooltip.UpdateText(skillDesignElement.GetName(), skillDesignElement.GetDescription());
        }

        _tooltip.Show();

        _tooltipBox.transform.parent = _tooltipArrow;

        _tooltipArrow.transform.position =
            skillUi.transform.position -
            new Vector3(0, Utils.ConvertToMatchHeightRatio(_tooltipArrow.rectTransform().rect.height / 2), 0);

        _tooltipBox.transform.parent = _tooltip.transform;
        _tooltipBox.rectTransform().anchoredPosition = new Vector2(0, _tooltipBox.rectTransform().anchoredPosition.y);
    }

    private void OnPurchaseSuccess()
    {
        _heroData.UnlockHero();
        _heroData.ResetPowerData();

        Load(_heroData);
        SaveManager.Instance.SaveData();
        onUnlock?.Invoke(_heroData);
        SaveManager.Instance.Data.ReminderData.NewHeroOnHudEquip.Add(_heroData.UniqueID);

        // var oldHeroesPrimitive = SaveManager.Instance.Data.LastData.Inventory.ListHeroData;
        // oldHeroesPrimitive.ToList().Find(x => x.UniqueID == _heroData.UniqueID).UnlockHero();

        TopLayerCanvas.instance.ShowHUDForce(EnumHUD.HUD_HERO_UNLOCK, false, null, _heroData);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.PURCHASE_HERO);
    }

    public void Unlock()
    {
        var unlockHeroElement = DesignHelper.GetUnlockHeroDesignElement(_heroData.UniqueID);
        var unlockCurrency = unlockHeroElement.GetCurrencyType();
        var currencyType = Utils.ConvertToCurrencyType(unlockCurrency.Type);

        if (unlockCurrency.Type == CostType.IAP)
        {
            Action onBuy = () =>
            {
                IAPManager.instance.PurchaseIAP(unlockHeroElement.getIAPProductID(), isSuccess =>
                {
                    // Request IAP 
                    if (isSuccess)
                    {
                        OnPurchaseSuccess();
                    }
                    else
                    {
                        MasterCanvas.CurrentMasterCanvas.ShowPurchaseFail();
                    }
                });
            };

            if (MasterCanvas.CurrentMasterCanvas.CurrentHUD._hudType == EnumHUD.HUD_HERO)
            {
                _popupAskPurchaseHero.Show(_heroData, unlockCurrency, unlockHeroElement, () => { onBuy(); });
            }
            else
            {
                onBuy();
            }
        }
        else
        {
            if (CurrencyModels.instance.IsEnough(currencyType, (long) unlockCurrency.Value))
            {
                if (GamePlayController.instance != null)
                {
                    CurrencyModels.instance.AddCurrency(currencyType, (long) -unlockCurrency.Value);
                    OnPurchaseSuccess();
                }
                else
                {
                    _popupAskPurchaseHero.Show(_heroData, unlockCurrency, unlockHeroElement, () =>
                    {
                        CurrencyModels.instance.AddCurrency(currencyType, (long) -unlockCurrency.Value);
                        OnPurchaseSuccess();
                    });
                }
            }
            else
            {
                MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(currencyType, (long) unlockCurrency.Value);
            }
        }
    }

    public void Upgrade()
    {
        var cost = GetUpgradeCost();

        if (CurrencyModels.instance.IsEnough(CurrencyType.GOLD, (long) cost))
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.GOLD, (long) -cost);
            _heroData.LevelUpHero();
            _heroData.ResetPowerData();

            Load(_heroData);
            SaveManager.Instance.SetDataDirty();
            // SaveManager.Instance.SaveData();
            onUpgrade?.Invoke(_heroData);

            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_UPGRADE);
        }
        else
        {
            MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(CurrencyType.GOLD, (long) cost, false);
        }
    }

    public void ResetView()
    {
        //Load();
        //_starsGroup.EnableStars(_heroData.Rank);
    }

    public void OnButtonShard()
    {
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_SHARD_CHAR, false, null, _heroData);
    }

    public void ResetLayer()
    {
        _popupAskPurchaseHero.Hide();
    }

    public void OnMaxButtonClick()
    {
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_SHARD_CHAR, false, null, _heroData);
        //MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.PROMO_TO_UP_LV.AsLocalizeString());
    }
}