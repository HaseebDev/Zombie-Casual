using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class DismantleHelper : MonoBehaviour
{
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private CurrencyUI _gold;
    [SerializeField] private CurrencyUI _weapon;
    [SerializeField] private CurrencyUI _armour;

    private Tuple<long, long, long> _currentResources;
    private Action<WeaponData, WeaponDesign> onDismantle;
    private WeaponData _weaponData;
    private WeaponDesign _weaponDesign;

    public void SetOnDismantleCallback(Action<WeaponData, WeaponDesign> callback)
    {
        onDismantle = callback;
    }

    public void Show(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        gameObject.SetActive(true);
        _equipmentUi.Load(weaponData, weaponDesign);

        _weaponData = weaponData;
        _weaponDesign = weaponDesign;
        _currentResources = weaponData.GetDismantleResource();

        bool hasGold = _currentResources.Item1 > 0;
        bool hasWeaponScroll = _currentResources.Item2 > 0;
        bool hasArmourScroll = _currentResources.Item3 > 0;

        _gold.gameObject.SetActive(hasGold);
        _weapon.gameObject.SetActive(hasWeaponScroll);
        _armour.gameObject.SetActive(hasArmourScroll);

        _gold.Load(new RewardData(REWARD_TYPE.GOLD, _currentResources.Item1));
        _weapon.Load(new RewardData(REWARD_TYPE.SCROLL_WEAPON, _currentResources.Item2));
        _armour.Load(new RewardData(REWARD_TYPE.SCROLL_ARMOUR, _currentResources.Item3));
    }

    public void Confirm()
    {
        List<RewardData> rewardDatas = new List<RewardData>();
        bool hasGold = _currentResources.Item1 > 0;
        bool hasWeaponScroll = _currentResources.Item2 > 0;
        bool hasArmourScroll = _currentResources.Item3 > 0;

        if (hasGold)
            rewardDatas.Add(new RewardData(REWARD_TYPE.GOLD, _currentResources.Item1));
        if (hasWeaponScroll)
            rewardDatas.Add(new RewardData(REWARD_TYPE.SCROLL_WEAPON, _currentResources.Item2));
        if (hasArmourScroll)
            rewardDatas.Add(new RewardData(REWARD_TYPE.SCROLL_ARMOUR, _currentResources.Item3));

        MainMenuCanvas.instance.ShowRewardSimpleHUD(rewardDatas, true);
        onDismantle?.Invoke(_weaponData, _weaponDesign);
        gameObject.SetActive(false);
    }
}