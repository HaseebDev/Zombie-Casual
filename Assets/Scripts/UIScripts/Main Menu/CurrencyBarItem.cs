using Framework.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using Random = System.Random;

public class CurrencyBarItem : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private LocalizedTMPTextUI _value;

    private CurrencyType _type;
    private Action<CurrencyType> _onAddButtonClick;

    public void UpdateText(long value)
    {
        if (_value != null)
            _value.text = Converter.CurrencyConvert(value);
    }

    public void Load(CurrencyType type, Action<CurrencyType> onAddCallback)
    {
        _type = type;
        _onAddButtonClick = onAddCallback;

        // if (type == CurrencyType.GOLD || type == CurrencyType.DIAMOND)
        ResourceManager.instance.GetCurrencySprite(type, _icon);
    }

    public void OnAddClick()
    {
        _onAddButtonClick?.Invoke(_type);
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_BUTTON);
    }
}