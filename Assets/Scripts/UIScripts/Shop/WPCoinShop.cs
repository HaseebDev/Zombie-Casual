using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using MEC;
using QuickType.Shop;
using UnityEngine;

public class WPCoinShop : BaseShop
{
    public override void ResetLayer()
    {
        if (_shopItemUis == null || _shopItemUis.Count == 0)
        {
            return;
        }
        
        int index = 0;
        bool loadFirstPack = false;

        foreach (var shopDesignElement in DesignManager.instance.shopDesign.ShopDesignElement)
        {
            if (shopDesignElement.Id.Contains(ShopType.ToString()))
            {
                shopDesignElement.ResetData();

                var shopItem = _shopItemUis[index++];
                bool isFreePack = false;

                if (!loadFirstPack)
                {
                    isFreePack = SaveGameHelper.IsFreePack(ShopType, shopDesignElement);
                }

                if (isFreePack)
                {
                    var cloneDesign = SaveGameHelper.ConvertToFreePack(ShopType, shopDesignElement);//.Clone();
                    shopItem.Load(cloneDesign);
                }
                else
                {
                    shopDesignElement.ResetData();
                    shopItem.Load(shopDesignElement);
                }
                
                loadFirstPack = true;
            }
        }
    }
    
     protected override IEnumerator<float> LoadShop()
    {
        ClearOldItems();
        float scale = 1;
        bool loadFirstPack = false;

        foreach (var shopDesignElement in DesignManager.instance.shopDesign.ShopDesignElement)
        {
            if (shopDesignElement.Id.Contains(ShopType.ToString()))
            {
                shopDesignElement.ResetData();

                var shopItem = Instantiate(_shopItemUiPrefab, _shopItemHolder);
                _shopItemUis.Add(shopItem);
                
                bool isFreePack = false;

                if (!loadFirstPack)
                {
                    isFreePack = SaveGameHelper.IsFreePack(ShopType, shopDesignElement);
                }

                if (isFreePack)
                {
                    var cloneDesign = SaveGameHelper.ConvertToFreePack(ShopType, shopDesignElement);//.Clone();
                    shopItem.Load(cloneDesign);
                }
                else
                {
                    shopDesignElement.ResetData();
                    shopItem.Load(shopDesignElement);
                }

                shopItem.SetOnPurchaseCallback(OnPurchase);
                shopItem.SetScale(scale);
                if (_bgSprite != null)
                    shopItem.SetBg(_bgSprite);
                scale += 0.1f;
                
                loadFirstPack = true;
                yield return Timing.WaitForOneFrame;
            }
        }
    }

    public override void OnPurchase(CostData costData, List<RewardData> rewardData, ShopItemUI shopItemUi,
        Action<bool> callBack)
    {
        base.OnPurchase(costData, rewardData, shopItemUi, (reached) =>
        {
            if (reached)
            {
                if (SaveGameHelper.IsFreePack(ShopType, shopItemUi.ShopDesignElement))
                {
                    SaveGameHelper.SaveBoughFreePack(shopItemUi.ShopDesignElement);
                }
                
                Load();
            }
        });
    }
}
