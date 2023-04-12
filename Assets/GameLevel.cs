using System;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using Doozy.Engine.Extensions;
using Ez.Pooly;
using MEC;
using QuickType;
using QuickType.MapObstacle;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class GameLevel : MonoBehaviour
{
    public List<Transform> _listHeroesMarker;
    public List<Transform> _listTowerMarker;
    public SkillZoneMarkers _skillZoneMarker;
    public Transform _markerManualHero;


    [Header("Random Obstacles")] public Transform RandomObstacleParents;

    public Transform _castleTrf;
    public Transform _attackableLine;

    [Header("For Clamp zombie speed!!!")] public Transform _clampZombieSpeedLine;

    private List<Health> _listPlayerHealthBoxes;
    public CastleHealth castleHealth;

    [Header("Screen Castle HP Bar")] public HealthBar _screenHealthBarPrefab;

    [Header("Camera")] public Camera _camera;

    public Dictionary<string, Character> _dictCharacter;

    private GameMode _mode = GameMode.NONE;

    [Header("Direct Light")] public Light _gameDirectLight;

    public DecorDataElement MapDecorData { get; private set; }
    public MapDecor _mapDecor { get; private set; }

    public Character ManualHero { get; private set; }


    public Transform obstacleParent;
    //public void Initialize(GameMode mode, MAP_NAME mapName = MAP_NAME.MAP_CITY)
    //{
    //    Timing.RunCoroutine(InitializeCoroutine(mode, mapName));
    //}

    private MAP_NAME _currentMapName = MAP_NAME.NONE;

    public IEnumerator<float> InitializeCoroutine(GameMode mode, MAP_NAME mapName = MAP_NAME.MAP_CITY_DAY)
    {
        _currentMapName = mapName;
        _listPlayerHealthBoxes = new List<Health>();
        _mode = mode;
        _dictCharacter = new Dictionary<string, Character>();
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_TEAM_SLOTS, new Action(ResetTeamInBattle));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(InitTeamCoroutine()));
        if (this._mapDecor != null)
        {
            Destroy(this._mapDecor);
        }

        // var mapDecorPrefab = Resources.Load<MapDecor>(string.Format("Levels/Decors/{0}", mapName.ToString()));
        MapDecorData = ResourceManager.instance.GetDecorSetting(mapName);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(InitMapDecor(mode)));

        if (_mode == GameMode.CAMPAIGN_MODE ||
            _mode == GameMode.IDLE_MODE)
        {
            castleHealth.gameObject.SetActiveIfNot(true);
            //castleHealth.UpdateCamera();

            Vector2 hpBarPoint = Camera.main.WorldToScreenPoint(castleHealth.transform.position);
            var _hpBar = InGameCanvas.instance.SpawnCastleHPBar(hpBarPoint);
            castleHealth.hpBar = _hpBar;
            castleHealth.Initialize(0);

            _listPlayerHealthBoxes.Add(castleHealth);
        }
        else
        {
            castleHealth.gameObject.SetActiveIfNot(false);
        }

        yield break;
    }

    public IEnumerator<float> ResetMapDecor(MAP_NAME mapName)
    {
        if (this._currentMapName != mapName)
        {
            this._currentMapName = mapName;

            if (this._mapDecor != null)
            {
                GameObject.Destroy(this._mapDecor.gameObject);
            }

            MapDecorData = ResourceManager.instance.GetDecorSetting(mapName);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(InitMapDecor(GameMaster.instance.currentMode)));
        }
    }

    private int _lastRandomMap = -1;

    public void CleanObstacles()
    {
        for (int i = obstacleParent.childCount - 1; i >= 0; i--)
        {
            var child = obstacleParent.GetChild(i);
            if (child != null)
            {
                var mapObstacle = child.GetComponent<BaseMapObstacle>();
                if (mapObstacle != null)
                {
                    mapObstacle.OnDie(true);
                    Pooly.Despawn(child);
                }
            }

        }
    }

    public void GenerateObstacle()
    {
        CleanObstacles();
        if (_mode == GameMode.IDLE_MODE)
        {
            return;
        }

        int currentLevel = _mode == GameMode.CAMPAIGN_MODE
            ? SaveGameHelper.GetCurrentCampaignLevel()
            : SaveGameHelper.GetCurrentIdleLevel();

        // Get obstacle ratio design from level
        MapObstacleRatioDesignElement design = DesignHelper.GetCurrentMapObstacleRatioDesign(currentLevel);

        // Find obstacle map data by id
        var mapData = ResourceManager.instance.mapObstacleDataSo.Datas.Find(x => x.id == design.MapObstacleId);
        if (mapData == null)
        {
            mapData = ResourceManager.instance.mapObstacleDataSo.Datas.Last();
        }

        // Lad wrapper
        MapObstacleDataWrapper wrapper = mapData.datasWrapper;

        // Just load test in editor
#if UNITY_EDITOR
        if (ResourceManager.instance.mapObstacleDataSo.useTest)
        {
            wrapper = ResourceManager.instance.mapObstacleDataSo.testModel;
        }
#endif

        float totalHP = SaveManager.Instance.Data.CalcTotalCastleHP();
        var currentMapName = DesignHelper.GetCurrentMapByLevel();

        if (currentMapName == MAP_NAME.NONE)
            currentMapName = MAP_NAME.MAP_DESERT_DAY;

        foreach (var data in wrapper.DataElements)
        {
            string prefabName = "";
            ObstacleByMapName byMapName = null;

            if (data.prefabByName != null && data.prefabByName.Count > 0)
            {
                byMapName = data.prefabByName.Find(x =>
                    x.mapName.Contains(currentMapName)); // == MAP_TYPE.ALL || x.mapType == mapType);

                if (byMapName == null)
                {
                    byMapName = data.prefabByName.Find(x => x.mapName.Contains(MAP_NAME.MAP_DESERT_DAY));
                }


                if (byMapName == null)
                    byMapName = data.prefabByName.Find(x => x.mapName.Contains(MAP_NAME.NONE));

                if (byMapName != null)
                    prefabName = byMapName.prefabName;
            }

            var prefabTransform = ResourceManager.instance
                .GetMapObstacle(data.scale, data.type == OBSTACLE_TYPE.EXPLODE, prefabName);

            if (prefabTransform == null)
            {
                Debug.LogError($"{mapData.id},SCALE {data.scale}, {data.type}, {prefabName}");
            }

            var prefab = prefabTransform.transform;

            var obstacleDesignData = DesignHelper.GetObstacleData(data.scale, data.type);

            var obstacle = Pooly.Spawn<ObstacleProtectGate>(prefab,
                data.position,
                Quaternion.identity, obstacleParent);

            obstacle.transform.localScale = byMapName?.scale ?? Vector3.one;
            if (byMapName != null)
                obstacle.transform.rotation = Quaternion.Euler(byMapName.rotation);

            // Debug.LogError(data.scale + " " + data.type); 
            float hp = totalHP * obstacleDesignData.HpPercent / 100f;
            // Debug.LogError("TOTAL HP "+ totalHP +"HP : " + hp + " PERCENT " + obstacleDesignData.HpPercent);
            obstacle.Initialize(hp);
            obstacle.HideAllParticle();

            if (data.type == OBSTACLE_TYPE.EXPLODE)
            {
                var explodeComponent = (ObstacleExplode)obstacle;
                explodeComponent.SetDamage(obstacleDesignData.BaseDmg +
                                           obstacleDesignData.StepDmg * GamePlayController.instance.CurrentLevel);
            }
        }
    }

    public void ResetCastleHealth()
    {
        castleHealth.ResetHPBar();
    }

    public void UpdateFormation()
    {
        var formations = SaveManager.Instance.Data.GameData.TeamSlots;
        List<string> newHeroes = new List<string>();
        for (int i = 0; i < formations.Count; i++)
        {
            var hero = formations[i];
            if (hero != null && hero != GameConstant.NONE)
            {
                //SaveManager.Instance.Data.ApplyFreeSlotMember(hero.UniqueID);
                newHeroes.Add(hero);

                if (_dictCharacter.ContainsKey(hero))
                {
                    _dictCharacter[hero].transform.SetParent(_listHeroesMarker[i]);
                    _dictCharacter[hero].transform.localPosition = Vector3.zero;
                    _dictCharacter[hero].transform.localScale = Vector3.one;
                }
                else
                {
                    Timing.RunCoroutine(SpawnCharacter(hero, false, _listHeroesMarker[i], (heroPref) =>
                    {
                        _dictCharacter.Add(hero, heroPref);
                        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_UPGRADE_BUTTON_VIEW, hero);

                        // Check to show highlight
                        InGameCanvas.instance._gamePannelView.OnUpdateFormation();
                    }));
                }
            }
        }

        // Remove old heroes
        List<string> removedHero = new List<string>();
        foreach (var character in _dictCharacter)
        {
            if (!newHeroes.Contains(character.Key))
            {
                // character.Value.DestroyHero();
                character.Value.DestroyHero();
                // Destroy(character.Value.gameObject);
                removedHero.Add(character.Key);
            }
        }

        foreach (var heroId in removedHero)
        {
            SaveManager.Instance.Data.GetHeroData(heroId).ItemStatus = ITEM_STATUS.Available;
            _dictCharacter.Remove(heroId);
        }

        InGameCanvas.instance._gamePannelView.LockRemoveHero(newHeroes.Count < 4);

        SaveManager.Instance.SetDataDirty();

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_CAMPAIGN_DATA);

        // Check to show highlight
        InGameCanvas.instance._gamePannelView.OnUpdateFormation();
    }

    public void ChangeWeaponHeroInMap(HeroData heroData)
    {
        var WeaponData = SaveManager.Instance.Data.GetWeaponData(heroData.EquippedWeapon);
        var character = _dictCharacter[heroData.UniqueID];
        // character.Initialize(heroData);
        character.UpdateWeaponData(WeaponData);

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_UPGRADE_BUTTON_VIEW, heroData.UniqueID);
    }

    public void OnUpgradeWeapon(HeroData heroData)
    {
        var character = _dictCharacter[heroData.UniqueID];
        character.ResetHeroData();
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_UPGRADE_BUTTON_VIEW, heroData.UniqueID);
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.RESET_TEAM_SLOTS, new Action(ResetTeamInBattle));
    }

    public IEnumerator<float> InitTeamCoroutine()
    {
        var teamData = SaveManager.Instance.Data.GameData.TeamSlots;
        bool waitTask = true;
        if (teamData != null && teamData.Count > 0)
        {
            int index = 0;

            Character heroPref = null;
            foreach (var hero in teamData)
            {
                if (hero != GameConstant.NONE)
                {
                    waitTask = true;
                    Timing.RunCoroutine(SpawnCharacter(hero, false, _listHeroesMarker[index], (heroCallback) =>
                    {
                        heroPref = heroCallback;
                        waitTask = false;
                    }));
                    while (waitTask)
                        yield return Timing.WaitForOneFrame;

                    _dictCharacter.Add(hero, heroPref);
                }

                index++;
            }
        }

        var manualData = SaveManager.Instance.Data.Inventory.ManualHero;

        waitTask = true;
        Timing.RunCoroutine(SpawnCharacter(manualData.UniqueID, false, transform, (heroCallback) =>
        {
            ManualHero = heroCallback;
            waitTask = false;
        }));
        while (waitTask)
            yield return Timing.WaitForOneFrame;

        ManualHero.EnableHero(false);
    }

    public void ResetGameLevel()
    {
        if (_listPlayerHealthBoxes != null)
        {
            foreach (var health in _listPlayerHealthBoxes)
            {
                health.gameObject.SetActiveIfNot(true);
            }
        }

        if (_dictCharacter != null)
        {
            foreach (var h in _dictCharacter)
            {
                h.Value.ResetHero();
            }
        }


        var listRemains = GameObject.FindObjectsOfType<AutoDespawnParticles>();
        if (listRemains != null && listRemains.Length > 0)
        {
            for (int i = listRemains.Length - 1; i >= 0; i--)
            {
                listRemains[i].CleanUp();
            }
        }

        var listSkills = GameObject.FindObjectsOfType<BaseCharacterUltimate>();
        if (listSkills != null && listSkills.Length > 0)
        {
            for (int i = 0; i < listSkills.Length; i++)
            {
                listSkills[i].ResetSkill(true);
            }
        }

        GameMaster.instance.ResetEffectController();

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.REPLAY_CAMPAIGN_BATTLE);


    }

    private IEnumerator<float> SpawnCharacter(string heroID, bool spawnHpBar, Transform parent,
        Action<Character> callback)
    {
        Character character = null;
        //Character heroPref = ResourceManager.instance.GetHeroPrefab(heroID);
        var op = Addressables.LoadAssetAsync<GameObject>(heroID);
        while (!op.IsDone)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (op.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"cant loaded hero {heroID}");
            callback?.Invoke(null);
            yield break;
        }

        Character heroPref = op.Result.GetComponent<Character>();
        if (heroPref != null)
        {
            character = Instantiate(heroPref);
            HeroData heroData = SaveManager.Instance.Data.GetHeroData(heroID);
            character.transform.SetParent(parent);
            character.transform.localPosition = Vector3.zero;
            character.transform.localScale = Vector3.one;
            character.gameObject.SetActiveIfNot(true);
            character.Initialize(heroData);
            yield return Timing.WaitForOneFrame;
            if (spawnHpBar)
            {
                //PlayerHealth heroHealth = Pooly.Spawn<PlayerHealth>(POOLY_PREF.PLAYER_HEALTH, Vector3.zero,
                //    Quaternion.identity, character.transform);

                AddressableManager.instance.SpawnObjectAsync<PlayerHealth>(POOLY_PREF.PLAYER_HEALTH, (hp) =>
                {
                    PlayerHealth heroHealth = hp;
                    heroHealth.transform.SetParent(character.transform);
                    heroHealth.transform.localPosition = Vector3.zero;
                    heroHealth.transform.localRotation = Quaternion.identity;

                    if (heroHealth != null)
                    {
                        var Power = heroData.GetPowerDataByGameMode();
                        heroHealth.Initialize(Power.Hp);
                        heroHealth.transform.localPosition = Vector3.zero;

                        character.ApplyHealth(heroHealth);
                        _listPlayerHealthBoxes.Add(heroHealth);
                    }
                });
            }
        }

        callback?.Invoke(character);
        yield break;
    }

    public Vector3 GetManualHeroPos()
    {
        return this._markerManualHero.position;
    }

    public void ResetTeamInBattle()
    {
        var teamData = SaveManager.Instance.Data.GameData.TeamSlots;
        if (teamData != null && teamData.Count > 0)
        {
            int index = 0;
            foreach (var hero in teamData)
            {
                if (!_dictCharacter.ContainsKey(hero) && hero != GameConstant.NONE)
                {
                    //Character character = SpawnCharacter(hero, _mode == GameMode.IDLE_MODE);
                    Timing.RunCoroutine(SpawnCharacter(hero, false, _listHeroesMarker[index].transform,
                        (character) => { _dictCharacter.Add(character.HeroID, character); }));
                }

                index++;
            }
        }

        GamePlayController.instance.campaignData?.ResetData();
    }

    public Health FindRandPlayer()
    {
        Health result = null;

        if (_listPlayerHealthBoxes != null && _listPlayerHealthBoxes.Count > 0)
        {
            int rand = Random.Range(0, _listPlayerHealthBoxes.Count);
            result = _listPlayerHealthBoxes[rand];
        }

        return result;
    }

    public void CleanUp()
    {
        foreach (KeyValuePair<string, Character> hero in _dictCharacter)
        {
            hero.Value.DestroyHero();
        }

        if (ManualHero)
            ManualHero.DestroyHero();

        ResetGameLevel();
    }

    public void ResetLevel()
    {
        foreach (KeyValuePair<string, Character> hero in _dictCharacter)
        {
            hero.Value.RestartLevel();
        }

        if (ManualHero)
            ManualHero.RestartLevel();

        ResetGameLevel();
    }

    #region Random Obstacles In Map

    public static int MIN_RAND_OBS = 0;
    public static int MAX_RAND_OBS = 3;

    private List<BaseMapObstacle> _listMapObstacles;
    private Transform[] _listObstacleMarkers;

    public void InitializeRandObstacle()
    {
        // int numObs = UnityEngine.Random.Range(MIN_RAND_OBS, MAX_RAND_OBS + 1);
        // _listMapObstacles = new List<BaseMapObstacle>();
        // _listObstacleMarkers = RandomObstacleParents.GetComponentsInChildren<Transform>();
        //
        // var listRandObs = DesignHelper.PickRandomMapObstacles(numObs);
        // if (listRandObs != null && listRandObs.Count > 0)
        // {
        //     float castleHP = GamePlayController.instance.CastleHP.GetHPWithCoeff();
        //     foreach (var item in listRandObs)
        //     {
        //         AddressableManager.instance.SpawnObjectAsync<BaseMapObstacle>(item, (obs) =>
        //         {
        //             var ds = DesignHelper.GetMapObstacleDesign(item);
        //             var _hp = castleHP * ds.HpPercent * 1.0f / 100f;
        //             obs.Initialize(_hp);
        //             _listMapObstacles.Add(obs);
        //         });
        //     }
        // }
    }

    #endregion

    #region Map Decoration

    public IEnumerator<float> InitMapDecor(GameMode mode)
    {
        if (this.MapDecorData != null)
        {
            var op = Addressables.LoadAssetAsync<GameObject>(MapDecorData.DecorPrefab);
            while (!op.IsDone)
                yield return Timing.WaitForOneFrame;

            _mapDecor = GameObject.Instantiate(op.Result, transform).GetComponent<MapDecor>();
            _mapDecor.Initialize(mode);
            _mapDecor.transform.localPosition = Vector3.zero;
            _mapDecor.transform.localScale = Vector3.one;

            if (_mapDecor._listBarrierViews != null && _mapDecor._listBarrierViews.Count > 0)
            {
                this.castleHealth.meshBarriers = _mapDecor._listBarrierViews;
            }

            ResetMapEnviroment();
        }
        else
            yield break;
    }

    public void UpdateRuntimeMapDecorEditor()
    {
        if (_mapDecor != null)
            ResetMapEnviroment();
    }

    public void ResetMapEnviroment()
    {
        _mapDecor.SetGroundColor(MapDecorData._mapGroundColor);
        _mapDecor.ResetObjectsColor(this._currentMapName);
        RenderSettings.ambientGroundColor = MapDecorData._groundColor * MapDecorData._groundIntensity;

        RenderSettings.ambientEquatorColor = MapDecorData._equatorColor * MapDecorData._equatorIntensity;
        RenderSettings.ambientSkyColor = MapDecorData._sourceColor * MapDecorData._sourceIntensity;
        RenderSettings.reflectionIntensity = MapDecorData.IntensityMultiplie;
        RenderSettings.reflectionBounces = MapDecorData.Bounces;

        _gameDirectLight.color = MapDecorData._directLightColor;
        _gameDirectLight.intensity = MapDecorData._directLightIntensity;
    }

    #endregion

    public void Update()
    {
#if UNITY_EDITOR
        UpdateRuntimeMapDecorEditor();
#endif
    }
}