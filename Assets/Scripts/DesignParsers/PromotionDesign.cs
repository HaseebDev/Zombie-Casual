using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickType.Attribute;
using QuickType.Shop;
using UnityEngine;

namespace QuickType.Promotion
{
    public partial class PromotionDesign
    {
        [JsonProperty("PromotionDesign", Required = Required.Always)]
        public List<PromotionDesignElement> PromotionDesignElement { get; set; }
    }

    public partial class PromotionDesignElement : ShopDesignElement
    {
        [JsonProperty("Description", Required = Required.Always)]
        public string Description { get; set; }
    }

    public partial class PromotionDesignElement
    {
        public CostData GetOldCost()
        {
            return new CostData(GetCost().Type, OldCostValue);
        }
    }

    public partial class PromotionDesign
    {
        public static PromotionDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<PromotionDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}