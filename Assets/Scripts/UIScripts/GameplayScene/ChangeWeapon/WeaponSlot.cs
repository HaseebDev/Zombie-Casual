using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using com.datld.data;
using DG.Tweening;
using QuickEngine.Extensions;
using QuickType;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private Image _heroAvatar;
    [SerializeField] private TextMeshProUGUI _dps;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private UIShiny _upgradeShiny;
    [SerializeField] private UIShiny _upgradeSuccessShiny;
    [SerializeField] private GameObject _costIcon;
    [SerializeField] public Button _upgradeButton;
    [SerializeField] private Button _maxButton;
    [SerializeField] private ReminderUI _reminderUi;
    [SerializeField] private GameObject _fusionAvailable;

    private HeroData _heroData;
    private WeaponData _weaponData;

    private Action<HeroData> _onUpgrade;
    private Action<HeroData, Vector3> _onOpenChangeWeapon;

    private void Awake()
    {
        CurrencyModels.instance.AddCallback(OnUpdateCurrency);
    }

    private void OnUpdateCurrency(CurrencyType currencyType, long num)
    {
        if (currencyType == CurrencyType.WEAPON_SCROLL)
        {
            UpdateUpgradeButtonState();
        }
    }


    private void OnDestroy()
    {
        CurrencyModels.instance.RemoveCallback(OnUpdateCurrency);
    }


    public void SetOnUpgradeCallback(Action<HeroData> callback)
    {
        _onUpgrade = callback;
    }

    public void SetOnShowChangeWeaponCallback(Action<HeroData, Vector3> callback)
    {
        _onOpenChangeWeapon = callback;
    }

    public bool HasWeapon()
    {
        return _weaponData != null && _heroData != null;
    }

    public void Load(string heroID, int i)
    {
        if (heroID.IsNullOrEmpty() || heroID == "NONE")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            ResourceManager.instance.GetHeroAvatar(heroID,_heroAvatar);
            _heroData = SaveManager.Instance.Data.GetHeroData(heroID);
            _weaponData = _heroData.GetEquippedWeapon();

            UpdateInfo();
        }
    }

    public void UpdateInfo()
    {
        if(_weaponData == null)
            return;
        
        var wpDesign = DesignHelper.GetWeaponDesign(_weaponData);
        var upgradeCost = SaveGameHelper.GetUpgradeCost(_weaponData);
        _costText.text = upgradeCost.weaponScroll.ToString();

        // if (_weaponData.IsMaxLevel())
        // {
        //     _costText.text = "MAX";
        //     _upgradeButton.interactable = false;
        //     _costIcon.SetActive(false);
        // }
        // else
        // {
        //     _costText.text = upgradeCost.weaponScroll.ToString();
        //     _upgradeButton.interactable = true;
        //     _costIcon.SetActive(true);
        // }

        _weaponData.ResetPowerData();
        // Debug.LogError($"Weapon {_weaponData.WeaponID}, dmg: {_weaponData.FinalPowerData.Dmg}, firerate: {_weaponData.FinalPowerData.Firerate}");
        _dps.text = $"{_weaponData.FinalPowerData.PercentDmg:0.##}%";

        if (_weaponData != null)
        {
            _equipmentUi.Load(_weaponData, wpDesign);
        }

        UpdateUpgradeButtonState();
    }

    public void UpdateUpgradeButtonState()
    {
        if (_weaponData == null)
            return;

        bool canUpgrade = CanUpgrade();
        _upgradeShiny.enabled = canUpgrade;
        // _upgradeButton.image.color =
        //     canUpgrade ? _upgradeButton.colors.normalColor : _upgradeButton.colors.disabledColor;
        _maxButton.gameObject.SetActive(_weaponData.IsMaxLevel());
    }

    public void Upgrade()
    {
        var upgradeCost = SaveGameHelper.GetUpgradeCost(_weaponData);

        if (CanUpgrade())
        {
            _weaponData.LevelUpWeapon();//Level++;
            _weaponData.ResetPowerData();
            UpdateInfo();

            CurrencyModels.instance.AddCurrency(CurrencyType.WEAPON_SCROLL, -upgradeCost.weaponScroll);
            MissionManager.Instance.TriggerMission(MissionType.UPGRADE_EQUIPMENT,_weaponData.WeaponID);

            _onUpgrade?.Invoke(_heroData);

            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_UPGRADE);
            
            _upgradeSuccessShiny.effectPlayer.duration = 0.25f;
            _upgradeSuccessShiny.Play();
            DOVirtual.DelayedCall(0.25f, () => { _upgradeSuccessShiny.Play(); });

            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY);
            SaveManager.Instance.SetDataDirty();
        }
        else
        {
            MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(CurrencyType.WEAPON_SCROLL, upgradeCost.weaponScroll, false);
        }
    }

    public bool CanUpgrade()
    {
        var upgradeCost = SaveGameHelper.GetUpgradeCost(_weaponData);
        return !_weaponData.IsMaxLevel() &&
               CurrencyModels.instance.IsEnough(CurrencyType.WEAPON_SCROLL, upgradeCost.weaponScroll);
    }


    public void ShowChangeWeaponPanel()
    {
        _onOpenChangeWeapon?.Invoke(_heroData,
            transform.position +
            new Vector3(0, (transform.rectTransform().rect.height / 2f - 50f) * Screen.height / 2688f));

        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_BUTTON);
    }

    public void LoadReminder(int quantity)
    {
        _reminderUi.Load(quantity);
        // _reminderUi.Show(isShow);
    }

    public void OnMaxButtonClick()
    {
        MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.FUSION_TO_UP_LV.AsLocalizeString());
    }

    public void ShowFusionAvailable(bool isShow)
    {
        _fusionAvailable.SetActive(isShow);
    }
}
