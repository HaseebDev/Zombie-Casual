using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using Google.Protobuf.WellKnownTypes;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class EquipmentFusionUI : EquipmentUI
{
    [SerializeField] private GameObject _statusPanel;
    [SerializeField] private GameObject _selected;
    [SerializeField] private GameObject _locked;
    [SerializeField] private LocalizedTMPTextUI _statusText;

    private bool _isSelected = false;
    private bool _isLocked = false;

    public override void HideUnNecessaryInfo()
    {
        _statusPanel.SetActive(false);
    }

    public override void Load(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        base.Load(weaponData, weaponDesign);
        _statusPanel.SetActive(false);

        if (weaponData.ItemStatus == ITEM_STATUS.Choosing)
        {
            _statusText.textName = LOCALIZE_ID_PREF.EQUIPPED;
            _statusPanel.SetActive(true);
        }

        if (weaponData.Rank == GameConstant.MAXIMUM_RANK)
        {
            _statusText.textName = LOCALIZE_ID_PREF.MAX;
            _statusPanel.SetActive(true);
        }
    }

    protected override void OnClick()
    {
        if (!_isSelected && !_isLocked)
            base.OnClick();
    }

    public void SetIsSelected(bool isSelect)
    {
        _isSelected = isSelect;
        _selected.SetActive(_isSelected);
    }

    public void SetIsLocked(bool isLocked)
    {
        _isLocked = isLocked;
        _locked.SetActive(_isLocked);
    }
}