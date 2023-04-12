using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using Newtonsoft.Json;
using QuickType.Attribute;
using UnityEngine;

namespace QuickType.LocationPack
{
    public partial class LocationPackDesign
    {
        [JsonProperty("LocationPackDesign", Required = Required.Always)]
        public List<LocationPackDesignElement> LocationPackDesignElements { get; set; }
    }

    public partial class LocationPackDesignElement
    {
        [JsonProperty("ID", Required = Required.Default)]
        public string Id { get; set; }

        [JsonProperty("Icon", Required = Required.Default)]
        public string Icon { get; set; }

        [JsonProperty("Name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("Description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("Cost", Required = Required.Always)]
        public float Cost { get; set; }

        [JsonProperty("OldCostValue", Required = Required.Always)]
        public float OldCostValue { get; set; }

        [JsonProperty("Gold", Required = Required.Always)]
        public int Gold { get; set; }

        [JsonProperty("Diamond", Required = Required.Always)]
        public int Diamond { get; set; }

        [JsonProperty("Revive", Required = Required.Always)]
        public int Revive { get; set; }

        [JsonProperty("KeyLegendaryChest", Required = Required.Always)]
        public int KeyLegendaryChest { get; set; }

        [JsonProperty("Scroll", Required = Required.Always)]
        public int Scroll { get; set; }

        [JsonProperty("LegendaryEquipment", Required = Required.Always)]
        public int LegendaryEquipment { get; set; }

        [JsonProperty("UnlockLocationID", Required = Required.Always)]
        public string UnlockLocationId { get; set; }

        [JsonProperty("UnlockLevel", Required = Required.Always)]
        public int UnlockLevel { get; set; }

        [JsonProperty("Android_ProductID", Required = Required.Default)]
        public string AndroidProductID { get; set; }

        [JsonProperty("IOS_ProductID", Required = Required.Default)]
        public string IOSProductID { get; set; }
    }

    public partial class LocationPackDesignElement
    {
        public List<RewardData> GetRewards()
        {
            var result = new List<RewardData>();
            if (Gold != 0)
                result.Add(new RewardData(REWARD_TYPE.GOLD, Gold));
            if (Diamond != 0)
                result.Add(new RewardData(REWARD_TYPE.DIAMOND, Diamond));
            if (Revive != 0)
                result.Add(new RewardData(REWARD_TYPE.REVIVE, Revive));
            if (KeyLegendaryChest != 0)
                result.Add(new RewardData(REWARD_TYPE.KEY_CHEST_LEGENDARY, KeyLegendaryChest));
            if (Scroll != 0)
            {
                // REWARD_TYPE[] scrollTypes = new REWARD_TYPE[]
                // {
                //     REWARD_TYPE.SCROLL_ARMOUR,
                //     REWARD_TYPE.SCROLL_WEAPON
                // };

                result.Add(new RewardData(REWARD_TYPE.SCROLL_WEAPON, Scroll));
            }

            if (LegendaryEquipment != 0)
            {
                for (int i = 0; i < LegendaryEquipment; i++)
                {
                    WeaponData wpData = SaveGameHelper.RandomWeaponData(RankConstant.LEGENDARY);
                    result.Add(new RewardData(REWARD_TYPE.RANDOM_EQUIP, 1, wpData));
                }
            }

            return result;
        }
    }

    public partial class LocationPackDesign
    {
        public static LocationPackDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<LocationPackDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }

    public partial class LocationPackDesignElement
    {
        public string GetPriceString()
        {
            string price = this.Cost.ToString();

            string productID = GetIAPProductID();
            var priceStr = IAPManager.instance.GetProductPrice(productID);

            if (!string.IsNullOrEmpty(priceStr))
                price = priceStr;

            return price;
        }

        public string GetIAPProductID()
        {
            string result = "";

#if UNITY_ANDROID
            result = this.AndroidProductID;
#else
                result = this.IOSProductID;
#endif

            return result;
        }
    }
}