using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using com.datld.data;
using com.datld.talent;
using Doozy.Engine.Extensions;
using Google.Protobuf.Collections;
using QuickType.Attribute;
using QuickType.Hero;
using QuickType.Weapon;
using Random = UnityEngine.Random;
using QuickType;
using QuickType.Talent;
using QuickType.Ascend;
using QuickType.Shop;

[Serializable]
public class ResponseData
{
    public int statusCode;
    public bool success;
}

public static class SaveGameHelper
{
    public const int DATA_VERSION = 5;
    public const string DEFAULT_WEAPON_ID = "Weapon_01";
    public const string MANUAL_HERO = "MANUAL_HERO";

    #region Default Data

    public static string CreateUniqueID()
    {
        return RandomString(10);
    }

    public static UserData defaultData()
    {
        UserData _data = new UserData();
        _data.DayTrackingData = new DayTrackingValue();
        _data.MetaData = new MetaData();
        _data.MetaData.UserLevel = 0;
        _data.MetaData.UserName = "User_Default";
        _data.MetaData.UserID = FBUtils.RandomString(12);
        _data.MetaData.MacAddress = SystemInfo.deviceUniqueIdentifier;
        if (SocialLogin.instance.IsEnable())
            _data.MetaData.FireBaseID = SocialLogin.instance._firebaseLogin.FirebaseAuthID;
        else
            _data.MetaData.FireBaseID = "";
        _data.MetaData.Revision = 0;
        _data.MetaData.CountLogin = 0;
        _data.MetaData.FirstTimeJoinGame = true;
        _data.MetaData.FirstTimeJoinTimeStamp = TimeService.instance.GetCurrentTimeStamp();

        _data.TutorialData = new TutorialData();
        _data.ManualHeroData = new ManualHeroData();
        _data.ReminderData = new ReminderData();
        _data.Inventory = new UserInventory();
        _data.Inventory.TotalGold = _data.Inventory.TotalPill = _data.Inventory.TotalDiamond = 0;



        //var defaultWeapon = defaultWeaponData("Weapon_01");
        //_data.Inventory.ListWeaponData.Add(defaultWeapon);

        //var defaultHero = defaultHeroData("HERO_1", defaultWeapon.UniqueID);
        //_data.Inventory.ListHeroData.Add(defaultHero);

        _data.GameData = new GameData();
        _data.GameData.CampaignProgress = defaultPlayProgress();
        _data.GameData.IdleProgress = defaultPlayProgress();
        for (int i = 0; i < GameConstant.MAX_HERO_SLOT; i++)
        {
            _data.GameData.TeamSlots.Add(GameConstant.NONE);
        }

        _data.StarData = new StarData();

        _data.ShopData = new ShopData();
        _data.ShopData.HasPurchaseIAP = false;
        _data.ShopData.LastDayResetFreeStuff = -1;
        _data.ShopData.LastReceiveFreeAdsRareChestTime = 0;
        _data.ShopData.LastBuyFreeDiamondPackTime = 0;
        _data.ShopData.LastBuyFreeGoldPackTime = 0;
        _data.ShopData.LastBuyFreeWPCoinPackTime = 0;
        _data.ShopData.LastReceiveFreeAdsLegendaryChestTime = 0;
        _data.ShopData.StackFreeAdsRareChestTime = 0;
        _data.ShopData.StackFreeAdsLegendaryChestTime = 0;
        for (int i = 0; i < 6; i++)
            _data.ShopData.StackChestEquipRank.Add(0);

        _data.LocationPackSave = new LocationPackSave();
        _data.SettingData = new SettingData();
        _data.SettingData.IsBgmOn = _data.SettingData.IsSfxOn = true;
        _data.SettingData.IsFreeStuffNotificationOn = _data.SettingData.IsRareChestNotificationOn =
            _data.SettingData.IsLegendaryChestNotificationOn = true;
        _data.SettingData.Language = LocalizeController.Instance.currentLanguage;

        _data.LastData = _data.Clone();
        return _data;
    }

    public static bool RefreshNewDay(this UserData data)
    {
        bool isNewDay = false;

        var diffDay = TimeService.instance.GetTimeStampBeginDay() - data.MetaData.LastBeginDayTS;
        if (diffDay >= TimeService.DAY_SEC)
        {
            isNewDay = true;
            data.MetaData.LastBeginDayTS = TimeService.instance.GetTimeStampBeginDay();
        }

        if (isNewDay)
        {
            data.DayTrackingData.TodayReviveAds = 0;
            data.DayTrackingData.TodayReviveDiamond = 0;
            data.DayTrackingData.TodayEarnAddonAirDrop = 0;
            data.DayTrackingData.TodayDoubleCompleteLevelReward = 0;
            data.MetaData.DayLogin++;
            AnalyticsManager.instance.LogEvent(
                AnalyticsConstant.getEventName(AnalyticsConstant.ANALYTICS_ENUM.DAY_LOGIN) +
                $"_day_{SaveManager.Instance.Data.MetaData.DayLogin}",
                new LogEventParam("dayLogin", data.MetaData.DayLogin));
        }

        return isNewDay;
    }

    #endregion

    #region UserData

    public static bool HasAvailableHero()
    {
        foreach (var hero in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (hero.UniqueID != GameConstant.MANUAL_HERO)
            {
                if (hero.ItemStatus == ITEM_STATUS.Available)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static TalentData GetTalentData(TalentDesignElement talentDesignElement)
    {
        var talenDatas = SaveManager.Instance.Data.Inventory.ListTalentData.ToList();
        return talenDatas.Find(x => x.TalentID == talentDesignElement.ID);
    }

    public static ITEM_STATUS GetTalentStatus(TalentDesignElement talentDesignElement)
    {
        var talenDatas = SaveManager.Instance.Data.Inventory.ListTalentData;
        if (talenDatas == null)
        {
            return ITEM_STATUS.Locked;
        }

        var data = talenDatas.ToList().Find(x => x.TalentID == talentDesignElement.ID);
        if (data == null)
        {
            return ITEM_STATUS.Locked;
        }
        else
        {
            if (data.TalentLevel == 0)
            {
                return ITEM_STATUS.Disable;
            }

            return ITEM_STATUS.Available;
        }
    }


    public static string GetLastLocationId()
    {
        return DesignHelper.GetLocationIdByLevel(GetMaxCampaignLevel());
    }

    public static float CalcTotalCastleHP(this UserData data)
    {
        float totalHp = 0;

        foreach (var hero in data.GameData.TeamSlots)
        {
            var heroData = data.GetHeroData(hero);
            if (heroData != null)
            {
                totalHp += heroData.FinalPowerData.Hp;
            }
        }

        return totalHp;
    }

    public static int GetCanUpRankHeroCount()
    {
        int count = 0;
        // Hero enough shard
        foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (heroData.UniqueID != GameConstant.MANUAL_HERO && heroData.HasEnoughShardToUpRank())
            {
                count++;
            }
        }

        return count;
    }

    public static bool HasEnoughShardToUpRank(this HeroData heroData)
    {
        var maxRank = DesignHelper.GetHeroMaxRank(heroData.UniqueID);
        bool IsMaxRank = heroData.Rank >= maxRank;

        if (!IsMaxRank)
        {
            var nextRankDs = DesignHelper.GetHeroDesign(heroData.UniqueID, heroData.Rank + 1, heroData.GetHeroLevel());
            if (heroData.CurrentShard >= nextRankDs.ShardRequire)
            {
                return true;
            }
        }

        return false;
    }

    public static float CalcTotalCastleArmour(this UserData data)
    {
        float totalArmour = 0;


        return totalArmour;
    }

    public static bool PurchaseIdleBundle(this UserData data)
    {
        bool result = false;
        for (int i = 0; i < data.Inventory.ListAddOnItems.Count; i++)
        {
            if (data.Inventory.ListAddOnItems[i].ItemID == GameConstant.ADD_ON_SPEED_UP ||
                data.Inventory.ListAddOnItems[i].ItemID == GameConstant.ADD_ON_AUTO_MANUAL_HERO)
            {
                data.Inventory.ListAddOnItems[i].IsUnlimitedItem = true;
                result = true;
            }
        }

        return result;
    }

    #endregion

    #region Inventory

    public static void ActiveNoAds()
    {
        SaveManager.Instance.Data.RemoveAds = true;
        AchieveManager.AchievePurchaseIAP();
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_REMOVE_ADS);
        AdsManager.instance.HideAdsBanner(null);
    }

    public static void SaveBoughFreePack(ShopDesignElement shopDesignElement)
    {
        if (shopDesignElement.isFreePack)
        {
            long currentTime = TimeService.instance.GetCurrentTimeStamp();

            if (shopDesignElement.DiamondReward != 0)
            {
                SaveManager.Instance.Data.ShopData.LastBuyFreeDiamondPackTime = currentTime;
            }
            else if (shopDesignElement.GoldReward != 0)
            {
                SaveManager.Instance.Data.ShopData.LastBuyFreeGoldPackTime = currentTime;
            }
            else if (shopDesignElement.WeaponScrollReward != 0)
            {
                SaveManager.Instance.Data.ShopData.LastBuyFreeWPCoinPackTime = currentTime;
            }
        }
    }

    public static ShopDesignElement ConvertToFreePack(ShopType shopType, ShopDesignElement shopDesignElement)
    {
        ShopDesignElement result = shopDesignElement.Clone();

        result.isFreePack = true;
        result.CostType = CostType.ADS.ToString();
        result.CostValue = 0;

        switch (shopType)
        {
            case ShopType.DIAMOND:
                result.DiamondReward = 50;
                break;
            case ShopType.GOLD:
                break;
            case ShopType.WEAPON_COIN:
                break;
        }

        return result;
    }

    public static bool IsFreePack(ShopDesignElement shopDesignElement)
    {
        if (shopDesignElement.DiamondReward != 0)
        {
            return IsFreePack(ShopType.DIAMOND, shopDesignElement);
        }

        if (shopDesignElement.GoldReward != 0)
        {
            return IsFreePack(ShopType.GOLD, shopDesignElement);
        }

        if (shopDesignElement.WeaponScrollReward != 0)
        {
            return IsFreePack(ShopType.WEAPON_COIN, shopDesignElement);
        }

        return false;
    }

    public static bool IsFreePack(ShopType shopType, ShopDesignElement shopDesignElement)
    {
        if (!shopDesignElement.Id.Contains("_1"))
        {
            return false;
        }

        long now = TimeService.instance.GetCurrentTimeStamp(true);
        long lastReceiveTimeStamp;
        TimeSpan timeSpan;

        switch (shopType)
        {
            case ShopType.DIAMOND:
                lastReceiveTimeStamp = SaveManager.Instance.Data.ShopData.LastBuyFreeDiamondPackTime;
                break;
            case ShopType.GOLD:
                lastReceiveTimeStamp = SaveManager.Instance.Data.ShopData.LastBuyFreeGoldPackTime;
                break;
            case ShopType.WEAPON_COIN:
                lastReceiveTimeStamp = SaveManager.Instance.Data.ShopData.LastBuyFreeWPCoinPackTime;
                break;

            default:
                return false;
        }

        timeSpan = TimeSpan.FromSeconds(now - lastReceiveTimeStamp);
        return timeSpan.TotalHours > 24;
    }

    public static bool IsMaxDailyAddOnAirDrop()
    {
        var airdropDesign = DesignHelper.GetSkillDesign(GameConstant.ADD_ON_AIR_DROP);
        return SaveManager.Instance.Data.DayTrackingData.TodayEarnAddonAirDrop >= airdropDesign.Number;
    }

    public static bool AddHeroScroll(this UserData data, string heroId, long value, bool showUnlocked = true)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        HeroData heroData = data.GetHeroData(heroId);
        result = heroData.AddShardHero((int)value, showUnlocked);
        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool AddArmourScroll(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.ARMOUR_SCROLL, value);

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool AddWeaponScroll(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.WEAPON_SCROLL, value);

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    //gold
    public static bool AddGold(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.GOLD, value);
        // data.Inventory.TotalGold += value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool DecreaseGold(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        data.Inventory.TotalGold -= value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    //diamond
    public static bool AddDiamond(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.DIAMOND, value);
        // data.Inventory.TotalDiamond += value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool DecreaseDiamond(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.DIAMOND, -value);
        // data.Inventory.TotalDiamond += value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    //Tokens
    public static bool AddTokens(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.TOKEN, value);
        // data.Inventory.TotalDiamond += value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool DecreaseTokens(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.TOKEN, -value);
        // data.Inventory.TotalDiamond += value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    //pill
    public static bool AddPill(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.PILL, value);
        // data.Inventory.TotalDiamond += value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool DecreasePill(this UserData data, long value)
    {
        bool result = true;
        value = (long)Mathf.Abs(value);
        CurrencyModels.instance.AddCurrency(CurrencyType.PILL, -value);
        // data.Inventory.TotalDiamond += value;

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    //basic add rewards
    public static bool AddReward(this UserData data, RewardData reward)
    {
        bool success = false;

        CurrencyType currencyEvent = CurrencyType.NONE;

        switch (reward._type)
        {
            case REWARD_TYPE.GOLD:
                success = data.AddGold(reward._value);
                currencyEvent = CurrencyType.GOLD;
                break;
            case REWARD_TYPE.DIAMOND:
                success = data.AddDiamond(reward._value);
                currencyEvent = CurrencyType.DIAMOND;
                break;
            case REWARD_TYPE.TOKEN:
                success = data.AddTokens(reward._value);
                currencyEvent = CurrencyType.TOKEN;
                break;
            case REWARD_TYPE.PILL:
                success = data.AddPill(reward._value);
                currencyEvent = CurrencyType.PILL;
                break;
            case REWARD_TYPE.SCROLL_ARMOUR:
                success = data.AddArmourScroll(reward._value);
                currencyEvent = CurrencyType.ARMOUR_SCROLL;
                break;
            case REWARD_TYPE.SCROLL_HERO:
                success = data.AddHeroScroll((string)reward._extends, reward._value);
                break;
            case REWARD_TYPE.SCROLL_WEAPON:
                success = data.AddWeaponScroll(reward._value);
                currencyEvent = CurrencyType.WEAPON_SCROLL;
                break;
            case REWARD_TYPE.CHEST:
                Debug.LogError("AddRewards error!!! logic add must be done!!!!");
                success = false;
                break;
            case REWARD_TYPE.KEY_CHEST_RARE:
                success = data.AddRareKey((int)reward._value);
                break;
            case REWARD_TYPE.KEY_CHEST_LEGENDARY:
                success = data.AddLegendaryKey((int)reward._value);
                break;
            case REWARD_TYPE.KEY_CHEST_HERO:
                success = data.AddHeroChestKey((int)reward._value);
                break;
            case REWARD_TYPE.REVIVE:
                success = false;
                Debug.LogError("Revive not define yet");
                break;
            case REWARD_TYPE.EQUIP:
            case REWARD_TYPE.RANDOM_EQUIP:
                Debug.LogError(reward._extends);
                //WeaponData wData = (WeaponData)reward._extends;
                var type = reward._extends.GetType();
                WeaponData wData = null;
                if (type.Equals(typeof(WeaponData)))
                {
                    wData = (WeaponData)reward._extends;
                }
                else if (type.Equals(typeof(string)))
                {
                    var equipReward = DesignHelper.FormatEquipRewardStr((string)reward._extends);
                    wData = defaultWeaponData(equipReward.Key, equipReward.Value);
                }

                success = wData != null ? data.AddWeapon(wData) : false;
                break;
            case REWARD_TYPE.REFILL_HP:
                GamePlayController.instance?.RefillHPGamePlay(reward._value);
                break;
            case REWARD_TYPE.ADD_ON:
                data.AddAddONItem((string)reward._extends, (int)reward._value);
                success = true;
                break;
            default:
                break;
        }

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY, currencyEvent);

        return success;
    }

    public static bool AddRewards(this UserData data, List<RewardData> listReward)
    {
        bool result = false;
        foreach (var item in listReward)
        {
            result = data.AddReward(item);
        }

        return result;
    }

    public static bool HasWeapon(this HeroData data)
    {
        return data.EquippedWeapon != "";
    }

    public static bool HasArmour(this HeroData data)
    {
        return data.EquippedArmour != "";
    }

    public static List<RewardData> AddIdleRewards(this UserData data, CustomBattleReward rewardData)
    {
        List<RewardData> result = new List<RewardData>();

        var taskDone = false;
        foreach (var rwd in rewardData.Rewards)
        {
            var rwData = new RewardData(rwd.RewardId, rwd.Value);
            taskDone = data.AddReward(rwData);
            if (taskDone)
                result.Add(rwData);
        }

        if (taskDone)
        {
            var task = data.SetEarnedIdleReward(rewardData);
            if (!task)
            {
                Debug.LogError("Earned idle reward error!!!");
            }

            SaveManager.Instance.SetDataDirty();
        }

        return result;
    }

    public static bool SetEarnedIdleReward(this UserData data, CustomBattleReward rewardData)
    {
        bool result = false;
        var idleProgress = data.GetPlayProgress(GameMode.IDLE_MODE);
        //check init
        var lastCollect =
            data.Inventory.ListClaimedIdleRewards.FirstOrDefault(x => x.LevelReward == rewardData.LevelReward);
        if (lastCollect != null)
        {
            foreach (var item in rewardData.Rewards)
            {
                var exists = lastCollect.ListEarnedReward.FirstOrDefault(x => x.RewardID == item.RewardId);
                if (exists != null)
                {
                    ++exists.CountEarned;
                }
                else
                {
                    lastCollect.ListEarnedReward.Add(new RewardTracker()
                    {
                        RewardID = item.RewardId,
                        CountEarned = 1
                    });
                }
            }

            result = true;
        }
        else
        {
            IdleRewardTracker idleTrack = new IdleRewardTracker();
            idleTrack.LevelReward = rewardData.LevelReward;
            idleTrack.SeasonNum = idleProgress.SeasonNum;
            foreach (var item in rewardData.Rewards)
            {
                idleTrack.ListEarnedReward.Add(new RewardTracker()
                {
                    RewardID = item.RewardId,
                    CountEarned = 1
                });
            }

            data.Inventory.ListClaimedIdleRewards.Add(idleTrack);

            result = true;
        }


        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool CheckIsClaimedRewardIdleMode(this UserData data, PlayProgress idleProgress,
        CustomBattleReward rewardData)
    {
        bool isClaimed = false;

        var exists = data.Inventory.ListClaimedIdleRewards.FirstOrDefault(x => x.LevelReward == rewardData.LevelReward);
        isClaimed = exists != null && exists.SeasonNum == idleProgress.SeasonNum;

        return isClaimed;
    }

    //public static bool AddCampaignRewards(this UserData data, CustomBattleReward rewardData)
    //{
    //    bool success = false;
    //    var taskAllDone = false;

    //    //check if in custom level reward;

    //    foreach (var rwd in rewardData.Rewards)
    //    {
    //        taskAllDone = data.AddReward(new RewardData(rwd.RewardId, rwd.Value));
    //    }

    //    //check custom reward


    //    SaveManager.Instance.SetDataDirty();
    //    success = true;

    //    return success;
    //}

    public static Tuple<List<Reward>, List<Reward>> AddCampaignRewards(this UserData data, int finishedLevel,
        int totalStars)
    {
        List<Reward> listRewards = new List<Reward>();
        List<Reward> listBonusRewards = new List<Reward>();

        var taskSuccess = false;

        //base design
        var baseRewardDesign = DesignHelper.GetBaseLevelRewardDesign(finishedLevel);
        if (baseRewardDesign != null)
        {
            foreach (var rwd in baseRewardDesign.Rewards)
            {
                //var offsetLevel = (finishedLevel - baseRewardDesign.StartLevel);
                var rwdValue =
                    (long)(rwd.BaseValue + (finishedLevel - baseRewardDesign.StartLevel) * rwd.RewardMultiply);
                taskSuccess = data.AddReward(new RewardData(rwd.RewardId, rwdValue));
                if (taskSuccess)
                    listRewards.Add(new Reward()
                    {
                        RewardId = rwd.RewardId,
                        Value = rwdValue
                    });
            }

            //angel exp
            long expToAdd =
                (long)(baseRewardDesign.BaseExpAngel + baseRewardDesign.AngelExpMultiplier * finishedLevel);
            data.AddExpManualHero(expToAdd);
        }

        //custom normal rewards
        RewardData diamondRwdData = null;
        Reward diamondRwd = null;

        var customReward = DesignHelper.GetCustomLevelRewardDesign(finishedLevel);
        if (customReward != null)
        {
            var customNormal = $"{GameConstant.CAMPAIGN_REWARD_PREFIX}{finishedLevel}";
            //check collected one time Reward
            if (!data.CheckClaimedOneTimeReward(customNormal))
            {
                foreach (var rwd in customReward.Rewards)
                {
                    if (DesignHelper.ConvertToRewardType(rwd.RewardId) == REWARD_TYPE.DIAMOND)
                    {
                        continue;
                    }

                    taskSuccess = data.AddReward(new RewardData(rwd.RewardId, rwd.Value));
                    if (taskSuccess)
                    {
                        listBonusRewards.Add(rwd);
                    }
                }

                if (taskSuccess)
                {
                    data.SetClaimedOneTimeRewards(customNormal);
                }
            }
        }

        //custom 3stars rewards
        if (totalStars >= 3)
        {
            foreach (var rwd in customReward.Rewards)
            {
                if (DesignHelper.ConvertToRewardType(rwd.RewardId) == REWARD_TYPE.DIAMOND)
                {
                    if (diamondRwdData == null)
                    {
                        diamondRwdData = new RewardData(rwd.RewardId, rwd.Value);
                        diamondRwd = rwd;
                    }
                    else
                    {
                        diamondRwdData._value += rwd.Value;
                        diamondRwd.Value += rwd.Value;
                    }

                    continue;
                }
            }

            if (diamondRwdData != null && diamondRwd != null)
            {
                var custom3Stars = $"{GameConstant.CAMPAIGN_REWARD_PREFIX}{finishedLevel}3s";
                //check collected one time Reward
                if (!data.CheckClaimedOneTimeReward(custom3Stars))
                {
                    taskSuccess = data.AddReward(diamondRwdData);
                    if (taskSuccess)
                    {
                        listBonusRewards.Add(diamondRwd);
                    }

                    if (taskSuccess)
                    {
                        data.SetClaimedOneTimeRewards(custom3Stars);
                    }
                }
            }
        }

        return new Tuple<List<Reward>, List<Reward>>(listRewards, listBonusRewards);
    }

    public static Tuple<List<Reward>, List<Reward>> GetCampaignRewards(this UserData data, int finishedLevel,
        int totalStars)
    {
        List<Reward> listRewards = new List<Reward>();
        List<Reward> listBonusRewards = new List<Reward>();
        //base design
        var baseRewardDesign = DesignHelper.GetBaseLevelRewardDesign(finishedLevel);
        if (baseRewardDesign != null)
        {
            foreach (var rwd in baseRewardDesign.Rewards)
            {
                //var offsetLevel = (finishedLevel - baseRewardDesign.StartLevel);
                var rwdValue =
                    (long)(rwd.BaseValue + (finishedLevel - baseRewardDesign.StartLevel) * rwd.RewardMultiply);
                listRewards.Add(new Reward()
                {
                    RewardId = rwd.RewardId,
                    Value = rwdValue
                });
            }
        }

        //custom normal rewards
        RewardData diamondRwdData = null;
        Reward diamondRwd = null;

        var customReward = DesignHelper.GetCustomLevelRewardDesign(finishedLevel);
        if (customReward != null)
        {
            var customNormal = $"{GameConstant.CAMPAIGN_REWARD_PREFIX}{finishedLevel}";
            //check collected one time Reward
            if (!data.CheckClaimedOneTimeReward(customNormal))
            {
                foreach (var rwd in customReward.Rewards)
                {
                    if (DesignHelper.ConvertToRewardType(rwd.RewardId) == REWARD_TYPE.DIAMOND)
                    {
                        continue;
                    }

                    listBonusRewards.Add(rwd);
                }
            }
        }

        //custom 3stars rewards
        if (totalStars >= 3)
        {
            foreach (var rwd in customReward.Rewards)
            {
                if (DesignHelper.ConvertToRewardType(rwd.RewardId) == REWARD_TYPE.DIAMOND)
                {
                    if (diamondRwdData == null)
                    {
                        diamondRwdData = new RewardData(rwd.RewardId, rwd.Value);
                        diamondRwd = rwd;
                    }
                    else
                    {
                        diamondRwdData._value += rwd.Value;
                        diamondRwd.Value += rwd.Value;
                    }
                }
            }

            if (diamondRwdData != null && diamondRwd != null)
            {
                var custom3Stars = $"{GameConstant.CAMPAIGN_REWARD_PREFIX}{finishedLevel}3s";
                //check collected one time Reward
                if (!data.CheckClaimedOneTimeReward(custom3Stars))
                {
                    listBonusRewards.Add(diamondRwd);
                }
            }
        }

        return new Tuple<List<Reward>, List<Reward>>(listRewards, listBonusRewards);
    }

    public static bool SetClaimedOneTimeRewards(this UserData data, string rewardID)
    {
        var exists = data.Inventory.ListClaimedOneTimeRewards.FirstOrDefault(x => x.RewardID == rewardID);
        if (exists != null)
        {
            Debug.LogError($"SetClaimedOneTimeRewards failed! already claimed {rewardID}");
            return false;
        }
        else
        {
            data.Inventory.ListClaimedOneTimeRewards.Add(new RewardTracker()
            {
                RewardID = rewardID,
                EarnedTS = TimeService.instance.GetCurrentTimeStamp()
            });
        }


        return true;
    }


    public static List<RewardData> MergeReward(List<RewardData> listA, List<RewardData> listB)
    {
        List<RewardData> result = new List<RewardData>();

        foreach (var item in listA)
        {
            result.Add(new RewardData(item));
        }

        foreach (var item in listB)
        {
            var exists = result.FirstOrDefault(x => x._type == item._type && x._extends == item._extends);
            if (exists != null)
            {
                exists._value += item._value;
            }
            else
            {
                result.Add(item);
            }
        }

        return result;
    }

    //other inventory
    public static void ResetTalenData(this TalentData talentData)
    {
        var talentDesign = DesignHelper.GetTalentDesign(talentData);
        talentData.TalentValue = talentDesign.GetValue(talentData.TalentLevel);
    }

    public static bool AddHero(this UserData data, string heroID)
    {
        bool result = true;
        var exists = data.GetHeroData(heroID);
        if (exists == null)
        {
            var ds = DesignHelper.GetHeroDesign(heroID, 1, 1);
            var weaponData = defaultWeaponData(ds.DefaultWeapon);
            data.Inventory.ListWeaponData.Add(weaponData);
            var heroData = defaultHeroData(heroID, weaponData.UniqueID);
            heroData.EquippedWeapon = weaponData.UniqueID;
            weaponData.ItemStatus = ITEM_STATUS.Choosing;
            heroData.ResetPowerData();
            data.Inventory.ListHeroData.Add(heroData);
            heroData.ResetHeroUltimates(ds);
            if (heroData.UniqueID == "HERO_5")
            {
                heroData.Rank = 3;
                heroData.ResetPowerData();
            }

            if (heroData.UniqueID == "HERO_3")
            {
                heroData.Rank = 2;
                heroData.ResetPowerData();
            }

            var tempHeroData = heroData.Clone();
            tempHeroData.ItemStatus = ITEM_STATUS.Locked;
            SaveManager.Instance.Data.LastData.Inventory.ListHeroData.Add(tempHeroData);

            if (!SaveManager.Instance.Data.LastData.Inventory.ListWeaponData.Contains(weaponData))
                SaveManager.Instance.Data.LastData.Inventory.ListWeaponData.Add(weaponData);
        }
        else
        {
            Debug.LogError($"AddHero failed !!!!");
            result = false;
        }

        return result;
    }

    public static bool AddWeapon(this UserData data, WeaponData weapon)
    {
        bool result = true;
        data.Inventory.ListWeaponData.Add(weapon);

        return result;
    }

    public static bool AddChest(this UserData data, ChestData chest)
    {
        bool result = true;
        data.Inventory.ListChestData.Add(chest);
        return result;
    }

    public static bool AddRareKey(this UserData data, int value)
    {
        data.Inventory.TotalRareKey += value;
        return true;
    }

    public static bool AddLegendaryKey(this UserData data, int value)
    {
        data.Inventory.TotalLegendaryKey += value;
        return true;
    }

    public static bool AddHeroChestKey(this UserData data, int value)
    {
        data.Inventory.TotalHeroChestKey += value;
        return true;
    }

    public static bool CheckClaimedOneTimeReward(this UserData data, string rewardID)
    {
        var checkExists = data.Inventory.ListClaimedOneTimeRewards.FirstOrDefault(x => x.RewardID == rewardID);
        return checkExists != null;
    }

    public static AddOnItem AddAddONItem(this UserData data, string itemID, int value,
        ITEM_STATUS status = ITEM_STATUS.Available, long expiredDuration = -1)
    {
        AddOnItem result = null;

        result = data.Inventory.ListAddOnItems.FirstOrDefault(x => x.ItemID == itemID);
        if (result == null)
        {
            result = new AddOnItem()
            {
                ItemID = itemID,
                ItemCount = value,
                Status = status,
                ExpiredDuration = expiredDuration
            };
            data.Inventory.ListAddOnItems.Add(result);

            var tempResult = result.Clone();
            tempResult.ItemCount = 0;
            SaveManager.Instance.Data.LastData.Inventory.ListAddOnItems.Add(tempResult);
        }
        else
        {
            result.ItemCount += Mathf.Abs(value);
            result.Status = status;
        }

        //FOR NON CONSUMABLE ADDON ITEM
        if (itemID == GameConstant.ADD_ON_AIR_DROP)
        {
            result.ItemCount = -999;
        }

        SaveManager.Instance.SetDataDirty();

        return result;
    }

    public static AddOnItem GetAddOnItem(this UserData data, string itemID)
    {
        AddOnItem result = null;

        result = data.Inventory.ListAddOnItems.FirstOrDefault(x => x.ItemID == itemID);
        if (result == null && itemID.Contains("ADD_ON"))
        {
            var _design = DesignHelper.GetSkillDesign(itemID);
            if (_design != null)
                result = SaveManager.Instance.Data.AddAddONItem(itemID, 0, ITEM_STATUS.Available, (long)_design.Value);
        }

        return result;
    }

    public static void ResetAddOnItem(this UserData data, string itemID)
    {
        var item = data.GetAddOnItem(itemID);
        item.Status = ITEM_STATUS.Available;
    }

    public static bool ConsumeAddOnItem(this UserData data, string itemID, int numConsume = 1,
        bool forceConsume = false)
    {
        bool result = false;

        var item = data.GetAddOnItem(itemID);
        int num = Math.Abs(numConsume);
        if (item.IsUnlimitedItem)
        {
            result = true;
        }
        else if (item != null && item.IsConsumableAddOn() && item.ItemCount >= num)
        {
            if (!item.IsSpecialAddOn() && (forceConsume || item.Status == ITEM_STATUS.Available ||
                                           item.Status == ITEM_STATUS.Choosing))
            {
                item.ItemCount -= num;
                if (item.ItemCount <= 0)
                {
                    if (item.Status != ITEM_STATUS.Choosing)
                        item.Status = ITEM_STATUS.Disable;
                }
            }
        }

        SaveManager.Instance.SetDataDirty();
        return result;
    }

    public static bool IsConsumableAddOn(this AddOnItem item)
    {
        if (item != null && item.ItemCount <= -999)
            return false;
        return true;
    }

    public static bool IsSpecialAddOn(this AddOnItem item)
    {
        if (item != null && (item.ItemID == GameConstant.ADD_ON_SPEED_UP ||
                             item.ItemID == GameConstant.ADD_ON_AUTO_MANUAL_HERO ||
                             item.ItemID == GameConstant.ADD_ON_AIR_DROP))
            return true;
        return false;
    }

    #endregion

    #region Power Data

    public static PowerData defaultPowerData(float hp, float armour, float dmg, float firerate, float hsPercent,
        float range, float critPercent, float percentDmg = 0f)
    {
        PowerData data = new PowerData()
        {
            Hp = hp,
            Armour = armour,
            Dmg = dmg,
            Firerate = firerate,
            HeadshotPercent = hsPercent,
            Range = range,
            CritPercent = critPercent,
            PercentDmg = percentDmg
        };

        return data;
    }

    // return a - b
    public static PowerData MinusPowerData(PowerData a, PowerData b)
    {
        if (a == null && b != null)
            return new PowerData(b);
        else if (b == null && a != null)
            return new PowerData(a);
        else if (a == null && b == null)
            return null;

        PowerData result = new PowerData(a);
        result.Dmg -= b.Dmg;
        result.HeadshotPercent -= b.HeadshotPercent;
        result.Hp -= b.Hp;
        result.Armour -= b.Armour;
        result.Firerate -= b.Firerate;
        result.Range -= b.Range;
        result.CritPercent -= b.CritPercent;
        result.PercentDmg -= b.PercentDmg;

        return result;
    }

    public static PowerData AddPowerData(PowerData a, PowerData b)
    {
        if (a == null && b != null)
            return new PowerData(b);
        else if (b == null && a != null)
            return new PowerData(a);
        else if (a == null && b == null)
            return null;

        PowerData result = new PowerData(a);
        result.Dmg += b.Dmg;
        result.HeadshotPercent += b.HeadshotPercent;
        result.Hp += b.Hp;
        result.Armour += b.Armour;
        result.Firerate += b.Firerate;
        result.Range += b.Range;
        result.CritPercent += b.CritPercent;
        result.PercentDmg += b.PercentDmg;
        result.ReduceSkillCountDownPercent += b.ReduceSkillCountDownPercent;

        return result;
    }

    public static PowerData GetPowerDataByGameMode(this HeroData data)
    {
        data.ResetPowerData();
        PowerData result = data.FinalPowerData;

        if (GameMaster.instance.currentMode == GameMode.IDLE_MODE)
            result = AddPowerData(data.FinalPowerData, data.IdleUpradedPower);


        return result;
    }

    public static PowerData GetPowerDataByGameMode(this HeroData data, GameMode mode)
    {
        data.ResetPowerData();
        PowerData result = data.FinalPowerData;

        if (mode == GameMode.IDLE_MODE)
            result = AddPowerData(data.FinalPowerData, data.IdleUpradedPower);
        return result;
    }

    public static float CalcDPS(this PowerData data)
    {
        return data.Dmg * data.Firerate;
    }

    public static float CalcCurrentTeamDPS(this UserData data, GameMode mode)
    {
        float dps = 0f;

        foreach (var hero in data.GameData.TeamSlots)
        {
            var heroData = data.GetHeroData(hero);
            if (heroData != null)
            {
                var power = heroData.GetPowerDataByGameMode(mode);
                dps += power.CalcDPS();
            }
        }

        return dps;
    }

    #endregion

    #region Hero Data

    public static bool IsUnlocked(this HeroData data)
    {
        return data.ItemStatus == ITEM_STATUS.Choosing || data.ItemStatus == ITEM_STATUS.Available;
    }

    public static HeroData defaultHeroData(string heroID, string equippedWeaponID)
    {
        HeroData data = new HeroData();

        data.UniqueID = heroID;
        data.Rank = 1;

        data.IdleRank = 1;
        data.IdleLevel = 1;

        data.BaseHeroPower = new PowerData();
        //data.LevelUpHero(1);
        data.SetHeroLevel(1);

        data.ItemStatus = ITEM_STATUS.Locked;
        data.EquippedWeapon = equippedWeaponID;

        return data;
    }

    public static bool UnlockHero(this HeroData data)
    {
        bool result = false;
        if (data.ItemStatus == ITEM_STATUS.Locked)
        {
            data.ItemStatus = ITEM_STATUS.Available;
            result = true;
        }

        return result;
    }

    public static bool AddTeamMember(this UserData data, string heroID)
    {
        bool success = false;
        var heroData = data.GetHeroData(heroID);

        var exists = data.GameData.TeamSlots.Contains(heroID);
        if (!exists && heroData != null && heroData.ItemStatus == ITEM_STATUS.Available)
        {
            data.GameData.TeamSlots.Add(heroID);
            heroData.ItemStatus = ITEM_STATUS.Choosing;

            success = true;
        }

        return success;
    }

    public static bool ApplyFreeSlotMember(this UserData data, string heroID)
    {
        bool success = false;
        var heroData = data.GetHeroData(heroID);

        var exists = data.GameData.TeamSlots.Contains(heroID);
        if (!exists && heroData != null && heroData.ItemStatus == ITEM_STATUS.Available)
        {
            for (int i = 0; i < data.GameData.TeamSlots.Count; i++)
            {
                if (data.GameData.TeamSlots[i] == GameConstant.NONE)
                {
                    data.GameData.TeamSlots[i] = heroID;
                    heroData.ItemStatus = ITEM_STATUS.Choosing;
                    success = true;
                    break;
                }
            }
        }

        return success;
    }

    public static bool FreeAllSlotMember(this UserData data)
    {
        bool result = true;

        for (int i = 0; i < data.GameData.TeamSlots.Count; i++)
        {
            data.GameData.TeamSlots[i] = GameConstant.NONE;
        }

        foreach (var hero in data.Inventory.ListHeroData)
        {
            if (hero.ItemStatus == ITEM_STATUS.Choosing)
                hero.ItemStatus = ITEM_STATUS.Available;
        }


        return result;
    }

    public static HeroData GetHeroData(this UserData data, string hero_id)
    {
        if (hero_id == "NONE")
            return null;

        var result = data.Inventory.ListHeroData.FirstOrDefault(x => x.UniqueID == hero_id);
        return result;
    }

    public static bool EquipWeapon(this HeroData hero, string weaponUID)
    {
        bool result = false;
        var findWeapon =
            SaveManager.Instance.Data.Inventory.ListWeaponData.FirstOrDefault(x => x.UniqueID == weaponUID);
        if (findWeapon != null)
        {
            hero.EquippedWeapon = weaponUID;
        }
        else
        {
            Debug.LogError($"Cant find weapon with {weaponUID}");
        }

        return result;
    }

    public static bool LevelUpHero(this HeroData hero)
    {
        int nextLevel = hero.GetHeroLevel() + 1;
        return hero.LevelUpHero(nextLevel);
    }

    public static bool LevelUpHero(this HeroData hero, int targetLevel)
    {
        bool result = false;
        var exists = DesignManager.instance.DictHeroDesign.ContainsKey(hero.UniqueID);
        if (exists)
        {
            var ds = DesignManager.instance.DictHeroDesign[hero.UniqueID].FirstOrDefault(x => x.Rarity == hero.Rank);
            if (ds != null)
            {
                if (targetLevel > ds.MaxLevel)
                {
                    Debug.LogError($"max level !!! check againg!!!");
                    return false;
                }

                hero.SetHeroLevel(targetLevel);
                hero.ResetPowerData();
                MissionManager.Instance.TriggerMission(MissionType.UPGRADE_HERO, hero.UniqueID);
                return true;
            }
        }
        else
        {
            Debug.LogError($"Could not find design {hero.UniqueID}");
        }

        return result;
    }

    public static bool LevelUpHeroIdleMode(this HeroData hero, int targetLevel)
    {
        bool result = false;
        var exists = DesignManager.instance.DictIdleHeroDesign.ContainsKey(hero.UniqueID);
        if (exists)
        {
            var ds = DesignManager.instance.DictIdleHeroDesign[hero.UniqueID]
                .LastOrDefault(x => targetLevel >= x.StartLevel);
            if (ds != null)
            {
                if (targetLevel > ds.MaxLevel)
                {
                    Debug.LogError($"max Idle level !!! check Idle Hero Design again!!!");
                    return false;
                }

                hero.IdleLevel = targetLevel;
                hero.ResetPowerData();

                return true;
            }
        }
        else
        {
            Debug.LogError($"Could not find design {hero.UniqueID}");
        }

        return result;
    }

    public static void ResetPowerData(this HeroData data)
    {
        //TO DO: CALC POWER DATA OF HERO!!!!
        // if (data.UniqueID == GameConstant.MANUAL_HERO)
        // {
        //     // Update level of manual hero to calculate power
        //     data.Level = SaveManager.Instance.Data.ManualHeroData.LevelPotion;
        // }
        var heroFinalPower = CalculatePowerHeroData(data);
        // Debug.LogError("BASE " + heroFinalPower);
        PowerData weaponPower = CalculateBonusFromWeapon(data);
        // Debug.LogError("WEAPON " + weaponPower);
        heroFinalPower = AddPowerData(heroFinalPower, weaponPower);
        // Debug.LogError("FINAL " + heroFinalPower);

        var talentPower = data.CalculateTalentPowerData();
        heroFinalPower = AddPowerData(heroFinalPower, talentPower);

        data.FinalPowerData = heroFinalPower;

        //manual bonus data
        if (data.UniqueID == GameConstant.MANUAL_HERO)
        {
            var manualDs = DesignHelper.GetManualHeroDesign(data.Rank);
            if (manualDs != null)
            {
                var campaignProgress = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE);
                data.FinalPowerData.Dmg += manualDs.BonusDmgPerLevel * campaignProgress.MaxLevel;
            }
        }

        //IdlePower
        data.ResetIdlePowerData();

        //Hero Hit Effects
        SaveManager.Instance.SetDataDirty();
    }

    public static PowerData CalculatePowerHeroData(this HeroData data)
    {
        var findDesign = DesignManager.instance.DictHeroDesign.ContainsKey(data.UniqueID);
        if (!findDesign)
        {
            Debug.LogError($"Cant find design with {data.UniqueID}");
            return null;
        }

        var allRankDesign = DesignManager.instance.DictHeroDesign[data.UniqueID];
        var ds = allRankDesign.FirstOrDefault(x => x.Rarity == data.Rank);
        var DefaultPower = defaultPowerData(ds.Hp, ds.Armour, ds.Dmg, (float)ds.FireRate, ds.HeadshotPercent,
            ds.Range, ds.CritPercent);

        var lvl = data.GetHeroLevel() - 1;
        var UpgradedPower = defaultPowerData(lvl * ds.HpUprade, lvl * ds.AmourUpgrade, lvl * ds.DmgUprade,
            lvl * ds.FireRateUprade, lvl * ds.HeadshotPercentUpgrade, lvl * ds.RangeUprade,
            lvl * ds.CritPercentUpgrade);

        data.BaseHeroPower = AddPowerData(DefaultPower, UpgradedPower);

        //RarityDmgMultiply
        data.BaseHeroPower.Dmg += data.BaseHeroPower.Dmg * ds.RarityDmgMultiply * 1.0f / 100f;

        return data.BaseHeroPower;
    }

    public static PowerData CalculateTalentPowerData(this HeroData data)
    {
        var talentAttributes = TalentManager.GetAttributeValue();
        var talentPower = AddAttributePowerData(data, talentAttributes);
        return talentPower;
    }

    public static PowerData CalculatePowerBaseAndTalentHeroData(this HeroData data)
    {
        var total = AddPowerData(data.CalculatePowerHeroData(), data.CalculateTalentPowerData());
        return total;
    }

    public static PowerData CalculateBonusFromWeapon(this HeroData data)
    {
        var defaultPower = CalculatePowerHeroData(data);

        PowerData weaponPower = null;

        if (data.EquippedWeapon != "")
        {
            var wData = SaveManager.Instance.Data.GetWeaponData(data.EquippedWeapon);
            if (wData != null)
            {
                var weaponDesign = DesignHelper.GetWeaponDesign(wData);
                if (weaponDesign != null)
                {
                    var listAttributes = weaponDesign.GetListEffectSkillDesigns();
                    if (listAttributes != null)
                    {
                        data.ListWeaponAttributes.Clear();
                        foreach (var attr in listAttributes)
                        {
                            data.ListWeaponAttributes.Add(new AttributeData()
                            {
                                AttributeID = attr.Item1,
                                AttributeValue = (float)attr.Item2
                            });
                        }
                    }
                }

                wData.ResetPowerData();
                weaponPower = wData.FinalPowerData;
            }
        }

        // if (data.EquippedArmour != "")
        // {
        //     var wData = SaveManager.Instance.Data.GetWeaponData(data.EquippedArmour);
        //     if (wData != null)
        //     {
        //         wData.ResetPowerData();
        //         weaponPower = AddPowerData(weaponPower, wData.FinalPowerData);
        //         weaponPower = AddPowerData(weaponPower, wData.CalculatePowerFromSpecialAttribute());
        //
        //         var weaponDesign = DesignHelper.GetWeaponDesign(wData);
        //         if (weaponDesign != null)
        //         {
        //             var listAttributes = weaponDesign.GetListEffectSkillDesigns();
        //             if (listAttributes != null)
        //             {
        //                 data.ListWeaponAttributes.Clear();
        //                 foreach (var attr in listAttributes)
        //                 {
        //                     data.ListWeaponAttributes.Add(new AttributeData()
        //                     {
        //                         AttributeID = attr.Item1,
        //                         AttributeValue = (float) attr.Item2
        //                     });
        //                 }
        //             }
        //         }
        //     }
        // }

        PowerData result = new PowerData();
        if (weaponPower != null)
        {
            var dmgMultiply = (weaponPower.PercentDmg * 1.0f / 100f);
            result.Dmg = defaultPower.Dmg * dmgMultiply;
            result.Armour = weaponPower.Armour;
            result.CritPercent = weaponPower.CritPercent;
            result.HeadshotPercent = weaponPower.HeadshotPercent;
            result.Hp = weaponPower.Hp;
            result.Range = weaponPower.Range;
            result.Firerate = weaponPower.Firerate;
            result.ReduceSkillCountDownPercent = weaponPower.ReduceSkillCountDownPercent;
        }

        return result;
    }

    public static WeaponData GetEquippedWeapon(this HeroData heroData)
    {
        var wpData = SaveManager.Instance.Data.GetWeaponData(heroData.EquippedWeapon);
        return wpData;
    }

    public static PowerData CalculatePowerFromSpecialAttribute(this WeaponData wData)
    {
        var wDesign = DesignHelper.GetWeaponDesign(wData);
        var specialAttrtibute = wDesign.GetListSpecialAttributeDesigns();
        var weaponPower = new PowerData();

        foreach (var attribute in specialAttrtibute)
        {
            string effectType = attribute.Item1;
            float value = (float)attribute.Item2;

            if (effectType.Contains(EffectType.PASSIVE_INCREASE_DMG.ToString()))
            {
                weaponPower.Dmg += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_INCREASE_PERCENT_DMG.ToString()))
            {
                weaponPower.PercentDmg += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_INCREASE_ARMOUR.ToString()))
            {
                weaponPower.Armour += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_INCREASE_CRIT.ToString()))
            {
                weaponPower.CritPercent += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT.ToString()))
            {
                weaponPower.HeadshotPercent += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_INCREASE_HP.ToString()))
            {
                weaponPower.Hp += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_INCREASE_RANGE.ToString()))
            {
                weaponPower.Range += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_INCREASE_FIRERATE.ToString()))
            {
                weaponPower.Firerate += value;
            }
            else if (effectType.Contains(EffectType.PASSIVE_REDUCE_COUNTDOWN_ULTI.ToString()))
            {
                weaponPower.ReduceSkillCountDownPercent += value;
            }
        }

        return weaponPower;
    }

    /// <summary>
    /// To Calc Idle Upgrade data
    /// </summary>
    /// <param name="data"></param>
    public static void ResetIdlePowerData(this HeroData data)
    {
        var ds = DesignHelper.GetIdleHeroDesign(data.UniqueID, data.IdleLevel);
        if (ds != null)
        {
            // Skip calculate of manual hero
            if (data.UniqueID == GameConstant.MANUAL_HERO)
            {
                data.IdleUpradedPower = defaultPowerData(0, 0, 0, 0, 0, 0, 0);
            }
            else
            {
                var lvl = data.IdleLevel - ds.StartLevel;
                data.IdleUpradedPower = defaultPowerData(ds.Hp + lvl * ds.HpUprade, 0, ds.Dmg + lvl * ds.DmgUprade,
                    0, 0, 0, 0);
            }

            SaveManager.Instance.SetDataDirty();
        }
    }

    public static bool AscendIdlePowerData(this HeroData data)
    {
        bool success = false;
        var ds = DesignHelper.GetIdleHeroDesign(data.UniqueID, data.IdleLevel);
        if (ds != null)
        {
            var lvl = 1;

            data.IdleLevel = 1;
            data.IdleRank = 1;
            data.ResetPowerData();

            success = true;
            SaveManager.Instance.SetDataDirty();
        }

        return success;
    }

    public static bool ResetHeroUltimates(this HeroData data, HeroDesign ds)
    {
        bool success = true;
        data.ListUltimates.Clear();
        if (ds.Skill01 != "NILL")
        {
            if (data.ListUltimates.Count < 1)
            {
                if (!data.ListUltimates.Contains(ds.Skill01))
                    data.ListUltimates.Add(ds.Skill01);
            }
            else
            {
                if (data.ListUltimates[0] != ds.Skill01)
                {
                    data.ListUltimates[0] = ds.Skill01;
                }
            }
        }

        if (ds.Skill02 != "NILL")
        {
            if (data.ListUltimates.Count < 2)
            {
                if (!data.ListUltimates.Contains(ds.Skill02))
                    data.ListUltimates.Add(ds.Skill02);
            }
            else
            {
                if (data.ListUltimates[1] != ds.Skill02)
                {
                    data.ListUltimates[1] = ds.Skill02;
                }
            }
        }

        if (ds.Skill03 != "NILL")
        {
            if (data.ListUltimates.Count < 3)
            {
                if (!data.ListUltimates.Contains(ds.Skill03))
                    data.ListUltimates.Add(ds.Skill03);
            }
            else
            {
                if (data.ListUltimates[2] != ds.Skill03)
                {
                    data.ListUltimates[2] = ds.Skill03;
                }
            }
        }

        data.InitPassiveSkill = "";
        if (ds.PassiveSkill != "NILL")
        {
            data.InitPassiveSkill = ds.PassiveSkill;
        }

        SaveManager.Instance.SetDataDirty();
        return success;
    }

    public static EffectHit defaultEffectHit(this HeroData data, string SkillID, float customValue = -1)
    {
        EffectHit result = new EffectHit();

        var Design = DesignHelper.GetSkillDesign(SkillID);
        if (Design != null)
        {
            result.SkillID = SkillID;
            result.Type = DesignHelper.ConvertPassiveSkillToEffectType(SkillID);
            result.Duration = Design.Duration;
            result.Value = (customValue != -1) ? customValue : Design.Value;
            result.BaseDmg = data.BaseHeroPower.Dmg;
            result.OwnerID = data.UniqueID;
            result.Chance = Design.Chance;
            result.Number = (int)Design.Number;
        }

        return result;
    }

    public static EffectHit GetInitEffectHit(this HeroData data)
    {
        if (!string.IsNullOrEmpty(data.InitPassiveSkill))
            return data.defaultEffectHit(data.InitPassiveSkill);
        else
            return null;
    }

    public static List<EffectHit> GetListEquippedWeaponEffectHit(this HeroData data)
    {
        List<EffectHit> result = new List<EffectHit>();

        if (data.ListWeaponAttributes != null && data.ListWeaponAttributes.Count > 0)
        {
            foreach (var attr in data.ListWeaponAttributes)
            {
                var effect = data.defaultEffectHit(attr.AttributeID, attr.AttributeValue);
                if (effect.Type != EffectType.NONE)
                {
                    result.Add(effect);
                }
            }
        }

        return result;
    }

    public static bool AddExpManualHero(this UserData data, long exp)
    {
        bool result = false;

        // var heroData = data.GetHeroData(GameConstant.MANUAL_HERO);
        // if (heroData != null)
        // {
        //     var design = DesignHelper.GetHeroDesign(heroData);
        //     var targetExp = heroData.CurrentExp + exp;
        //     var currentLevelRequireExp =
        //         design.UpgradeCost + design.UpgradeCost * design.PriceMultiplyLevel * (heroData.Level + 1);
        //     if (targetExp >= currentLevelRequireExp)
        //     {
        //         //levelup hero
        //         targetExp -= (long)currentLevelRequireExp;
        //         heroData.Level += 1;
        //         if (heroData.Level > design.MaxLevel)
        //         {
        //             //rank up hereo
        //             heroData.Rank++;
        //         }
        //
        //         currentLevelRequireExp = design.UpgradeCost +
        //                                  design.UpgradeCost * design.PriceMultiplyLevel * (heroData.Level + 1);
        //     }
        //
        //     heroData.CurrentExp = targetExp;
        //     heroData.TargetExp = (int)currentLevelRequireExp;
        //
        //     heroData.ResetPowerData();
        //
        //     SaveManager.Instance.SetDataDirty();
        // }

        return result;
    }

    //shard
    public static bool AddShardHero(this HeroData hero, int shard = 1, bool showUnlocked = true)
    {
        bool result = false;

        int maxRank = DesignHelper.GetHeroMaxRank(hero.UniqueID);

        if (maxRank > 0 && hero.Rank < maxRank)
        {
            int nextRank = hero.Rank + 1 >= maxRank ? maxRank : hero.Rank + 1;
            var nextRankDs = DesignHelper.GetHeroDesign(hero.UniqueID, nextRank, hero.GetHeroLevel());

            hero.CurrentShard += Math.Abs(shard);
            //rank up hero
            //if (hero.CurrentShard >= nextRankDs.ShardRequire)
            //{
            //    hero.Rank += 1;
            //    hero.CurrentShard = 0;
            //    hero.ResetPowerData();
            //}

            SaveManager.Instance.SetDataDirty();
            result = true;
        }
        else
        {
            result = false;
            Debug.Log($"[AddShardHero] {hero.UniqueID} is max rank!!!");
        }

        if (hero.ItemStatus == ITEM_STATUS.Locked)
        {
            long totalShardNeed = 0;
            for (int i = 1; i <= hero.Rank; i++)
            {
                var design = DesignHelper.GetHeroDesign(hero.UniqueID, i, hero.GetHeroLevel());
                totalShardNeed += design.ShardRequire;
            }

            if (hero.CurrentShard >= totalShardNeed)
            {
                hero.CurrentShard -= (int)totalShardNeed;
                hero.UnlockHero();
                SaveManager.Instance.Data.ReminderData.NewHeroOnHudEquip.Add(hero.UniqueID);
                if (showUnlocked)
                    TopLayerCanvas.instance.ShowHUDForce(EnumHUD.HUD_HERO_UNLOCK, false, null, hero);
            }
        }

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.ADD_SHARD_HERO, hero);

        return result;
    }

    public static bool RankUpHero(this HeroData hero)
    {
        bool result = false;
        int maxRank = DesignHelper.GetHeroMaxRank(hero.UniqueID);

        if (maxRank > 0 && hero.Rank < maxRank)
        {
            int nextRank = hero.Rank + 1 >= maxRank ? maxRank : hero.Rank + 1;
            var nextRankDs = DesignHelper.GetHeroDesign(hero.UniqueID, nextRank, hero.GetHeroLevel());
            if (hero.CurrentShard >= nextRankDs.ShardRequire)
            {
                hero.Rank += 1;
                hero.CurrentShard -= (int)nextRankDs.ShardRequire;
                hero.ResetPowerData();
                result = true;
            }
        }


        return result;
    }

    public static bool IsEquipByUnlockedHero(WeaponData weaponData)
    {
        foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (heroData.UniqueID != GameConstant.MANUAL_HERO && heroData.UniqueID != "HERO_DEMO" &&
                (heroData.ItemStatus == ITEM_STATUS.Available || heroData.ItemStatus == ITEM_STATUS.Choosing) &&
                (heroData.EquippedWeapon == weaponData.UniqueID || heroData.EquippedArmour == weaponData.UniqueID))
            {
                return true;
            }
        }

        return false;
    }

    public static HeroData GetEquipByHero(WeaponData weaponData)
    {
        foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (heroData.EquippedWeapon == weaponData.UniqueID || heroData.EquippedArmour == weaponData.UniqueID)
            {
                if (heroData.UniqueID != GameConstant.MANUAL_HERO && heroData.UniqueID != "HERO_DEMO" &&
                    (heroData.EquippedWeapon == weaponData.UniqueID || heroData.EquippedArmour == weaponData.UniqueID))
                {
                    return heroData;
                }
            }
        }

        return null;
    }

    public static HeroData GetEquipByHeroIncludeAllHero(WeaponData weaponData)
    {
        foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (heroData.EquippedWeapon == weaponData.UniqueID || heroData.EquippedArmour == weaponData.UniqueID)
            {
                if (heroData.EquippedWeapon == weaponData.UniqueID || heroData.EquippedArmour == weaponData.UniqueID)
                {
                    return heroData;
                }
            }
        }

        return null;
    }


    public static long GetHeroCampaignCost(this HeroData data, int targetLevel)
    {
        long cost = 0;
        var allRankDS = DesignHelper.GetListHeroDesignByRank(data.UniqueID);
        var heroDesign1 = DesignHelper.GetHeroDesignByLevel(data.UniqueID, targetLevel);

        //int targetIndex = allRankDS.IndexOf(heroDesign);
        //if (heroDesign != null)
        //{
        //    //cost = (long)(heroDesign.UpgradeCost + heroDesign.UpgradeCostStep * (targetLevel - heroDesign.StartLevel));
        //    int levelPow = 1;
        //    for (int i = 0; i <= targetIndex; i++)
        //    {
        //        var ds = allRankDS[i];
        //        var nextDs = i + 1 <= targetIndex ? allRankDS[i + 1] : null;
        //        if (i == targetIndex)
        //        {
        //            levelPow = (int)(targetLevel - ds.StartLevel);
        //        }
        //        else
        //        {
        //            if (nextDs != null)
        //                levelPow = (int)(nextDs.StartLevel - ds.StartLevel);
        //            else
        //                levelPow = (int)(ds.MaxLevel - ds.StartLevel);
        //        }

        //        cost += (ds.UpgradeCost * Mathf.Pow(1 + (ds.UpgradeCostStep * 1.0f / 100f), levelPow));
        //    }

        //}

        int levelPow = (int)(targetLevel - heroDesign1.StartLevel);
        cost = (long)(heroDesign1.UpgradeCost * Mathf.Pow(1 + (heroDesign1.UpgradeCostStep * 1.0f / 100f), levelPow));

        // Debug.LogError($"UPGRADE COST {heroDesign1.UpgradeCost}, STEP {heroDesign1.UpgradeCostStep}, POW {levelPow}");
        return cost;
    }

    public static long GetHeroIdleCost(this HeroData data, int targetIdleLevel)
    {
        float cost = 0;
        var allRankDS = DesignHelper.GetListIdleHeroDesignByRank(data.UniqueID);
        var heroDesign = DesignHelper.GetIdleHeroDesign(data.UniqueID, targetIdleLevel);

        //var targetIndex = allRankDS.IndexOf(heroDesign);

        //if (heroDesign != null)
        //{
        //    int levelPow = 1;
        //    for (int i = 0; i <= targetIndex; i++)
        //    {
        //        var ds = allRankDS[i];
        //        if (i == targetIndex)
        //        {
        //            levelPow = (int)(targetIdleLevel - ds.StartLevel);
        //        }
        //        else
        //        {
        //            levelPow = (int)(ds.MaxLevel - ds.StartLevel);
        //        }

        //        cost += (ds.UpgradeToken * Mathf.Pow(1 + (ds.UpgradeTokenStep * 1.0f / 100f), levelPow));
        //    }

        //}
        int levelPow = (int)(targetIdleLevel - heroDesign.StartLevel);
        cost = heroDesign.UpgradeToken * Mathf.Pow(1 + (heroDesign.UpgradeTokenStep * 1.0f / 100f), levelPow);
        return (long)cost;
    }

    #endregion

    #region Weapon

    public static Dictionary<string, ObscuredInt> weaponLevelTemp = new Dictionary<string, ObscuredInt>();
    public static Dictionary<string, ObscuredInt> heroLevelTemp = new Dictionary<string, ObscuredInt>();

    public static void LevelUpHeroObfuse(this HeroData heroData)
    {
        heroData.SetHeroLevel(heroData.GetHeroLevel() + 1);    
    }

    public static ObscuredInt GetHeroLevel(this HeroData heroData)
    {
        int level = heroData.Level;
        if (heroLevelTemp.ContainsKey(heroData.UniqueID))
        {
            level = heroLevelTemp[heroData.UniqueID];
        }
        else
        {
            heroLevelTemp[heroData.UniqueID] = level;
        }

        return level;
    }


    public static void SetHeroLevel(this HeroData heroData, int level)
    {
        heroLevelTemp[heroData.UniqueID] = level;
    }
    
    public static void LevelUpWeapon(this WeaponData weaponData)
    {
        weaponData.SetWeaponLevel(weaponData.GetWeaponLevel() + 1);    
    }
    
    public static ObscuredInt GetWeaponLevel(this WeaponData weaponData)
    {
        int level = weaponData.Level;
        if (weaponLevelTemp.ContainsKey(weaponData.UniqueID))
        {
            level = weaponLevelTemp[weaponData.UniqueID];
        }
        else
        {
            weaponLevelTemp[weaponData.UniqueID] = level;
        }

        return level;
    }

    public static void SetWeaponLevel(this WeaponData weaponData, int level)
    {
        weaponLevelTemp[weaponData.UniqueID] = level;
    }
    

    public static void Dismantle(this WeaponData weaponData)
    {
        foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (heroData.EquippedWeapon == weaponData.UniqueID)
                heroData.EquippedWeapon = "";

            if (heroData.EquippedArmour == weaponData.UniqueID)
                heroData.EquippedArmour = "";
        }

        SaveManager.Instance.Data.Inventory.ListWeaponData.Remove(weaponData);
        SaveManager.Instance.SaveData();
    }

    public static bool IsMaxLevel(this WeaponData data)
    {
        var wpDesign = DesignHelper.GetWeaponDesign(data);
        return data.GetWeaponLevel() >= wpDesign.MaxLevel;
    }

    public static bool CanFusionWith(this WeaponData data, WeaponData compare)
    {
        return compare.ItemStatus != ITEM_STATUS.Choosing && data.Rank == compare.Rank &&
               data.WeaponID == compare.WeaponID;
    }

    // T1 - Gold, T2 - Weapon scroll, T3 - Armour scroll
    public static Tuple<long, long, long> GetDismantleResource(this WeaponData data)
    {
        var weaponDesign = DesignHelper.GetWeaponDesign(data);
        float totalGold = weaponDesign.SellPrice;
        // +
        //                   Convert.ToInt64(MathUtils.CalculateSumArithmeticSequenceAtIndex(weaponDesign.UpGold,
        //                       weaponDesign.UpGoldStep, data.Level - 1)) * 0.3f;

        int upgradeTime = data.GetWeaponLevel() - 1;
        long totalWeaponScroll = 0;

        if (upgradeTime > 0)
        {
            var wpDesignByLevel = DesignHelper.GetWeaponDesignByLevel(data.WeaponID, data.GetWeaponLevel());

            List<WeaponDesign> lowerRank = new List<WeaponDesign>();

            foreach (var VARIABLE1 in DesignManager.instance.DictWeaponDesign[data.WeaponID])
            {
                if (VARIABLE1.Rarity < wpDesignByLevel.Rarity)
                {
                    lowerRank.Add(VARIABLE1);
                }
            }

            long totalLastRank = 0;
            foreach (var VARIABLE in lowerRank)
            {
                int upgradeTimeInThisRankTemp = (int)(VARIABLE.MaxLevel - VARIABLE.StartLevel);
                totalLastRank += (long)(upgradeTimeInThisRankTemp * VARIABLE.UpWeaponScroll + VARIABLE.UpWeaponStep *
                    (MathUtils.CalculateSumArithmeticSequenceAtIndex(0, 1,
                        upgradeTimeInThisRankTemp + 2) - 1));
            }

            int upgradeTimeInThisRank = data.GetWeaponLevel() - (int)wpDesignByLevel.StartLevel;
            if (upgradeTimeInThisRank > 0)
            {
                long currentUpgradeRank = (long)(upgradeTimeInThisRank * wpDesignByLevel.UpWeaponScroll +
                                                  wpDesignByLevel.UpWeaponStep *
                                                  (MathUtils.CalculateSumArithmeticSequenceAtIndex(0, 1,
                                                      upgradeTimeInThisRank + 2) - 1));
                totalWeaponScroll = currentUpgradeRank + totalLastRank;
            }
            else
            {
                totalWeaponScroll = totalLastRank;
            }
        }

        // long totalArmourScroll = level == 0
        //     ? 0
        //     : weaponDesign.UpArmourScroll + Convert.ToInt64(
        //         MathUtils.CalculateSumArithmeticSequenceAtIndex(weaponDesign.UpArmourScroll,
        //             weaponDesign.UpArmourStep, level));
        return new Tuple<long, long, long>((long)totalGold, totalWeaponScroll, 0);
    }

    public static WeaponData defaultWeaponData(string weaponTypeID, int rank = 1)
    {
        WeaponData result = null;
        result = new WeaponData();
        result.Level = 1;
        result.Rank = rank;
        result.WeaponID = weaponTypeID;
        result.UniqueID = weaponTypeID + RandomString(10);
        result.ResetPowerData();
        return result;
    }

    public static WeaponData RandomWeaponData(int rank, List<string> ignoreWeapon = null)
    {
        WeaponData result = null;

        List<string> allWpId = new List<string>();
        foreach (var wpId in DesignManager.instance.DictWeaponDesign)
        {
            // Just random weapon
            if (wpId.Value[0].EquipType == 0)
            {
                if (ignoreWeapon == null)
                    allWpId.Add(wpId.Key);
                else
                {
                    if (!ignoreWeapon.Contains(wpId.Key))
                    {
                        allWpId.Add(wpId.Key);
                    }
                }
            }
        }

        result = new WeaponData();
        result.Level = 1;
        result.Rank = rank;

        //
        string id = allWpId[Random.Range(0, allWpId.Count)];

        // string id = allWpId[0];
        result.WeaponID = id;
        result.UniqueID = id + RandomString(10);
        result.ResetPowerData();
        return result;
    }

    // public static WeaponData RandomWeaponData()
    // {
    //     WeaponData result = null;
    //
    //     List<string> allWpId = new List<string>();
    //     foreach (var wpId in DesignManager.instance.DictWeaponDesign)
    //     {
    //         allWpId.Add(wpId.Key);
    //     }
    //
    //     result = new WeaponData();
    //     result.Level = Random.Range(1, 20);
    //     result.Rank = Random.Range(1, 7);
    //
    //     //
    //     string id = allWpId[Random.Range(0, allWpId.Count)];
    //
    //     // string id = allWpId[0];
    //     result.WeaponID = id;
    //     result.UniqueID = id + RandomString(10);
    //     result.ResetPowerData();
    //     return result;
    // }

    public static WeaponData GetWeaponData(this UserData data, string weaponUId)
    {
        var result = data.Inventory.ListWeaponData.FirstOrDefault(x => x.UniqueID == weaponUId);
        result.ResetPowerData();
        return result;
    }

    public static List<WeaponData> GetAllWeaponDatas(this UserData data)
    {
        return data.Inventory.ListWeaponData.ToList();
    }

    public static WeaponData AddWeapon(this UserData data, string weaponTypeID)
    {
        var newWeapon = defaultWeaponData(weaponTypeID);
        if (newWeapon != null)
        {
            data.Inventory.ListWeaponData.Add(newWeapon);
            return newWeapon;
        }

        else
        {
            Debug.LogError("AddWeapon failled!!!!");
            return null;
        }
    }

    public static void EquipWeapon(this HeroData heroData, WeaponData weaponData)
    {
        heroData.UnEquipWeapon();
        weaponData.ResetPowerData();
        heroData.EquippedWeapon = weaponData.UniqueID;
        weaponData.ItemStatus = ITEM_STATUS.Choosing;
        heroData.ResetPowerData();
    }

    public static void UnEquipWeapon(this HeroData heroData, int equipType = 0)
    {
        string unEquipedItemId;

        // Unequip current item
        if (equipType == GameConstant.EQUIP_TYPE_WEAPON)
        {
            unEquipedItemId = heroData.EquippedWeapon;
            heroData.EquippedWeapon = "";
        }
        else
        {
            unEquipedItemId = heroData.EquippedArmour;
            heroData.EquippedArmour = "";
        }

        // Set status to avalaible.
        if (unEquipedItemId != "")
        {
            WeaponData unequipedItem = SaveManager.Instance.Data.GetWeaponData(unEquipedItemId);
            unequipedItem.ItemStatus = ITEM_STATUS.Available;
        }

        heroData.ResetPowerData();
    }

    // T1 - GOLD
    // T2 - Weapon Scroll
    // T3 - Armor Scroll
    //public static WeaponUpgradeCostData GetUpgradeCost(WeaponData weaponData)
    //{
    //    var weaponDesign = DesignHelper.GetWeaponDesignByLevel(weaponData.WeaponID, weaponData.Level + 1);

    //    int lvl = weaponData.Level + 1 - (int)weaponDesign.StartLevel;
    //    if (lvl < 0)
    //    {
    //        Debug.LogError("GetUpgradeCost sth went wrong!!!!");
    //    }

    //    WeaponUpgradeCostData defaultCost = new WeaponUpgradeCostData(weaponDesign);

    //    WeaponUpgradeCostData deltaCost = new WeaponUpgradeCostData(
    //        Convert.ToInt64(lvl * weaponDesign.UpGoldStep),
    //        Convert.ToInt64(lvl * weaponDesign.UpWeaponStep),
    //        Convert.ToInt64(lvl * weaponDesign.UpArmourStep));

    //    WeaponUpgradeCostData result = defaultCost + deltaCost;
    //    return result;
    //}

    public static WeaponUpgradeCostData GetUpgradeCost(WeaponData weaponData)
    {
        int targetLevel = weaponData.GetWeaponLevel() + 1;
        var weaponDesign1 = DesignHelper.GetWeaponDesignByLevel(weaponData.WeaponID, targetLevel);
        var allRanksDs = DesignHelper.GetWeaponDesignAllRanks(weaponData.WeaponID);

        float upGold = 0;
        float upWeapon = 0;
        float upArmour = 0;

        //for (int i = 1; i <= weaponDesign.Rarity; i++)
        //{
        //    var ds = allRanksDs[i - 1];
        //    int levelPow = 1;
        //    if (i == weaponDesign.Rarity)
        //    {
        //        levelPow = (int)(targetLevel - ds.StartLevel);
        //    }
        //    else
        //    {
        //        levelPow = (int)(ds.MaxLevel - ds.StartLevel);
        //    }

        //    upGold += ds.UpGold * Mathf.Pow((1 + ds.UpGoldStep * 1.0f / 100f), levelPow);
        //    upWeapon += ds.UpWeaponScroll * Mathf.Pow((1 + ds.UpWeaponStep * 1.0f / 100f), levelPow);
        //    upArmour += ds.UpArmourScroll * Mathf.Pow((1 + ds.UpArmourStep * 1.0f / 100f), levelPow);

        //}
        int levelPow = (int)(targetLevel - weaponDesign1.StartLevel);
        upGold = weaponDesign1.UpGold * Mathf.Pow((1 + weaponDesign1.UpGoldStep * 1.0f / 100f), levelPow);
        upWeapon = weaponDesign1.UpWeaponScroll * Mathf.Pow((1 + weaponDesign1.UpWeaponStep * 1.0f / 100f), levelPow);
        upArmour = weaponDesign1.UpArmourScroll * Mathf.Pow((1 + weaponDesign1.UpArmourStep * 1.0f / 100f), levelPow);

        WeaponUpgradeCostData deltaCost = new WeaponUpgradeCostData(
            Convert.ToInt64(upGold),
            Convert.ToInt64(upWeapon),
            Convert.ToInt64(upArmour));

        return deltaCost;
    }

    public static List<Tuple<string, double>> GetListAttributeDesigns(WeaponData weaponData)
    {
        List<Tuple<string, double>> result = new List<Tuple<string, double>>();

        var defaultAndUpgrade = AddPowerData(weaponData.DefaultPower, weaponData.UpgradedPower);
        if (defaultAndUpgrade.PercentDmg != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_PERCENT_DMG.ToString(),
                defaultAndUpgrade.PercentDmg));
        }

        if (defaultAndUpgrade.HeadshotPercent != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT.ToString(),
                defaultAndUpgrade.HeadshotPercent));
        }

        if (defaultAndUpgrade.Firerate != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_FIRERATE.ToString(),
                defaultAndUpgrade.Firerate));
        }

        if (defaultAndUpgrade.Dmg != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_DMG.ToString(),
                defaultAndUpgrade.Dmg));
        }

        if (defaultAndUpgrade.CritPercent != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_CRIT.ToString(),
                defaultAndUpgrade.CritPercent));
        }

        if (defaultAndUpgrade.Range != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_RANGE.ToString(),
                defaultAndUpgrade.Range));
        }

        if (defaultAndUpgrade.Hp != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_HP.ToString(),
                defaultAndUpgrade.Hp));
        }

        if (defaultAndUpgrade.Armour != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_ARMOUR.ToString(),
                defaultAndUpgrade.Armour));
        }

        // // Add special attributes
        // var specialAttributes = weaponData.GetListSpecialAttributeDesigns();
        // if (specialAttributes.Count > 0)
        // {
        //     foreach (var specialAttribute in specialAttributes)
        //     {
        //         result.Add(specialAttribute);
        //     }
        // }
        return result;
    }

    public static List<Tuple<string, double>> GetListAttributeFromPowerData(PowerData FinalPowerData)
    {
        List<Tuple<string, double>> result = new List<Tuple<string, double>>();
        if (FinalPowerData.Firerate != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_FIRERATE.ToString(),
                FinalPowerData.Firerate));
        }

        if (FinalPowerData.Dmg != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_DMG.ToString(),
                FinalPowerData.Dmg));
        }

        if (FinalPowerData.PercentDmg != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_PERCENT_DMG.ToString(),
                FinalPowerData.PercentDmg));
        }

        if (FinalPowerData.HeadshotPercent != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT.ToString(),
                FinalPowerData.HeadshotPercent));
        }

        if (FinalPowerData.CritPercent != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_CRIT.ToString(),
                FinalPowerData.CritPercent));
        }

        if (FinalPowerData.Range != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_RANGE.ToString(), FinalPowerData.Range));
        }

        if (FinalPowerData.Hp != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_HP.ToString(), FinalPowerData.Hp));
        }

        if (FinalPowerData.Armour != 0)
        {
            result.Add(new Tuple<string, double>(EffectType.PASSIVE_INCREASE_ARMOUR.ToString(), FinalPowerData.Armour));
        }

        // // Add special attributes
        // var specialAttributes = weaponData.GetListSpecialAttributeDesigns();
        // if (specialAttributes.Count > 0)
        // {
        //     foreach (var specialAttribute in specialAttributes)
        //     {
        //         result.Add(specialAttribute);
        //     }
        // }
        return result;
    }

    public static void ResetPowerData(this WeaponData data)
    {
        var ds = DesignHelper.GetWeaponDesign(data);
        if (ds != null)
        {
            int lvl = data.GetWeaponLevel() - 1;

            data.Type = ds.HandType;
            data.DefaultPower = defaultPowerData(ds.Hp, ds.Armour, 0, (float)ds.FireRate,
                ds.HeadshotPercent, ds.Range, ds.CritPercent, ds.PercentDmg);

            data.UpgradedPower = defaultPowerData(lvl * ds.HpUpgrade,
                lvl * ds.ArmourUpgrade,
                0,
                lvl * ds.FireRateUpgrade,
                lvl * ds.HeadshotPercentUpgrade,
                lvl * ds.RangeUprade,
                lvl * ds.CritPercentUpgrade,
                (float)(lvl * 1.0f * ds.PercentDmgUpgrade));

            var basePower = AddPowerData(data.DefaultPower, data.UpgradedPower);
            var fromSpecialAttribute = data.CalculatePowerFromSpecialAttribute();
            data.FinalPowerData = AddPowerData(basePower, fromSpecialAttribute);
            // Debug.LogError("BASE " + basePower);
            // Debug.LogError("SPECIAL " + fromSpecialAttribute);
            // Debug.LogError("FINAL " + data.FinalPowerData);
        }

        else
        {
            Debug.LogError("ResetPowerData failled");
        }
    }

    public static PowerData CalculatePowerDataFromRank(this WeaponData data, int rank)
    {
        var result = defaultPowerData(0, 0, 0, 0, 0, 0, 0);

        var ds = DesignHelper.GetWeaponDesign(data.WeaponID, rank, data.GetWeaponLevel());
        if (ds != null)
        {
            int lvl = data.GetWeaponLevel() - 1;

            var defaultPower = defaultPowerData(ds.Hp, ds.Armour, 0, (float)ds.FireRate,
                ds.HeadshotPercent, ds.Range, ds.CritPercent, ds.PercentDmg);

            var upgradePower = defaultPowerData(lvl * ds.HpUpgrade,
                lvl * ds.ArmourUpgrade,
                0,
                lvl * ds.FireRateUpgrade,
                lvl * ds.HeadshotPercentUpgrade,
                lvl * ds.RangeUprade,
                lvl * ds.CritPercentUpgrade,
                (float)(lvl * 1.0f * ds.PercentDmgUpgrade));

            result = AddPowerData(defaultPower, upgradePower);
            // var fromSpecialAttribute = data.CalculatePowerFromSpecialAttribute();
            // result = AddPowerData(basePower, fromSpecialAttribute);
            // Debug.LogError("BASE " + basePower);
            // Debug.LogError("SPECIAL " + fromSpecialAttribute);
            // Debug.LogError("FINAL " + data.FinalPowerData);
        }

        else
        {
            Debug.LogError("ResetPowerData failled");
        }

        return result;
    }

    #endregion

    #region SyncData

    public static bool SyncData(this UserData data)
    {
        bool forceSave = false;
        forceSave = data.SyncDesignData();
        forceSave = data.SyncDataVersion();
        forceSave = data.SyncIdleModeData();
        data.RefreshNewDay();
        ++data.MetaData.CountLogin;
        PlayerPrefs.SetInt(PLAYER_PREF.COUNT_LOGIN, data.MetaData.CountLogin);
        return forceSave;
    }

    public static bool SyncDesignData(this UserData data)
    {
        bool forceSave = false;

        //sync hero Data
        foreach (KeyValuePair<string, List<HeroDesign>> entry in DesignManager.instance.DictHeroDesign)
        {
            // not exist in safve
            var exists = data.Inventory.ListHeroData.FirstOrDefault(x => x.UniqueID == entry.Key);
            if (exists == null)
            {
                data.AddHero(entry.Key);
            }
            else
            {
                var ds = DesignHelper.GetHeroDesign(exists.UniqueID, exists.Rank, exists.GetHeroLevel());
                exists.ResetHeroUltimates(ds);
                exists.ResetPowerData();
            }
        }

        forceSave = true;
        return forceSave;
    }

    public static bool SyncDataVersion(this UserData data)
    {
        bool forceSave = false;

        var _version = data.MetaData.VersionData;
        while (_version <= DATA_VERSION)
        {
            switch (_version)
            {
                case 1:
                    break;
                case 2:
                    if (data.DayTrackingData == null)
                    {
                        data.DayTrackingData = new DayTrackingValue();
                    }

                    if (data.ReminderData == null)
                    {
                        data.ReminderData = new ReminderData();
                    }

                    break;
                case 3:
                    // force airdrop to non consumable item!!!
                    var airDrop = data.GetAddOnItem(GameConstant.ADD_ON_AIR_DROP);
                    if (airDrop != null)
                    {
                        airDrop.ItemCount = -999;
                    }
                    else
                    {
                        data.AddAddONItem(GameConstant.ADD_ON_AIR_DROP, -999);
                    }

                    break;
                case 4:
                    break;
                case 5:
                    if (data.AdsTracker == null)
                    {
                        data.AdsTracker = new AdsTracker();
                    }
                    break;
                default:
                    break;
            }

            ++_version;
            forceSave = true;
        }

        data.MetaData.VersionData = DATA_VERSION;
        return forceSave;
    }

    public static void FirstTimeJoinGame(this UserData data)
    {
        var defaultHero = SaveManager.Instance.Data.GetHeroData("HERO_1");
        defaultHero.UnlockHero();
        var manualHero = SaveManager.Instance.Data.GetHeroData(MANUAL_HERO);
        manualHero.UnlockHero();
        var oldHeroesPrimitive = SaveManager.Instance.Data.LastData.Inventory.ListHeroData;
        oldHeroesPrimitive.ToList().Find(x => x.UniqueID == defaultHero.UniqueID).UnlockHero();
        oldHeroesPrimitive.ToList().Find(x => x.UniqueID == manualHero.UniqueID).UnlockHero();
        SaveManager.Instance.Data.MetaData.DayLogin = 1;
        AnalyticsManager.instance.LogEvent(
            AnalyticsConstant.getEventName(AnalyticsConstant.ANALYTICS_ENUM.DAY_LOGIN) + $"_day_{1}",
            new LogEventParam("dayLogin", data.MetaData.DayLogin));

        // #if UNITY_EDITOR
        //Debug.LogError("Remember remove");
        //var hero1 = SaveManager.Instance.Data.GetHeroData("HERO_2");
        //hero1.UnlockHero();
        //hero1 = SaveManager.Instance.Data.GetHeroData("HERO_3");
        //hero1.UnlockHero();
        //hero1 = SaveManager.Instance.Data.GetHeroData("HERO_4");
        //hero1.UnlockHero();
        //hero1 = SaveManager.Instance.Data.GetHeroData("HERO_5");
        //hero1.UnlockHero();
        //hero1 = SaveManager.Instance.Data.GetHeroData("HERO_6");
        //hero1.UnlockHero();
        //hero1 = SaveManager.Instance.Data.GetHeroData("HERO_7");
        //hero1.UnlockHero();
        //hero1 = SaveManager.Instance.Data.GetHeroData("HERO_8");
        //hero1.UnlockHero();
        // #endif
        SaveManager.Instance.Data.Inventory.ManualHero = manualHero;
        SaveManager.Instance.Data.ApplyFreeSlotMember("HERO_1");
        SaveManager.Instance.Data.AdsTracker = new AdsTracker();


        //add amount of exp first time to init data!!
        SaveManager.Instance.Data.AddExpManualHero(10);
        SaveManager.Instance.SetDataDirty();
    }

    public static bool SyncIdleModeData(this UserData data)
    {
        bool forceSave = false;

        forceSave = data.CheckAndResetIdleMode();

        return forceSave;
    }

    #endregion

    #region UTILS

    private static System.Random random = new System.Random();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static bool IsDefault<T>(this T value) where T : struct

    {
        bool isDefault = value.Equals(default(T));
        return isDefault;
    }

    #endregion

    #region Progress Data

    public static PlayProgress defaultPlayProgress()
    {
        PlayProgress result = new PlayProgress();
        result.CurrentLevel = 1;
        result.CurrentWave = 0;
        return result;
    }

    public static PlayProgress GetPlayProgress(this UserData data, GameMode mode)
    {
        PlayProgress result = null;
        switch (mode)
        {
            case GameMode.NONE:
                break;
            case GameMode.CAMPAIGN_MODE:
                result = data.GameData.CampaignProgress;
                break;
            case GameMode.IDLE_MODE:
                result = data.GameData.IdleProgress;
                break;
            default:
                break;
        }

        return result;
    }

    public static bool OverwritePlayerProgress(this UserData data, GameMode mode, int level)
    {
        bool result = false;

        PlayProgress progress = null;
        switch (mode)
        {
            case GameMode.NONE:
                break;
            case GameMode.CAMPAIGN_MODE:
                progress = data.GameData.CampaignProgress;
                break;
            case GameMode.IDLE_MODE:
                progress = data.GameData.IdleProgress;
                break;
            default:
                break;
        }

        if (progress != null)
        {
            progress.CurrentLevel = level;
            if (level >= progress.MaxLevel)
            {
                progress.MaxLevel = level;
            }

            result = true;
        }

        return result;
    }

    public static bool AddIdleLevel(this UserData data, int levelAdd)
    {
        bool result = false;

        PlayProgress progress = data.GameData.IdleProgress;

        if (progress != null)
        {
            progress.CurrentLevel += levelAdd;
            if (progress.CurrentLevel >= progress.MaxLevel)
            {
                progress.MaxLevel = progress.CurrentLevel;
            }

            result = true;
        }

        return result;
    }

    public static bool AscendIdlePlayProgress(this UserData data, long earnPill, long returnToken)
    {
        bool success = true;
        data.GameData.IdleProgress.CurrentLevel = 1;
        data.GameData.IdleProgress.CurrentWave = 1;
        data.GameData.IdleProgress.CollectedValue = 0;
        data.Inventory.TotalPill += earnPill;
        data.Inventory.TotalToken += returnToken;
        data.Inventory.TotalPillFromAscend += earnPill;
        foreach (var hero in data.Inventory.ListHeroData)
        {
            if (hero.IsUnlocked())
            {
                hero.AscendIdlePowerData();
            }
        }

        SaveManager.Instance.SetDataDirty();
        return success;
    }

    public static List<RewardData> AscendIdlePlayProgress(this UserData data, BaseAscendRewardDesign design)
    {
        List<RewardData> result = new List<RewardData>();
        var currentIdleLevel = data.GetPlayProgress(GameMode.IDLE_MODE).CurrentLevel;
        if (design != null)
        {
            var ds = design.BaseAscendReward.LastOrDefault(x => currentIdleLevel >= x.StartLevel);
            if (ds != null)
            {
                long earnPill = ds.BasePill + ds.StepPill * (currentIdleLevel - ds.StartLevel);
                long earnToken = ds.BaseTokenReturn + ds.StepTokenReturn * (currentIdleLevel - ds.StartLevel);

                data.GameData.IdleProgress.CurrentLevel = 1;
                data.GameData.IdleProgress.CurrentWave = 1;
                data.GameData.IdleProgress.CollectedValue = 0;
                data.Inventory.TotalPill += earnPill;
                data.Inventory.TotalToken += earnToken;
                data.Inventory.TotalPillFromAscend += earnPill;
                foreach (var hero in data.Inventory.ListHeroData)
                {
                    if (hero.IsUnlocked())
                    {
                        hero.AscendIdlePowerData();
                    }
                }

                result.Add(new RewardData(REWARD_TYPE.TOKEN, earnToken));
                result.Add(new RewardData(REWARD_TYPE.PILL, earnPill));


                SaveManager.Instance.SetDataDirty();
            }
        }

        return result;
    }

    public static bool CheckAndResetIdleMode(this UserData data, bool forceReset = false)
    {
        bool isReset = false;
        var idleProgress = data.GetPlayProgress(GameMode.IDLE_MODE);
        if (idleProgress != null)
        {
            long curTs = TimeService.instance.GetCurrentTimeStamp();
            if (forceReset || curTs >= idleProgress.ResetModeTS)
            {
                //start reset idle Mode
                DateTime today = TimeService.instance.GetCurrentDateTime();
                if (today.Day >= 1 && today.Day < 15)
                {
                    idleProgress.ResetModeTS = TimeService.instance.GetNextDayTSofMonth(curTs, 15);
                }
                else
                {
                    idleProgress.ResetModeTS = TimeService.instance.GetNextDayTSofMonth(curTs, 1);
                }

                idleProgress.MaxLevel = 1;
                idleProgress.CurrentLevel = 1;
                idleProgress.LastLogoutTS = 0;
                idleProgress.TotalTimePlayed = 0;
                ++idleProgress.SeasonNum;

                //foreach (var rwdTrack in data.Inventory.ListClaimedIdleRewards)
                //{
                //    rwdTrack.SeasonNum = ++idleProgress.SeasonNum;
                //}
            }
        }

        return isReset;
    }

    public static List<RewardData> GetListValidIdleRewards(this UserData data, CustomBattleReward customDesign)
    {
        var idleTracker =
            data.Inventory.ListClaimedIdleRewards.FirstOrDefault(x => x.LevelReward == customDesign.LevelReward);
        List<RewardData> result = new List<RewardData>();
        foreach (var item in customDesign.Rewards)
        {
            bool validAdd = false;
            if (item.Loop < 0)
            {
                validAdd = true;
            }
            else
            {
                if (idleTracker != null)
                {
                    var rewardTrack = idleTracker.ListEarnedReward.FirstOrDefault(x => x.RewardID == item.RewardId);
                    validAdd = rewardTrack != null && rewardTrack.CountEarned < item.Loop;
                }
                else
                    validAdd = true;
            }

            if (validAdd)
            {
                result.Add(new RewardData()
                {
                    _type = DesignHelper.ConvertToRewardType(item.RewardId),
                    _value = item.Value,
                    _extends = item.RewardId
                });
            }
        }

        return result;
    }

    /// <summary>
    /// Stars Campgain Mpode
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int GetStarAtLevel(int level)
    {
        if (level >= SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel)
        {
            return 0;
        }

        CheckStarException(level);
        return SaveManager.Instance.Data.StarData.LevelStars[level - 1];
    }

    public static int GetTotalStar()
    {
        return SaveManager.Instance.Data.StarData.LevelStars.Sum();
    }

    public static void SaveStar(int star, int level)
    {
        var levelStars = SaveManager.Instance.Data.StarData.LevelStars;
        CheckStarException(level);

        if (levelStars[level - 1] < star)
            levelStars[level - 1] = star;

        SaveManager.Instance.SetDataDirty();
    }

    private static void CheckStarException(int level)
    {
        var levelStars = SaveManager.Instance.Data.StarData.LevelStars;
        // Not init yet
        if (levelStars.Count < level)
        {
            for (int i = levelStars.Count; i < level; i++)
            {
                levelStars.Add(0);
            }
        }
    }

    public static bool IsReplayable(this UserData data, GameMode mode, int level)
    {
        var progress = data.GetPlayProgress(mode);
        if (progress != null)
        {
            return level <= progress.MaxLevel;
        }

        return false;
    }

    public static void SetCurrentLevel(this PlayProgress progress, int curLevel)
    {
        progress.CurrentLevel = curLevel;
        if (progress.CurrentLevel >= progress.MaxLevel)
        {
            progress.MaxLevel = curLevel;
        }
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Attribute is term of All static power values!!!!!
    /// </summary>
    /// <param name="listAttributes"></param>
    /// <returns></returns>
    public static PowerData AddAttributePowerData(this HeroData data, List<Tuple<EffectType, float>> listAttributes)
    {
        PowerData powerData = null;
        if (listAttributes != null && listAttributes.Count > 0)
        {
            powerData = new PowerData();
            foreach (var pair in listAttributes)
            {
                switch (pair.Item1)
                {
                    case EffectType.PASSIVE_INCREASE_CRIT_DMG:
                        break;
                    case EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT:
                        powerData.HeadshotPercent += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_FIRERATE:
                        powerData.Firerate += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_PERCENT_DMG:
                        powerData.PercentDmg += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_HP:
                        powerData.Hp += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_DODGE_RATE:
                        //???
                        break;
                    case EffectType.PASSIVE_INCREASE_DMG:
                        powerData.Dmg += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_RANGE:
                        powerData.Range += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_CRIT:
                        powerData.CritPercent += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_ARMOUR:
                        powerData.Armour += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_HERO_BASE_DAMAGE:
                        powerData.Dmg += pair.Item2;
                        break;
                    case EffectType.PASSIVE_INCREASE_FIRERATE_PERCENT:
                        powerData.Firerate += data.BaseHeroPower.Firerate * (pair.Item2 * 1.0f / 100f);
                        break;
                    default:
                        break;
                }
            }
        }

        return powerData;
    }

    public static void CheckToUnlockHero()
    {
        foreach (var VARIABLE in DesignManager.instance.unlockHeroDesign.UnlockHeroDesignElements)
        {
            if (VARIABLE.Available
                && VARIABLE.CostType == "FREE" &&
                SaveGameHelper.GetMaxCampaignLevel() >= VARIABLE.UnlockLevel &&
                SaveManager.Instance.Data.GetHeroData(VARIABLE.HeroId).ItemStatus == ITEM_STATUS.Locked)
            {
                string heroID = VARIABLE.HeroId;

                var defaultHero = SaveManager.Instance.Data.GetHeroData(heroID);
                if (defaultHero == null)
                    SaveManager.Instance.Data.AddHero(heroID);

                defaultHero = SaveManager.Instance.Data.GetHeroData(heroID);
                defaultHero.UnlockHero();
                SaveManager.Instance.Data.ReminderData.NewHeroOnHudEquip.Add(defaultHero.UniqueID);
                TopLayerCanvas.instance.ShowHUDForce(EnumHUD.HUD_HERO_UNLOCK, false, null, defaultHero);
            }
        }
    }

    #endregion

    #region User Info

    /// <summary>
    /// is manual hero level
    /// </summary>
    /// <returns></returns>
    public static int GetUserCurrentLevel(this UserData data)
    {
        int level = 0;

        var heroData = data.GetHeroData(GameConstant.MANUAL_HERO);
        if (heroData != null)
        {
            level = heroData.GetHeroLevel();
        }

        return level;
    }

    public static Tuple<int, int> GetUserCurrentLevelProgress(this UserData data)
    {
        Tuple<int, int> result = null;

        var heroData = data.GetHeroData(GameConstant.MANUAL_HERO);
        if (heroData != null)
        {
            result = new Tuple<int, int>((int)heroData.CurrentExp, (int)heroData.TargetExp);
        }

        return result;
    }

    public static int GetMaxCampaignLevel()
    {
        int level = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel;
        if (level < 1)
            level = 1;

        return level;
    }

    public static int GetCurrentCampaignLevel()
    {
        return SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
    }

    public static int GetCurrentIdleLevel()
    {
        return SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE).CurrentLevel;
    }

    #endregion
}
