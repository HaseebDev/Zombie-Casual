using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class EquipmentUI2 : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _name;
    [SerializeField] private LocalizedTMPTextUI _rank;
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private GameObject _shining;

    public WeaponData WeaponData;
    public WeaponDesign WeaponDesign;

    public EquipmentUI EquipmentUi
    {
        get => _equipmentUi;
        set => _equipmentUi = value;
    }

    public void Load(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        WeaponData = weaponData;
        WeaponDesign = weaponDesign;

        RankDefine rankDefine = ResourceManager.instance.GetRankDefine(weaponData.Rank);
        _name.textName = weaponDesign.Name;
        _rank.textName = rankDefine.name;
        _name.targetTMPText.color = rankDefine.color;
        _rank.targetTMPText.color = rankDefine.color;

        _equipmentUi.Load(weaponData, weaponDesign);
        _shining.SetActive(weaponData.Rank >= RankConstant.LEGENDARY);
    }
}