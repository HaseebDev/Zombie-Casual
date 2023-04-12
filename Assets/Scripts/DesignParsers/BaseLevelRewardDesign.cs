﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var baseLevelRewardDesign = BaseLevelRewardDesign.FromJson(jsonString);

namespace QuickType.BaseLevelReward
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class BaseLevelRewardDesign
    {
        [JsonProperty("BaseCampaignReward", Required = Required.Always)]
        public List<BaseRewardElement> RewardArr { get; set; }
    }

    public partial class BaseRewardElement
    {
        [JsonProperty("StartLevel", Required = Required.Always)]
        public int StartLevel { get; set; }

        [JsonProperty("BaseExpAngel", Required = Required.Always)]
        public int BaseExpAngel { get; set; }

        [JsonProperty("StepExpAngel", Required = Required.Always)]
        public int StepExpAngel { get; set; }

        [JsonProperty("AngelExpMultiplier", Required = Required.Always)]
        public float AngelExpMultiplier { get; set; }

        [JsonProperty("Rewards", Required = Required.Always)]
        public List<BaseReward> Rewards { get; set; }
    }

    public partial class BaseReward
    {
        [JsonProperty("RewardID", Required = Required.Always)]
        public string RewardId { get; set; }

        [JsonProperty("BaseValue", Required = Required.Always)]
        public long BaseValue { get; set; }

        [JsonProperty("RewardMultiply", Required = Required.Always)]
        public double RewardMultiply { get; set; }
    }


    public partial class BaseLevelRewardDesign
    {
        public static BaseLevelRewardDesign FromJson(string json) => JsonConvert.DeserializeObject<BaseLevelRewardDesign>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this BaseLevelRewardDesign self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {

                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}
