using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Google.Protobuf.WellKnownTypes;
using QuickType.Shop;
using static SaveGameHelper;
using com.datld.data;
using System.Linq;
using Doozy.Engine.Utils.ColorModels;
using UnityEngine.AddressableAssets;
using MEC;
using UnityEngine.UI;
using Ez.Pooly;
using QuickEngine.Extensions;

[Serializable]
public class SpriteKey
{
    public string ID;
    public Sprite sprite;
}

[Serializable]
public class GunWithType
{
    public WEAPON_TYPE type;
    public Gun prefab;
    public Launcher launcher;
}

[Serializable]
public class CustomLimitPrefab
{
    public string PrefabID;
    public int Limit;
}

[Serializable]
public class SpecialBulletPrefab
{
    public EffectType _type = EffectType.NONE;
    public Transform _bulletRef;
}



public class ResourceManager : BaseSystem<ResourceManager>
{
    public const string GAME_RESOURCE_PATH = "GameData";
    public static ResourceManager instance;

    [Header("Scriptable Object")] public HeroResourcesSO _heroResources;
    public ResourceDefineManager resourceDefineManager;

    //public Dictionary<string, Sprite> equipSprites;
    public Dictionary<long, RankDefine> rankDefine;
    public Dictionary<CurrencyType, AssetReference> currencySprites;
    public Dictionary<string, AssetReference> heroSprites;
    public Dictionary<string, AssetReference> heroInactiveSprites;
    public Dictionary<string, UltimateSpriteDefine> ultimateSprites;
    public Dictionary<REWARD_TYPE, AssetReference> rewardSprites;
    public List<AssetReference> randomEquipSprites;
    public Dictionary<string, AssetReference> shopSprites;
    public Dictionary<string, AssetReference> talentSprites;
    public Dictionary<CostType, AssetReference> costTypeSprites;
    public Dictionary<Vector2, List<GameObject>> mapObstacleModels;
    public Dictionary<Vector2, List<GameObject>> mapObstacleExplodeModels;
    public MapObstacleDataSO mapObstacleDataSo;

    [Header("Effects use")]
    //public AutoDespawnParticles _parBloodFX;
    //public AutoDespawnParticles _parExplodeZombie;
    public EffectResourcesSO _effectResources;

    [Header("IAP Manager")] public IAPManagerSO _iAPManagerSO;

    [Header("Mask")] public LayerMask _maskZombieOnly;
    public LayerMask _maskHero;
    public LayerMask _maskHeroAndZombie;


    [Header("SFX&BGM")] public SoundBankSO _soundBank;

    [Header("GUN")] public GunResourceSO _gunBank;

    [Header("ZombieSpawn")] public ZombieSpawnDefineSO _zomSpawnDefine;

    private Dictionary<AssetReference, Sprite> LoadedSprite;

    [Header("Limit Pooly Prefab")] public List<CustomLimitPrefab> _listCustomLimitPrefab;

    [Header("Special Bullets")]
    public List<SpecialBulletPrefab> _listSpecialBullet;

    [Header("Decor Settings")]
    public MapDecorSO MapDecorSO;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadedSprite = new Dictionary<AssetReference, Sprite>();

        // SpriteDefineManager
        //equipSprites = new Dictionary<string, Sprite>();
        rankDefine = new Dictionary<long, RankDefine>();
        currencySprites = new Dictionary<CurrencyType, AssetReference>();
        heroSprites = new Dictionary<string, AssetReference>();
        heroInactiveSprites = new Dictionary<string, AssetReference>();
        ultimateSprites = new Dictionary<string, UltimateSpriteDefine>();
        rewardSprites = new Dictionary<REWARD_TYPE, AssetReference>();
        shopSprites = new Dictionary<string, AssetReference>();
        talentSprites = new Dictionary<string, AssetReference>();
        costTypeSprites = new Dictionary<CostType, AssetReference>();
        randomEquipSprites = resourceDefineManager.randomEquipSprites;

        resourceDefineManager.ConvertDataToDictionary(rankDefine, currencySprites,
            ultimateSprites, rewardSprites, shopSprites, talentSprites, costTypeSprites);

        foreach (var heroResource in _heroResources.listHeroResources)
        {
            heroSprites.Add(heroResource.HeroID, heroResource.spActiveAddress);
            heroInactiveSprites.Add(heroResource.HeroID, heroResource.spInActiveAddress);
        }
    }

    public GameObject GetMapObstacle(Vector2 size, bool isExplode, string prefabName)
    {
        if (prefabName.IsNullOrEmpty())
        {
            return isExplode ? mapObstacleExplodeModels[size].PickRandom() : mapObstacleModels[size].PickRandom();
        }
        else
        {
            var listResult = isExplode ? mapObstacleExplodeModels[size] : mapObstacleModels[size];
            return listResult.Find(x => x.name.Contains(prefabName));
        }
    }

    public void GetSprite(AssetReference reference, Action<Sprite> callback)
    {
        if (reference == null)
        {
            callback?.Invoke(null);
            return;
        }

        Timing.RunCoroutine(GetSpriteCoroutine(reference, callback));
    }

    private IEnumerator<float> GetSpriteCoroutine(AssetReference reference, Action<Sprite> callback)
    {
        if (reference != null &&  !reference.IsValid())
        {
            // Debug.LogError("START GET SPRITE" + reference);

            var op = reference.LoadAssetAsync<Sprite>();
            while (!op.IsDone)
                yield return Timing.WaitForOneFrame;

            LoadedSprite.Add(reference, op.Result);
            callback?.Invoke(op.Result);
        }
        else
        {
            // Debug.LogError("LOADED " + reference);

            if (LoadedSprite.ContainsKey(reference))
            {
                callback?.Invoke(LoadedSprite[reference]);
            }
            else
            {
                while (!LoadedSprite.ContainsKey(reference))
                    yield return Timing.WaitForOneFrame;

                callback?.Invoke(LoadedSprite[reference]);
            }
        }
    }

    #region HERO

    public void GetRandomEquipSpriteByRank(int rank, Action<Sprite> callback)
    {
        GetSprite(randomEquipSprites[rank - 1], callback);
    }

    //public Character GetHeroPrefab(string heroID)
    //{
    //    var path = $"{GAME_RESOURCE_PATH}/Hero/{heroID}";
    //    Character result = Resources.Load<Character>(path);
    //    return result;
    //}

    public void GetHeroAvatar(string heroID, Image img)
    {
        heroSprites.TryGetValue(heroID, out var result);
        GetSprite(result, s => { img.sprite = s; });
    }

    public void GetHeroAvatar(string heroID, Action<Sprite> callback)
    {
        heroSprites.TryGetValue(heroID, out var result);
        GetSprite(result, callback);
    }

    public void GetHeroAvatarInactive(string heroID, Action<Sprite> callback)
    {
        heroInactiveSprites.TryGetValue(heroID, out var result);
        GetSprite(result, callback);
    }

    #endregion

    public Zombie GetZombiePrefab(string zombieID)
    {
        var path = $"{GAME_RESOURCE_PATH}/Zombie/{zombieID}";
        Zombie result = Resources.Load<Zombie>(path);
        return result;
    }

    public void GetShopItemSprite(string shopItemID, Action<Sprite> callback)
    {
        shopSprites.TryGetValue(shopItemID, out var result);
        GetSprite(result, callback);
    }

    public void GetTalentSprite(string talentID, Action<Sprite> callback)
    {
        talentSprites.TryGetValue(talentID, out var result);
        GetSprite(result, callback);
    }

    public void GetTalentRankSprite(int rank, Action<Sprite> callback)
    {
        // Debug.LogError(rank);
        GetSprite(resourceDefineManager.talentRankSpriteDefines[rank - 1], callback);
    }

    public void GetEquipSprite(string idSprite, Action<Sprite> callback)
    {
        Sprite result = null;
        //equipSprites.TryGetValue(idSprite, out var result);
        var gunData = _gunBank.GetGunVariant(idSprite);
        if (gunData != null)
        {
            GetSprite(gunData.gunIconAddress, callback);
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    public RankDefine GetRankDefine(long rank)
    {
        rankDefine.TryGetValue(rank, out var result);
        return result;
    }

    public void GetEquipTypeSprite(int equipType, Action<Sprite> callback)
    {
        GetSprite(resourceDefineManager.equipTypeSpriteDefines[equipType].spriteAddress,
            sprite => { callback?.Invoke(sprite); });
        // return resourceDefineManager.equipTypeSpriteDefines[equipType].sprite;
    }

    public void GetRewardSprite(REWARD_TYPE rewardID, Action<Sprite> callback, string extendedData = null)
    {
        Sprite result = null;
        switch (rewardID)
        {
            case REWARD_TYPE.NONE:
                break;
            case REWARD_TYPE.DIAMOND:
                GetCurrencySprite(CurrencyType.DIAMOND, callback);
                return;
            case REWARD_TYPE.GOLD:
                GetCurrencySprite(CurrencyType.GOLD, callback);
                return;
            case REWARD_TYPE.TOKEN:
                GetCurrencySprite(CurrencyType.TOKEN, callback);
                return;
            case REWARD_TYPE.PILL:
                GetCurrencySprite(CurrencyType.PILL, callback);
                return;
            case REWARD_TYPE.SCROLL_ARMOUR:
                GetCurrencySprite(CurrencyType.ARMOUR_SCROLL, callback);
                return;
            case REWARD_TYPE.SCROLL_WEAPON:
                GetCurrencySprite(CurrencyType.WEAPON_SCROLL, callback);
                return;
            case REWARD_TYPE.SCROLL_HERO:
                string formatHero = DesignHelper.FormatHeroScrollReward(extendedData);
                GetHeroAvatar(formatHero, callback);
                return;
            case REWARD_TYPE.CHEST:
                Debug.LogError("Not define yet!!!");
                break;
            case REWARD_TYPE.KEY_CHEST_RARE:
            case REWARD_TYPE.KEY_CHEST_LEGENDARY:
            case REWARD_TYPE.KEY_CHEST_HERO:
            case REWARD_TYPE.REVIVE:
                GetSprite(rewardSprites[rewardID], callback);
                return;
            case REWARD_TYPE.EQUIP:
                //result = rewardSprites[rewardID];
                var formattedEquip = DesignHelper.FormatEquipRewardStr((string)extendedData);
                var equipDesign = DesignHelper.GetWeaponDesign(formattedEquip.Key, formattedEquip.Value, 1);
                if (equipDesign != null)
                {
                    GetEquipSprite(equipDesign.WeaponId, callback);
                    return;
                }

                break;
            case REWARD_TYPE.ADD_ON:
                GetUltimateSprite(extendedData, callback);
                return;
            case REWARD_TYPE.RANDOM_EQUIP:
                GetRandomEquipSpriteByRank(Int32.Parse(extendedData), callback);
                return;
            default:
                break;
        }

        callback?.Invoke(result);
    }

    public void GetMarkerBG(MARKER_STATE state, Image img)
    {
        GetSprite(resourceDefineManager.makerSprites[(int)state], (s) => { img.sprite = s; });
    }

    public void GetStarSprite(bool isActive, Image img)
    {
        int index = isActive ? 1 : 0;
        GetSprite(resourceDefineManager.starSprites[index], (s) => { img.sprite = s; });
    }

    public void GetCurrencySprite(CurrencyType type, Action<Sprite> callback)
    {
        currencySprites.TryGetValue(type, out var result);
        GetSprite(result, callback);
    }

    public void GetCurrencySprite(CurrencyType type, Image image, Action<Sprite> callback = null)
    {
        GetCurrencySprite(type, s =>
        {
            image.sprite = s;
            callback?.Invoke(s);
        });
    }

    public void GetCostTypeSprite(CostType type, Action<Sprite> callback)
    {
        switch (type)
        {
            case CostType.NONE:
                break;
            case CostType.DIAMOND:
            case CostType.GOLD:
                GetCurrencySprite(type.ConvertToCurrencyType(), callback);
                return;
            case CostType.FREE:
            case CostType.ADS:
            case CostType.IAP:
                costTypeSprites.TryGetValue(type, out var result);
                GetSprite(result, callback);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void GetUltimateSprite(string ultimateID, Action<Sprite> callback)
    {
        UltimateSpriteDefine result = null;
        var prefixSkill = DesignHelper.ConvertSkillID(ultimateID);
        ultimateSprites.TryGetValue(prefixSkill, out result);

        GetSprite(result.spriteAddress, callback);
        // return result?.sprite;
    }

    public UltimateSpriteDefine GetFullUltimateSprite(string ultimateID)
    {
        UltimateSpriteDefine result = null;
        var prefixSkill = DesignHelper.ConvertSkillID(ultimateID);
        ultimateSprites.TryGetValue(prefixSkill, out result);

        return result;
    }

    //public Gun GetGunPrefab(string weaponID)
    //{
    //    Gun result = null;
    //    var gunDef = _gunBank.GetGunVariant(weaponID);
    //    if (gunDef != null)
    //        result = gunDef.gunPrefab;

    //    if (result == null)
    //        Debug.LogError($"Cant get gun pref {weaponID}");
    //    return result;
    //}

    public void SpawnGun(string weaponID, Action<Gun> callback)
    {
        Timing.RunCoroutine(SpawnGunAsync(weaponID, callback));
    }

    public IEnumerator<float> SpawnGunAsync(string weaponID, Action<Gun> callback)
    {
        var gunDef = _gunBank.GetGunVariant(weaponID);
        if (gunDef != null)
        {
            if (gunDef.gunAddress != null)
            {
                var op = gunDef.gunAddress.InstantiateAsync(null);

                while (!op.IsDone)
                    yield return Timing.WaitForOneFrame;

                callback?.Invoke(op.Result.GetComponent<Gun>());
            }
            else
            {
                Debug.LogError($"SpawnGunAsync fail with {weaponID}");
            }
        }
    }

    public IEnumerator<float> LoadMapObstacleModel()
    {
        if (mapObstacleModels == null)
        {
            var opObstacle = Addressables.LoadAssetAsync<MapObstacleDataSO>("MapObstacleDataSO");
            while (!opObstacle.IsDone)
                yield return Timing.WaitForOneFrame;

            mapObstacleDataSo = opObstacle.Result;
            mapObstacleModels = new Dictionary<Vector2, List<GameObject>>();
            mapObstacleExplodeModels = new Dictionary<Vector2, List<GameObject>>();

            foreach (var VARIABLE in resourceDefineManager.mapObstacleDefines)
            {
                var result = new List<GameObject>();

                foreach (var VARIABLE1 in VARIABLE.prefabs)
                {
                    var op = VARIABLE1.LoadAssetAsync<GameObject>();
                    while (!op.IsDone)
                    {
                        yield return Timing.WaitForOneFrame;
                    }

                    result.Add(op.Result);
                }


                if (VARIABLE.explodePrefabs != null && VARIABLE.explodePrefabs.Count > 0)
                {
                    var explodeResult = new List<GameObject>();
                    foreach (var VARIABLE1 in VARIABLE.explodePrefabs)
                    {
                        var op = VARIABLE1.LoadAssetAsync<GameObject>();
                        while (!op.IsDone)
                        {
                            yield return Timing.WaitForOneFrame;
                        }

                        explodeResult.Add(op.Result);
                    }

                    mapObstacleExplodeModels.Add(VARIABLE.size, explodeResult);
                }

                mapObstacleModels.Add(VARIABLE.size, result);
            }
        }

        yield break;
    }

    public OverWriteBullet GetOverwriteBullet(string weaponID)
    {
        OverWriteBullet result = null;
        var gunVariant = _gunBank.GetGunVariant(weaponID);
        if (gunVariant != null)
        {
            result = gunVariant.overwriteBullet;
        }

        return result;
    }

    public Launcher getLauncherByType(WEAPON_TYPE type)
    {
        //var exists = _listWeaponDefine.FirstOrDefault(x => x.type == type);
        //return exists != null ? exists.launcher : null;

        Launcher launcher = null;

        var launcherDef = _gunBank.GetLauncherVariant(type);
        if (launcherDef != null)
            launcher = launcherDef.launcher;

        if (launcher == null)
        {
            Debug.LogError($"Cant gegetLauncherByType {type}");
        }

        return launcher;
    }

    #region Custom limit Prefab

    public void LimitAllPrefabs()
    {
        foreach (var item in _listCustomLimitPrefab)
        {
            Pooly.ResetClonesToLimit(item.PrefabID, item.Limit);
        }
    }

    #endregion

    public DecorDataElement GetDecorSetting(MAP_NAME name)
    {
        DecorDataElement result = null;
        result = this.MapDecorSO.mapDecors.FirstOrDefault(x => x.mapName == name);
        return result;
    }

    public Transform GetSpecialBulletPrefab(EffectType effectType)
    {
        var finder = _listSpecialBullet.FirstOrDefault(x => x._type == effectType);
        if (finder != null)
            return finder._bulletRef;
        else
            return null;
    }
}