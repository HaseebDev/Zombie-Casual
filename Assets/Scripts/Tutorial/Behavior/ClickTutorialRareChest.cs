using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTutorialRareChest : ClickTutorialType
{
    protected override void OnShow()
    {
        base.OnShow();
        // Add a key
        if (SaveManager.Instance.Data.Inventory.TotalRareKey == 0)
            SaveManager.Instance.Data.Inventory.TotalRareKey = 1;
        //SaveManager.Instance.SaveData();

        foreach (var rareChestHelper in FindObjectsOfType<RareChestHelper>())
        {
            rareChestHelper.Load();
        }

        FindObjectOfType<HUDShop>().SnapToShopType(ShopType.CHEST);
    }
}