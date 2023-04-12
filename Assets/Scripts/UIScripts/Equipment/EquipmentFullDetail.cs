using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using com.datld.data;
using com.datld.talent;
using DG.Tweening;
using Doozy.Engine.UI;
using Ez.Pooly;
using MEC;
using QuickType;
using QuickType.Attribute;
using QuickType.Weapon;
using Spine;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using Random = UnityEngine.Random;

public enum EquipState
{
    NONE,
    NORMAL,
    EQUIPPING,
    REWARD
}

public class EquipmentFullDetail : MonoBehaviour
{
    [SerializeField] private AutoUpdateLayoutGroup _masterLayout;

    [Header("UI")] [SerializeField] private LocalizedTMPTextUI _name;
    [SerializeField] private UIShiny _upgradeSuccessShiny;
    [SerializeField] private LocalizedTMPTextUI _rank;
    [SerializeField] private LocalizedTMPTextUI _level;
    [SerializeField] private LocalizedTMPTextUI _maxLevel;
    [SerializeField] private LocalizedTMPTextUI _range;
    [SerializeField] private LocalizedTMPTextUI _fireRate;
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _unEquipButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private GameObject _maxButton;
    [SerializeField] private UIShiny _upgradeShiny;
    [SerializeField] private StarsGroup _starsGroup;

    [Header("Upgrade cost")] [SerializeField]
    private LocalizedTMPTextUI _scrollCost;

    [SerializeField] private Image _scrollIcon;

    [Header("Reference")] [SerializeField] private Transform _baseAttributeHolder;
    [SerializeField] private UpgradableAttributeUI _attributeUiPrefab;
    [SerializeField] private AttributeUILocked _attributeLockedUiPrefab;
    [SerializeField] private DismantleHelper _dismantleHelper;

    public Action HideCallback;
    public Action<WeaponData> onEquip = null;
    public Action<WeaponData> onUnEquip = null;
    public Action onUpgrade = null;

    private WeaponData _weaponData;
    private WeaponDesign _weaponDesign;
    private WeaponUpgradeCostData _upgradeCost;
    public WeaponData WeaponData => _weaponData;

    private EquipState _state = EquipState.NORMAL;
    private List<UpgradableAttributeUI> _upgradableAttributes;
    private float _totalDelay = 0;
    private List<Tween> _upgradeTweens;

    public EquipState State
    {
        get { return _state; }
        set { _state = value; }
    }


    public void Init()
    {
        CurrencyModels.instance.AddCallback(OnAddCurrency);
        _starsGroup.Initialize(GameConstant.MAXIMUM_RANK);
    }

    private void OnDestroy()
    {
        CurrencyModels.instance.RemoveCallback(OnAddCurrency);
    }

    private void OnAddCurrency(CurrencyType type, long totalValue)
    {
        if (gameObject != null && gameObject.activeInHierarchy)
            CheckUpgradeButtonStat();
    }

    public void Load(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        gameObject.SetActive(true);

        _weaponData = weaponData;
        _weaponDesign = weaponDesign;
        var rankDefine = ResourceManager.instance.GetRankDefine(_weaponDesign.Rarity);

        _level.text = weaponData.GetWeaponLevel().ToString();
        _maxLevel.text = $"/{weaponDesign.MaxLevel.ToString()}";

        _name.textName = _weaponDesign.Name;
        _rank.textName = rankDefine.name;

        _starsGroup.EnableStars(_weaponData.Rank);
        _fireRate.textName = DesignHelper.GetAttributeTextDisplay("FIRE_RATE", weaponData.FinalPowerData.Firerate);
        _range.textName = DesignHelper.GetAttributeTextDisplay("RANGE", weaponData.FinalPowerData.Range);
        // _description.text = _weaponDesign.DescriptionId;

        _name.targetTMPText.color = rankDefine.color;
        _rank.targetTMPText.color = rankDefine.color;

        _equipmentUi.Load(weaponData, weaponDesign);
        _equipmentUi.HideLevelText();
        // _rankBG.sprite = rankDefine.sprite;
        // _icon.sprite = ResourceManager.instance.GetEquipSprite(_weaponDesign.Icon);

        UpdateUpgradeCost();
        LoadAttribute();
        CheckState();
        _state = EquipState.NORMAL;
        _masterLayout.Reload();
    }

    private void CheckState()
    {
        switch (_state)
        {
            case EquipState.NONE:
                break;
            case EquipState.NORMAL:
                _equipButton.gameObject.SetActive(true);
                _sellButton.gameObject.SetActive(true);
                _upgradeButton.gameObject.SetActive(true);
                _unEquipButton.gameObject.SetActive(false);
                break;
            case EquipState.EQUIPPING:
                _equipButton.gameObject.SetActive(false);
                _sellButton.gameObject.SetActive(false);
                _upgradeButton.gameObject.SetActive(true);
                _unEquipButton.gameObject.SetActive(true);
                break;
            case EquipState.REWARD:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        CheckUpgradeButtonStat();
    }

    private void LoadAttribute()
    {
        DestroyAllAttributeUI();
        LoadBaseAttributeUI();
    }

    private void ReloadAfterUpgrade()
    {
        _level.text = $"{_weaponData.GetWeaponLevel()}";
        _maxLevel.text = $"/{DesignHelper.GetWeaponDesign(_weaponData).MaxLevel}";

        UpdateUpgradeCost();
        CheckUpgradeButtonStat();

        // _upgradeButton.interactable = false;
        _upgradeTweens.Add(DOVirtual.DelayedCall(_totalDelay, () =>
        {
            DestroyAllAttributeUI();
            LoadBaseAttributeUI();
            // _upgradeButton.interactable = true;
        }));
    }

    private void CheckUpgradeButtonStat()
    {
        _upgradeShiny.enabled = CanUpgrade();
        // _upgradeButton.interactable = CanUpgrade();
        _upgradeButton.gameObject.SetActive(!_weaponData.IsMaxLevel());
        _maxButton.gameObject.SetActive(_weaponData.IsMaxLevel());
    }

    private void LoadBaseAttributeUI()
    {
        var fakeWeaponData = CreateFakeWeaponDataAfterUpgrade(_weaponData);
        var fakeAttributes = SaveGameHelper.GetListAttributeDesigns(fakeWeaponData);
        var attributes = SaveGameHelper.GetListAttributeDesigns(_weaponData);

        var allAttributeUIs = new List<AttributeUI>();
        var allAttributeId = new List<string>();
        _upgradableAttributes = new List<UpgradableAttributeUI>();

        bool isMaxLevel = _weaponData.IsMaxLevel();

        // Load base stat
        for (int i = 0; i < attributes.Count; i++)
        {
            var attribute = attributes[i];
            var fakeAttribute = fakeAttributes[i];
            double delta = fakeAttribute.Item2 - attribute.Item2;

            if (attribute.Item1.Contains(EffectType.PASSIVE_INCREASE_FIRERATE.ToString()) ||
                attribute.Item1.Contains(EffectType.PASSIVE_INCREASE_RANGE.ToString()))
                continue;

            var attributeUI =
                InstantiateAttributeUI(_attributeUiPrefab.gameObject,
                    _baseAttributeHolder); // Instantiate(_attributeUiPrefab, _baseAttributeHolder);

            if (!isMaxLevel && fakeAttribute.Item1 == attribute.Item1 && delta > 0)
            {
                attributeUI.LoadWithBonus(attribute.Item1, attribute.Item2, fakeAttribute.Item2);
                _upgradableAttributes.Add((UpgradableAttributeUI) attributeUI);
            }
            else
            {
                attributeUI.Load(attribute.Item1, attribute.Item2);
            }

            if (!allAttributeId.Contains(attribute.Item1))
            {
                allAttributeId.Add(attribute.Item1);
                allAttributeUIs.Add(attributeUI);
            }

            SpawnShadow();
        }

        // Load special attribute
        var specialAttributes = _weaponDesign.GetListSpecialAttributeDesigns();
        foreach (var attribute in specialAttributes)
        {
            if (attribute.Item1.Contains(EffectType.PASSIVE_INCREASE_FIRERATE.ToString()) ||
                attribute.Item1.Contains(EffectType.PASSIVE_INCREASE_RANGE.ToString()))
                continue;

            var specialAttributeUI =
                InstantiateAttributeUI(_attributeUiPrefab.gameObject,
                    _baseAttributeHolder); // Instantiate(_attributeUiPrefab, _baseAttributeHolder);
            specialAttributeUI.Load(attribute.Item1, attribute.Item2);

            if (!allAttributeId.Contains(attribute.Item1))
            {
                allAttributeId.Add(attribute.Item1);
                allAttributeUIs.Add(specialAttributeUI);
            }

            SpawnShadow();
        }

        TalentManager.AddTalentBonus(allAttributeUIs);

        // Load locked attribute
        var lockedAttribute = _weaponDesign.GetNextLockedAttribute();
        if (lockedAttribute != null)
        {
            var rankDefine = ResourceManager.instance.GetRankDefine(lockedAttribute.Item3);
            var lockedAttributeUi =
                (AttributeUILocked) InstantiateAttributeUI(_attributeLockedUiPrefab.gameObject, _baseAttributeHolder);

            lockedAttributeUi.Load(lockedAttribute.Item1, lockedAttribute.Item2, rankDefine);
            SpawnShadow();
        }

        Pooly.Despawn(_baseAttributeHolder.GetChild(_baseAttributeHolder.childCount - 1));
        // Destroy(_baseAttributeHolder.GetChild(_baseAttributeHolder.childCount - 1).gameObject);
        // Debug.LogError("BASE" + SaveGameHelper.AddPowerData(_weaponData.DefaultPower, _weaponData.UpgradedPower));
        // Debug.LogError("FINAL " + _weaponData.FinalPowerData);
    }

    private AttributeUI InstantiateAttributeUI(GameObject prefab, Transform parent)
    {
        bool isContains = Pooly.IsContainKey(prefab.name);
        AttributeUI result = null;

        if (isContains)
        {
            result = Pooly.Spawn(prefab.name, Vector3.zero, Quaternion.identity).GetComponent<AttributeUI>();
        }
        else
        {
            result = Pooly.Spawn(prefab.transform, Vector3.zero, Quaternion.identity).GetComponent<AttributeUI>();
        }
        
        result.transform.SetParent(parent);
        result.transform.localScale = Vector3.one;
        return result;
    }

    private void SpawnShadow()
    {
        // var shadow = Pool> Instantiate(_shadowBeetweenPrefab, _baseAttributeHolder);
        var shadow = Pooly.Spawn(POOLY_PREF.SHADOW_BETWEEN_FULL_EQUIP, Vector3.zero, Quaternion.identity);
        shadow.SetParent(_baseAttributeHolder);
        shadow.localScale = Vector3.one;
        shadow.gameObject.SetActive(true);
    }

    private void DestroyAllAttributeUI()
    {
        _baseAttributeHolder.DespawnAllChild();
        // for (int i = 1; i < _baseAttributeHolder.childCount; i++)
        // {
        //     Destroy(_baseAttributeHolder.GetChild(i).gameObject);
        // }
    }

    // private void HideAllUpgradeMaterialUI()
    // {
    //     _upgradeArmourScrollMaterialUI.gameObject.SetActive(false);
    //     _upgradeWeaponScrollMaterialUI.gameObject.SetActive(false);
    // }

    private void UpdateUpgradeCost()
    {
        _upgradeCost = SaveGameHelper.GetUpgradeCost(_weaponData);
        if (_upgradeCost.HasArmourScroll())
        {
            _scrollCost.text = "x" + FBUtils.CurrencyConvert(_upgradeCost.armourScroll);
            ResourceManager.instance.GetCurrencySprite(CurrencyType.ARMOUR_SCROLL, _scrollIcon);
        }
        else if (_upgradeCost.HasWeaponScroll())
        {
            _scrollCost.text = "x" + FBUtils.CurrencyConvert(_upgradeCost.weaponScroll);
            ResourceManager.instance.GetCurrencySprite(CurrencyType.WEAPON_SCROLL, _scrollIcon);
        }

        // HideAllUpgradeMaterialUI();
        // if (_upgradeCost.HasWeaponScroll())
        // {
        //     _upgradeWeaponScrollMaterialUI.Load(CurrencyType.WEAPON_SCROLL, _upgradeCost.weaponScroll);
        // }
        //
        // if (_upgradeCost.HasArmourScroll())
        // {
        //     _upgradeArmourScrollMaterialUI.Load(CurrencyType.ARMOUR_SCROLL, _upgradeCost.armourScroll);
        // }
    }

    public void Upgrade()
    {
        //NetworkDetector.instance.checkInternetConnection((reached) => { StartUpgrade(); });

        StartUpgrade();
    }

    private void StartUpgrade()
    {
        if (_weaponData.IsMaxLevel())
            return;

        bool isEnoughGold = CurrencyModels.instance.IsEnough(CurrencyType.GOLD, _upgradeCost.gold);
        bool isEnoughWeaponScroll =
            CurrencyModels.instance.IsEnough(CurrencyType.WEAPON_SCROLL, _upgradeCost.weaponScroll);
        bool isEnoughArmourScroll =
            CurrencyModels.instance.IsEnough(CurrencyType.ARMOUR_SCROLL, _upgradeCost.armourScroll);

        if (isEnoughGold && isEnoughArmourScroll && isEnoughWeaponScroll)
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.GOLD, -_upgradeCost.gold);
            CurrencyModels.instance.AddCurrency(CurrencyType.WEAPON_SCROLL, -_upgradeCost.weaponScroll);
            CurrencyModels.instance.AddCurrency(CurrencyType.ARMOUR_SCROLL, -_upgradeCost.armourScroll);

            _weaponData.LevelUpWeapon();// Level++;
            MissionManager.Instance.TriggerMission(MissionType.UPGRADE_EQUIPMENT,_weaponData.WeaponID);
            
            _weaponData.ResetPowerData();
            SaveManager.Instance.SetDataDirty();

            onUpgrade?.Invoke();
            PlayUpgradeAnim();
            ReloadAfterUpgrade();


            _upgradeSuccessShiny.effectPlayer.duration = 0.25f;
            _upgradeSuccessShiny.Play();
            DOVirtual.DelayedCall(0.25f, () => { _upgradeSuccessShiny.Play(); });
        }
        else
        {
            if (!isEnoughGold)
                MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.GOLD, _upgradeCost.gold, false);
            else if (!isEnoughArmourScroll)
                MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.ARMOUR_SCROLL, _upgradeCost.armourScroll, false);
            else
                MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.WEAPON_SCROLL, _upgradeCost.weaponScroll, false);
        }
    }

    private void KillAllUpgradeTween()
    {
        if (_upgradeTweens == null)
            _upgradeTweens = new List<Tween>();

        foreach (var tween in _upgradeTweens)
        {
            tween.Kill();
        }
    }

    private void PlayUpgradeAnim()
    {
        KillAllUpgradeTween();

        _totalDelay = 0;
        float delayEachTurn = 0.2f;
        foreach (var upgradeUi in _upgradableAttributes)
        {
            _upgradeTweens.Add(DOVirtual.DelayedCall(_totalDelay, () => { upgradeUi.PlayUpgradeAnim(); }));
            _totalDelay += delayEachTurn;
        }

        _totalDelay += 1f;
    }

    private bool CanUpgrade()
    {
        bool isEnoughGold = CurrencyModels.instance.IsEnough(CurrencyType.GOLD, _upgradeCost.gold);
        bool isEnoughWeaponScroll =
            CurrencyModels.instance.IsEnough(CurrencyType.WEAPON_SCROLL, _upgradeCost.weaponScroll);
        bool isEnoughArmourScroll =
            CurrencyModels.instance.IsEnough(CurrencyType.ARMOUR_SCROLL, _upgradeCost.armourScroll);

        return isEnoughGold && isEnoughArmourScroll && isEnoughWeaponScroll;
    }

    public void Equip()
    {
        onEquip?.Invoke(_weaponData);
        gameObject.SetActive(false);
    }

    public void UnEquip()
    {
        onUnEquip?.Invoke(_weaponData);
        gameObject.SetActive(false);
    }

    private WeaponData CreateFakeWeaponDataAfterUpgrade(WeaponData weaponData, int levelUpgrade = 1)
    {
        var result = weaponData.Clone();
        result.UniqueID = FBUtils.RandomString(15);
        result.Level = weaponData.GetWeaponLevel() + levelUpgrade;
        result.SetWeaponLevel(weaponData.GetWeaponLevel() + levelUpgrade); 
        result.ResetPowerData();
        return result;
    }

    public void Dismantle()
    {
        _dismantleHelper.Show(_weaponData, _weaponDesign);
    }

    public void OnMaxButtonClick()
    {
        MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.FUSION_TO_UP_LV.AsLocalizeString());
    }

    public void OnHide()
    {
        _weaponData = null;
        _weaponDesign = null;
    }

    public void Hide()
    {
        HideCallback?.Invoke();
    }

    public void SetOnDismantleCallback(Action<WeaponData, WeaponDesign> dismantleEquip)
    {
        _dismantleHelper.SetOnDismantleCallback(dismantleEquip);
    }
}