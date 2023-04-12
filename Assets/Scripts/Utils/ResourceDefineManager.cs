using System;
using System.Collections;
using System.Collections.Generic;
using QuickType.Shop;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using Enum = System.Enum;

[Serializable]
public class ContainSpriteBaseClass
{
    public AssetReference spriteAddress;

    public void GetSprite(Action<Sprite> callback)
    {
        ResourceManager.instance.GetSprite(spriteAddress, sprite => { callback?.Invoke(sprite); });
    }
}

[Serializable]
public class EquipSpriteDefine : ContainSpriteBaseClass
{
    public string id;
}

[Serializable]
public class UltimateSpriteDefine : EquipSpriteDefine
{
    public AssetReference bgAddress;

    public UltimateSpriteDefine(string id)
    {
        this.id = id;
    }
}

[Serializable]
public class RankDefine : ContainSpriteBaseClass
{
    public long rank;
    public string name;
    public Color color;
}

[Serializable]
public class CurrencySpriteDefine : ContainSpriteBaseClass
{
    public CurrencyType type;

    public CurrencySpriteDefine(CurrencyType type)
    {
        this.type = type;
    }
}

//
// [Serializable]
// public class AttributeDefine
// {
//     public AttributeID id;
//     public Sprite sprite;
// }

[Serializable]
public class EquipTypeSpriteDefine : ContainSpriteBaseClass
{
    public int type;
}

[Serializable]
public class RewardSpriteDefine : ContainSpriteBaseClass
{
    public REWARD_TYPE type;
}

[Serializable]
public class CostTypeSpriteDefine : ContainSpriteBaseClass
{
    public CostType type;
}

[Serializable]
public class MapObstacleDefine
{
    public Vector2 size;
    public List<AssetReference> prefabs;
    public List<AssetReference> explodePrefabs;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourceDefineManager", order = 1)]
public class ResourceDefineManager : ScriptableObject
{
    //public List<EquipSpriteDefine> equipSpriteDefines;

    public List<RankDefine> rankDefines;

    // public List<AttributeDefine> attributeDefines;
    public List<CurrencySpriteDefine> currencySpriteDefines;
    public List<EquipTypeSpriteDefine> equipTypeSpriteDefines;
    public List<UltimateSpriteDefine> ultimateSpriteDefines;
    public List<RewardSpriteDefine> rewardSpriteDefines;
    public List<EquipSpriteDefine> shopSpriteDefines;
    public List<EquipSpriteDefine> talentSpriteDefines;
    public List<CostTypeSpriteDefine> costTypeSpriteDefines;
    public List<AssetReference> talentRankSpriteDefines;
    public List<AssetReference> randomEquipSprites;
    public List<AssetReference> starSprites;
    public List<AssetReference> makerSprites;
    public List<MapObstacleDefine> mapObstacleDefines;
    
    [Button("Save Data")]
    public void SaveData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    
    private void OnEnable()
    {
        // if (ultimateSpriteDefines != null)
        // {
        //     string groupName = "Ultimate";
        //     
        //     var addressableSettings =AddressableAssetSettingsDefaultObject.Settings;
        //     if (addressableSettings.IsNotUniqueGroupName(groupName))
        //         addressableSettings.RemoveGroup(addressableSettings.FindGroup(groupName));
        //     // Recreate new Group
        //     var newGroup = addressableSettings.CreateGroup(groupName, typeof(UnityEngine.Object), false, false);
        //     
        //     foreach (var VARIABLE in ultimateSpriteDefines)
        //     {
        //         var assetPath =  AssetDatabase.GetAssetPath(VARIABLE.bg); 
        //         var relativeAssetPath = assetPath.Replace(Application.dataPath, "Assets");
        //         var assetGUID = AssetDatabase.AssetPathToGUID(relativeAssetPath);
        //         addressableSettings.CreateOrMoveEntry(assetGUID, newGroup);
        //         
        //     }
        // }
        // Init currency enum
        // if (currencySpriteDefines != null && currencySpriteDefines.Count == 0)
        // {
        //     currencySpriteDefines = new List<CurrencySpriteDefine>();
        //     foreach (CurrencyType currencyType in (CurrencyType[]) Enum.GetValues(typeof(CurrencyType)))
        //     {
        //         currencySpriteDefines.Add(new CurrencySpriteDefine(currencyType,null));
        //     } 
        // } 

        // if (talentSpriteDefines != null && talentSpriteDefines.Count == 0)
        // {
        //     talentSpriteDefines = new List<EquipSpriteDefine>();
        //
        //     foreach (TALENT currencyType in (TALENT[]) Enum.GetValues(typeof(TALENT)))
        //     {
        //         var a = new EquipSpriteDefine();
        //         a.id = currencyType.ToString();
        //         a.sprite = null;
        //         talentSpriteDefines.Add(a);
        //     }
        // }

        // if (ultimateSpriteDefines != null)
        // {
        //     //talentSpriteDefines = new List<EquipSpriteDefine>();
        //
        //     foreach (EffectType effectType in (EffectType[]) Enum.GetValues(typeof(EffectType)))
        //     {
        //         var a = new EquipSpriteDefine();
        //         a.id = effectType.ToString();
        //         a.sprite = null;
        //         ultimateSpriteDefines.Add(a);
        //     }
        // }
    }

    public void ConvertDataToDictionary(
        //Dictionary<string, Sprite> equipDict,
        Dictionary<long, RankDefine> rankDict,
        Dictionary<CurrencyType, AssetReference> currencySpriteDict,
        Dictionary<string, UltimateSpriteDefine> ultimateSpritesDict,
        Dictionary<REWARD_TYPE, AssetReference> rewardsSpritesDict,
        Dictionary<string, AssetReference> shopDict,
        Dictionary<string, AssetReference> talentDict,
        Dictionary<CostType, AssetReference> costTypeDict)
    {
        //foreach (var equipSpriteDefine in equipSpriteDefines)
        //{
        //    equipDict.Add(equipSpriteDefine.id, equipSpriteDefine.sprite);
        //}

        foreach (var rankDefine in rankDefines)
        {
            rankDict.Add(rankDefine.rank, rankDefine);
        }

        // foreach (var attributeDefine in attributeDefines)
        // {
        //     attributeDict.Add(attributeDefine.id.ToString(), attributeDefine.sprite);
        // }

        foreach (var currencySpriteDefine in currencySpriteDefines)
        {
            currencySpriteDict.Add(currencySpriteDefine.type,currencySpriteDefine.spriteAddress);
            // ResourceManager.instance.GetSprite(currencySpriteDefine.spriteAddress,
                // sprite => { currencySpriteDict.Add(temp.type, sprite); });
        }

        foreach (var ultiDef in ultimateSpriteDefines)
        {
            ultimateSpritesDict[ultiDef.id] = ultiDef;
        }

        foreach (var rewarDef in rewardSpriteDefines)
        {
            rewardsSpritesDict.Add(rewarDef.type, rewarDef.spriteAddress);
        }

        foreach (var shopDef in shopSpriteDefines)
        {
            shopDict.Add(shopDef.id, shopDef.spriteAddress); //shopDef.sprite);
        }

        foreach (var talentDef in talentSpriteDefines)
        {
            talentDict.Add(talentDef.id, talentDef.spriteAddress); // talentDef.sprite);
        }

        foreach (var cost in costTypeSpriteDefines)
        {
            costTypeDict.Add(cost.type, cost.spriteAddress);
        }
    }
}