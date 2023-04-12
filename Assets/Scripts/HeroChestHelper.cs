using System;
using System.Collections;
using System.Collections.Generic;
using QuickType;
using QuickType.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroChestHelper : MonoBehaviour
{
    
    public TextMeshProUGUI costText;
    public Image costIcon;
    public ReminderUI reminderUI;
    public LoadIAPButton loadIapButton;

    private bool _isIap = false;
    public Action<List<RewardData>> OnOpen;

    public virtual void ResetLayer()
    {
        int heroChestKey = (int) SaveManager.Instance.Data.Inventory.TotalHeroChestKey;
        bool useKey = heroChestKey > 0;
        
        if (useKey)
        {
            ResourceManager.instance.GetRewardSprite(REWARD_TYPE.KEY_CHEST_HERO, s => { costIcon.sprite = s; });
            loadIapButton.CompleteLoad();
            costText.text = $"1/{heroChestKey}";
        }
        else
        {
            string productPrice = IAPManager.instance.GetProductPrice(IAPConstant.hero_chest_x1);
            costText.text = productPrice;
            loadIapButton.StartLoadCost(productPrice,IAPConstant.hero_chest_x1,costText);
        }
        
        _isIap = !useKey;
        costIcon.gameObject.SetActive(useKey);
        reminderUI.Load(heroChestKey);
    }

    public void OnPurchaseClick()
    {
        if (_isIap)
        {
            // DO IAP
            IAPManager.instance.PurchaseIAP(IAPConstant.hero_chest_x1, isSuccess =>
            {
                if (isSuccess)
                {
                    CreateRewards();
                    ResetLayer();
                }
            });
        }
        else
        {
            SaveManager.Instance.Data.Inventory.TotalHeroChestKey--;
            // LOAD REWARD
            CreateRewards();
            ResetLayer();
        }
    }

    private void CreateRewards()
    {
        ChestHeroDesignElement chestDesign = DesignManager.instance.chestHeroDesign.ChestHeroDesignElements.PickRandom();
        OnOpen?.Invoke(chestDesign.GetRewards());
        // TopLayerCanvas.instance.ShowRewardSimpleHUD(chestDesign.GetRewards(),true);    
    }
}