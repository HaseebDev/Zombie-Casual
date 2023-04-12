using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchieveManager
{
    public static bool HasPurchasedDiamond()
    {
        return SaveManager.Instance.Data.ShopData.HasPurchaseDiamond;
    }

    public static void AchievePurchaseDiamond()
    {
        SaveManager.Instance.Data.ShopData.HasPurchaseDiamond = true;
        SaveManager.Instance.SaveData();
    }

    public static void AchievePurchaseIAP()
    {
        SaveManager.Instance.Data.ShopData.HasPurchaseIAP = true;
        SaveManager.Instance.SaveData();
        // PlayerPrefs.SetInt(AchieveConstant.HAS_PURCHASED_IAP, 1);
    }

    public static bool HasPurchaseIAP()
    {
        return SaveManager.Instance.Data.ShopData.HasPurchaseIAP;
        // return PlayerPrefs.GetInt(AchieveConstant.HAS_PURCHASED_IAP, 0) == 1;
    }
}