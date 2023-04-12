using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QuickType.Attribute;
using UnityEngine;

namespace QuickType.Talent
{
    public enum TalentID
    {
        STRENGTH,
        POWER,
        RECOVER,
        RANGE_BLOCK,
        MELEE_BLOCK,
        AGILITY,
        VAMPIRE,
        POWER_OF_ANGLE,
        HAND_OF_MIDAS,
        GREEDY,
        CRITICAL_HIT,
        WHEEL_OF_FORTUNE
    }

    public partial class TalentDesign
    {
        [JsonProperty("TalentDesign", Required = Required.Always)]
        public List<TalentDesignElement> TalentDesignElements { get; set; }
    }

    public partial class TalentDesignElement
    {
        [JsonProperty("ID", Required = Required.Always)]
        public string ID { get; set; }

        [JsonProperty("Name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("Rank", Required = Required.Always)]
        public int Rank { get; set; }

        [JsonProperty("Icon", Required = Required.Always)]
        public string Icon { get; set; }

        [JsonProperty("Description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("CampaignUnlockLevel", Required = Required.Always)]
        public int CampaignUnlockLevel { get; set; }

        [JsonProperty("SpecialUnlock", Required = Required.Always)]
        public string SpecialUnlock { get; set; }

        [JsonProperty("IsPercent", Required = Required.Always)]
        public bool IsPercent { get; set; }

        [JsonProperty("MaximumLevel", Required = Required.Always)]
        public int MaximumLevel { get; set; }

        [JsonProperty("ValueLevel1", Required = Required.Always)]
        public float ValueLevel1 { get; set; }

        [JsonProperty("ValueLevel2", Required = Required.Always)]
        public float ValueLevel2 { get; set; }

        [JsonProperty("ValueLevel3", Required = Required.Always)]
        public float ValueLevel3 { get; set; }

        [JsonProperty("ValueLevel4", Required = Required.Always)]
        public float ValueLevel4 { get; set; }

        [JsonProperty("ValueLevel5", Required = Required.Always)]
        public float ValueLevel5 { get; set; }

        [JsonProperty("ValueLevel6", Required = Required.Always)]
        public float ValueLevel6 { get; set; }

        [JsonProperty("ValueLevel7", Required = Required.Always)]
        public float ValueLevel7 { get; set; }

        [JsonProperty("ValueLevel8", Required = Required.Always)]
        public float ValueLevel8 { get; set; }

        [JsonProperty("ValueLevel9", Required = Required.Always)]
        public float ValueLevel9 { get; set; }

        [JsonProperty("ValueLevel10", Required = Required.Always)]
        public float ValueLevel10 { get; set; }
    }

    public partial class TalentDesignElement
    {
        public float GetValue(int level)
        {
            switch (level)
            {
                case 0:
                    return 0;
                case 1:
                    return ValueLevel1;
                case 2:
                    return ValueLevel2;
                case 3:
                    return ValueLevel3;
                case 4:
                    return ValueLevel4;
                case 5:
                    return ValueLevel5;
                case 6:
                    return ValueLevel6;
                case 7:
                    return ValueLevel7;
                case 8:
                    return ValueLevel8;
                case 9:
                    return ValueLevel9;
                case 10:
                    return ValueLevel10;
                default:
                    return ValueLevel10;
            }
        }
    }

    public partial class TalentDesign
    {
        public static TalentDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<TalentDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}