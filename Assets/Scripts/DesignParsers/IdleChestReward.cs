﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var idleChestReward = IdleChestReward.FromJson(jsonString);

namespace QuickType.IdleChest
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class IdleChestReward
    {
        [JsonProperty("IdleChestReward", Required = Required.Always)]
        public List<IdleChestRewardElement> IdleChestRewardIdleChestReward { get; set; }
    }

    public partial class IdleChestRewardElement
    {
        [JsonProperty("Level", Required = Required.Always)]
        public long Level { get; set; }

        [JsonProperty("GoldPerSec", Required = Required.Always)]
        public double GoldPerSec { get; set; }

        [JsonProperty("MaxTime", Required = Required.Always)]
        public long MaxTime { get; set; }

        [JsonProperty("TokenPerSec", Required = Required.Always)]
        public double TokenPerSec { get; set; }

        [JsonProperty("MaxTokenTime", Required = Required.Always)]
        public long MaxTokenTime { get; set; }


    }

    public partial class IdleChestReward
    {
        public static IdleChestReward FromJson(string json) => JsonConvert.DeserializeObject<IdleChestReward>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this IdleChestReward self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
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
