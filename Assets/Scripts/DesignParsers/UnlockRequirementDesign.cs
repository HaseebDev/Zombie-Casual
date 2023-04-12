
using UnityEngine;

namespace QuickType
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class UnlockRequirementDesign
    {
        [JsonProperty("UnlockRequirementDesign", Required = Required.Always)]
        public List<UnlockRequirementDesignElement> UnlockRequirementDesignElements { get; set; }
    }

    public partial class UnlockRequirementDesignElement
    {
        [JsonProperty("ID", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("StartTime", Required = Required.Always)]
        public string StartTime { get; set; }

        [JsonProperty("EndTime", Required = Required.Always)]
        public string EndTime { get; set; }

        [JsonProperty("LevelUnlock", Required = Required.Always)]
        public int LevelUnlock { get; set; }

        [JsonProperty("DateLogin", Required = Required.Always)]
        public int DateLogin { get; set; }

        [JsonProperty("Series", Required = Required.Always)]
        public string Series { get; set; }

        [JsonProperty("Available", Required = Required.Always)]
        public bool Available { get; set; }
    }

    public partial class UnlockRequirementDesignElement
    {
        public bool HasTime()
        {
            return StartTime != "0";
        }

        public DateTime GetStartTime()
        {
            return DateTime.ParseExact(StartTime, "MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public DateTime GetEndTime()
        {
            return DateTime.ParseExact(EndTime, "MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public bool IsAvailable()
        {
            DateTime now = TimeService.instance.GetCurrentDateTime();
            bool checkDate = true;
            bool checkFromDate = true;

            // skip 
            if (!Available)
                return Available;

            if (DateLogin != 0)
            {
                long timeSpanSec = TimeService.instance.GetCurrentTimeStamp() -
                                   SaveManager.Instance.Data.MetaData.FirstTimeJoinTimeStamp;
                // + 3 * 3600 * 24; // Minus 3 day 

                TimeSpan timeSpan = TimeSpan.FromSeconds(timeSpanSec);
                checkFromDate = timeSpan.TotalDays >= DateLogin;
            }

            if (HasTime())
            {
                checkDate = now >= GetStartTime() && now <= GetEndTime();
            }

            bool checkLevel = SaveGameHelper.GetMaxCampaignLevel() >= LevelUnlock;
            return checkDate && checkLevel && checkFromDate;
        }
    }

    public partial class UnlockRequirementDesign
    {
        public static UnlockRequirementDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<UnlockRequirementDesign>(json, QuickType.Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this UnlockRequirementDesign self) =>
            JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }

            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (long) untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}