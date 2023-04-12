using System;
using System.Collections;
using System.Collections.Generic;
using QuickType;
using QuickType.UnlockHero;
using UnityEngine;

public class ShopHeroChest : MonoBehaviour
{
    public HeroChestHelper heroChestHelper;
    public HeroChestHelperx5 heroChestHelperx5;

    public ChestHeroOpenResult openResult;

    private void Awake()
    {
        heroChestHelper.OnOpen = OpenChestX1;
        heroChestHelperx5.OnOpen = OpenChestX5;
    }

    private void OpenChestX1(List<RewardData> rewardDatas)
    {
        openResult.OpenChestX1(rewardDatas);
    }
    
    private void OpenChestX5(List<RewardData> rewardDatas)
    {
        openResult.OpenChestX5(rewardDatas);
    }

    public void ResetLayer()
    {
        bool isUnlocked = DesignHelper.GetUnlockRequirementLevel(UnlockRequireId.SHOP_HERO_CHEST).Item1;
        if (isUnlocked)
        {
            gameObject.SetActive(true);

            heroChestHelper.ResetLayer();
            heroChestHelperx5.ResetLayer();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}