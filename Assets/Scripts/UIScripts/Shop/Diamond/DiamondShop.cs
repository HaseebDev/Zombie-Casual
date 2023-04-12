using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.talent;
using Doozy.Engine.Extensions;
using MEC;
using QuickType.Shop;
using UnityEngine;

public class DiamondShop : BaseShop
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
                var shopItem = (DiamondItemUI) _shopItemUis[index++];
                
                bool isFreePack = false;
                if (!loadFirstPack)
                {
                    isFreePack = SaveGameHelper.IsFreePack(ShopType, shopDesignElement);
                }

                if (isFreePack)
                {
                    var cloneDesign = SaveGameHelper.ConvertToFreePack(ShopType,shopDesignElement);
                    shopItem.Load(cloneDesign);
                    shopItem.SetX2(false);
                }
                else
                {
                    shopItem.Load(shopDesignElement);
                    shopItem.SetX2(!AchieveManager.HasPurchasedDiamond());
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
                var shopItem = (DiamondItemUI) Instantiate(_shopItemUiPrefab, _shopItemHolder);
                _shopItemUis.Add(shopItem);
                
                bool isFreePack = false;
                if (!loadFirstPack)
                {
                    isFreePack = SaveGameHelper.IsFreePack(ShopType, shopDesignElement);
                }

                if (isFreePack)
                {
                    var cloneDesign = SaveGameHelper.ConvertToFreePack(ShopType,shopDesignElement);
                    shopItem.Load(cloneDesign);
                    shopItem.SetX2(false);
                }
                else
                {
                    shopItem.Load(shopDesignElement);
                    shopItem.SetX2(!AchieveManager.HasPurchasedDiamond());
                }

                shopItem.SetOnPurchaseCallback(OnPurchase);
                shopItem.SetScale(scale);
                scale += 0.1f;

                loadFirstPack = true;

                yield return Timing.WaitForOneFrame;
            }
        }
    }

    public void CheckTalent(List<RewardData> rewardData)
    {
        if (ModelTalent.bonusDiamondShopPercent != 0)
        {
            // Debug.LogError("BONUS GEM " + ModelTalent.bonusDiamondShopPercent);

            foreach (var reward in rewardData)
            {
                long old = reward._value;
                long bonus = (long) (reward._value * ModelTalent.bonusDiamondShopPercent);
                reward._value += bonus;

                // Debug.LogError($"Before {old}, after{reward._value}");
            }
        }
    }

    public override void OnPurchase(CostData costData, List<RewardData> rewardData, ShopItemUI shopItemUi,
        Action<bool> callBack)
    {
        CheckTalent(rewardData);
        base.OnPurchase(costData, rewardData, shopItemUi, (reached) =>
        {
            if (reached)
            {
                if (shopItemUi.ShopDesignElement.CostType == CostType.IAP.ToString())
                {
                    if (!AchieveManager.HasPurchasedDiamond())
                    {
                        AchieveManager.AchievePurchaseDiamond();
                    }
                }
                else
                {
                    SaveGameHelper.SaveBoughFreePack(shopItemUi.ShopDesignElement);
                }

                Load();
            }
        });
    }
}