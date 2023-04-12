using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuickType.Promotion;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.UI;

public class PromotionItemUI : ShopItemUI
{
    [SerializeField] private Text _oldText1;
    [SerializeField] private Text _promotionText;

    public override void Load(ShopDesignElement shopDesignElement)
    {
        base.Load(shopDesignElement);
        PromotionDesignElement promotionDesignElement = (PromotionDesignElement) shopDesignElement;

        var costData = promotionDesignElement.GetCost();
        var oldCostData = promotionDesignElement.GetOldCost();

        if (oldCostData.Value != 0 && oldCostData.Value != costData.Value)
        {
            _oldText1.text = oldCostData.PriceStr;
            _oldText1.gameObject.SetActive(true);
        }
        else
        {
            _oldText1.gameObject.SetActive(false);
        }

        _promotionText.text = promotionDesignElement.Description;
    }
}