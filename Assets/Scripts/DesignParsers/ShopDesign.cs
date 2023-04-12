using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickType.Attribute;
using QuickType.Promotion;
using UnityEngine;
using UnityExtensions;

namespace QuickType.Shop
{
    public enum CostType
    {
        NONE,
        IAP,
        DIAMOND,
        GOLD,
        FREE,
        ADS
    }

    public class CostData
    {
        public CostType Type;
        public float Value;
        public string PriceStr;

        public CostData(CostType type, float value)
        {
            Type = type;
            Value = value;
            PriceStr = value.ToString();
        }

        public bool IsIAP()
        {
            return Type == CostType.IAP;
        }
    }


    public partial class ShopDesign
    {
        [JsonProperty("ShopDesign", Required = Required.Always)]
        public List<ShopDesignElement> ShopDesignElement { get; set; }
    }

    public partial class ShopDesignElement
    {
        [JsonProperty("ID", Required = Required.Default)]
        public string Id { get; set; }

        [JsonProperty("Icon", Required = Required.Default)]
        public string Icon { get; set; }

        [JsonProperty("Name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("CostType", Required = Required.Always)]
        public string CostType { get; set; }

        [JsonProperty("CostValue", Required = Required.Always)]
        public float CostValue { get; set; }

        [JsonProperty("GoldReward", Required = Required.Always)]
        public int GoldReward { get; set; }

        [JsonProperty("DiamondReward", Required = Required.Always)]
        public int DiamondReward { get; set; }

        [JsonProperty("PillReward", Required = Required.Always)]
        public int PillReward { get; set; }

        [JsonProperty("TokenReward", Required = Required.Always)]
        public int TokenReward { get; set; }

        [JsonProperty("KeyRareChestReward", Required = Required.Always)]
        public int KeyRareChestReward { get; set; }

        [JsonProperty("KeyLegendaryChestReward", Required = Required.Always)]
        public int KeyLegendaryChestReward { get; set; }

        [JsonProperty("ArmourScrollReward", Required = Required.Always)]
        public int ArmourScrollReward { get; set; }

        [JsonProperty("WeaponScrollReward", Required = Required.Always)]
        public int WeaponScrollReward { get; set; }

        [JsonProperty("GoodChoice", Required = Required.Default)]
        public bool GoodChoice { get; set; }

        [JsonProperty("Bonus", Required = Required.Default)]
        public int Bonus { get; set; }

        [JsonProperty("OldCostValue", Required = Required.Default)]
        public int OldCostValue { get; set; }

        [JsonProperty("Android_ProductID", Required = Required.Default)]
        public string AndroidProductID { get; set; }

        [JsonProperty("IOS_ProductID", Required = Required.Default)]
        public string IOSProductID { get; set; }
    }


    [Serializable]
    public partial class ShopDesignElement
    {
        private int _lastResetLevel = 0;
        public bool isFreePack = false;
        public void ResetData()
        {
            int currentLevel = SaveGameHelper.GetMaxCampaignLevel();
            if (currentLevel != _lastResetLevel)
            {
                var shopValueByLevelDesign = DesignHelper.GetShopValueByLevel(Id, currentLevel);
                if (shopValueByLevelDesign != null)
                {
                    GoldReward = (int) shopValueByLevelDesign.BaseGold;
                    PillReward = (int) shopValueByLevelDesign.BasePill;
                    WeaponScrollReward = (int) shopValueByLevelDesign.BaseWeaponScroll;
                }
            }
        
            _lastResetLevel = currentLevel;
        }

        public CostData GetCost()
        {
            CostData result = new CostData(this.CostType.ToEnum<CostType>(), CostValue);
            if (result.Type == Shop.CostType.IAP && IAPManager.instance != null)
            {
                string productID = getIAPProductID();
                result.PriceStr = IAPManager.instance.GetProductPrice(productID);
            }
            else
            {
                result.PriceStr = FBUtils.CurrencyAddComma(CostValue);
            }

            return result;
        }

        public string getIAPProductID()
        {
            string result = "";

            if (this.CostType.ToEnum<CostType>() == Shop.CostType.IAP)
            {
#if UNITY_ANDROID
                result = this.AndroidProductID;
#else
                result = this.IOSProductID;
#endif
            }

            return result;
        }

        public virtual List<RewardData> GetRewardWithBonus()
        {
            ResetData();
            List<RewardData> result = new List<RewardData>();
            if (GoldReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.GOLD, GoldReward + Bonus));
            }

            if (DiamondReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.DIAMOND, DiamondReward + Bonus));
            }

            if (PillReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.PILL, PillReward + Bonus));
            }

            if (TokenReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.TOKEN, TokenReward + Bonus));
            }

            if (KeyRareChestReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.KEY_CHEST_RARE, KeyRareChestReward + Bonus));
            }

            if (KeyLegendaryChestReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.KEY_CHEST_LEGENDARY, KeyLegendaryChestReward + Bonus));
            }

            if (ArmourScrollReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.SCROLL_ARMOUR, ArmourScrollReward + Bonus));
            }

            if (WeaponScrollReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.SCROLL_WEAPON, WeaponScrollReward + Bonus));
            }

            return result;
        }

        public virtual List<RewardData> GetReward()
        {
            ResetData();

            List<RewardData> result = new List<RewardData>();
            if (GoldReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.GOLD, GoldReward));
            }

            if (DiamondReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.DIAMOND, DiamondReward));
            }

            if (PillReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.PILL, PillReward));
            }

            if (TokenReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.TOKEN, TokenReward));
            }

            if (KeyRareChestReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.KEY_CHEST_RARE, KeyRareChestReward));
            }

            if (KeyLegendaryChestReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.KEY_CHEST_LEGENDARY, KeyLegendaryChestReward));
            }

            if (ArmourScrollReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.SCROLL_ARMOUR, ArmourScrollReward));
            }

            if (WeaponScrollReward != 0)
            {
                result.Add(new RewardData(REWARD_TYPE.SCROLL_WEAPON, WeaponScrollReward));
            }

            return result;
        }
    }

    public partial class ShopDesign
    {
        public static ShopDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<ShopDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}