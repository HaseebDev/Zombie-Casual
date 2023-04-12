using System;
using System.Collections;
using System.Collections.Generic;
using QuickType;
using QuickType.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HeroChestHelperx5 : MonoBehaviour
{
    public TextMeshProUGUI costText;
    public LoadIAPButton loadIapButton;
    public Action<List<RewardData>> OnOpen;


    public void ResetLayer()
    {
        string productPrice = IAPManager.instance.GetProductPrice(IAPConstant.hero_chest_x5);
        costText.text = productPrice;
        loadIapButton.StartLoadCost(productPrice, IAPConstant.hero_chest_x5, costText);
    }

    public void OnPurchaseClick()
    {
        IAPManager.instance.PurchaseIAP(IAPConstant.hero_chest_x5, isSuccess =>
        {
            if (isSuccess)
            {
                // LOAD REWARD
                List<RewardData> rewardDatas = new List<RewardData>();

                for (int i = 0; i < 5; i++)
                {
                    ChestHeroDesignElement chestDesign =
                        DesignManager.instance.chestHeroDesign.ChestHeroDesignElements.PickRandom();
                    var tepmReward = chestDesign.GetRewards();
                    foreach (var VARIABLE in tepmReward)
                    {
                        rewardDatas.Add(VARIABLE);
                    }
                }

                OnOpen?.Invoke(rewardDatas);
                // TopLayerCanvas.instance.ShowRewardSimpleHUD(rewardDatas, true);
                ResetLayer(); 
            }
        });
    }
}