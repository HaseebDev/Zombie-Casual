using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using Doozy.Engine.Utils.ColorModels;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMaterialUI : MonoBehaviour
{
    [SerializeField] private Text content;
    [SerializeField] private Text valueText;
    [SerializeField] private Image icon;
    [SerializeField] private Color notEnoughColor;
    
    private CurrencyType _type;
    private long _value;
    private bool _isInit = false;

    private void Init()
    {
        if (!_isInit)
        {
            CurrencyModels.instance.AddCallback(OnAddCurrency);
            _isInit = true;
        }
    }

    private void OnDestroy()
    {
        CurrencyModels.instance.RemoveCallback(OnAddCurrency);
    }

    private void OnAddCurrency(CurrencyType type, long totalValue)
    {
        Load(_type, _value);
    }

    public void Load(CurrencyType type, long valueLong)
    {
        Init();
        _type = type;
        _value = valueLong;

        string name = "";
        long remains = 0;

        switch (type)
        {
            case CurrencyType.GOLD:
                break;
            case CurrencyType.DIAMOND:
                break;
            case CurrencyType.TOKEN:
                break;
            case CurrencyType.PILL:
                break;
            case CurrencyType.WEAPON_SCROLL:
                name = "Weapon scroll";
                remains = CurrencyModels.instance.WeaponScrolls;
                break;
            case CurrencyType.ARMOUR_SCROLL:
                name = "Armour scroll";
                remains = CurrencyModels.instance.ArmourScrolls;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        content.text = $"{name}: ";
        valueText.text = $"{remains}/{valueLong}";
        ResourceManager.instance.GetCurrencySprite(type,icon);
        valueText.color = remains >= valueLong ? Color.white : notEnoughColor;
        
        gameObject.SetActive(true);
    }
}