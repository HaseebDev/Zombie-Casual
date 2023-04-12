using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QuickType.Weapon;

namespace QuickType.Attribute
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    // public enum AttributeID
    // {
    //     CRIT_DMG,
    //     HEAD_SHOT,
    //     FIRE_RATE,
    //     DAMAGE_PERCENT,
    //     HP_PERCENT,
    //     DODGE_RATE,
    //     //HEALING,
    //     //DEAL_EQUIP,
    //     DAMAGE,
    //     ATK_SPEED,
    //     RANGE,
    //     HP,
    //     CRIT_PERCENT,
    //     ARMOUR,
    //     HERO_BASE_DAMAGE,
    //     FIRE_RATE_PERCENT,
    // }
    //
    // public static class PercentAttribute
    // {
    //     public static List<AttributeID> percentAttributes = new List<AttributeID>()
    //     {
    //         AttributeID.CRIT_DMG,
    //         AttributeID.HEAD_SHOT,
    //         AttributeID.DAMAGE_PERCENT,
    //         AttributeID.HP_PERCENT,
    //         AttributeID.DODGE_RATE,
    //         AttributeID.CRIT_PERCENT,
    //         AttributeID.FIRE_RATE_PERCENT
    //     };
    // }
    //
    // public partial class AttributeDesign
    // {
    //     [JsonProperty("AttributeDesign", Required = Required.Always)]
    //     public List<AttributeElement> AttributeElement { get; set; }
    // }
    //
    // public partial class AttributeElement
    // {
    //     [JsonProperty("AttributeID", Required = Required.Always)]
    //     public string AttributeId { get; set; }
    //
    //     [JsonProperty("Name", Required = Required.Always)]
    //     public string Name { get; set; }
    //
    //     [JsonProperty("Description", Required = Required.Always)]
    //     public string Description { get; set; }
    //
    //     public bool IsPercent()
    //     {
    //         AttributeID enumValue;
    //         Enum.TryParse(AttributeId, out enumValue);
    //         return PercentAttribute.percentAttributes.Contains(enumValue);
    //     }
    // }
    //
    // public partial class AttributeDesign
    // {
    //     public static AttributeDesign FromJson(string json) =>
    //         JsonConvert.DeserializeObject<AttributeDesign>(json, QuickType.Attribute.Converter.Settings);
    // }
    //
    //
    // internal static partial class Converter
    // {
    //     public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //     {
    //         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //         DateParseHandling = DateParseHandling.None,
    //         Converters =
    //         {
    //             new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
    //         },
    //     };
    // }
}