using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickType.Attribute;
using QuickType.Shop;
using UnityEngine;

namespace QuickType.UnlockHero
{
    public partial class UnlockHeroDesign
    {
        [JsonProperty("UnlockHeroDesign", Required = Required.Always)]
        public List<UnlockHeroDesignElement> UnlockHeroDesignElements { get; set; }
    }

    public partial class UnlockHeroDesignElement
    {
        [JsonProperty("HeroID", Required = Required.Always)]
        public string HeroId { get; set; }

        [JsonProperty("Android_ProductID", Required = Required.Always)]
        public string Android_ProductId { get; set; }

        [JsonProperty("IOS_ProductID", Required = Required.Always)]
        public string IOS_ProductId { get; set; }

        [JsonProperty("CostType", Required = Required.Always)]
        public string CostType { get; set; }

        [JsonProperty("UnlockLevel", Required = Required.Always)]
        public int UnlockLevel { get; set; }

        [JsonProperty("Available", Required = Required.Always)]
        public bool Available { get; set; }
        
        [JsonProperty("CostValue", Required = Required.Always)]
        public float CostValue { get; set; }
        
        [JsonProperty("LevelToShow", Required = Required.Always)]
        public int LevelToShow { get; set; }

        public CostData GetCurrencyType()
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
                result = this.Android_ProductId;
#else
                result = this.IOSProductID;
#endif
            }

            return result;
        }
    }

    public partial class UnlockHeroDesign
    {
        public static UnlockHeroDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<UnlockHeroDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}