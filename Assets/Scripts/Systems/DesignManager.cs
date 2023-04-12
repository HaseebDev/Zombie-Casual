using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using QuickType;
using QuickType.Hero;
using QuickType.Zombie;
using QuickType.Weapon;
using System.Linq;
using com.datld.data;
using JetBrains.Annotations;
using QuickEngine.Extensions;
using QuickType.Attribute;
using QuickType.Chest;
using QuickType.Fusion;
using QuickType.LevelWave;
using QuickType.ZombieRatio;
using QuickType.IdleDesign;
using QuickType.IdleHero;
using QuickType.RewardDesign;
using QuickType.SkillDesign;
using QuickType.UnlockHero;
using Random = UnityEngine.Random;
using QuickType.Config;
using QuickType.LocationPack;
using QuickType.Promotion;
using QuickType.Shop;
using QuickType.Talent;
using QuickType.TalentUpgrade;
using QuickType.BaseLevelReward;
using QuickType.AirDrop;
using QuickType.AttributeTextDisplay;
using QuickType.FreeStuff;
using QuickType.StarReward;
using QuickType.HeroAngel;
using QuickType.IdleChest;
using UnityEngine.AddressableAssets;
using QuickType.Ascend;
using QuickType.ShopValueByLevelDesign;
using QuickType.SkipLevel;
using QuickType.MapObstacle;
using QuickType.MapByLevel;

[Serializable]
public enum QuickSkillID
{
    PASSIVE_HIT_FIRE,
    PASSIVE_HIT_LIGHTING,
    PASSIVE_HIT_FROZEN,
    PASSIVE_HIT_BOMB,
    PASSIVE_HIT_POISON,
}

public class DesignManager : BaseSystem<DesignManager>
{
    public static DesignManager instance;
    public static string DESIGN_PATH = "Designs/";

    //List DesignData
    public Dictionary<string, List<HeroDesign>> DictHeroDesign { get; private set; }
    public Dictionary<string, List<IdleHeroDesign>> DictIdleHeroDesign { get; private set; }

    public Dictionary<string, List<WeaponDesign>> DictWeaponDesign { get; private set; }

    // public Dictionary<string, AttributeElement> DictAttributeDesign { get; private set; }
    public RewardDesign rewardDesign { get; private set; }
    public StarRewardDesign starRewardDesign { get; private set; }
    public AttributeTextDisplay attributeTextDisplay { get; private set; }
    public UnlockRequirementDesign UnlockRequirementDesign { get; private set; }

    //base reward

    public SkillDesign skillDesign { get; private set; }
    public FusionDesign fusionDesign { get; private set; }
    public UnlockHeroDesign unlockHeroDesign { get; private set; }
    public ChestDesign chestDesign { get; private set; }
    public GeneralConfig generalConfigDesign { get; private set; }
    public ShopDesign shopDesign { get; private set; }
    public FreeStuffDesign freeStuffDesign { get; private set; }
    public PromotionDesign promotionDesign { get; private set; }
    public LocationPackDesign locationPackDesign { get; private set; }
    public TalentDesign talentDesign { get; private set; }
    public TalentUpgradeDesign talentUpgradeDesign { get; private set; }
    public ManualHeroDesign manualHeroDesign { get; private set; }
    public IdleChestReward idleChestDesign { get; private set; }
    public BaseAscendRewardDesign baseAscendRewardDesign { get; private set; }
    public SkipIdleLevelDesign skipIdleLeveDesign { get; private set; }
    public ShopValueByLevelDesign shopValueByLevelDesign { get; private set; }
    public MapByLevelDesign mapByLevelDesign { get; private set; }

    public ChestHeroDesign chestHeroDesign { get; private set; }
    public MissionDesign missionDesign{ get; private set; }
    public MissionDurationDesign missionDurationDesign{ get; private set; }

    //cached here!!!!!!!!!!!
    public Dictionary<string, SkillDesignElement> _dictSkillDesign { get; private set; }

    /// <summary>
    /// some design may load in runtime for performance!!!
    /// </summary>

    //runtime load
    public ZombieDesign ListZombieDesign { get; private set; }
    public ZombieRatioDesign zombieRatioDesign { get; private set; }
    public BaseLevelRewardDesign BaseCampaignRewardDesign { get; private set; }
    public CustomCampaignRewardDesign CustomCampaignRewardDesign { get; private set; }
    public CustomIdleRewardDesign CustomIdleRewardDesign { get; private set; }

    //Level Designs
    public List<LevelElementDesign> LevelCampaignDesign { get; private set; }
    public List<LevelElementDesign> LevelIdleDesign { get; private set; }

    public MapObstacleDesign MapObstacleDesign { get; private set; }
    public MapObstacleRatioDesign MapObstacleRatio { get; private set; }

    #region Runtime Load Design

    public AirDropDesign airDropDesign { get; private set; }

    public void LoadAirdropDesign()
    {
        if (airDropDesign == null)
            Timing.RunCoroutine(LoadAirDropDesignCoroutine());
    }

    public IEnumerator<float> LoadAirDropDesignCoroutine()
    {
        if (airDropDesign == null)
        {
            TextAsset jsonFile = null;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("AirDropDesign", (text) =>
                    {
                        jsonFile = text;
                        if (jsonFile)
                        {
                            airDropDesign = AirDropDesign.FromJson(jsonFile.text);
                        }
                    })));
        }
    }

    public IEnumerator<float> LoadSkipIdleLevelDesignCoroutine(Action<bool> callback)
    {
        if (skipIdleLeveDesign == null)
        {
            TextAsset jsonFile = null;
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("SkipIdleLevelDesign", (text) =>
            {
                jsonFile = text;
                if (jsonFile)
                {
                    skipIdleLeveDesign = SkipIdleLevelDesign.FromJson(jsonFile.text);
                }

                callback?.Invoke(text != null);
            })));
        }
    }

    #endregion

    private void Awake()
    {
        instance = this;
    }

    public override IEnumerator<float> InitializeCoroutineHandler()
    {
        TextAsset jsonFile = null;
        //WeaponDesign

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("WeaponDesign", (text) =>
         {
             jsonFile = text;
         })));
        try
        {
            //Weapon Design
            //jsonFile = Resources.Load(DESIGN_PATH + "WeaponDesign") as TextAsset;
            if (jsonFile)
            {
                var allWeapon = AllWeaponDes.FromJson(jsonFile.text);
                DictWeaponDesign = new Dictionary<string, List<WeaponDesign>>();
                if (allWeapon != null && allWeapon.WeaponDesign.Count > 0)
                {
                    foreach (var w in allWeapon.WeaponDesign)
                    {
                        if (DictWeaponDesign.ContainsKey(w.WeaponId))
                        {
                            DictWeaponDesign[w.WeaponId].Add(w);
                        }
                        else
                        {
                            DictWeaponDesign.Add(w.WeaponId, new List<WeaponDesign>() { w });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }
        yield return Timing.WaitForOneFrame;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("HeroDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //Hero design
            //jsonFile = Resources.Load(DESIGN_PATH + "HeroDesign") as TextAsset;
            if (jsonFile)
            {
                var listHeroDesign = AllHeroDesign.FromJson(jsonFile.text);
                DictHeroDesign = new Dictionary<string, List<HeroDesign>>();
                if (listHeroDesign != null && listHeroDesign.HeroDesign.Count > 0)
                {
                    foreach (var h in listHeroDesign.HeroDesign)
                    {
                        if (DictHeroDesign.ContainsKey(h.HeroId))
                        {
                            DictHeroDesign[h.HeroId].Add(h);
                        }
                        else
                        {
                            DictHeroDesign.Add(h.HeroId, new List<HeroDesign>() { h });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }
        yield return Timing.WaitForOneFrame;


        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("IdleHeroDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //DictIdleHeroDesign
            //jsonFile = Resources.Load(DESIGN_PATH + "IdleHeroDesign") as TextAsset;
            if (jsonFile)
            {
                var allIdleDes = AllIdleHeroDesign.FromJson(jsonFile.text);
                DictIdleHeroDesign = new Dictionary<string, List<IdleHeroDesign>>();
                if (allIdleDes != null && allIdleDes.IdleHeroDesign.Count > 0)
                {
                    foreach (var h in allIdleDes.IdleHeroDesign)
                    {
                        if (DictIdleHeroDesign.ContainsKey(h.HeroId))
                        {
                            DictIdleHeroDesign[h.HeroId].Add(h);
                        }
                        else
                        {
                            DictIdleHeroDesign.Add(h.HeroId, new List<IdleHeroDesign>() { h });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }
        yield return Timing.WaitForOneFrame;

        //Zombie design
        //yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ZombieDesign", (text) =>
        //{
        //    jsonFile = text;
        //})));
        //try
        //{
        //    //jsonFile = Resources.Load(DESIGN_PATH + "ZombieDesign") as TextAsset;
        //    if (jsonFile)
        //    {
        //        ListZombieDesign = ZombieDesign.FromJson(jsonFile.text);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"Parse Data error {ex}");
        //}



        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("CampaignLevelDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //Campaign Level design
            //jsonFile = Resources.Load(DESIGN_PATH + "CampaignLevelDesign") as TextAsset;
            if (jsonFile)
            {
                var levelDs = CampaignLevelDesign.FromJson(jsonFile.text);
                if (levelDs != null)
                {
                    LevelCampaignDesign = levelDs.LevelArr;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }


        //IdleDesign

        //ZombieRatioDesign

        //yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ZombieRatioDesign", (text) =>
        //{
        //    jsonFile = text;
        //})));
        //try
        //{
        //    // //zombie ratio
        //    //jsonFile = Resources.Load(DESIGN_PATH + "ZombieRatioDesign") as TextAsset;
        //    if (jsonFile)
        //    {
        //        zombieRatioDesign = ZombieRatioDesign.FromJson(jsonFile.text);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"Parse Data error {ex}");
        //}

        //RewardDesign
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("RewardDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "RewardDesign") as TextAsset;
            if (jsonFile)
            {
                rewardDesign = RewardDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        //CampaignRewardDesign
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("CustomCampaignReward", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //campaign Reward design
            //jsonFile = Resources.Load(DESIGN_PATH + "CustomCampaignReward") as TextAsset;
            if (jsonFile)
            {
                CustomCampaignRewardDesign = CustomCampaignRewardDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        ////IdleRewardDesign 
        //yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("IdleRewardDesign", (text) =>
        //{
        //    jsonFile = text;
        //})));
        //try
        //{
        //    //jsonFile = Resources.Load(DESIGN_PATH + "IdleRewardDesign") as TextAsset;
        //    if (jsonFile)
        //    {
        //        CustomIdleRewardDesign = CustomIdleRewardDesign.FromJson(jsonFile.text);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"Parse Data error {ex}");
        //}

        yield return Timing.WaitForOneFrame;

        //SkillDesign
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("SkillDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            // jsonFile = Resources.Load(DESIGN_PATH + "SkillDesign") as TextAsset;
            if (jsonFile)
            {
                skillDesign = SkillDesign.FromJson(jsonFile.text);
                if (skillDesign != null)
                {
                    _dictSkillDesign = new Dictionary<string, SkillDesignElement>();
                    foreach (var skill in skillDesign.Skills)
                    {
                        _dictSkillDesign.Add(skill.SkillId, skill);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Mission Design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("MissionDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "FusionDesign") as TextAsset;
            if (jsonFile)
            {
                missionDesign = MissionDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;
        
        // Mission Duration Design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("MissionDurationDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "FusionDesign") as TextAsset;
            if (jsonFile)
            {
                missionDurationDesign = MissionDurationDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;
        
        // Fusion Design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("FusionDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "FusionDesign") as TextAsset;
            if (jsonFile)
            {
                fusionDesign = FusionDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Unlock hero design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("UnlockHeroDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "UnlockHeroDesign") as TextAsset;
            if (jsonFile)
            {
                unlockHeroDesign = UnlockHeroDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Chest design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ChestDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "ChestDesign") as TextAsset;
            if (jsonFile)
            {
                chestDesign = ChestDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        //general config
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("GeneralConfig", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "GeneralConfig") as TextAsset;
            if (jsonFile)
            {
                generalConfigDesign = GeneralConfig.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Promotion design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("PromotionDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            // jsonFile = Resources.Load(DESIGN_PATH + "PromotionDesign") as TextAsset;
            if (jsonFile)
            {
                promotionDesign = PromotionDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Shop design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ShopDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            // jsonFile = Resources.Load(DESIGN_PATH + "ShopDesign") as TextAsset;
            if (jsonFile)
            {
                shopDesign = ShopDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Free stuff design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("FreeStuffDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "FreeStuffDesign") as TextAsset;
            if (jsonFile)
            {
                freeStuffDesign = FreeStuffDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ChestHeroDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //campaign Reward design
            //jsonFile = Resources.Load(DESIGN_PATH + "CustomCampaignReward") as TextAsset;
            if (jsonFile)
            {
                chestHeroDesign = ChestHeroDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;
        
        
        // locationPackDesign
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("LocationPackDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "LocationPackDesign") as TextAsset;
            if (jsonFile)
            {
                locationPackDesign = LocationPackDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Talent design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("TalentDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "TalentDesign") as TextAsset;
            if (jsonFile)
            {
                talentDesign = TalentDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Talent upgrade design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("TalentUpgradeDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "TalentUpgradeDesign") as TextAsset;
            if (jsonFile)
            {
                talentUpgradeDesign = TalentUpgradeDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        //yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("BaseCampaignReward", (text) =>
        //{
        //    jsonFile = text;
        //})));
        //try
        //{
        //    //campaign Reward design
        //    // jsonFile = Resources.Load(DESIGN_PATH + "BaseCampaignReward") as TextAsset;
        //    if (jsonFile)
        //    {
        //        BaseCampaignRewardDesign = BaseLevelRewardDesign.FromJson(jsonFile.text);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"Parse Data error {ex}");
        //}

        yield return Timing.WaitForOneFrame;

        //Star Reward design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("StarRewardDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "StarRewardDesign") as TextAsset;
            if (jsonFile)
            {
                starRewardDesign = StarRewardDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        //Attribute text design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("AttributeTextDisplay", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "AttributeTextDisplay") as TextAsset;
            if (jsonFile)
            {
                attributeTextDisplay = AttributeTextDisplay.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        //ManualHero design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ManualHeroDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "ManualHeroDesign") as TextAsset;
            if (jsonFile)
            {
                manualHeroDesign = ManualHeroDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        //Idle chest design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("IdleChestReward", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "IdleChestReward") as TextAsset;
            if (jsonFile)
            {
                this.idleChestDesign = IdleChestReward.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Unlock requirement design
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("UnlockRequirementDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "UnlockRequirementDesign") as TextAsset;
            if (jsonFile)
            {
                this.UnlockRequirementDesign = UnlockRequirementDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;

        // Shop value by level
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ShopValueByLevelDesign", (text) =>
        {
            jsonFile = text;
        })));
        try
        {
            //jsonFile = Resources.Load(DESIGN_PATH + "UnlockRequirementDesign") as TextAsset;
            if (jsonFile)
            {
                this.shopValueByLevelDesign = ShopValueByLevelDesign.FromJson(jsonFile.text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Parse Data error {ex}");
        }

        yield return Timing.WaitForOneFrame;


        if (GameMaster.ENABLE_IDLE_MODE)
        {

            if (this.CustomIdleRewardDesign == null)
            {
                //IdleRewardDesign 
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("CustomIdleRewardDesign", (text) =>
                {
                    jsonFile = text;
                })));
                try
                {
                    //jsonFile = Resources.Load(DESIGN_PATH + "IdleRewardDesign") as TextAsset;
                    if (jsonFile)
                    {
                        CustomIdleRewardDesign = CustomIdleRewardDesign.FromJson(jsonFile.text);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Parse Data error {ex}");
                }
            }
        }

        yield return Timing.WaitForOneFrame;

        if (this.BaseCampaignRewardDesign == null)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("BaseCampaignReward", (text) =>
            {
                jsonFile = text;
            })));
            try
            {
                //campaign Reward design
                // jsonFile = Resources.Load(DESIGN_PATH + "BaseCampaignReward") as TextAsset;
                if (jsonFile)
                {
                    BaseCampaignRewardDesign = BaseLevelRewardDesign.FromJson(jsonFile.text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Parse Data error {ex}");
            }
        }

        Debug.Log("Game Complete init");
        // yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadDesignCoroutine()));
        OnInitializeComplete?.Invoke(true);
    }

    public IEnumerator<float> LoadDesignsGotoGame()
    {
        TextAsset jsonFile = null;

        if (this.ListZombieDesign == null)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ZombieDesign", (text) =>
            {
                jsonFile = text;
            })));
            try
            {
                //jsonFile = Resources.Load(DESIGN_PATH + "ZombieDesign") as TextAsset;
                if (jsonFile)
                {
                    ListZombieDesign = ZombieDesign.FromJson(jsonFile.text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Parse Data error {ex}");
            }
        }

        if (this.mapByLevelDesign == null)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("MapByLevel", (text) =>
            {
                jsonFile = text;
            })));
            try
            {

                if (jsonFile)
                {
                    mapByLevelDesign = MapByLevelDesign.FromJson(jsonFile.text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Parse Data error {ex}");
            }
        }

        if (this.zombieRatioDesign == null)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("ZombieRatioDesign", (text) =>
            {
                jsonFile = text;
            })));
            try
            {
                // //zombie ratio
                //jsonFile = Resources.Load(DESIGN_PATH + "ZombieRatioDesign") as TextAsset;
                if (jsonFile)
                {
                    zombieRatioDesign = ZombieRatioDesign.FromJson(jsonFile.text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Parse Data error {ex}");
            }
        }

        //if (this.BaseCampaignRewardDesign == null)
        //{
        //    yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("BaseCampaignReward", (text) =>
        //    {
        //        jsonFile = text;
        //    })));
        //    try
        //    {
        //        //campaign Reward design
        //        // jsonFile = Resources.Load(DESIGN_PATH + "BaseCampaignReward") as TextAsset;
        //        if (jsonFile)
        //        {
        //            BaseCampaignRewardDesign = BaseLevelRewardDesign.FromJson(jsonFile.text);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError($"Parse Data error {ex}");
        //    }
        //}

        if (this.MapObstacleDesign == null)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("MapObstacleDesign", (text) =>
            {
                jsonFile = text;
            })));
            try
            {
                if (jsonFile)
                {
                    MapObstacleDesign = MapObstacleDesign.FromJson(jsonFile.text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Parse Data error {ex}");
            }
        }

        if (this.MapObstacleRatio == null)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("MapObstacleRatioDesign", (text) =>
            {
                jsonFile = text;
            })));
            try
            {
                if (jsonFile)
                {
                    MapObstacleRatio = MapObstacleRatioDesign.FromJson(jsonFile.text);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Parse Data error {ex}");
            }
        }




        //if(this.CustomCampaignRewardDesign == null)
        //{
        //    yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("CustomCampaignReward", (text) =>
        //    {
        //        jsonFile = text;
        //    })));
        //    try
        //    {
        //        //campaign Reward design
        //        //jsonFile = Resources.Load(DESIGN_PATH + "CustomCampaignReward") as TextAsset;
        //        if (jsonFile)
        //        {
        //            CustomCampaignRewardDesign = CustomCampaignRewardDesign.FromJson(jsonFile.text);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError($"Parse Data error {ex}");
        //    }
        //}

        if (GameMaster.ENABLE_IDLE_MODE)
        {
            if (this.LevelIdleDesign == null)
            {
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("IdleLevelDesign", (text) =>
                {
                    jsonFile = text;
                })));
                try
                {
                    //Idle Level design
                    //jsonFile = Resources.Load(DESIGN_PATH + "IdleLevelDesign") as TextAsset;
                    if (jsonFile)
                    {
                        var levelDs = IdleLevelDesign.FromJson(jsonFile.text);
                        if (levelDs != null)
                        {
                            LevelIdleDesign = levelDs.LevelArr;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Parse Data error {ex}");
                }

            }
        }

    }

    IEnumerator<float> LoadTextAsset(string fileName, Action<TextAsset> callback)
    {
        var op = Addressables.LoadAssetAsync<TextAsset>(fileName);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
        {
            Debug.LogError($"LoadTextAsset failed! {fileName}");
        }

        callback?.Invoke(op.Result);

        //Resources.UnloadAsset(op.Result);
        // DestroyImmediate(op.Result, true);
    }

    public IEnumerator<float> LoadBaseAscendDesign(Action<BaseAscendRewardDesign> callback)
    {
        if (baseAscendRewardDesign == null)
        {
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTextAsset("BaseAscendReward", (jsonFile) =>
            {
                if (jsonFile)
                {
                    baseAscendRewardDesign = BaseAscendRewardDesign.FromJson(jsonFile.text);
                    callback?.Invoke(baseAscendRewardDesign);
                }
                else
                    callback?.Invoke(null);
            })));
        }
        else
        {
            callback?.Invoke(baseAscendRewardDesign);
        }
    }
}

public static class DesignHelper
{
    // public static string DEFAULT_NULL = "~NONE~";
    public static string DEFAULT_NULL = "NULL";
    public static Dictionary<string, long> EquipNameScore;

    public static MissionDurationDesignElement GetMissionDurationDesign(int duration, int level)
    {
        if (level == 0)
            level = 1;

        //return DesignManager.instance.MapObstacleRatio.MapObstacleRatioDesignElements.LastOrDefault(x => x.Level <= level);
        var result = DesignManager.instance.missionDurationDesign.MissionDurationDesignElements.LastOrDefault(x => x.Duration == duration && x.Level <= level);
        
        if(result == null)
            result = DesignManager.instance.missionDurationDesign.MissionDurationDesignElements.FirstOrDefault(x => x.Duration == duration);
        
        return result;
    }

    public static MissionDesignElement GetMissionDesign(string id)
    {
        return DesignManager.instance.missionDesign.MissionDesignElements.Find(x => x.Id == id);
    }
    
    public static Dictionary<string,List<FreeStuffDesignElement>> GetDictFreeStuffDesign()
    {
        Dictionary<string, List<FreeStuffDesignElement>> byIds =
            new Dictionary<string, List<FreeStuffDesignElement>>();
        
        foreach (var element in DesignManager.instance.freeStuffDesign.FreeStuffDesignElements)
        {
            string id = element.Id.Split('/')[0];
            if (byIds.ContainsKey(id))
            {
                byIds[id].Add(element);
            }
            else
            {
                byIds.Add(id, new List<FreeStuffDesignElement>() {element});
            }
        }

        return byIds;
    }
    
    public static UnlockRequirementDesignElement GetUnlockRequirement(string id)
    {
        return DesignManager.instance.UnlockRequirementDesign.UnlockRequirementDesignElements.Find(x => x.Id == id);
    }

    public static Tuple<bool, int> GetUnlockRequirementLevel(string id)
    {
        var design = GetUnlockRequirement(id);
        if (design == null)
            return new Tuple<bool, int>(true, 0);
        else
        {
            return new Tuple<bool, int>(design.IsAvailable(), design.LevelUnlock);
        }
    }

    public static bool IsRequirementAvailable(string id)
    {
        var unlockRequire =
            DesignManager.instance.UnlockRequirementDesign.UnlockRequirementDesignElements.Find(x => x.Id == id);
        return unlockRequire == null || unlockRequire.IsAvailable();
    }

    public static int GetTotalTalentUpgradeByLevel(int level)
    {
        // Debug.LogError(
        // $"Campaign level {level}, total {DesignManager.instance.talentUpgradeDesign.TalentUpgradeDesignElements.FindLastIndex(x => x.LevelCampaign <= level) + 1}");

        return DesignManager.instance.talentUpgradeDesign.TalentUpgradeDesignElements.FindLastIndex(x =>
            x.LevelCampaign <= level) + 1;
    }

    public static string GetValueTextFromAttribute(double value, SkillDesignElement skillDesignElement)
    {
        string valueText = $"{value:0}";

        if (skillDesignElement.SkillId == EffectType.PASSIVE_INCREASE_FIRERATE.ToString() ||
            skillDesignElement.IsPercent())
        {
            valueText = $"{value:0.##}";
            if (skillDesignElement.IsPercent())
            {
                valueText = $"{value:0.##}%";
            }
        }

        return valueText;
    }

    public static string GetAttributeTextDisplay(string id, float value)
    {
        AttributeDisplay min =
            DesignManager.instance.attributeTextDisplay.AttributeDisplay.First(x => x.Id.Contains(id));
        AttributeDisplay max =
            DesignManager.instance.attributeTextDisplay.AttributeDisplay.Last(x => x.Id.Contains(id));

        // Debug.LogError(min.Min + " " + max.Max);
        if (value < min.Min)
            return min.Description;

        if (value > max.Max)
            return max.Description;

        string result = "";

        foreach (var attribute in DesignManager.instance.attributeTextDisplay.AttributeDisplay)
        {
            if (attribute.Id.Contains(id))
            {
                if (value > attribute.Min && value <= attribute.Max)
                    return attribute.Description;
            }
        }

        return result;
    }

    public static TalentUpgradeDesignElement GetTalentUpgradeDesign(int level)
    {
        return DesignManager.instance.talentUpgradeDesign.TalentUpgradeDesignElements[level];
    }

    public static TalentDesignElement GetTalentDesign(TalentData talentData)
    {
        return DesignManager.instance.talentDesign.TalentDesignElements.Find(x => x.ID == talentData.TalentID);
    }

    public static UnlockHeroDesignElement GetUnlockHeroDesignElement(string id)
    {
        return DesignManager.instance.unlockHeroDesign.UnlockHeroDesignElements.Find(x => x.HeroId == id);
    }

    public static LocationPackDesignElement GetLocationPackDesign(string locationID)
    {
        return DesignManager.instance.locationPackDesign.LocationPackDesignElements.Find(x =>
            x.UnlockLocationId == locationID);
    }

    public static string GetLocationIdByLevel(int level)
    {
        if (level < 1)
            level = 1;
        return DesignManager.instance.LevelCampaignDesign[level - 1].LocationId;
    }

    public static long GetWeaponNameScore(string id)
    {
        if (EquipNameScore == null)
        {
            EquipNameScore = new Dictionary<string, long>();
            int startScore = 1;
            foreach (var weapon in DesignManager.instance.DictWeaponDesign)
            {
                EquipNameScore.Add(weapon.Key, startScore++);
            }
        }

        return EquipNameScore[id];
    }

    public static CurrencyType ConvertToCurrencyType(this RewardData data)
    {
        var result = CurrencyType.NONE;
        switch (data._type)
        {
            case REWARD_TYPE.GOLD:
                result = CurrencyType.GOLD;
                break;
            case REWARD_TYPE.DIAMOND:
                result = CurrencyType.DIAMOND;
                break;
            case REWARD_TYPE.TOKEN:
                result = CurrencyType.TOKEN;
                break;
            case REWARD_TYPE.PILL:
                result = CurrencyType.PILL;
                break;
            case REWARD_TYPE.SCROLL_ARMOUR:
                result = CurrencyType.ARMOUR_SCROLL;
                break;
            case REWARD_TYPE.SCROLL_WEAPON:
                result = CurrencyType.WEAPON_SCROLL;
                break;
            case REWARD_TYPE.SCROLL_HERO:
                break;
            case REWARD_TYPE.CHEST:
                break;
            case REWARD_TYPE.KEY_CHEST_LEGENDARY:
                break;
            case REWARD_TYPE.KEY_CHEST_RARE:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }

    public static REWARD_TYPE ConvertToRewardType(CurrencyType type)
    {
        REWARD_TYPE result = REWARD_TYPE.NONE;
        switch (type)
        {
            case CurrencyType.NONE:
                break;
            case CurrencyType.GOLD:
                result = REWARD_TYPE.GOLD;
                break;
            case CurrencyType.DIAMOND:
                result = REWARD_TYPE.DIAMOND;
                break;
            case CurrencyType.TOKEN:
                result = REWARD_TYPE.TOKEN;
                break;
            case CurrencyType.PILL:
                result = REWARD_TYPE.PILL;
                break;
            case CurrencyType.WEAPON_SCROLL:
                result = REWARD_TYPE.SCROLL_WEAPON;
                break;
            case CurrencyType.ARMOUR_SCROLL:
                result = REWARD_TYPE.SCROLL_ARMOUR;
                break;
            default:
                break;
        }

        return result;
    }

    public static WeaponDesign GetWeaponDesign(WeaponData weaponData)
    {
        DesignManager.instance.DictWeaponDesign.TryGetValue(weaponData.WeaponID, out var list);

        if (list != null)
        {
            var result = list.LastOrDefault(x => x.Rarity == weaponData.Rank && weaponData.GetWeaponLevel() >= x.StartLevel);
            if (result == null)
                result = list.FirstOrDefault(x => x.Rarity == weaponData.Rank);

            if (result == null)
            {
                Debug.LogError($"Cant GetWeaponDesign {weaponData.WeaponID}");
            }

            return result;
        }
        else
        {
            Debug.LogError($"Cant GetWeaponDesign {weaponData.WeaponID}");
            return null;
        }
    }

    public static WeaponDesign GetWeaponDesignByLevel(string weaponID, int targetLevel)
    {
        DesignManager.instance.DictWeaponDesign.TryGetValue(weaponID, out var list);
        if (list != null)
        {
            var result = list.LastOrDefault(x => targetLevel >= x.StartLevel);
            if (result == null)
            {
                Debug.LogError($"Cant GetWeaponDesign {weaponID}");
            }

            return result;
        }
        else
        {
            Debug.LogError($"Cant GetWeaponDesign {weaponID}");
            return null;
        }

    }

    public static List<WeaponDesign> GetWeaponDesignAllRanks(string weaponID)
    {
        List<WeaponDesign> result = new List<WeaponDesign>();
        DesignManager.instance.DictWeaponDesign.TryGetValue(weaponID, out var list);
        if (list != null)
        {
            return list;
        }
        else
        {
            Debug.LogError($"Cant GetWeaponDesign {weaponID}");
            return null;
        }

    }

    public static WeaponDesign GetWeaponDesign(string weaponID, int rank, int level)
    {
        var result = DesignManager.instance.DictWeaponDesign[weaponID].LastOrDefault(x => x.Rarity == rank && level >= x.StartLevel);
        if (result == null)
        {
            result = DesignManager.instance.DictWeaponDesign[weaponID]
                .FirstOrDefault(x => x.Rarity == rank);
        }

        return result;
    }

    public static HeroDesign GetHeroDesign(HeroData heroData)
    {
        return GetHeroDesign(heroData.UniqueID, heroData.Rank, heroData.GetHeroLevel());
    }

    public static HeroDesign GetHeroDesign(string heroID, int rank, int level)
    {
        HeroDesign result = null;

        var exists = DesignManager.instance.DictHeroDesign.ContainsKey(heroID);
        if (!exists)
        {
            Debug.LogError($"Cant find hero {heroID}");
            return null;
        }

        result = DesignManager.instance.DictHeroDesign[heroID].LastOrDefault(x => level >= x.StartLevel && x.Rarity == rank);

        if (result == null)
        {
            result = DesignManager.instance.DictHeroDesign[heroID].FirstOrDefault(x => x.Rarity == rank);
        }

        if (result == null)
        {
            result = GetHeroDesignByLevel(heroID, level);
        }



        return result;
    }

    public static HeroDesign GetHeroDesignByLevel(string heroID, int TargetLevel)
    {
        HeroDesign result = null;

        var exists = DesignManager.instance.DictHeroDesign.ContainsKey(heroID);
        if (!exists)
        {
            Debug.LogError($"Cant find hero {heroID}");
            return null;
        }

        result = DesignManager.instance.DictHeroDesign[heroID].LastOrDefault(x => TargetLevel >= x.StartLevel);
        return result;
    }

    public static List<HeroDesign> GetListHeroDesignByRank(string heroID)
    {
        List<HeroDesign> result = new List<HeroDesign>();
        var exists = DesignManager.instance.DictHeroDesign.ContainsKey(heroID);
        if (!exists)
        {
            Debug.LogError($"Cant find hero {heroID}");
            return null;
        }

        result = DesignManager.instance.DictHeroDesign[heroID];

        return result;
    }

    public static int GetHeroMaxRank(string heroID)
    {
        int maxRank = -1;
        List<HeroDesign> listRank = null;
        DesignManager.instance.DictHeroDesign.TryGetValue(heroID, out listRank);
        if (listRank != null && listRank.Count >= 0)
            maxRank = (int)listRank.Last().Rarity;
        return maxRank;
    }

    public static IdleHeroDesign GetIdleHeroDesign(string heroID, int level)
    {
        IdleHeroDesign result = null;
        var exists = DesignManager.instance.DictIdleHeroDesign.ContainsKey(heroID);
        if (!exists)
        {
            Debug.LogError($"Cant find hero {heroID}");
            return null;
        }

        result = DesignManager.instance.DictIdleHeroDesign[heroID].LastOrDefault(x => level >= x.StartLevel);
        return result;
    }

    public static List<IdleHeroDesign> GetListIdleHeroDesignByRank(string heroID)
    {
        List<IdleHeroDesign> result = new List<IdleHeroDesign>();
        var exists = DesignManager.instance.DictIdleHeroDesign.ContainsKey(heroID);
        if (!exists)
        {
            Debug.LogError($"Cant find hero {heroID}");
            return null;
        }

        result = DesignManager.instance.DictIdleHeroDesign[heroID];
        return result;
    }

    public static bool IsEnoughUpgradeMaterial(WeaponUpgradeCostData weaponUpgradeCostData)
    {
        bool isEnoughGold = CurrencyModels.instance.IsEnough(CurrencyType.GOLD, weaponUpgradeCostData.gold);
        bool isEnoughWeaponScroll =
            CurrencyModels.instance.IsEnough(CurrencyType.WEAPON_SCROLL, weaponUpgradeCostData.weaponScroll);
        bool isEnoughArmourScroll =
            CurrencyModels.instance.IsEnough(CurrencyType.ARMOUR_SCROLL, weaponUpgradeCostData.armourScroll);
        return isEnoughGold && isEnoughArmourScroll && isEnoughWeaponScroll;
    }

    public static LevelElementDesign GetCampaignLevelDesign(int level)
    {
        LevelElementDesign result = null;
        result = DesignManager.instance.LevelCampaignDesign.Find(x => x.Level == level);
        if (result == null)
        {
            Debug.LogError($"Cant find level campaign {level}");
        }

        return result;
    }

    public static LevelElementDesign GetIdleLevelDesign(int level)
    {
        LevelElementDesign result = null;
        result = DesignManager.instance.LevelIdleDesign.Find(x => x.Level == level);
        if (result == null)
        {
            Debug.LogError($"Cant find level campaign {level}");
        }

        return result;
    }

    public static WaveControl GetWaveControlByType(this LevelElementDesign levelDs, int wave)
    {
        WaveControl result = null;

        int WaveIndex = wave - 1;
        if (WaveIndex < 0 || wave > levelDs.TotalWave)
        {
            Debug.LogError($"GetWaveControlByType errror!!! with {wave}");
            return null;
        }

        switch (levelDs.WaveType)
        {
            case QuickType.LevelWave.WaveType.Sequence:
                int waveCounter = 0;
                foreach (var waveDs in levelDs.WaveControl)
                {
                    waveCounter += (int)waveDs.Loop;
                    if (WaveIndex < waveCounter)
                    {
                        result = waveDs;
                        break;
                    }
                }

                break;
            case QuickType.LevelWave.WaveType.Zigzac:
                int totalWave = levelDs.WaveControl.Count;
                if (WaveIndex == levelDs.TotalWave - 1)
                {
                    result = levelDs.WaveControl[totalWave - 1];
                }
                else
                {
                    var targetIndex = WaveIndex % (totalWave - 1);
                    result = levelDs.WaveControl[targetIndex];
                }


                break;
            default:
                break;
        }

        if (result == null)
        {
            Debug.LogError($"Cant not find {wave} in {levelDs.Level}");
        }

        return result;
    }

    public static Dictionary<string, ZombieElement> _dictZombieCached = new Dictionary<string, ZombieElement>();

    public static ZombieElement GetZombieDesign(string zombieID)
    {
        ZombieElement result = null;
        _dictZombieCached.TryGetValue(zombieID, out result);
        if (result == null)
        {
            result = DesignManager.instance.ListZombieDesign.ZombieElement.FirstOrDefault(x => x.ZombieId == zombieID);
            if (!_dictZombieCached.ContainsKey(zombieID))
                _dictZombieCached.Add(zombieID, result);
        }

        return result;
    }

    public static void GetEquipSprite(string idSprite, Action<Sprite> callback)
    {
        ResourceManager.instance.GetEquipSprite(idSprite, callback);
    }

    public static RankDefine GetRankDefine(long rank)
    {
        RankDefine result = null;
        ResourceManager.instance.rankDefine.TryGetValue(rank, out result);
        return result;
    }

    public static ZombieRatioDesignElement getZombieRatioDesign(string ratioID)
    {
        ZombieRatioDesignElement result = null;
        if (DesignManager.instance.zombieRatioDesign != null)
        {
            result = DesignManager.instance.zombieRatioDesign.ZombieRatioDesignZombieRatioDesign.FirstOrDefault(x =>
                x.RatioId == ratioID);
            if (result == null)
            {
                Debug.LogError($"Cant find ratio design {ratioID}");
            }
        }
        else
        {
            Debug.LogError("ZombieRatioDesignElement null!!!");
        }

        return result;
    }

    public static SkipIdleLevelDesignElement GetSkipIdleLevelDesign(int idleLevel)
    {
        return DesignManager.instance.skipIdleLeveDesign.SkipIdleLevelDesignSkipIdleLevelDesign.LastOrDefault(x => idleLevel >= x.Level);
    }

    public static int GetMaxLevelIdleMode()
    {
        return (int)GetConfigDesign(GameConstant.MAX_LEVEL_IDLE_MODE).Value;
    }

    #region REWARD

    //public static List<RewardData> ConvertToRewardData(this CampaignReward design)
    //{
    //    List<RewardData> result = new List<RewardData>();

    //    if (design != null)
    //    {
    //        if (design.EarnGold > 0)
    //        {
    //            result.Add(new RewardData(REWARD_TYPE.GOLD, design.EarnGold));
    //        }

    //        if (design.EarnPill > 0)
    //        {
    //            result.Add(new RewardData(REWARD_TYPE.PILL, design.EarnPill));
    //        }

    //        // if (design.EarnScrap > 0)
    //        // {
    //        //     // result.Add(new RewardData(SaveGameHelper.REWARD_TYPE.SCRAP, design.EarnScrap));
    //        // }

    //        if (design.EarnToken > 0)
    //        {
    //            result.Add(new RewardData(REWARD_TYPE.TOKEN, design.EarnToken));
    //        }
    //    }

    //    return result;
    //}

    public static RewardDesignElement GetRewardDesign(string RewardID)
    {
        RewardDesignElement result = null;

        if (RewardID.Contains("NONE"))
            return null;

        if (DesignManager.instance.rewardDesign != null)
        {
            result = DesignManager.instance.rewardDesign.Rewards.FirstOrDefault(x => x.RewardID == RewardID);

            if (result == null)
            {
                Debug.LogError($"GetRewardDesign error with {RewardID}");
            }
        }
        else
        {
            Debug.LogError("rewardDesign is null!!!!");
        }

        return result;
    }

    public static BaseRewardElement GetBaseLevelRewardDesign(int finishedLevel)
    {
        BaseRewardElement baseReward = null;
        //foreach (var item in DesignManager.instance.BaseCampaignRewardDesign.RewardArr)
        //{
        //    if (finishedLevel >= item.StartLevel)
        //    {
        //        baseReward = item;
        //        break;
        //    }
        //}

        baseReward = DesignManager.instance.BaseCampaignRewardDesign.RewardArr.LastOrDefault(x => finishedLevel >= x.StartLevel);

        return baseReward;
    }

    public static CustomBattleReward GetCustomLevelRewardDesign(int level)
    {
        CustomBattleReward result = null;

        result = DesignManager.instance.CustomCampaignRewardDesign.Rewards.FirstOrDefault(x => x.LevelReward == level);
        return result;
    }

    public static CustomBattleReward GetNearestNextCustomReward(int curLevel)
    {
        CustomBattleReward result = null;

        result = DesignManager.instance.CustomCampaignRewardDesign.Rewards.FirstOrDefault(
            x => x.LevelReward >= curLevel);
        return result;
    }

    public static REWARD_TYPE ConvertToRewardType(string rewardID)
    {
        REWARD_TYPE result = REWARD_TYPE.NONE;

        var rwd = rewardID.ToUpper();

        if (rwd.Contains("GOLD"))
            result = REWARD_TYPE.GOLD;
        else if (rwd.Contains("DIAMOND"))
            result = REWARD_TYPE.DIAMOND;
        else if (rwd.Contains("TOKEN"))
            result = REWARD_TYPE.TOKEN;
        else if (rwd.Contains("PILL"))
            result = REWARD_TYPE.PILL;
        else if (rwd.Contains("SCROLL_ARMOUR"))
            result = REWARD_TYPE.SCROLL_ARMOUR;
        else if (rwd.Contains("SCROLL_WEAPON"))
            result = REWARD_TYPE.SCROLL_WEAPON;
        else if (rwd.Contains("SCROLL_HERO"))
            result = REWARD_TYPE.SCROLL_HERO;
        else if (rwd.Contains("KEY_CHEST_RARE"))
            result = REWARD_TYPE.KEY_CHEST_RARE;
        else if (rwd.Contains("KEY_CHEST_HERO"))
            result = REWARD_TYPE.KEY_CHEST_HERO;
        else if (rwd.Contains("KEY_CHEST_LEGENDARY"))
            result = REWARD_TYPE.KEY_CHEST_LEGENDARY;
        else if (rwd.Contains("ADD_ON"))
            result = REWARD_TYPE.ADD_ON;
        else if (
            rwd.Contains("EQUIP") ||
            rwd.Contains("WEAPON") ||
            rwd.Contains("ARMOUR") ||
            rwd.Contains("PISTOL") ||
            rwd.Contains("RIFLE") ||
            rwd.Contains("BAZOOKA"))
            result = REWARD_TYPE.EQUIP;
        else
        {
            Debug.LogError($"Can not convert {rwd}");
        }

        return result;
    }

    public static CurrencyType GetCurrencyByGameMode(GameMode mode)
    {
        CurrencyType result = CurrencyType.NONE;

        if (mode == GameMode.CAMPAIGN_MODE)
            result = CurrencyType.GOLD;
        else
            result = CurrencyType.TOKEN;

        return result;
    }

    public static RewardData defaultRewarData(REWARD_TYPE type, long value, object extraData = null)
    {
        return new RewardData()
        {
            _type = type,
            _value = value,
            _extends = extraData
        };
    }

    #endregion

    #region Skill

    public static SkillDesignElement GetSkillDesign(string skillID)
    {
        SkillDesignElement result = null;

        DesignManager.instance._dictSkillDesign.TryGetValue(skillID, out result);

        if (result == null)
        {
            //then try add postfix "/1"
            DesignManager.instance._dictSkillDesign.TryGetValue(skillID + "/1", out result);
        }

        return result;
    }

    public static List<SkillDesignElement> GetSkillDesignByType(SkillType skillType)
    {
        List<SkillDesignElement> result = new List<SkillDesignElement>();

        foreach (var item in DesignManager.instance._dictSkillDesign)
        {
            if (item.Value.SkillType == skillType)
            {
                result.Add(item.Value);
            }
        }

        return result;
    }

    public static EffectType ConvertPassiveSkillToEffectType(string skillID)
    {
        //get skill level
        skillID = ConvertSkillID(skillID);

        EffectType result = EffectType.NONE;

        result = skillID.ParseEnum<EffectType>(EffectType.NONE);
        if (result == EffectType.NONE)
        {
            Debug.LogError($"Cant convert to EffectType with {skillID}");
        }

        return result;
    }

    public static string ConvertEffectTypeToPassiveSkill(EffectType type)
    {
        string result = "";
        result = type.ToString().Trim();
        return result;
    }

    public static string ConvertSkillID(string skillID)
    {
        var splits = skillID.Split('/');
        if (splits != null && splits.Length > 0)
        {
            //check last char is number
            var last = splits[splits.Length - 1];
            int SkillLevel = 0;
            int.TryParse(last, out SkillLevel);

            if (SkillLevel != 0)
            {
                skillID = splits[0];
            }
        }

        return skillID;
    }

    #endregion

    #region General Config

    public static ConfigDesign GetConfigDesign(string key)
    {
        ConfigDesign result = null;

        result = DesignManager.instance.generalConfigDesign.Config.FirstOrDefault(x => x.KeyId == key);
        if (result == null)
        {
            Debug.LogError($"cant find config design with {key}");
        }

        return result;
    }

    #endregion

    #region AirDrop

    public static List<AirDropElement> GetAirDropDesign(string AirDropID, int currentLevel)
    {
        List<AirDropElement> result = new List<AirDropElement>();
        if (DesignManager.instance.airDropDesign == null)
        {
            Debug.LogError("Airdrop design not loaded!!!");
            return null;
        }

        foreach (var item in DesignManager.instance.airDropDesign.Arr)
        {
            if (item.AirDropId.Contains(AirDropID) && currentLevel >= item.LevelRequire)
            {
                result.Add(item);
            }
        }
        //result = DesignManager.instance.airDropDesign.Arr.FirstOrDefault(x => x.AirDropId == AirDropID);

        return result;
    }

    public static AirDropElement PickRandAirDrop(this List<AirDropElement> _listAirDrop)
    {
        AirDropElement result = null;
        if (_listAirDrop != null && _listAirDrop.Count > 0)
        {
            var randIdenx = UnityEngine.Random.Range(0, _listAirDrop.Count);
            result = _listAirDrop[randIdenx];
        }

        return result;
    }

    public static List<AirDropElement> GetAirDropDesignByLevel(int level)
    {
        List<AirDropElement> result = new List<AirDropElement>();
        if (DesignManager.instance.airDropDesign == null)
        {
            Debug.LogError("Airdrop design not loaded!!!");
            return null;
        }

        var levelIdeal = DesignManager.instance.airDropDesign.Arr.LastOrDefault(x =>
            level >= x.LevelRequire && !x.AirDropId.Contains("DAILY_FREE_ADD_ON"));
        if (levelIdeal != null)
        {
            string ID = levelIdeal.AirDropId;
            //var splits = levelIdeal.AirDropId.Split('/');
            //if (splits != null && splits.Length > 0)
            //    ID = splits[0];

            result = GetAirDropDesign(ID, level);
        }

        return result;
    }

    public static List<AirDropElement> GetAirDropDesignByLevelWithPrefix(int level, string prefix)
    {
        List<AirDropElement> result = new List<AirDropElement>();
        if (DesignManager.instance.airDropDesign == null)
        {
            Debug.LogError("Airdrop design not loaded!!!");
            return null;
        }

        var levelIdeal = DesignManager.instance.airDropDesign.Arr.LastOrDefault(x =>
          level >= x.LevelRequire && x.AirDropId.Contains(prefix));
        if (levelIdeal != null)
        {
            string ID = levelIdeal.AirDropId;
            var splits = levelIdeal.AirDropId.Split('/');
            if (splits != null && splits.Length > 0)
                ID = splits[0];

            result = GetAirDropDesign(ID, level);
        }

        return result;
    }

    public static Tuple<List<RewardData>, List<RewardData>> GetListAirDropRewards(GameMode gameMode, int currentLevel)
    {
        Tuple<List<RewardData>, List<RewardData>> result = null;

        var listAirDrop = GetAirDropDesignByLevelWithPrefix(currentLevel, "ADS_AIR_DROP");
        if (listAirDrop != null)
        {
            List<RewardData> listNormal = new List<RewardData>();
            List<RewardData> listExtra = new List<RewardData>();

            var randReward = listAirDrop.PickRandAirDrop();

            float numCurrency = gameMode == GameMode.CAMPAIGN_MODE
                ? randReward.BaseGold + randReward.StepGold * currentLevel
                : randReward.BaseToken + randReward.StepToken * currentLevel;
            RewardData currencyRwd = defaultRewarData(
                gameMode == GameMode.CAMPAIGN_MODE ? REWARD_TYPE.GOLD : REWARD_TYPE.TOKEN, (int)numCurrency);

            if (numCurrency > 0)
            {
                listNormal.Add(currencyRwd);
                listExtra.Add(currencyRwd);
            }

            Dictionary<string, float> dictRand = new Dictionary<string, float>();
            Dictionary<string, long> dictNum = new Dictionary<string, long>();
            foreach (var extra in randReward.Extra)
            {
                if (!dictRand.ContainsKey(extra.RewardId))
                {
                    dictRand.Add(extra.RewardId, extra.Weight);
                    dictNum.Add(extra.RewardId, extra.Value);
                }

            }

            for (int i = 0; i < randReward.NumExtra; i++)
            {
                var randExtra = dictRand.RandomElementByWeight(x => x.Value);
                var designReward = GetRewardDesign(randExtra.Key);
                if (designReward != null)
                {
                    long value = dictNum[designReward.RewardID];
                    listExtra.Add(defaultRewarData(designReward.RewardType, value, designReward.RewardID));
                }

                if (dictRand.Count > 1)
                    dictRand.Remove(randExtra.Key);
            }

            result = new Tuple<List<RewardData>, List<RewardData>>(listNormal, listExtra);
        }


        return result;
    }

    public static List<RewardData> GetAirDropRewardsByPrefix(GameMode gameMode, int currentLevel, string prefix)
    {
        List<RewardData> result = new List<RewardData>();
        var listAirDrop = GetAirDropDesignByLevelWithPrefix(currentLevel, prefix);
        if (listAirDrop != null)
        {
            var randReward = listAirDrop.PickRandAirDrop();

            float numCurrency = gameMode == GameMode.CAMPAIGN_MODE
                ? randReward.BaseGold + randReward.StepGold * currentLevel
                : randReward.BaseToken + randReward.StepToken * currentLevel;
            RewardData currencyRwd = defaultRewarData(
                gameMode == GameMode.CAMPAIGN_MODE ? REWARD_TYPE.GOLD : REWARD_TYPE.TOKEN, (int)numCurrency);

            if (numCurrency > 0)
            {
                result.Add(currencyRwd);
            }

            Dictionary<string, float> dictRand = new Dictionary<string, float>();
            Dictionary<string, long> dictNum = new Dictionary<string, long>();
            foreach (var extra in randReward.Extra)
            {
                if (!dictRand.ContainsKey(extra.RewardId))
                {
                    dictRand.Add(extra.RewardId, extra.Weight);
                    dictNum.Add(extra.RewardId, extra.Value);
                }

            }

            for (int i = 0; i < randReward.NumExtra; i++)
            {
                var randExtra = dictRand.RandomElementByWeight(x => x.Value);
                var designReward = GetRewardDesign(randExtra.Key);
                if (designReward != null)
                {
                    long value = dictNum[designReward.RewardID];
                    result.Add(defaultRewarData(designReward.RewardType, value, designReward.RewardID));
                }

                if (dictRand.Count > 1)
                    dictRand.Remove(randExtra.Key);
            }

        }

        return result;

    }

    public static List<RewardData> GetListDailyFreeAdsOnDesign()
    {
        List<RewardData> result = new List<RewardData>();
        int campaignLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel;
        var listAirDrop = GetAirDropDesign("DAILY_FREE_ADD_ON", campaignLevel);
        Dictionary<string, float> dictRand = new Dictionary<string, float>();
        foreach (var extra in listAirDrop.PickRandAirDrop().Extra)
        {
            dictRand.Add(extra.RewardId, extra.Weight);
        }


        for (int i = 0; i < listAirDrop.PickRandAirDrop().NumExtra; i++)
        {
            var randExtra = dictRand.RandomElementByWeight(x => x.Value);
            var designReward = GetRewardDesign(randExtra.Key);
            if (designReward != null)
            {
                result.Add(defaultRewarData(designReward.RewardType, 1, designReward.RewardID));
            }

            if (dictRand.Count > 1)
                dictRand.Remove(randExtra.Key);
        }

        return result;
    }

    #endregion

    #region Level Deisng

    public static List<SubwaveDispenser> ParseSubwaveDispenser(string subWave)
    {
        if (subWave == null)
            return null;
        List<SubwaveDispenser> result = null;
        var pairs = subWave.Split(',');
        if (pairs != null && pairs.Length > 0)
        {
            result = new List<SubwaveDispenser>();
            foreach (var p in pairs)
            {
                var splitter = p.Split(':');
                if (splitter.Length >= 2)
                {
                    result.Add(new SubwaveDispenser()
                    {
                        totalPercent = int.Parse(splitter[0].Trim()),
                        delaySpawn = FBUtils.ParseFloatFromString(splitter[1].Trim())
                    });
                }
            }
        }

        return result;
    }

    #endregion

    #region ManualHeroDesign

    public static ManualHeroDesignElement GetManualHeroDesign(int rarity)
    {
        ManualHeroDesignElement result = null;

        if (DesignManager.instance.manualHeroDesign.ListManualHeroDesign != null &&
            DesignManager.instance.manualHeroDesign.ListManualHeroDesign.Count > 0)
        {
            result =
                DesignManager.instance.manualHeroDesign.ListManualHeroDesign.FirstOrDefault(x => x.Rarity == rarity);
        }

        if (result == null)
        {
            Debug.LogError($"CANT FIND ManualHeroDesignElement with rarity {rarity} ");
        }

        return result;
    }

    #endregion

    #region Idle Chest Reward

    public static IdleChestRewardElement GetIdleChestDesign(int currentCampaignLevel)
    {
        IdleChestRewardElement result = null;

        if (DesignManager.instance.idleChestDesign.IdleChestRewardIdleChestReward != null &&
            DesignManager.instance.idleChestDesign.IdleChestRewardIdleChestReward.Count > 0)
        {
            result = DesignManager.instance.idleChestDesign.IdleChestRewardIdleChestReward.LastOrDefault(x =>
                currentCampaignLevel >= x.Level);
        }

        if (result == null)
        {
            Debug.LogError("GetIdleChestDesign faill!!!");
        }

        return result;
    }

    #endregion

    #region AscendReward
    public static BaseAscendReward GetBaseAscendRewardsByLevel(int idleLevel)
    {
        BaseAscendReward ds = null;
        ds = DesignManager.instance.baseAscendRewardDesign.BaseAscendReward.LastOrDefault(x => idleLevel >= x.StartLevel);
        return ds;
    }

    #endregion

    #region Format reward string
    public static KeyValuePair<string, int> FormatEquipRewardStr(string rawRewardString)
    {
        var spliter = rawRewardString.Split('/');
        if (spliter != null && spliter.Length >= 2)
        {
            int rank = 1;
            int.TryParse(spliter[1], out rank);
            return new KeyValuePair<string, int>(spliter[0], rank);
        }
        return new KeyValuePair<string, int>(rawRewardString, 1);
    }

    public static string FormatHeroScrollReward(string rawRewardString)
    {
        var formatted = rawRewardString.Replace("SCROLL_", "");
        return formatted;

    }
    #endregion

    #region Map Obstacles

    public static MapObstacleRatioDesignElement GetCurrentMapObstacleRatioDesign(int level)
    {
        if (level == 0)
            level = 1;

        return DesignManager.instance.MapObstacleRatio.MapObstacleRatioDesignElements.LastOrDefault(x => x.Level <= level);
    }

    public static MapObstaclesDesignElement GetObstacleData(Vector2 scale, OBSTACLE_TYPE type)
    {
        return DesignManager.instance.MapObstacleDesign.MapObstaclesDesignElements.Find(x =>
            x.GetSize() == scale && x.ObstacleType == type);
    }

    #endregion

    #region Map By LEVEL
    public static MAP_NAME GetMapByLevel(int level)
    {
        MAP_NAME result = MAP_NAME.MAP_DESERT_DAY;
        if (DesignManager.instance.mapByLevelDesign != null)
        {
            var finder = DesignManager.instance.mapByLevelDesign.MapByLevel.LastOrDefault(x => level >= x.CampaignLevel);
            if (finder != null)
                result = finder.MapId;
            else
                Debug.LogError($"Cant find map by level in level {level}");
        }

        return result;
    }

    public static MAP_NAME GetCurrentMapByLevel()
    {
        return GetMapByLevel(SaveGameHelper.GetCurrentCampaignLevel());
    }
    #endregion

    public static ShopValueByLevelDesignElement GetShopValueByLevel(string id, int level)
    {
        return DesignManager.instance.shopValueByLevelDesign.ShopValueByLevelDesignShopValueByLevelDesign.LastOrDefault(x => id == x.Id && level >= x.StartLevel);
    }


    public static string GetRandomHeroID()
    {
        List<string> availableHero = new List<string>();
        
        foreach (var VARIABLE in DesignManager.instance.unlockHeroDesign.UnlockHeroDesignElements)
        {
            if (VARIABLE.Available)
            {
                availableHero.Add(VARIABLE.HeroId);
            }
        }

        return availableHero.PickRandom();
    }
}