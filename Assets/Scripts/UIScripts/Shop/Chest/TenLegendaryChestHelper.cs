using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Chest;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using static AnalyticsConstant;

public class TenLegendaryChestHelper : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _costText;
    [SerializeField] private LocalizedTMPTextUI _oldCostText;

    public GameObject goOldPrice;

    protected UserInventory _userInventory;
    protected ChestDesignElement _chestDesignElement;
    public Action<ChestDesignElement, OpenResourceType> OnOpenChest { get; set; }

    private void Init()
    {
        if (_chestDesignElement == null)
        {
            _userInventory = SaveManager.Instance.Data.Inventory;
            _chestDesignElement =
                DesignManager.instance.chestDesign.ChestDesignElements.Find(x =>
                    x.ChestID == GameConstant.CHEST_LEGENDARY_TEN_ID);

            _costText.text = FBUtils.CurrencyAddComma(_chestDesignElement.DiamondSale);//.ToString();
            _oldCostText.text = FBUtils.CurrencyAddComma(_chestDesignElement.DiamondCost);

            if (_chestDesignElement != null)
            {
                if (_chestDesignElement.DiamondSale == _chestDesignElement.DiamondCost)
                {
                    //disable sale
                    goOldPrice.gameObject.SetActive(false);
                }
                else
                {
                    goOldPrice.gameObject.SetActive(true);
                }
            }
        }
    }

    public void Load()
    {
        Init();
    }

    public void OnBuy()
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (!reached)
        //        return;
        //    if (CurrencyModels.instance.IsEnough(CurrencyType.DIAMOND, _chestDesignElement.DiamondSale))
        //    {
        //        CurrencyModels.instance.AddCurrency(CurrencyType.DIAMOND, -_chestDesignElement.DiamondSale);
        //        OnOpenChest?.Invoke(_chestDesignElement, OpenResourceType.DIAMOND);
        //    }
        //    else
        //    {
        //        MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.DIAMOND, _chestDesignElement.DiamondSale);
        //    }
        //});

        if (CurrencyModels.instance.IsEnough(CurrencyType.DIAMOND, _chestDesignElement.DiamondSale))
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.DIAMOND, -_chestDesignElement.DiamondSale);
            OnOpenChest?.Invoke(_chestDesignElement, OpenResourceType.DIAMOND);
        }
        else
        {
            MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.DIAMOND, _chestDesignElement.DiamondSale);
        }

        AnalyticsManager.instance.LogEvent(ANALYTICS_ENUM.PURCHASE_CHEST, new LogEventParam("chest-id", "TenLegendaryChest"));
    }


}