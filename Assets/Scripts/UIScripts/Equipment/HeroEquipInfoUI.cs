using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI;
using MEC;
using QuickType.Attribute;
using QuickType.Weapon;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class HeroEquipInfoUI : MonoBehaviour
{
    // [SerializeField] private GameObject bottomPrefab;
    // [SerializeField] private GameObject equipmentHolderPrefab;
    // [SerializeField] private GameObject statusPrefab;
    // [SerializeField] private GameObject afterTextPrefab;
    // [SerializeField] private Transform statusParent;

    private GameObject _statusTextPrefab => afterText._statusTextPrefab;
    private Transform _statusTextHolder => afterText._statusTextHolder;
    private UIFloatingTextNotify _statusFloatingText => afterText._statusFloatingText;

    [SerializeField] private EquipmentHeroBottom bottom;
    [SerializeField] private EquipmentHeroStatus status;
    [SerializeField] private EquipmentHeroEquipHolder equipHolder;
    [SerializeField] private EquipmentHeroAfterEquipText afterText;

    // [SerializeField] private Image _heroImg;
    // [SerializeField] private LocalizedTMPTextUI _heroName;
    // [SerializeField] private StarHelper _starHelper;

    private EquipmentUI3 _equippedArmour => equipHolder._equippedArmour;
    private EquipmentUI3 _equippedWeapon => equipHolder._equippedWeapon;
    private GameObject _goLockedArmour => equipHolder._goLockedArmour;

    private LocalizedTMPTextUI _dmgTxt => status._dmgTxt;
    private LocalizedTMPTextUI _hpTxt => status._hpTxt;

    private DotSwitcher _dotSwitcher => bottom._dotSwitcher;
    private ReminderUI _newHeroNextButtonReminder => bottom._newHeroNextButtonReminder;
    private ReminderUI _newHeroReminder => bottom._newHeroReminder;
    private GameObject _lockManageHero => bottom._lockManageHero;

    public RawImage heroPreview;
    //[SerializeField] private ShopHeroPreview _heroPreview;

    // [SerializeField] private AttributeUI _armourAttributeUi;
    // [SerializeField] private AttributeUI _fireRateAttributeUi;
    // [SerializeField] private AttributeUI _rangeAttributeUi;

    [HideInInspector] public int currentHeroIndex = 0;
    public Action<WeaponData, WeaponDesign, EquipState> onShowFullDetail;

    private List<HeroData> _heroDatas;
    public HeroData CurrentHero => _heroDatas[currentHeroIndex];
    public EquipmentUI EquippedArmour => _equippedArmour;
    public EquipmentUI EquippedWeapon => _equippedWeapon;


    public CoroutineHandle LoadPrefab()
    {
        return Timing.RunCoroutine(LoadCoroutine());
    }

    private IEnumerator<float> LoadCoroutine()
    {
        AsyncOperationHandle<GameObject> op;
        // op = bottomPrefab.InstantiateAsync(transform);
        // while (!op.IsDone)
        //     yield return Timing.WaitForOneFrame;
        // bottom = op.Result.GetComponent<EquipmentHeroBottom>();
        // bottom = Instantiate(bottomPrefab, transform).GetComponent<EquipmentHeroBottom>();
        bottom.NextHeroCallback = NextHero;
        bottom.PrevHeroCallback = PrevHero;
        yield return Timing.WaitForOneFrame;

        // op = statusPrefab.InstantiateAsync(statusParent);
        // while (!op.IsDone)
        //     yield return Timing.WaitForOneFrame;
        // status = op.Result.GetComponent<EquipmentHeroStatus>();
        // status = Instantiate(statusPrefab, statusParent).GetComponent<EquipmentHeroStatus>();
        // yield return Timing.WaitForSeconds(0.1f);
        // yield return Timing.WaitForOneFrame;

        
        // op = equipmentHolderPrefab.InstantiateAsync(statusParent);
        // while (!op.IsDone)
        //     yield return Timing.WaitForOneFrame;
        // equipHolder = op.Result.GetComponent<EquipmentHeroEquipHolder>();
        // equipHolder = Instantiate(equipmentHolderPrefab, statusParent).GetComponent<EquipmentHeroEquipHolder>();
        // yield return Timing.WaitForSeconds(0.1f);
        // yield return Timing.WaitForOneFrame;


        // op = afterTextPrefab.InstantiateAsync(transform);
        // while (!op.IsDone)
            // yield return Timing.WaitForOneFrame;
        // afterText = op.Result.GetComponent<EquipmentHeroAfterEquipText>();
        // afterText = Instantiate(afterTextPrefab,transform).GetComponent<EquipmentHeroAfterEquipText>();
    }

    public void Init()
    {
        _heroDatas = new List<HeroData>();

        var allHeroes = SaveManager.Instance.Data.Inventory.ListHeroData.ToList();
        foreach (var heroData in allHeroes)
        {
            if (heroData.IsUnlocked() && heroData.UniqueID != GameConstant.MANUAL_HERO)
                _heroDatas.Add(heroData);
        }

        _equippedArmour.AddClickListener((wp, wd, equipUI) =>
        {
            if (GameMaster.ENABLE_ARMOUR_FEATURE)
                onShowFullDetail?.Invoke(wp, wd, EquipState.EQUIPPING);
        });
        _equippedWeapon.AddClickListener((wp, wd, equipUI) =>
        {
            onShowFullDetail?.Invoke(wp, wd, EquipState.EQUIPPING);
        });

        _dotSwitcher.Load(_heroDatas.Count, ShowHeroIndexWithOutUpdateDots);
        ShowHeroIndex(currentHeroIndex);
    }

    private void ShowHeroIndexWithOutUpdateDots(int index)
    {
        ShowHeroData(_heroDatas[index]);
    }

    private void ShowHeroIndex(int index)
    {
        ShowHeroData(_heroDatas[index]);
        _dotSwitcher.TurnOnDot(index);
    }

    private void ShowHeroData(HeroData heroData)
    {
        // _heroImg.sprite = ResourceManager.instance.GetHeroAvatar(heroData.UniqueID);
        // _heroName.textName = DesignHelper.GetHeroDesign(heroData.UniqueID, heroData.Rank).Name;

        LoadEquippedItem(heroData);

        // _starHelper.Load(heroData.Rank);
        UpdateAttribute();
        UpdateUpgradeIconState();
        //this._heroPreview.ResetAnimPreview(heroData);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_HERO_PREVIEW, heroData);

        ReminderManager.SeenNewHero(CurrentHero.UniqueID);
        _newHeroNextButtonReminder.Load(ReminderManager.HasNewHeroInHUDEquip().Item2.Count);
    }

    private void LoadEquippedItem(HeroData heroData)
    {
        if (heroData.HasArmour())
        {
            WeaponData wpData = SaveManager.Instance.Data.GetWeaponData(heroData.EquippedArmour);
            _equippedArmour.Load(wpData, DesignHelper.GetWeaponDesign(wpData));
        }
        else
        {
            _equippedArmour.Clear();
        }

        if (heroData.HasWeapon())
        {
            WeaponData wpData = SaveManager.Instance.Data.GetWeaponData(heroData.EquippedWeapon);
            _equippedWeapon.Load(wpData, DesignHelper.GetWeaponDesign(wpData));
        }
        else
        {
            _equippedWeapon.Clear();
        }

        //this._heroPreview.ResetAnimPreview(heroData);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_HERO_PREVIEW, heroData);
    }

    public void NextHero()
    {
        currentHeroIndex++;
        if (currentHeroIndex >= _heroDatas.Count)
            currentHeroIndex = 0;

        ShowHeroIndex(currentHeroIndex);
    }

    public void PrevHero()
    {
        currentHeroIndex--;
        if (currentHeroIndex < 0)
            currentHeroIndex = _heroDatas.Count - 1;

        ShowHeroIndex(currentHeroIndex);
    }

    public void Equip(WeaponData weaponData)
    {
        var weaponDesign = DesignHelper.GetWeaponDesign(weaponData);

        CurrentHero.ResetPowerData();
        var powerBeforeEquip = CurrentHero.FinalPowerData.Clone();

        UnEquip(weaponData, false);
        if (weaponDesign.EquipType == GameConstant.EQUIP_TYPE_WEAPON)
        {
            CurrentHero.EquippedWeapon = weaponData.UniqueID;
        }
        else
        {
            CurrentHero.EquippedArmour = weaponData.UniqueID;
        }

        CurrentHero.ResetPowerData();
        var powerAfterEquip = CurrentHero.FinalPowerData.Clone();

        ShowNewStatus(powerBeforeEquip, powerAfterEquip);

        weaponData.ItemStatus = ITEM_STATUS.Choosing;
        SaveManager.Instance.SaveData();

        LoadEquippedItem(CurrentHero);
        RefreshLayer();
    }

    private void ShowNewStatus(PowerData old, PowerData newPower)
    {
        var deltaPower = SaveGameHelper.MinusPowerData(newPower, old);
        var deltaAttribute = SaveGameHelper.GetListAttributeFromPowerData(deltaPower);
        _statusTextHolder.DestroyAllChild();

        if (deltaAttribute.Count != 0)
        {
            List<GameObject> bonusTexts = new List<GameObject>();
            List<GameObject> minusTexts = new List<GameObject>();

            foreach (var atributes in deltaAttribute)
            {
                var design = DesignManager.instance._dictSkillDesign[atributes.Item1];
                string dau = atributes.Item2 > 0 ? "+" : "";

                string value = $"{dau}{atributes.Item2:0}";
                if (design.IsPercent())
                {
                    value = $"{dau}{atributes.Item2:0.##}%";
                }

                if (atributes.Item1 == EffectType.PASSIVE_INCREASE_FIRERATE.ToString())
                {
                    value = $"{dau}{atributes.Item2:0.##}";
                }

                var statusText = Instantiate(_statusTextPrefab, _statusTextHolder);
                statusText.SetActive(true);

                var texts = statusText.GetComponentsInChildren<TextMeshProUGUI>();
                var nameText = texts[0];
                var valueText = texts[1];
                nameText.text = design.GetName().ToUpper();
                valueText.text = value.ToUpper();

                if (atributes.Item2 > 0)
                {
                    nameText.color = valueText.color = Color.green;
                    bonusTexts.Add(statusText);
                }
                else
                {
                    nameText.color = valueText.color = Color.red;
                    minusTexts.Add(statusText);
                }
            }

            foreach (var bonusText in bonusTexts)
            {
                bonusText.transform.SetAsFirstSibling();
            }

            if (bonusTexts.Count + minusTexts.Count > 0)
            {
                _statusFloatingText.Initialize("", 0, 200, 2.5f);
                _statusFloatingText.Show();
            }
            else
            {
                _statusFloatingText.Hide();
            }
        }
        else
        {
            _statusFloatingText.Hide();
        }
    }

    public void UnEquip(WeaponData data, bool needRefresh = true)
    {
        string unEquipedItemId = "";
        CurrentHero.ResetPowerData();
        var powerBeforeEquip = CurrentHero.FinalPowerData.Clone();

        var equipType = DesignHelper.GetWeaponDesign(data).EquipType;

        // Unequip current item
        if (equipType == GameConstant.EQUIP_TYPE_WEAPON)
        {
            unEquipedItemId = CurrentHero.EquippedWeapon;
            CurrentHero.EquippedWeapon = "";
            _equippedWeapon.Clear();
        }
        else
        {
            unEquipedItemId = CurrentHero.EquippedArmour;
            CurrentHero.EquippedArmour = "";
            _equippedArmour.Clear();
        }

        // Set status to avalaible.
        if (unEquipedItemId != "")
        {
            WeaponData unequipedItem = SaveManager.Instance.Data.GetWeaponData(unEquipedItemId);
            unequipedItem.ItemStatus = ITEM_STATUS.Available;
        }

        if (needRefresh)
        {
            LoadEquippedItem(CurrentHero);
            SaveManager.Instance.SaveData();
            RefreshLayer();
        }

        CurrentHero.ResetPowerData();
        var powerAfterEquip = CurrentHero.FinalPowerData.Clone();

        if (needRefresh)
            ShowNewStatus(powerBeforeEquip, powerAfterEquip);
    }

    public void RefreshLayer()
    {
        UpdateNewHeroes();
        _dotSwitcher.Load(_heroDatas.Count, ShowHeroIndexWithOutUpdateDots);
        ShowHeroIndex(currentHeroIndex);
        _newHeroNextButtonReminder.Load(ReminderManager.HasNewHeroInHUDEquip().Item2.Count);
        _lockManageHero.SetActive(
            !DesignHelper.IsRequirementAvailable(EnumHUD.HUD_HERO
                .ToString()));

        _goLockedArmour.gameObject.SetActiveIfNot(!GameMaster.ENABLE_ARMOUR_FEATURE);
    }

    private void UpdateNewHeroes()
    {
        var allHeroes = SaveManager.Instance.Data.Inventory.ListHeroData.ToList();
        foreach (var heroData in allHeroes)
        {
            if (heroData.IsUnlocked() && heroData.UniqueID != GameConstant.MANUAL_HERO &&
                !_heroDatas.Contains(heroData))
            {
                _heroDatas.Add(heroData);
            }
        }
    }

    private void UpdateUpgradeIconState()
    {
        if (CurrentHero.EquippedWeapon != "")
        {
            var weaponData = SaveManager.Instance.Data.GetWeaponData(CurrentHero.EquippedWeapon);
            var weapon = SaveGameHelper.GetUpgradeCost(weaponData);
            _equippedWeapon.ShowUpgradeIcon(DesignHelper.IsEnoughUpgradeMaterial(weapon) && !weaponData.IsMaxLevel());
        }
        else
        {
            _equippedWeapon.ShowUpgradeIcon(false);
        }

        if (CurrentHero.EquippedArmour != "")
        {
            var armourData = SaveManager.Instance.Data.GetWeaponData(CurrentHero.EquippedArmour);
            var armour = SaveGameHelper.GetUpgradeCost(armourData);
            _equippedArmour.ShowUpgradeIcon(DesignHelper.IsEnoughUpgradeMaterial(armour) && !armourData.IsMaxLevel());
        }
        else
        {
            _equippedArmour.ShowUpgradeIcon(false);
        }
    }

    private void UpdateAttribute()
    {
        CurrentHero.ResetPowerData();

        // var bonusPowerFromWeapon = CurrentHero.CalculateBonusFromWeapon();
        var heroPower = CurrentHero.FinalPowerData;

        _dmgTxt.text = $"{heroPower.Dmg:0}";
        _hpTxt.text = $"{heroPower.Hp:0}";
        // _dmgAttributeUi.LoadWithNoName(AttributeID.DAMAGE.ToString(), heroPower.Dmg);
        // _hpAttributeUi.LoadWithNoName(AttributeID.HP.ToString(), heroPower.Hp);
        // _armourAttributeUi.LoadWithBonus(AttributeID.ARMOUR.ToString(), heroPower.Armour,
        //     bonusPowerFromWeapon.Armour, false);
        // _fireRateAttributeUi.LoadWithBonus(AttributeID.FIRE_RATE.ToString(), heroPower.Firerate,
        //     bonusPowerFromWeapon.Firerate, false);
        // _rangeAttributeUi.LoadWithBonus(AttributeID.RANGE.ToString(), heroPower.Range,
        //     bonusPowerFromWeapon.Range, false);
    }

 

    public void UpdateNewHeroReminder(int getCanUpRankHeroCount)
    {
        _newHeroReminder.Load(getCanUpRankHeroCount);
    }
}