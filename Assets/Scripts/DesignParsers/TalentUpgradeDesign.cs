using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickType.Attribute;
using UnityEngine;

namespace QuickType.TalentUpgrade
{
    public partial class TalentUpgradeDesign
    {
        [JsonProperty("TalentUpgradeDesign", Required = Required.Always)]
        public List<TalentUpgradeDesignElement> TalentUpgradeDesignElements { get; set; }
    }

    public partial class TalentUpgradeDesignElement
    {
        [JsonProperty("Level", Required = Required.Always)]
        public int Level { get; set; }

        [JsonProperty("Potion", Required = Required.Always)]
        public int Potion { get; set; }

        [JsonProperty("LevelCampaign", Required = Required.Always)]
        public int LevelCampaign { get; set; }
    }

    public partial class TalentUpgradeDesign
    {
        public static TalentUpgradeDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<TalentUpgradeDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}