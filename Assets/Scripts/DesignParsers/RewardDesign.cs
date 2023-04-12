﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do one of these:
//
//    using QuickType;
//
//    var welcome = Welcome.FromJson(jsonString);
//    var arr = Arr.FromJson(jsonString);
//    var convert = Convert.FromJson(jsonString);

public enum REWARD_TYPE
{
    NONE,
    GOLD,
    TOKEN,
    PILL,
    SCROLL_ARMOUR,
    SCROLL_WEAPON,
    SCROLL_HERO,
    CHEST,
    DIAMOND,
    KEY_CHEST_RARE,
    KEY_CHEST_LEGENDARY,
    EQUIP,
    REVIVE,
    REFILL_HP,
    ADD_ON,
    RANDOM_EQUIP,
    LEVEL_CAMPAIGN,
    LEVEL_IDLE,
    KEY_CHEST_HERO
}

namespace QuickType.RewardDesign
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class RewardDesign
    {
        [JsonProperty("RewardDesign", Required = Required.Always)]
        public List<RewardDesignElement> Rewards { get; set; }
    }

    public partial class RewardDesignElement
    {
        [JsonProperty("RewardID", Required = Required.Always)]
        public String RewardID { get; set; }

        [JsonProperty("RewardType", Required = Required.Always)]
        public REWARD_TYPE RewardType { get; set; }

        [JsonProperty("Name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("DefaultValue", Required = Required.Always)]
        public float DefaultValue { get; set; }

        [JsonProperty("Description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("RequireValue", Required = Required.Always)]
        public double RequireValue { get; set; }

        [JsonProperty("ExpiredTime", Required = Required.Always)]
        public double ExpiredTime { get; set; }

    }


    public partial class RewardDesign
    {
        public static RewardDesign FromJson(string json) => JsonConvert.DeserializeObject<RewardDesign>(json, QuickType.RewardDesign.Converter.Settings);
    }

    public partial class RewardDesignElement
    {
        public static RewardDesignElement FromJson(string json) => JsonConvert.DeserializeObject<RewardDesignElement>(json, QuickType.RewardDesign.Converter.Settings);
    }

    public class Convert
    {
        public static Dictionary<string, object> FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, object>>(json, QuickType.RewardDesign.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this RewardDesign self) => JsonConvert.SerializeObject(self, QuickType.RewardDesign.Converter.Settings);
        public static string ToJson(this RewardDesignElement self) => JsonConvert.SerializeObject(self, QuickType.RewardDesign.Converter.Settings);
        public static string ToJson(this Dictionary<string, object> self) => JsonConvert.SerializeObject(self, QuickType.RewardDesign.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                RewardTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class RewardTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(REWARD_TYPE) || t == typeof(REWARD_TYPE?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);


            return value.ParseEnum<REWARD_TYPE>(REWARD_TYPE.NONE);
            throw new Exception("Cannot unmarshal type RewardType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (REWARD_TYPE)untypedValue;
            switch (value)
            {
                case REWARD_TYPE.NONE:
                    break;
                case REWARD_TYPE.GOLD:
                    serializer.Serialize(writer, "GOLD");
                    break;
                case REWARD_TYPE.TOKEN:
                    serializer.Serialize(writer, "TOKEN");
                    break;
                case REWARD_TYPE.PILL:
                    serializer.Serialize(writer, "PILL");
                    break;
                case REWARD_TYPE.SCROLL_ARMOUR:
                    serializer.Serialize(writer, "SCROLL_ARMOUR");
                    break;
                case REWARD_TYPE.SCROLL_WEAPON:
                    serializer.Serialize(writer, "SCROLL_WEAPON");
                    break;
                case REWARD_TYPE.SCROLL_HERO:
                    serializer.Serialize(writer, "SCROLL_HERO");
                    break;
                case REWARD_TYPE.CHEST:
                    serializer.Serialize(writer, "CHEST");
                    break;
                default:
                    break;
            }

            throw new Exception("Cannot marshal type RewardType");
        }

        public static readonly RewardTypeConverter Singleton = new RewardTypeConverter();
    }
}
