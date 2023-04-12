using System;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;
using QuickType.LevelWave;
using MEC;
using UnityEngine.AddressableAssets;

public class ZombieDispenser : MonoBehaviour
{
    private CoroutineHandle _spawnZomHandler;
    private CoroutineHandle _createZomHandler;

    private float OffsetSpawnX = 0f;
    private Dictionary<string, Zombie> _dictZombiePrefab;
    private Dictionary<string, bool> _dictLimitSpawnZombie;

    private void Awake()
    {
        spawnZone = GameObject.FindObjectOfType<SpawnZone>();
    }

    public void Initialize()
    {
        _dictZombiePrefab = new Dictionary<string, Zombie>();
        _dictLimitSpawnZombie = new Dictionary<string, bool>();
    }

    public void OnLevelReadyUp()
    {

    }

    //public void SpawnZombieForWawe(LevelWaveData data, bool _isBossWave, ref float repeatTimerWave, Action<List<Zombie>> SpawnComplete)
    //{
    //    LevelElementDesign ds = null;

    //    if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
    //        ds = DesignHelper.GetCampaignLevelDesign(data.level);
    //    else
    //        ds = DesignHelper.GetIdleLevelDesign(data.level);

    //    if (ds != null)
    //    {
    //        var waveController = ds.GetWaveControlByType(data.currentWave);
    //        if (waveController != null)
    //        {
    //            ZombieWaweData zomWaveData = new ZombieWaweData();
    //            zomWaveData.hpMultiplier = ds.ZombieHpMultiplier;
    //            zomWaveData.dmgMultiplier = ds.ZombieDmgMultiplier;
    //            zomWaveData.minCountOfZombie = (int)waveController.NumZombie;
    //            zomWaveData._listSubwaveDispenser = DesignHelper.ParseSubwaveDispenser(waveController.WaveDispenser);

    //            if (zomWaveData._listSubwaveDispenser == null || zomWaveData._listSubwaveDispenser.Count <= 0)
    //            {
    //                zomWaveData._listSubwaveDispenser = new List<SubwaveDispenser>();
    //                zomWaveData._listSubwaveDispenser.Add(new SubwaveDispenser()
    //                {
    //                    totalPercent = 100,
    //                    delaySpawn = 0
    //                });
    //            }

    //            var zombieRatioDs = DesignHelper.getZombieRatioDesign(waveController.ZombieRatio);
    //            if (zombieRatioDs != null)
    //            {
    //                zomWaveData.zombiesAndRatio = zombieRatioDs.Ratio;
    //            }

    //            var totalHP = CalcTotalWaveHP(zomWaveData);
    //            var waveDuration = CalcTimeSpawnWave(zomWaveData);
    //            repeatTimerWave = data.WaveDuration + waveDuration;
    //            InGameCanvas.instance._gamePannelView.ResetLevelWaveInfo(data.level, data.currentWave + 1, data.totalWave, repeatTimerWave);


    //            _spawnZomHandler = Timing.RunCoroutine(SpawnZombies(zomWaveData, _isBossWave, data.currentWave, (listZom) =>
    //             {
    //                 SpawnComplete?.Invoke(listZom);
    //             }));

    //        }

    //    }
    //    else
    //    {
    //        Debug.LogError("SpawnZombieForWawe error, design is null!!!");
    //        SpawnComplete?.Invoke(null);
    //    }
    //}

    public void SpawnZombieForWawe(LevelWaveData data, ZombieWaweData zomWaveData, bool _isBossWave, ref float repeatTimerWave, Action<List<Zombie>> SpawnComplete)
    {
        var totalHP = CalcTotalWaveHP(zomWaveData);
        var waveDuration = CalcTimeSpawnWave(zomWaveData);
        repeatTimerWave = data.WaveDuration + waveDuration;
        InGameCanvas.instance._gamePannelView.ResetLevelWaveInfo(data.level, data.currentWave, data.totalWave, repeatTimerWave);
        _spawnZomHandler = Timing.RunCoroutine(SpawnZombies(zomWaveData, _isBossWave, data.currentWave, (listZom) =>
        {
            SpawnComplete?.Invoke(listZom);
        }));
    }

    public float CalcTotalWaveHP(ZombieWaweData waveData)
    {
        float totalHp = 0f;
        if (waveData.zombiesAndRatio == null)
            return 0;

        foreach (var ratio in waveData.zombiesAndRatio)
        {
            var num = Mathf.RoundToInt(ratio.Percent / 100f * waveData.minCountOfZombie);
            if (num <= 0)
                num = 1;
            var zomDes = DesignHelper.GetZombieDesign(ratio.ZombieId);
            totalHp += num * zomDes.HP * waveData.hpMultiplier;
        }

        return totalHp;
    }

    public float CalcTimeSpawnWave(ZombieWaweData waveData)
    {
        float duration = 0f;

        if (waveData._listSubwaveDispenser != null && waveData._listSubwaveDispenser.Count > 0)
        {
            foreach (var subWave in waveData._listSubwaveDispenser)
            {
                duration += subWave.delaySpawn;
            }
        }

        //foreach (var ratio in waveData.zombiesAndRatio)
        //{
        //    var num = Mathf.RoundToInt(ratio.Percent / 100f * waveData.minCountOfZombie);
        //    if (num <= 0)
        //        num = 1;
        //    duration += num * TimeService.FRAME_SEC;
        //}

        return duration;
    }

    IEnumerator<float> SpawnZombies(ZombieWaweData _currentStageData, bool _isBossWave, int wave, Action<List<Zombie>> OnSpawnComplete)
    {
        float zomHp = 0f;
        float num = (float)_currentStageData.minCountOfZombie;
        List<Zombie> result = new List<Zombie>();
        var listsubDispenser = _currentStageData._listSubwaveDispenser;
        int dispenIndex = 0;

        if (listsubDispenser != null && listsubDispenser.Count > 0)
        {
            foreach (var dispenser in listsubDispenser)
            {
                var curNum = dispenser.totalPercent * num / 100f;
                for (int i = 0; i < _currentStageData.zombiesAndRatio.Count; i++)
                {
                    int num2 = Mathf.RoundToInt(curNum / 100f * (float)_currentStageData.zombiesAndRatio[i].Percent);
                    if (num2 <= 0 && dispenIndex == 0)
                        num2 = 1;

                    if (num2 <= 0)
                        continue;
                    Zombie zombiePrefab = null;
                    _dictZombiePrefab.TryGetValue(_currentStageData.zombiesAndRatio[i].ZombieId, out zombiePrefab);
                    if (zombiePrefab == null)
                    {
                        var op = Addressables.LoadAssetAsync<GameObject>(_currentStageData.zombiesAndRatio[i].ZombieId);
                        while (!op.IsDone)
                            yield return Timing.WaitForOneFrame;

                        zombiePrefab = op.Result.GetComponent<Zombie>();
                        if (!_dictZombiePrefab.ContainsKey(_currentStageData.zombiesAndRatio[i].ZombieId))
                            _dictZombiePrefab.Add(_currentStageData.zombiesAndRatio[i].ZombieId, zombiePrefab);

                    }


                    ZombieCreateData zombieCreateData = new ZombieCreateData
                    {
                        ZombieID = _currentStageData.zombiesAndRatio[i].ZombieId,
                        prefab = zombiePrefab,
                        count = num2,
                        priority = _currentStageData.zombiesAndRatio[i].Priority
                    };

                    _createZomHandler = Timing.RunCoroutine(CreateZombieCoroutine(zombieCreateData, _currentStageData.hpMultiplier, _currentStageData.dmgMultiplier, wave, dispenIndex, (totalHP, listZom) =>
                    {
                        zomHp += totalHP;
                        result.AddRange(listZom);
                        Logwin.Log($"lvl{LevelModel.instance.CurrentLevel} wave: {wave} -subwave: {dispenIndex}", $"Total Zom: {result.Count} - Total HP:{zomHp}", "[LEVEL DEBUG]");
                    }));

                    yield return Timing.WaitUntilDone(_createZomHandler);


                }
                var timeDelay = dispenser.delaySpawn / 1.0f * GameConstant.SPAWN_ZOM_SPEED_MULTIPLIER;
                yield return Timing.WaitForSeconds(timeDelay);
                dispenIndex++;

                if (spawnZone == null)
                {
                    spawnZone = GameObject.FindObjectOfType<SpawnZone>();
                }
                OffsetSpawnX = 0f;
                if (spawnZone != null && spawnZone.maxPoint != null)
                    MaxX = this.spawnZone.maxPoint.position.x;
                else
                    MaxX = 0f;


            }
        }
        else
        {
            Debug.LogError("listsubDispenser is null or empty!!!!");
        }
        OnSpawnComplete?.Invoke(result);
    }


    private float MaxX = 0f;
    private IEnumerator<float> CreateZombieCoroutine(ZombieCreateData zombieCreateData, float hpMultiplier, float dmgMultiplier, int wave, int subWave, Action<float, List<Zombie>> callback)
    {
        float zomHp = 0f;

        if (spawnZone == null)
        {
            spawnZone = GameObject.FindObjectOfType<SpawnZone>();
        }
        List<Zombie> result = new List<Zombie>();

        float startX = 0f;
        MaxX = this.spawnZone.maxPoint.position.x;

        for (int i = 0; i < zombieCreateData.count; i++)
        {
            var pos = this.spawnZone.GetRandomPoint(OffsetSpawnX);

            if (zombieCreateData.priority > 0)
            {
                pos = this.spawnZone.GetNearestForwardCastlePoint((zombieCreateData.priority - 1) * -1f);
            }


            Zombie component = Pooly.Spawn<Zombie>(zombieCreateData.prefab.transform, pos, Quaternion.identity, null);

            var zomDes = DesignHelper.GetZombieDesign(zombieCreateData.ZombieID);

            yield return Timing.WaitForOneFrame;
            component.Initialize(zomDes, hpMultiplier, dmgMultiplier, wave, subWave, i);
            result.Add(component);
            zomHp += zomDes.HP * hpMultiplier;
            GamePlayController.instance._waveController.AddZombie(component);

            GamePlayController.instance._waveController.AddDictZombie(wave, subWave, component);

            if (i == 0)
            {
                startX = component.transform.position.x;
            }
            if (i == zombieCreateData.count - 1)
            {
                OffsetSpawnX -= (Mathf.Abs(MaxX - startX));
                //Debug.Log($"OffsetSpawnX {OffsetSpawnX}");
            }

            if (Mathf.Abs(component.transform.position.x) >= MaxX)
                MaxX = component.transform.position.x;

            yield return Timing.WaitForOneFrame;
        }

        callback?.Invoke(zomHp, result);

    }

    private int numberOfStage;

    public List<ZombieWaweData> zombieDispenserDatas;

    //private List<Zombie> zombiesOnTheWave = new List<Zombie>();

    //public List<Zombie> ZombieOnTheWave { get { return this.zombiesOnTheWave; } }

    [SerializeField]
    private WaveDifficultController waveDifficultController;

    [SerializeField]
    private SpawnZone spawnZone;

    public void CleanUp()
    {
        Timing.KillCoroutines(_spawnZomHandler);
        Timing.KillCoroutines(_createZomHandler);
        OffsetSpawnX = 0;
        MaxX = 0f;
    }

}
