using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI3 : EquipmentUI
{
    [SerializeField] private Image _typeBG;
    [SerializeField] private Sprite _defaultBGSprite;
    [SerializeField] private GameObject _upgradeIcon;

    public void ShowUpgradeIcon(bool isShow)
    {
        _upgradeIcon.SetActive(isShow);
    }

    public override void Clear()
    {
        base.Clear();
        _typeBG.gameObject.SetActive(true);
        _bg.sprite = _defaultBGSprite;
    }

    public override void Load(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        base.Load(weaponData, weaponDesign);
        _typeBG.gameObject.SetActive(false);
    }
}