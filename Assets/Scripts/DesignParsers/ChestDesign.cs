using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using QuickType.Attribute;
using UnityEngine;

namespace QuickType.Chest
{
    public partial class ChestDesign
    {
        [JsonProperty("ChestDesign", Required = Required.Always)]
        public List<ChestDesignElement> ChestDesignElements { get; set; }
    }

    public partial class ChestDesignElement
    {
        [JsonProperty("ChestID", Required = Required.Always)]
        public string ChestID { get; set; }

        [JsonProperty("DiamondCost", Required = Required.Always)]
        public int DiamondCost { get; set; }

        [JsonProperty("DiamondCostContinue", Required = Required.Always)]
        public int DiamondCostContinue { get; set; }

        [JsonProperty("DiamondSale", Required = Required.Always)]
        public int DiamondSale { get; set; }

        [JsonProperty("ReceiveHours", Required = Required.Always)]
        public int ReceiveHours { get; set; }

        [JsonProperty("MaxStack", Required = Required.Always)]
        public int MaxStack { get; set; }

        [JsonProperty("Common", Required = Required.Always)]
        public float Common { get; set; }

        [JsonProperty("Rare", Required = Required.Always)]
        public float Rare { get; set; }

        [JsonProperty("Epic", Required = Required.Always)]
        public float Epic { get; set; }

        [JsonProperty("Legendary", Required = Required.Always)]
        public float Legendary { get; set; }

        [JsonProperty("Immortal", Required = Required.Always)]
        public float Immortal { get; set; }

        [JsonProperty("Unique", Required = Required.Always)]
        public float Unique { get; set; }

        [JsonProperty("LegendaryEquipStack", Required = Required.Always)]
        public int LegendaryEquipStack { get; set; }

        [JsonProperty("IgnoreWeapon", Required = Required.Always)]
        public string IgnoreWeapon { get; set; }

        public int GetRandomEquipRank()
        {
            float randomNumber = Random.Range(0f, 100f);
            float sub = 0;
            List<float> arr = new List<float>();
            arr.Add(Common);
            arr.Add(Rare);
            arr.Add(Epic);
            arr.Add(Legendary);
            arr.Add(Immortal);
            arr.Add(Unique);

            for (int i = 0; i < arr.Count; i++)
            {
                sub += arr[i];
                if (randomNumber <= sub)
                {
                    return i + 1;
                }
            }

            return 1;
        }

        public List<string> GetIgnoreWeapon()
        {
            if (IgnoreWeapon == DesignHelper.DEFAULT_NULL)
            {
                return null;
            }

            List<string> result = IgnoreWeapon.Split(',').ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = result[i].Trim();
            }
            

            return result;
        }
    }

    public partial class ChestDesign
    {
        public static ChestDesign FromJson(string json) =>
            JsonConvert.DeserializeObject<ChestDesign>(json, QuickType.SkillDesign.Converter.Settings);
    }
}