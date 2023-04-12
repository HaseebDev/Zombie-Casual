using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class DiamondItemUI : ShopItemUI
{
    [SerializeField] private GameObject x2Badget;
    private ShopDesignElement _origin;
    
    protected override void OnEnable()
    {
        // Skip reload diamond pack
        if (_origin != null)
        {
            _origin.ResetData();
            var rewards = _origin.GetReward();

            if (_value != (int) rewards[0]._value)
            {
                Load(_origin);
                var isFreePack = SaveGameHelper.IsFreePack(ShopType.DIAMOND, ShopDesignElement);
                if (isFreePack)
                    SetX2(false);
                else
                {
                    SetX2(!AchieveManager.HasPurchasedDiamond());
                }
            }
        }
    }


    public void SetX2(bool isOn)
    {
        if (isOn)
        {
            _origin = _shopDesignElement;
            _shopDesignElement = _shopDesignElement.Clone();
            _bonusPanel.SetActive(true);
            // _x2Text.text = $"1st time <color=#36CF3C>+{_shopDesignElement.GetReward()[0]._value}</color> diamonds";
            _bonusText.text = $"+{_shopDesignElement.GetReward()[0]._value}";
            _shopDesignElement.DiamondReward *= 2;
            _shopDesignElement.Bonus = 0; // Remove bonus
            _goodChoiceImg.SetActive(false);
            // Debug.LogError($"{gameObject.name} + active x2");
        }

        x2Badget.SetActive(isOn);
        // else
        // {
        //     _bonusPanel.SetActive(false);
        // }
    }
}