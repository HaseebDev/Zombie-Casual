using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDRemoveAds : BaseHUD
{
    public TextMeshProUGUI costText;
    public TextMeshProUGUI diamondBonusText;
    public TextMeshProUGUI goldBonusText;

    public LoadIAPButton costLoadButton;
    private int _bonusDiamond = 100;
    private int _bonusGold = 3000;

 
    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);

        diamondBonusText.text = "x" +  _bonusDiamond;
        goldBonusText.text = "x" +  _bonusGold;
        string priceStr = IAPManager.instance.GetProductPrice(IAPConstant.remove_ads);
        costText.text = priceStr;
        
        costLoadButton.StartLoadCost(priceStr,IAPConstant.remove_ads,costText);
        GamePlayController.instance.SetPauseGameplay(true);
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        GamePlayController.instance.SetPauseGameplay(false);
    }

    public void OnBuyButtonClick()
    {
        IAPManager.instance.PurchaseIAP(IAPConstant.remove_ads, success =>
        {
            if (success)
            {
                OnBuySuccess();
            }
            else
            {
                MasterCanvas.CurrentMasterCanvas.ShowPurchaseFail();
            }
        });
    }

    public void OnBuySuccess()
    {
        SaveGameHelper.ActiveNoAds();

        List<RewardData> rewardDatas = new List<RewardData>();
        rewardDatas.Add(new RewardData(REWARD_TYPE.DIAMOND,_bonusDiamond));
        rewardDatas.Add(new RewardData(REWARD_TYPE.GOLD,_bonusGold));
                
        MasterCanvas.CurrentMasterCanvas.ShowRewardSimpleHUD(rewardDatas,true);
        Hide();
    }
}
