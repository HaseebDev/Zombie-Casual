using com.datld.data;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum MAP_NAME
{
    NONE,
    MAP_CITY_DAY, //UNUSED
    MAP_DESERT_DAY,
    MAP_CITY_NIGHT,
    MAP_DESERT_NIGHT,
    MAP_SNOW_DAY,
    MAP_SNOW_NIGHT, //UNUSED
    MAP_RED_SOIL_DAY,
    MAP_RED_SOILD_NIGHT, //UNUSED
    MAP_GRASS_DAY,
    MAP_GRASS_NIGHT //UNUSED

}

public partial class SROptions
{
    private DEBUG_NET_STATE _netState = DEBUG_NET_STATE.NONE;

    [Category("NETWORK")]
    public DEBUG_NET_STATE NetState {
        get { return _netState; }
        set { _netState = value; }
    }

    private bool _disableUI = false;

    [Category("CAPTURE VIDEO MARKETING")]
    public bool DisableUI {
        get { return _disableUI; }
        set {
            _disableUI = value;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.CHEAT_DISABLE_OBJECT, _disableUI);
        }
    }

    private bool _disableObstacle = false;

    [Category("CAPTURE VIDEO MARKETING")]
    public bool DisableObstacle {
        get { return _disableObstacle; }
        set {
            _disableObstacle = value;
            GameObject.FindObjectOfType<GameLevel>().obstacleParent.gameObject.SetActive(value);
        }
    }

    [Category("CAPTURE VIDEO MARKETING")]
    public void PreinitSpawnTeam()
    {
        SaveManager.Instance.Data.GameData.TeamSlots[0] = "HERO_1";
        SaveManager.Instance.Data.GameData.TeamSlots[1] = "NONE";
        SaveManager.Instance.Data.GameData.TeamSlots[2] = "NONE";
        SaveManager.Instance.Data.GameData.TeamSlots[3] = "NONE";
        SaveManager.Instance.SaveData();
    }

    [Category("CAPTURE VIDEO MARKETING")]
    public void CheatSpawnTeam()
    {
        SaveManager.Instance.Data.GameData.TeamSlots[0] = "HERO_1";
        SaveManager.Instance.Data.GameData.TeamSlots[1] = "HERO_2";
        SaveManager.Instance.Data.GameData.TeamSlots[2] = "HERO_3";
        SaveManager.Instance.Data.GameData.TeamSlots[3] = "HERO_4";

        int i = 0;
        foreach (var VARIABLE in GameObject.FindObjectsOfType<HeroSlot>())
        {
            if (i <= 3)
            {
                VARIABLE.OnSelectCheat(
                    SaveManager.Instance.Data.GameData.TeamSlots[VARIABLE.transform.GetSiblingIndex()]);
            }

            i++;
        }

        SaveManager.Instance.SaveData();

        GamePlayController.instance.gameLevel.UpdateFormation();
    }

    // [Category("[TestFusionAnim]")]
    // public void ShowHUDFusion()
    // {
    //     MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_FUSION_RESULT,false, null,SaveManager.Instance.Data.Inventory.ListWeaponData.First());
    // }


    [Category("[GamePlay]")]
    public void CheatKillCastle()
    {
        var castleHealth = GameObject.FindObjectOfType<CastleHealth>();
        if (castleHealth)
        {
            castleHealth.SetDamage(10000f);
        }
    }

    [Category("[GamePlay]")]
    public void KillAllZombie()
    {
        GamePlayController.instance._waveController.ClearAllZombieOnWave();
    }

    [Category("[GamePlay]")]
    public void CompleteLevel1Stars()
    {
        GamePlayController.instance.CheatFinishLevel(1);
    }

    [Category("[GamePlay]")]
    public void CompleteLevel2Stars()
    {
        GamePlayController.instance.CheatFinishLevel(2);
    }

    [Category("[GamePlay]")]
    public void CompleteLevel3Stars()
    {
        GamePlayController.instance.CheatFinishLevel(3);
    }

    private bool _everyLevelShowInterstitial = false;

    [Category("[GamePlay]")]
    public bool EveryLeveLShowInterstitial {
        get { return _everyLevelShowInterstitial; }
        set { _everyLevelShowInterstitial = value; }
    }

    private bool _autoNextLevel = false;

    [Category("[GamePlay]")]
    public bool AutoNextLevel {
        get { return _autoNextLevel; }
        set { _autoNextLevel = value; }
    }

    private bool isPauseGamePlay = false;

    [Category("[GamePlay]")]
    public bool PauseGamePlay {
        get { return isPauseGamePlay; }
        set {
            isPauseGamePlay = value;
            GamePlayController.instance.SetPauseGameplay(isPauseGamePlay);
        }
    }

    [Category("[GamePlay]")]
    public float SetGameSpeed {
        get { return Time.timeScale; }
        set { Time.timeScale = value; }
    }

    [Category("[GamePlay]")]
    public void SpawnAirDrop()
    {
        Vector3 centerMap = GamePlayController.instance.gameLevel._skillZoneMarker.centerSkillMarker.position;
        GamePlayController.instance.SpawnAirDrop();
    }

    [Category("[GamePlay]")]
    public void SpawnFlyingAirDrop()
    {
        GamePlayController.instance.SpawnFlyingAirDrop();
    }

    [Category("[GamePlay]")]
    public void AddAllAddonItems()
    {
        foreach (var item in SaveManager.Instance.Data.Inventory.ListAddOnItems)
        {
            item.ItemCount += 1;
            if (item.Status == com.datld.data.ITEM_STATUS.Disable)
                item.Status = com.datld.data.ITEM_STATUS.Available;
        }

        SaveManager.Instance.SetDataDirty();
        InGameCanvas.instance?._gamePannelView.ResetAddOnPannel();
    }

    private int _campaignLevel = 1;
    private int _idleLevel = 1;

    [Category("[GamePlay]")]
    [Sort(5)]
    public int CampaignLevel {
        get { return _campaignLevel; }
        set { _campaignLevel = value; }
    }

    [Category("[GamePlay]")]
    [Sort(6)]
    public void SetCampaignLevel()
    {
        SaveManager.Instance.Data.OverwritePlayerProgress(GameMode.CAMPAIGN_MODE, _campaignLevel);
        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }

    [Category("[GamePlay]")]
    [Sort(6)]
    public void QuickPlayLevel()
    {
        SaveManager.Instance.Data.OverwritePlayerProgress(GameMode.CAMPAIGN_MODE, _campaignLevel);
        GameMaster.instance.GoToSceneGame(GameMode.CAMPAIGN_MODE, _campaignLevel);
    }


    [Category("[GamePlay]")]
    [Sort(7)]
    public void SetCampaignLevel31()
    {
        SaveManager.Instance.Data.OverwritePlayerProgress(GameMode.CAMPAIGN_MODE, 31);
        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }


    [Category("[GamePlay]")]
    [Sort(7)]
    public int IdleLevel {
        get { return _idleLevel; }
        set { _idleLevel = value; }
    }

    [Category("[GamePlay]")]
    [Sort(8)]
    public void SetIdleLevel()
    {
        SaveManager.Instance.Data.OverwritePlayerProgress(GameMode.IDLE_MODE, _idleLevel);
        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }

    [Category("[GamePlay]")]
    public void AddStar()
    {
        var starData = SaveManager.Instance.Data.StarData.LevelStars;
        if (starData.Count == 0)
            starData.Add(0);

        for (int i = 0; i < SaveGameHelper.GetCurrentCampaignLevel() - 1; i++)
        {
            try
            {
                if (starData[i] < 3)
                {
                    starData[i] += 1;
                    break;
                }
            }
            catch (Exception e)
            {
                starData.Add(1);
                break;
            }
        }

        var starPanel = GameObject.FindObjectOfType<HUDStarReward>();
        starPanel.ResetLayers();
    }

    [Category("[GamePlay]")]
    public void SpawnStarAnimation()
    {
        GameObject.FindObjectOfType<StarChangeAnimation>().SpawnStar(UnityEngine.Random.Range(1, 4));
    }

    [Category("[GamePlay]")]
    public void PlayHUDCompleteLevel()
    {
        GamePlayController.instance.ShowDialogCompleteLevelReward();
    }

    [Category("[GamePlay]")]
    public void AddRandomHeroShard()
    {
        int count = 20;
        List<HeroData> availableHeroes = new List<HeroData>();
        foreach (var VARIABLE in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (VARIABLE.UniqueID != GameConstant.MANUAL_HERO && VARIABLE.ItemStatus != ITEM_STATUS.Locked)
            {
                availableHeroes.Add(VARIABLE);
            }
        }

        availableHeroes.PickRandom().AddShardHero(count);
        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }

    [Category("[GamePlay]")]
    public void UnlockRandomHero()
    {
        List<HeroData> availableHeroes = new List<HeroData>();
        foreach (var VARIABLE in SaveManager.Instance.Data.Inventory.ListHeroData)
        {
            if (VARIABLE.ItemStatus == ITEM_STATUS.Locked)
            {
                availableHeroes.Add(VARIABLE);
            }
        }

        availableHeroes.PickRandom().UnlockHero();
        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }

    [Category("[GamePlay]")]
    public void ResetDailyAirdrop()
    {
        SaveManager.Instance.Data.DayTrackingData.TodayEarnAddonAirDrop = 0;
        SaveManager.Instance.SetDataDirty();
    }

    private bool remove_ad = false;

    [Category("[Inventory]")]
    public bool RemoveAds {
        get { return remove_ad; }
        set {
            remove_ad = value;
            if (remove_ad)
            {
                SaveGameHelper.ActiveNoAds();
            }

            SaveManager.Instance.Data.RemoveAds = remove_ad;
        }
    }

    [Category("[Inventory]")]
    public void BuyRemoveAds()
    {
        GameObject.FindObjectOfType<HUDRemoveAds>().OnBuySuccess();
    }

    private string _weaponID;

    [Category("[Inventory]")]
    [Sort(1)]
    public string WeaponID {
        get => _weaponID;
        set => _weaponID = value;
    }

    private int _rank;

    [Category("[Inventory]")]
    [Sort(2)]
    public int Rank {
        get => _rank;
        set => _rank = value;
    }


    [Category("[Inventory]")]
    [Sort(3)]
    public void AddWeapon()
    {
        var mp5 = SaveManager.Instance.Data.AddWeapon(_weaponID);
        mp5.Rank = _rank;
        mp5.ResetPowerData();
        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }


    [Category("[Inventory]")]
    public void AddMP5()
    {
        var mp5 = SaveManager.Instance.Data.AddWeapon("PISTOL_01");
        mp5.Rank = 2;
        mp5.ResetPowerData();
        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }

    [Category("[Inventory]")]
    public void AddHeroChestKey()
    {
        SaveManager.Instance.Data.Inventory.TotalHeroChestKey += 10;
    }


    [Category("[Inventory]")]
    public void Add20Weapon()
    {
        for (int i = 0; i < 20; i++)
        {
            WeaponData weaponData =
                SaveGameHelper.RandomWeaponData(UnityEngine.Random.Range(1,
                    5)); //, chestDesignElement.GetIgnoreWeapon());
            SaveManager.Instance.Data.Inventory.ListWeaponData.Add(weaponData);
        }

        MasterCanvas.CurrentMasterCanvas.CurrentHUD.ResetLayers();
    }



    // [Category("[Mission]")]
    // public void CheckToResetMission()
    // {
    //     MissionManager.Instance.CheckToReset();
    // }
    //

    private int _star = 0;

    private bool _enableDebugHero = false;

    [Category("[GamePlay]")]
    public bool EnableDebugHero {
        get { return _enableDebugHero; }
        set {
            _enableDebugHero = value;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.ENABLE_DEBUG_HERO_PANEL, _enableDebugHero);
        }
    }


    [Category("[GamePlay]")]
    public int SetStars {
        get { return _star; }
        set {
            _star = value;
            var starPanel = GameObject.FindObjectOfType<HUDStarReward>();
            starPanel.UpdateStarBar();
        }
    }

    [Category("[GamePlay]")]
    public bool NeverEndBattle {
        get {
            if (GamePlayController.instance != null)
            {
                return GamePlayController.instance.CheatNeverEndBattle;
            }
            else
                return false;
        }

        set {
            if (GamePlayController.instance != null)
            {
                GamePlayController.instance.SetCheatNeverEndBattle(value);
            }
        }
    }

    [Category("[GamePlay]")]
    public void SetCastleHP()
    {
        GamePlayController.instance.CastleHP.SetDamage(300);
    }

    [Category("[GamePlay]")]
    public void KillCastle()
    {
        GamePlayController.instance.CastleHP.SetDamage(int.MaxValue);
    }

    [Category("[GamePlay]")]
    public void ResetSessionIdleMode()
    {
        SaveManager.Instance.Data.CheckAndResetIdleMode(true);
    }



    [Category("[Setting]")]
    public void SwitchLanguage()
    {
        LocalizeController.Instance.SwitchLanguage();
    }

    private string _shardHeroID;


    [Category("[SHARD HERO]")]
    public string ShardHero {
        get { return _shardHeroID; }
        set { _shardHeroID = value; }
    }

    [Category("[SHARD HERO]")]
    public void AddShard()
    {
        var heroData = SaveManager.Instance.Data.GetHeroData(_shardHeroID);
        if (heroData != null)
        {
            heroData.AddShardHero(5);
        }
    }

    [Category("[CURRENCY]")]
    public void AddGold()
    {
        CurrencyModels.instance.Golds += 1000000000;
    }

    [Category("[CURRENCY]")]
    public void AddDiamond()
    {
        CurrencyModels.instance.Diamonds += 1000000000;
    }

    [Category("[CURRENCY]")]
    public void AddWeaponScroll()
    {
        CurrencyModels.instance.WeaponScrolls += 1000000000;
    }

    [Category("[CURRENCY]")]
    public void AddPotion()
    {
        CurrencyModels.instance.Pills += 1000000000;
    }

    [Category("[CURRENCY]")]
    public void AddToken()
    {
        CurrencyModels.instance.Tokens += 100000;
    }


    [Category("[ADS]")]
    [Sort(1)]
    public void LoadInterstitial()
    {
        AdsManager.instance.LoadAdsInterstitial((complete) => { });
    }

    [Category("[ADS]")]
    [Sort(2)]
    public void ShowInterstitial()
    {
        AdsManager.instance.ShowAdsInterstitial((success) => { });
    }

    [Category("[ADS]")]
    [Sort(3)]
    public void LoadRewardAds()
    {
        AdsManager.instance.LoadAdsReward((complete) => { });
    }

    [Category("[ADS]")]
    [Sort(4)]
    public void ShowRewardADS()
    {
        AdsManager.instance.ShowAdsReward((success, amount) => { });
    }

    [Category("[ADS]")]
    [Sort(5)]
    public void RequestAdsBanner()
    {
        AdsManager.instance.RequestAdsBanner((success) =>
        {
            Debug.Log($"RequestAdsBanner callback: {success}");
        });
    }

    [Category("[ADS]")]
    [Sort(6)]
    public void ShowAdsBanner()
    {
        AdsManager.instance.ShowAdsBanner((complete) => { });
    }


    [Category("[ADS]")]
    [Sort(7)]
    public void HideBannerAds()
    {
        AdsManager.instance.HideAdsBanner((complete) => { });
    }


    [Category("[ADS]")]
    [Sort(8)]
    public void ShowTestSuite()
    {
        var adController = AdsManager.instance._listAdsProvider[0];
        if (adController != null && adController is AdmobAdsController)
        {
            var admob = (AdmobAdsController)adController;
            admob.ShowMediationTestSuite();
        }
    }


    [Category("HUD")]
    public void ShowHUDRating()
    {
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_RATING, false);
    }

    [Category("Show HUD Loading")]
    public void ShowHUDLoading()
    {
        TopLayerCanvas.instance.ShowHUDLoading(true);
    }

    [Category("HIDE HUD Loading")]
    public void HideHUDLoading()
    {
        TopLayerCanvas.instance.ShowHUDLoading(false);
    }

    [Category("Random Weight")]
    [Sort(1)]
    public void RandomWeighted()
    {
        Dictionary<string, float> foo = new Dictionary<string, float>();
        foo.Add("Item 25% 1", 0.5f);
        foo.Add("Item 25% 2", 0.5f);
        foo.Add("Item 50%", 1f);
        foo.Add("Item 80%", 2f);

        for (int i = 0; i < 10; i++)
            Debug.Log($"Item Chosen {foo.RandomElementByWeight(e => e.Value)}");
    }

    [Category("Server Data")]
    public void ShowPopupMergeConfict()
    {
        ViewConflictData deviceData = new ViewConflictData()
        {
            UserName = SaveManager.Instance.Data.MetaData.UserName,
            CampaignLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel,
            TotalGold = SaveManager.Instance.Data.Inventory.TotalGold
        };

        ViewConflictData onCloudData = new ViewConflictData()
        {
            UserName = "Cloud",
            CampaignLevel = 99,
            TotalGold = 99999999
        };

        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_MERGE_CONFLICT, false, null, deviceData, onCloudData, null);
    }

    [Category("Server Data")]
    public void SyncDataToServer()
    {
        SaveManager.Instance.SyncDataToServer((complete) => { });
    }

    [Category("Server Data")]
    public void PullDataFromServer()
    {
        SaveManager.Instance.PullDataFromServer((complete, serverData) => { });
    }

    [Category("Server Data")]
    public void ClearServerData()
    {
        SaveManager.Instance.ClearLocalData();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [Category("Login Firebase")]
    public void LoginFirebase()
    {
        SocialLogin.instance.LoginFirebase((success, authID) =>
        {
            Debug.Log($"LoginFirebase callback : {success} {authID}");
        });
    }

    private MAP_NAME mapDebug = MAP_NAME.NONE;

    [Category("Map Debug")]
    public MAP_NAME Map {
        get { return mapDebug; }
        set { mapDebug = value; }
    }

    private bool _iapAlwaysApprove = false;

    [Category("IAP")]
    public bool IAPAlwaysApprove {
        get => _iapAlwaysApprove;
        set => _iapAlwaysApprove = value;
    }

    private int _levelCampaignReward;

    [Category("[CAMPAIGN REWARD]")]
    public int LevelCampaignReward {
        get { return _levelCampaignReward; }
        set { _levelCampaignReward = value; }
    }

    [Category("[CAMPAIGN REWARD]")]
    public void PrintLevelCampaignReward()
    {
        //var listRewards = SaveManager.Instance.Data.GetCampaignRewards(_levelCampaignReward);
        //string debugTxt = "";
        //if (listRewards != null && listRewards.Count > 0)
        //{
        //    for (int i = 0; i < listRewards.Count; i++)
        //    {
        //        debugTxt += $"{listRewards[i]._type}: {listRewards[i]._value} ({listRewards[i]._extends}) \n";
        //    }

        //    Debug.LogError($"CAMPAIGN DEBUG:\n {debugTxt}");
        //}
    }

    #region Optimzation
    [Category("Optimization")]
    public bool EnableTextDmg {
        get { return GameMaster.instance.OptmizationController.Data.EnableTextDmg; }
        set {
            GameMaster.instance.OptmizationController.SetEnableText(value);
        }
    }

    [Category("Optimization")]
    public bool EnableImpactHit {
        get { return GameMaster.instance.OptmizationController.Data.EnableImpactHit; }
        set {
            GameMaster.instance.OptmizationController.SetEnableImpactHit(value);
        }
    }

    [Category("Optimization")]
    public bool EnableBlood {
        get { return GameMaster.instance.OptmizationController.Data.EnableBlood; }
        set {
            GameMaster.instance.OptmizationController.SetEnableBlood(value);
        }
    }

    [Category("Optimization")]
    public bool EnableFlashWhenHit {
        get { return GameMaster.instance.OptmizationController.Data.EnableFlashWhenHit; }
        set {
            GameMaster.instance.OptmizationController.SetEnableFlashWhenHit(value);
        }
    }
    #endregion

    private int _levelSelect = 1;
    [Category("TEST HUD")]
    public int LevelSelect {
        get {
            return _levelSelect;
        }
        set {
            _levelSelect = value;
        }
    }

    [Category("TEST HUD")]
    public void ShowHUDLevelPreview()
    {
        int stars = SaveGameHelper.GetStarAtLevel(LevelSelect);
        //always get max!
        var tuppleRewards = SaveManager.Instance.Data.GetCampaignRewards(LevelSelect, 3);
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_PREVIEW_CAMPAIGN_LEVEL, false, null, tuppleRewards.Item1,
            tuppleRewards.Item2, LevelSelect, stars);

    }

    [Category("ADMOB")]
    public void EnableTestSuite()
    {

    }

    [Category("Test Remote Config")]
    public void TestRemoteConfig()
    {
        var value = FirebaseRemoteSystem.instance.GetConfigValue("FIRST_TIME_SHOW_INTERSTITIAL_LEVEL");
        Debug.Log($"FIRST_TIME_SHOW_INTERSTITIAL_LEVEL: {value.LongValue}");
    }


    [Category("Game Debug Data")]
    public bool IsAllZombieDie {
        get {
            if (GamePlayController.instance != null && GamePlayController.instance._waveController != null)
            {
                return !GamePlayController.instance._waveController.CheckToSurvialsZombie();
            }
            else
                return false;
        }
    }


    public bool IsLevelReadyToUp {
        get {
            if (GamePlayController.instance != null && GamePlayController.instance._waveController != null)
            {
                return GamePlayController.instance._waveController.IsLevelReadyToUp;
            }
            else
                return false;
        }
    }



}