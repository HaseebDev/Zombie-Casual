using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using Doozy.Engine.Extensions;
using UnityEngine;

public class ReminderManager
{
    private static UserData _lastUserData => SaveManager.Instance.Data.LastData;
    private static UserData _currentUserData => SaveManager.Instance.Data;
    private static ReminderData _reminderData => SaveManager.Instance.Data.ReminderData;

    private static bool _showedStuff = false;

    public static void Init()
    {
    }

    public static Tuple<bool, List<string>> HasNewHeroInHUDEquip()
    {
        return new Tuple<bool, List<string>>(_reminderData.NewHeroOnHudEquip.Count != 0,
            _reminderData.NewHeroOnHudEquip.ToList());
    }

    public static void SeenNewHero(string id)
    {
        _reminderData.NewHeroOnHudEquip.Remove(id);
    }

    public static int HasNewChest()
    {
        int total = RareChestHelper.GetStack() + LegendaryChestHelper.GetStack();
        return total;
    }

    public static int HasNewShopItem()
    {
        int total = 0;
        // var freeStuffs = FreeStuffShop.GetFreeStuffs();
        // long remainStuff = freeStuffs.Count - SaveManager.Instance.Data.ShopData.CurrentIndexFreeStuff;

        total = RareChestHelper.GetStack() + LegendaryChestHelper.GetStack();
        // if (remainStuff > 0 && !_showedStuff)
        // {
            // total++;
        // }

        return total;
    }

    public static void SaveCurrentShopState()
    {
        _showedStuff = true;
    }

    public static Tuple<bool, List<AddOnItem>> HasNewAddOn()
    {
        List<AddOnItem> result = new List<AddOnItem>();
        bool has = false;

        var currentAddOn = _currentUserData.Inventory.ListAddOnItems.ToList();
        var oldAddOn = _lastUserData.Inventory.ListAddOnItems.ToList();

        foreach (var adon in currentAddOn)
        {
            var design = DesignHelper.GetSkillDesign(adon.ItemID);
            var tempAdon = oldAddOn.Find(x => x.ItemID == adon.ItemID);
            if (tempAdon != null && design.SkillType == SkillType.ADD_ON)
            {
                // Debug.LogError($"ID {adon.ItemID}, OLD {tempAdon.ItemCount}, NEW {adon.ItemCount}");
                if (tempAdon.ItemCount < adon.ItemCount)
                {
                    has = true;
                    result.Add(adon);
                }
            }
        }

        return new Tuple<bool, List<AddOnItem>>(has, result);
    }

    public static Tuple<bool, List<HeroData>> HasNewHero()
    {
        List<HeroData> result = new List<HeroData>();
        bool has = false;

        var currentHero = _currentUserData.Inventory.ListHeroData.ToList();
        var oldHero = _lastUserData.Inventory.ListHeroData.ToList();

        foreach (var hero in currentHero)
        {
            var tempHero = oldHero.Find(x => x.UniqueID == hero.UniqueID);

            if (tempHero != null && tempHero.ItemStatus == ITEM_STATUS.Locked && tempHero.ItemStatus != hero.ItemStatus)
            {
                result.Add(hero);
                has = true;
                // Debug.LogError(
                // $"Hero {tempHero.UniqueID}, old status {tempHero.ItemStatus}, new status {hero.ItemStatus}");
            }
        }

        return new Tuple<bool, List<HeroData>>(has, result);
    }

    public static Tuple<bool, List<WeaponData>> HasNewEquip()
    {
        List<WeaponData> result = new List<WeaponData>();
        bool has = false;

        var currentWeapons = _currentUserData.Inventory.ListWeaponData.ToList();
        var oldWeapons = _lastUserData.Inventory.ListWeaponData.ToList();

        foreach (var wp in currentWeapons)
        {
            var tempWp = oldWeapons.Find(x => x.UniqueID == wp.UniqueID);

            if (tempWp == null)
            {
                result.Add(wp);
                has = true;
            }
        }

        return new Tuple<bool, List<WeaponData>>(has, result);
    }

    public static void SaveCurrentWeaponsState()
    {
        var currentWeapons = _currentUserData.Inventory.ListWeaponData.ToList();
        _lastUserData.Inventory.ListWeaponData.Clear();

        foreach (var wp in currentWeapons)
        {
            _lastUserData.Inventory.ListWeaponData.Add(wp.Clone());
        }

        // SaveManager.Instance.SaveData();
        SaveManager.Instance.SetDataDirty();
    }

    public static void SaveCurrentAddOnState()
    {
        var currentHeroes = _currentUserData.Inventory.ListAddOnItems.ToList();
        _lastUserData.Inventory.ListAddOnItems.Clear();

        foreach (var hero in currentHeroes)
        {
            _lastUserData.Inventory.ListAddOnItems.Add(hero.Clone());
        }

        // SaveManager.Instance.SaveData();
        SaveManager.Instance.SetDataDirty();
    }

    public static void SaveCurrentHeroState()
    {
        var currentHeroes = _currentUserData.Inventory.ListHeroData.ToList();
        _lastUserData.Inventory.ListHeroData.Clear();

        foreach (var hero in currentHeroes)
        {
            _lastUserData.Inventory.ListHeroData.Add(hero.Clone());
        }

        //SaveManager.Instance.SaveData();
        SaveManager.Instance.SetDataDirty();
    }

    public static void OnResetLayer(EnumHUD hud)
    {
        var newEquips = HasNewEquip();
        var newHeroes = HasNewHero();
        var canUprankHeroes = SaveGameHelper.GetCanUpRankHeroCount();
        var newShops = HasNewShopItem();

        MainMenuTab.Instance.ShowReminder(EnumHUD.HUD_EQUIPMENT,
            newEquips.Item1 || newHeroes.Item1 || canUprankHeroes != 0,
            newEquips.Item2.Count + newHeroes.Item2.Count + canUprankHeroes);
        MainMenuTab.Instance.ShowReminder(EnumHUD.HUD_SHOP, newShops > 0, newShops);
    }
}