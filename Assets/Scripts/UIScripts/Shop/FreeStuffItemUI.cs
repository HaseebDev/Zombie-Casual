using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using QuickType.FreeStuff;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class FreeStuffItemUI : ShopItemUI
{
    [SerializeField] private Image _lockImage;
    [SerializeField] private GameObject _soldOut;
    [SerializeField] private UIShiny _uiShiny1;

    private bool _canReceive = false;

    public bool CanReceive
    {
        get { return _canReceive; }
        set
        {
            _canReceive = value;
            _lockImage.gameObject.SetActive(!_canReceive);
            _buyButton.interactable = _canReceive;
            if (_uiShiny1 == null)
                _uiShiny1 = _uiShiny;

            _uiShiny.enabled = _canReceive;
        }
    }

    public override void Load(ShopDesignElement shopDesignElement)
    {
        base.Load(shopDesignElement);
        var reward = shopDesignElement.GetReward()[0];
        _valueText.text = $"x{reward._value.ToString()}";
        if (reward._type == REWARD_TYPE.ADD_ON)
        {
             ResourceManager.instance.GetUltimateSprite(reward._extends.ToString(), s =>
             {
                 _icon.sprite = s;
             });
        }
    }

    public void SoldOut()
    {
        CanReceive = false;
        _iconCurrency.gameObject.SetActive(false);
        _newText.gameObject.SetActive(false);
        _soldOut.SetActive(true);
    }

    public void Available()
    {
        if (_shopDesignElement.GetCost().Type != CostType.FREE)
            _iconCurrency.gameObject.SetActive(true);

        _newText.gameObject.SetActive(true);
        _soldOut.SetActive(false);
    }
}