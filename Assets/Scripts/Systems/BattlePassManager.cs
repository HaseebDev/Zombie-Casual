using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattlePassManager
{
    public static bool HasBattlePass = false;

    public static void Init()
    {
        HasBattlePass = SaveManager.Instance.Data.ShopData.HasBattlePass;
    }

    public static void BuyBP(Action<bool> onBuyCallback)
    {
        SaveManager.Instance.Data.ShopData.HasBattlePass = true;
        HasBattlePass = true;
        onBuyCallback?.Invoke(true);
    }
}