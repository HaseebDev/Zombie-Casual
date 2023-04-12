using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickType.Attribute;
using UnityEngine;

namespace QuickType.StarReward
{
    public partial class StarRewardDesign
    {
        [JsonProperty("StarRewardDesign", Required = Required.Always)]
        public List<StarRewardDesignElement> StarRewardDesignElements { get; set; }
    }

    public partial class StarRewardDesignElement
    {
        [JsonProperty("Star", Required = Required.Always)]
        public int Star { get; set; }

        [JsonProperty("FreeRewardType", Required = Required.Always)]
        public string FreeRewardType { get; set; }

        [JsonProperty("FreeRewardValue", Required = Required.Always)]
        public float FreeRewardValue { get; set; }

        [JsonProperty("BattlePassRewardType", Required = Required.Always)]
        public string BattlePassRewardType { get; set; }

        [JsonProperty("BattlePassRewardValue", Required = Required.Always)]
        public float BattlePassRewardValue { get; set; }
    }

    public partial class StarRewardDesign
    {
        public static StarRewardDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<StarRewardDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}