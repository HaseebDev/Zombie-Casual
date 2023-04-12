using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickType.Attribute;
using QuickType.Shop;
using UnityEngine;
using UnityExtensions;

namespace QuickType.FreeStuff
{
    public partial class FreeStuffDesign
    {
        [JsonProperty("FreeStuffDesign", Required = Required.Always)]
        public List<FreeStuffDesignElement> FreeStuffDesignElements { get; set; }
    }

    public partial class FreeStuffDesignElement : ShopDesignElement
    {
        [JsonProperty("SpecialItem", Required = Required.Always)]
        public string SpecialItem { get; set; }

        [JsonProperty("SpecialItemValue", Required = Required.Always)]
        public float SpecialItemValue { get; set; }

        [JsonProperty("Enable", Required = Required.Always)]
        public bool Enable { get; set; }
    }

    [Serializable]
    public partial class FreeStuffDesignElement
    {
        public override List<RewardData> GetReward()
        {
            List<RewardData> result = base.GetReward();
            if (SpecialItem != DesignHelper.DEFAULT_NULL)
            {
                result.Add(new RewardData(REWARD_TYPE.ADD_ON, (long) SpecialItemValue, SpecialItem));
            }

            return result;
        }

        public override List<RewardData> GetRewardWithBonus()
        {
            var result = base.GetRewardWithBonus();

            if (SpecialItem != DesignHelper.DEFAULT_NULL)
            {
                result.Add(new RewardData(REWARD_TYPE.ADD_ON, (long) SpecialItemValue, SpecialItem));
            }

            return result;
        }
    }

    public partial class FreeStuffDesign
    {
        public static FreeStuffDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<FreeStuffDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}