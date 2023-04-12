using System;
using System.Collections.Generic;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;
using UnityEngine;
using Ez.Pooly;
using com.datld.data;
using MEC;
using QuickType;
using QuickType.LevelWave;

[Serializable]
public class LevelWaveData
{
    public int level;
    public int currentWave;
    public float exp;
    public int totalWave;
    public float WaveDuration;
    public float LevelTotalHP;
}

public class WaweController : MonoBehaviour
{
    private bool _isApplied = false;
    private bool IsCreatingWave = false;

    private bool readyUpdate = false;
    private Dictionary<int, ZombieWaweData> _dictZomWaveData = new Dictionary<int, ZombieWaweData>();

    private bool IsBossWave()
    {
        return this._levelWaveData.currentWave >= this._levelWaveData.totalWave;
    }

    public void ResetLevelWaveData(GameMode mode, int startWave)
    {
        PlayProgress playprogress = SaveManager.Instance.Data.GetPlayProgress(mode);
        LevelElementDesign ds = null;
        if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
            ds = DesignHelper.GetCampaignLevelDesign(playprogress.CurrentLevel);
        else
            ds = DesignHelper.GetIdleLevelDesign(playprogress.CurrentLevel);

        _levelWaveData = new LevelWaveData();
        _levelWaveData.level = playprogress.CurrentLevel;
        _levelWaveData.currentWave = startWave;
        _levelWaveData.exp = 0;
        _levelWaveData.totalWave = (int)ds.TotalWave;
        _levelWaveData.WaveDuration = ds.NextWaveDuration;
        _levelWaveData.LevelTotalHP = this.CalcTotalZombieHPForLevel(ref _dictZomWaveData, startWave);
        Logwin.Log("[TotalZomHP]", $"{_levelWaveData.LevelTotalHP}", "LEVEL DEBUG");

        InGameCanvas.instance._gamePannelView.ResetZomHPBar(_levelWaveData.LevelTotalHP);
        InGameCanvas.instance._gamePannelView.InitLevelWaveInfo(_levelWaveData.level, _levelWaveData.totalWave);

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_IDLE_HUD, _levelWaveData.currentWave, _levelWaveData.totalWave);

    }

    private void Start()
    {
        this.Inject(); ;
        _dictZombieSpawned = new Dictionary<int, List<Zombie>>();
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_READY_UP, new Action(OnLevelReadyUp));

        if (this.zombiesOnTheWawe != null)
            this.zombiesOnTheWawe = new List<Zombie>();

    }

    public void ApplyBehaviour()
    {
        _isApplied = true;
        readyUpdate = false;
        zombieDispenser.Initialize();
        this.zombiesOnTheWawe = new List<Zombie>();
        _dictZombieSpawned = new Dictionary<int, List<Zombie>>();
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
        _preventCallOverlap = false;
    }

    public void StopBehaviour()
    {
        _isApplied = false;
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
    }

    private void OnLevelReadyUp()
    {
        this.levelReadyToUp = true;
    }

    private void OnLevelUp()
    {
        ResetLevelWaveData(GameMaster.instance.currentMode, 1);
        this.SpawnNextWawe();
        this.levelReadyToUp = false;
    }

    public List<Zombie> GetZombiesOnThisWawe()
    {
        return this.zombiesOnTheWawe;
    }

    public void SpawnNextWawe()
    {
        this.spawnNewWaweStarted = true;
        this.SendSpawnWaveNotification();
        LoadNextWawe();

        GamePlayController.instance.KillingZombieAward.CollectAllCoins();
        GamePlayController.instance.SetFinishLevel(false);

        this.levelReadyToUp = false;
    }

    public void RestartThisWave(int startWave = 1)
    {
        this._levelWaveData.currentWave = startWave;

        this.CreateWave();
        this.spawnNewWaweStarted = false;
        readyUpdate = true;

    }

    public float CalcTotalZombieHPForLevel(ref Dictionary<int, ZombieWaweData> _dictZomData, int startWave = 1)
    {
        _dictZomData.Clear();
        float result = 0;
        LevelElementDesign ds = null;
        if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
            ds = DesignHelper.GetCampaignLevelDesign(_levelWaveData.level);
        else
            ds = DesignHelper.GetIdleLevelDesign(_levelWaveData.level);

        if (ds != null)
        {
            for (int i = startWave; i <= _levelWaveData.totalWave; i++)
            {
                var waveController = ds.GetWaveControlByType(i);
                if (waveController != null)
                {
                    ZombieWaweData zomWaveData = new ZombieWaweData();
                    zomWaveData.hpMultiplier = ds.ZombieHpMultiplier;
                    zomWaveData.dmgMultiplier = ds.ZombieDmgMultiplier;
                    zomWaveData.minCountOfZombie = (int)waveController.NumZombie;
                    zomWaveData._listSubwaveDispenser = DesignHelper.ParseSubwaveDispenser(waveController.WaveDispenser);

                    if (zomWaveData._listSubwaveDispenser == null || zomWaveData._listSubwaveDispenser.Count <= 0)
                    {
                        zomWaveData._listSubwaveDispenser = new List<SubwaveDispenser>();
                        zomWaveData._listSubwaveDispenser.Add(new SubwaveDispenser()
                        {
                            totalPercent = 100,
                            delaySpawn = 0
                        });
                    }

                    var zombieRatioDs = DesignHelper.getZombieRatioDesign(waveController.ZombieRatio);
                    if (zombieRatioDs != null)
                    {
                        zomWaveData.zombiesAndRatio = zombieRatioDs.Ratio;
                    }

                    var waveHP = this.zombieDispenser.CalcTotalWaveHP(zomWaveData);
                    result += waveHP;
                    _dictZomData.Add(i, zomWaveData);
                }
            }
        }


        return result;
    }

    public void RestartLevel(int startWave = 1)
    {
        Timing.RunCoroutine(RestartLevelCoroutine(startWave));
    }

    IEnumerator<float> RestartLevelCoroutine(int startWave = 1)
    {
        if (GameMaster.instance.currentMode == GameMode.IDLE_MODE)
            timerIdle = 1f;


        this.zombieDispenser.CleanUp();
        var lisZom = GameObject.FindObjectsOfType<Zombie>();
        if (lisZom != null && lisZom.Length > 0)
        {
            foreach (var z in lisZom)
            {
                z.DestroyZombie();
            }
        }

        this.zombiesOnTheWawe.Clear();

        ClearZombieOnThisWawe();
        ResetLevelWaveData(GameMaster.instance.currentMode, startWave);
        yield return Timing.WaitForSeconds(0.2f);
        this.RestartThisWave(startWave);
        OnLevelLoaded?.Invoke();

        GamePlayController.instance.gameLevel.ResetCastleHealth();
    }

    //private int GetCurrentWave()
    //{
    //    for (int i = 0; i < this.zombieDispenser.zombieDispenserDatas.Count; i++)
    //    {
    //        if ( % (i + 1) == 0)
    //        {
    //            return i + 1;
    //        }
    //    }
    //    return 0;
    //}

    private void SendSpawnWaveNotification()
    {
        if (this._levelWaveData.currentWave == this._levelWaveData.totalWave)
        {
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.WAVE_STARTED, WaveType.BIG_WAVE);

        }
        else if (this._levelWaveData.currentWave < this._levelWaveData.totalWave)
        {

            EventSystemServiceStatic.DispatchAll(EVENT_NAME.WAVE_STARTED, WaveType.NORMAL_WAVE);

        }
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.ZOMBIE_WAVE_STARTED);
    }

    private void LoadNextWawe()
    {
        //_levelWaveData.currentWave++;
        this.UpDifficult();
        if (_levelWaveData.currentWave > this._levelWaveData.totalWave)
        {
            _levelWaveData.currentWave = 0;
        }
        this.CreateWave();
        this.spawnNewWaweStarted = false;

        OnLevelLoaded?.Invoke();
    }

    public void AddZombies(List<Zombie> listZom)
    {
        this.zombiesOnTheWawe.AddRange(listZom);
    }

    public void AddZombie(Zombie zom)
    {
        this.zombiesOnTheWawe.Add(zom);
    }

    private bool _preventCallOverlap = false;

    private void CreateWave()
    {
        if (_preventCallOverlap)
            return;

        _preventCallOverlap = true;

        IsCreatingWave = true;
        ZombieWaweData zomData = default(ZombieWaweData);
        _dictZomWaveData.TryGetValue(this._levelWaveData.currentWave, out zomData);
        repeatWaveTimer = 0f;
        this.zombieDispenser.SpawnZombieForWawe(this._levelWaveData, zomData, this.IsBossWave(), ref repeatWaveTimer, (listZom) =>
          {
              if (listZom != null && listZom.Count > 0)
              {
                  IsCreatingWave = false;
              }
              else
              {
                  Debug.LogError("Sth when wrong!!!!");
              }

          });

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.ZOMBIE_WAVE_STARTED);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_IDLE_HUD, _levelWaveData.currentWave, _levelWaveData.totalWave);

        _levelWaveData.currentWave++;
        if (_levelWaveData.currentWave > _levelWaveData.totalWave)
            _levelWaveData.currentWave = _levelWaveData.totalWave + 1;

        var playProgress = SaveManager.Instance.Data.GetPlayProgress(GameMaster.instance.currentMode);
        if (playProgress != null)
        {
            playProgress.CurrentWave = _levelWaveData.currentWave;
            playProgress.CurrentLevel = _levelWaveData.level;
            SaveManager.Instance.SetDataDirty();
        }

        Timing.CallDelayed(0.5f, () =>
        {
            _preventCallOverlap = false;
        });

        this.levelReadyToUp = false;
    }

    private List<Zombie> AddALiveZombiesToNewWave(List<Zombie> newWaveZombieList)
    {
        if (newWaveZombieList == null)
            return new List<Zombie>();

        for (int i = 0; i < this.zombiesOnTheWawe.Count; i++)
        {
            if (!this.zombiesOnTheWawe[i].IsDead)
            {
                newWaveZombieList.Add(this.zombiesOnTheWawe[i]);
            }
        }
        return newWaveZombieList;
    }

    private void UpDifficult()
    {
        this.waveDifficultController.UpDifficult(LevelModel.instance.CurrentLevel);
    }

    private void ClearZombieOnThisWawe()
    {
        for (int i = zombiesOnTheWawe.Count - 1; i >= 0; i--)
        {
            zombiesOnTheWawe[i].DestroyZombie();
        }

        zombiesOnTheWawe.Clear();

        _dictZombieSpawned.Clear();
    }

    public bool CheckToSurvialsZombie()
    {
        for (int i = 0; i < this.zombiesOnTheWawe.Count; i++)
        {
            if (!this.zombiesOnTheWawe[i].IsDead)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {



    }

    public void LevelUp()
    {
        LevelModel.instance.LevelUp();

    }

    [Inject]
    private EventManager eventManager;

    //[Inject]
    //private LevelModel levelModel;

    [SerializeField]
    public List<Zombie> zombiesOnTheWawe { get; private set; }

    [SerializeField]
    public ZombieDispenser zombieDispenser;

    [SerializeField]
    private WaveDifficultController waveDifficultController;

    [SerializeField]
    private float repeatWaveTimer = 5f;

    private float repeatWaveTimerDef = 10f;

    private float repeatWaveTimerBigWaveMode = 7f;

    [SerializeField]
    private float spawnWaveDelay = 1f;

    [SerializeField]
    private bool spawnNewWaweStarted;

    //private BigWaveChooser bigWaveChooser;
    [SerializeField]
    public LevelWaveData _levelWaveData;

    [SerializeField]
    private bool levelReadyToUp;

    public Action _actionLevelReadyUp;
    public Action _actionKillAllZombiesThisWave;
    public Action OnLevelLoaded;

    private float timerIdle = 0f;


    public bool IsLevelReadyToUp { get { return levelReadyToUp; } }

    public void CleanUp()
    {
        zombiesOnTheWawe.Clear();
        StopBehaviour();

        _dictZombieSpawned.Clear();
        this.zombieDispenser.CleanUp();
        //foreach (var zom in GetZombiesOnThisWawe())
        //{
        //    zom.DestroyZombie();
        //}

        var listZom = GameObject.FindObjectsOfType<Zombie>();
        if (listZom != null && listZom.Length > 0)
        {
            foreach (var z in listZom)
            {
                z.DestroyZombie();
            }
        }
        this.zombiesOnTheWawe.Clear();

    }

    public void UpdateWaveController(float _deltaTime)
    {
        if (!_isApplied || !readyUpdate)
        {
            // Debug.LogError("UpdateWaveController not update!!!");
            return;
        }


        if (this.zombiesOnTheWawe == null || this.spawnNewWaweStarted)
        {
            Debug.LogError("UpdateWaveController not update!!!");
            return;
        }

        if (timerIdle > 0)
        {
            timerIdle -= _deltaTime;
        }
        else
            timerIdle = 0;

        for (int i = zombiesOnTheWawe.Count - 1; i >= 0; i--)
        {
            zombiesOnTheWawe[i].UpdateZombie(_deltaTime);
            if (zombiesOnTheWawe[i].IsDead)
                zombiesOnTheWawe.RemoveAt(i);
        }

        if (!this.levelReadyToUp)
        {
            if (_levelWaveData.currentWave <= _levelWaveData.totalWave && GameMaster.instance.currentMode == GameMode.CAMPAIGN_MODE)
            {
                this.repeatWaveTimer -= Time.deltaTime;
                if (this.repeatWaveTimer <= 0f)
                {
                    this.CreateWave();
                }

            }
            else if (!this.CheckToSurvialsZombie())
            {
                if (GameMaster.instance.currentMode == GameMode.CAMPAIGN_MODE)
                {
                    DoLevelUp();
                }
                else if (GameMaster.instance.currentMode == GameMode.IDLE_MODE)
                {
                    if (_levelWaveData.currentWave > _levelWaveData.totalWave && timerIdle <= 0)
                    {
                        // GamePlayController.instance.CheckAndShowDialogSkipLevel();
                        DoLevelUpIdle();
                        timerIdle = 1.0f;

                    }
                    else if (_levelWaveData.currentWave <= _levelWaveData.totalWave && timerIdle <= 0f)
                    {
                        // GamePlayController.instance.CheckAndShowDialogSkipLevel();
                        this.CreateWave();
                        timerIdle = 1.0f;
                    }

                }

            }
        }

    }

    public void DoLevelUp()
    {
        LevelModel.instance.LevelReadyToUp();
        zombieDispenser.OnLevelReadyUp();
        Timing.CallDelayed(0.6f, () =>
        {
            if (_actionLevelReadyUp != null)
                _actionLevelReadyUp.Invoke();
            else
            {
                Debug.LogError("_actionLevelReadyUp is null!!!");
            }
        });

    }

    public void DoLevelUpIdle()
    {
        Timing.RunCoroutine(DoLevelUpIdleCoroutine());
    }


    /// <summary>
    /// LEVELUP IDLE MODE
    /// </summary>
    /// <returns></returns>
    IEnumerator<float> DoLevelUpIdleCoroutine()
    {
        MissionManager.Instance.TriggerMission(MissionType.FINISH_LEVEL, GameMode.IDLE_MODE);
        this.levelReadyToUp = true;
        //TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false, null, true);
        //yield return Timing.WaitForSeconds(0.5f);
        //Resources.UnloadUnusedAssets();
        //yield return Timing.WaitForOneFrame;
        //GC.Collect();
        //yield return Timing.WaitForOneFrame;

        if (GameMaster.instance.IsPowerSaving)
        {
            yield return Timing.WaitForSeconds(0.1f);
            yield return Timing.WaitForOneFrame;

            this.zombiesOnTheWawe.Clear();
            ResourceManager.instance.LimitAllPrefabs();

            yield return Timing.WaitForSeconds(0.3f);

            Resources.UnloadUnusedAssets();

            yield return Timing.WaitForOneFrame;
            GC.Collect(1, GCCollectionMode.Optimized);


        }
        LevelModel.instance.LevelReadyToUp();
        zombieDispenser.OnLevelReadyUp();
        // yield return Timing.WaitForSeconds(0.6f);
        _actionLevelReadyUp?.Invoke();

        //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
    }

    public void ClearAllZombieOnWave()
    {
        for (int i = zombiesOnTheWawe.Count - 1; i >= 0; i--)
        {
            zombiesOnTheWawe[i].health.SetDamage(zombiesOnTheWawe[i].health.GetHPWithCoeff());
        }
    }

    #region WAVE/ SUBWAVE ZOMBIE

    public Dictionary<int, List<Zombie>> _dictZombieSpawned { get; private set; }

    public void AddDictZombie(int wave, int subWave, Zombie zom)
    {
        _dictZombieSpawned.TryGetValue(GetWaveKey(wave, subWave), out var _listZom);
        if (_listZom == null)
        {
            _dictZombieSpawned.Add(GetWaveKey(wave, subWave), new List<Zombie>() { zom });
        }
        else
        {
            _listZom.Add(zom);
        }
    }

    private int GetWaveKey(int wave, int subwave)
    {
        return wave * 10 + subwave;
    }

    public void SetMultiplierSpeedForSubwave(int wave, int subwave, float multiplier = 1.0f)
    {
        _dictZombieSpawned.TryGetValue(GetWaveKey(wave, subwave), out var listZom);
        if (listZom != null)
        {
            foreach (var z in listZom)
            {
                z.SetZombieSpeedToNormal();
            }
        }
    }

    #endregion

}
