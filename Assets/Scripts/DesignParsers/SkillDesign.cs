﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do one of these:
//
//    using QuickType;
//
//    var skillDesign = SkillDesign.FromJson(jsonString);
//    var skill = Skill.FromJson(jsonString);
//    var convert = Convert.FromJson(jsonString);

using UnityEngine;

public enum EffectType
{
    NONE,
    PASSIVE_HIT_FIRE,
    PASSIVE_HIT_POISON,
    PASSIVE_HIT_STUN,
    PASSIVE_HIT_FROZEN,
    PASSIVE_HIT_LIGHTING,
    PASSIVE_HIT_BOMB,

    //WEAPON_ATTRIBUTE
    //INCREASE_CRIT_DMG,
    //INCREASE_HEADSHOT,
    //INCREASE_FIRERATE,
    //INCREASE_PERCENT_DMG,
    //INCREASE_HP,


    PASSIVE_DEAD_SHOT,
    PASSIVE_FURY_SHOT,
    PASSIVE_MADNESS,
    PASSIVE_RICOCHET,
    PASSIVE_BULLET_TRAIL,
    PASSIVE_PIERCE,
    PASSIVE_PUSHBACK,
    PASSIVE_AOE,
    PASSIVE_DIAGONAL,

    PASSIVE_INCREASE_CRIT_DMG,
    PASSIVE_INCREASE_HEADSHOT_PERCENT,
    PASSIVE_INCREASE_FIRERATE,
    PASSIVE_INCREASE_PERCENT_DMG,
    PASSIVE_INCREASE_HP,
    PASSIVE_INCREASE_DODGE_RATE,

    PASSIVE_INCREASE_RANGE,
    PASSIVE_INCREASE_CRIT,
    PASSIVE_INCREASE_ARMOUR,
    PASSIVE_INCREASE_HERO_BASE_DAMAGE,
    PASSIVE_INCREASE_FIRERATE_PERCENT,
    PASSIVE_INCREASE_DMG,

    PASSIVE_MULTIPLY_SPEED,
    PASSIVE_HIT_NOVA,
    PASSIVE_HIT_BURNING_AREA,
    PASSIVE_REDUCE_COUNTDOWN_ULTI
};

public enum ARMOUR_EFFECT_TYPE
{
    //ARMOUR ATTIBUTE
    NONE,
    RESPONSE_DMG,
    RESPONSE_FIRE,
    RESPONSE_LIGHTNING,
    RESPONSE_POISON
}


public enum SkillType
{
    NONE = 0,
    ACTIVE,
    ADD_ON,
    SPECIAL_ADD_ON,
    STATIC_ATTRIBUTE,
    ACTIVE_ATTRIBUTE
}

namespace QuickType.SkillDesign
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public static class PercentAttribute
    {
        public static List<EffectType> percentAttributes = new List<EffectType>()
        {
            EffectType.PASSIVE_INCREASE_CRIT,
            EffectType.PASSIVE_INCREASE_HEADSHOT_PERCENT,
            EffectType.PASSIVE_INCREASE_PERCENT_DMG,
            EffectType.PASSIVE_INCREASE_DODGE_RATE,
            EffectType.PASSIVE_INCREASE_FIRERATE_PERCENT
        };
    }


    public partial class SkillDesign
    {
        [JsonProperty("SkillDesign", Required = Required.Always)]
        public List<SkillDesignElement> Skills { get; set; }
    }

    public partial class SkillDesignElement
    {
        [JsonProperty("SkillID", Required = Required.Always)]
        public string SkillId { get; set; }

        [JsonProperty("SkillType", Required = Required.Always)]
        public SkillType SkillType { get; set; }

        [JsonProperty("Description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("Dmg", Required = Required.Always)]
        public float Dmg { get; set; }

        [JsonProperty("HeroBaseDmgPercent", Required = Required.Always)]
        public float HeroBaseDmgPercent { get; set; }

        [JsonProperty("Radius", Required = Required.Always)]
        public float Radius { get; set; }

        [JsonProperty("Duration", Required = Required.Always)]
        public float Duration { get; set; }

        [JsonProperty("Name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("Number", Required = Required.Always)]
        public float Number { get; set; }

        [JsonProperty("CountDown", Required = Required.Always)]
        public float CountDown { get; set; }

        [JsonProperty("Value", Required = Required.Always)]
        public float Value { get; set; }

        [JsonProperty("Chance", Required = Required.Always)]
        public float Chance { get; set; }

        [JsonProperty("AppearLevel", Required = Required.Always)]
        public int AppearLevel { get; set; }

        [JsonProperty("UnlockLevel", Required = Required.Always)]
        public int UnlockLevel { get; set; }
    }

    public partial class SkillDesignElement
    {
        public bool IsPercent()
        {
            foreach (var VARIABLE in PercentAttribute.percentAttributes)
            {
                if (SkillId.Contains(VARIABLE.ToString()))
                {
                    return true;
                }
            }

            EffectType enumValue;
            Enum.TryParse(SkillId, out enumValue);
            return PercentAttribute.percentAttributes.Contains(enumValue);
        }

        public string GetDescription()
        {
            return LocalizeController.GetText(ConvertToBaseDescription());
        }

        public string GetName()
        {
            return LocalizeController.GetText(ConvertToBaseName());
        }

        public string ConvertToBaseName()
        {
            string result = Name;
            if (result.Contains("/"))
            {
                int startIndex = result.IndexOf("/", StringComparison.Ordinal);
                result = result.Remove(startIndex, 2);
            }

            return result;
        }

        public string ConvertToBaseDescription()
        {
            string result = Description;
            if (result.Contains("/"))
            {
                int startIndex = result.IndexOf("/", StringComparison.Ordinal);
                result = result.Remove(startIndex, 2);
            }

            return result;
        }
    }

    public partial class SkillDesign
    {
        public static SkillDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<SkillDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }

    public partial class SkillDesignElement
    {
        public static SkillDesignElement FromJson(string json) =>
            JsonConvert.DeserializeObject<SkillDesignElement>(json, QuickType.SkillDesign.Converter.Settings);
    }

    public class Convert
    {
        public static Dictionary<string, object> FromJson(string json) =>
            JsonConvert.DeserializeObject<Dictionary<string, object>>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SkillDesign self) =>
            JsonConvert.SerializeObject(self, QuickType.Converter.Settings);

        public static string ToJson(this SkillDesignElement self) =>
            JsonConvert.SerializeObject(self, QuickType.Converter.Settings);

        public static string ToJson(this Dictionary<string, object> self) =>
            JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                EffectConverter.Singleton,
                SkillTypeConverter.Singleton,
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }

    internal class EffectConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(EffectType) || t == typeof(EffectType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "FIRE":
                    return EffectType.PASSIVE_HIT_FIRE;
                case "POISON":
                    return EffectType.PASSIVE_HIT_POISON;
                case "STUN":
                    return EffectType.PASSIVE_HIT_STUN;
                default:
                    return EffectType.NONE;
            }

            throw new Exception("Cannot unmarshal type Effect");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (EffectType)untypedValue;
            switch (value)
            {
                case EffectType.PASSIVE_HIT_FIRE:
                    serializer.Serialize(writer, "FIRE");
                    return;
                case EffectType.PASSIVE_HIT_POISON:
                    serializer.Serialize(writer, "POISON");
                    return;
                case EffectType.PASSIVE_HIT_STUN:
                    serializer.Serialize(writer, "STUN");
                    return;
                case EffectType.NONE:
                    serializer.Serialize(writer, "NONE");
                    return;
            }

            throw new Exception("Cannot marshal type Effect");
        }

        public static readonly EffectConverter Singleton = new EffectConverter();
    }


    internal class SkillTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SkillType) || t == typeof(SkillType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);

            return value.ParseEnum<SkillType>(SkillType.NONE);
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

            serializer.Serialize(writer, value.ToString());

            throw new Exception("Cannot marshal type RewardType");
        }

        public static readonly SkillTypeConverter Singleton = new SkillTypeConverter();
    }
}