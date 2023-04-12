using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QuickType.Fusion
{
    public partial class FusionDesign
    {
        [JsonProperty("FusionDesign", Required = Required.Always)]
        public List<FusionElement> FusionElements { get; set; }
    }

    public partial class FusionDesign
    {
        public static FusionDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<FusionDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }

    public partial class FusionElement
    {
        [JsonProperty("Rank", Required = Required.Always)]
        public int Rank { get; set; }

        [JsonProperty("Quantity", Required = Required.Always)]
        public int Quantity { get; set; }
        
        [JsonProperty("GoldCost", Required = Required.Always)]
        public long GoldCost{ get; set; }
    }
}