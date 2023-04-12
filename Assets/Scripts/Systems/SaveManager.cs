using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MEC;
using Google.Protobuf;
using com.datld.data;
using Doozy.Engine.Utils.ColorModels;
using Newtonsoft.Json;

public class SaveManager : BaseSystem<SaveManager>, ISaveGame<UserData>
{
    public static SaveManager Instance;

    private void Awake()
    {
        Instance = this;
        _isDataDirty = false;
    }

    public static readonly string SaveFileName = "save.dat";
    public static readonly string SaveFileDebug = "save_debug.json";

    public static readonly string BackupSaveFile = "backup_save.dat";

    private bool _enableAutoSave = true;
    private bool _isDataDirty;
    private UserData _userData;
    private bool _isNewDay;

    public bool EnableAutoSave => _enableAutoSave;
    public bool IsDataDirty => _isDataDirty;
    public bool IsNewDay => _isNewDay;
    public UserData Data => _userData;

    public void SaveHeroLevel(UserData userData)
    {
        var dictTemp = SaveGameHelper.heroLevelTemp;
        
        if (dictTemp != null && dictTemp.Count != 0)
        {
            var inventory = userData.Inventory.ListHeroData.ToList();
            var uniqueIdDictTemp = new Dictionary<string, HeroData>();
            foreach (var VARIABLE in inventory)
            {
                uniqueIdDictTemp[VARIABLE.UniqueID] = VARIABLE;
            }
            
            foreach (var VARIABLE in dictTemp)
            {
                if (uniqueIdDictTemp.ContainsKey(VARIABLE.Key))
                {
                    uniqueIdDictTemp[VARIABLE.Key].Level = VARIABLE.Value;
                }
            }
            // dictTemp.Clear();
        } 
    }
    
    public void SaveEquipmentLevel(UserData userData)
    {
        var dictTemp = SaveGameHelper.weaponLevelTemp;
        
        if (dictTemp != null && dictTemp.Count != 0)
        {
            var inventory = userData.Inventory.ListWeaponData.ToList();
            var uniqueIdDictTemp = new Dictionary<string, WeaponData>();
            foreach (var VARIABLE in inventory)
            {
                uniqueIdDictTemp[VARIABLE.UniqueID] = VARIABLE;
            }
            
            foreach (var VARIABLE in dictTemp)
            {
                if (uniqueIdDictTemp.ContainsKey(VARIABLE.Key))
                {
                    uniqueIdDictTemp[VARIABLE.Key].Level = VARIABLE.Value;
                }
            }
            // dictTemp.Clear();
        }
    }

    public UserData LoadData()
    {
        try
        {
            byte[] data = FileManager.LoadFile(SaveFileName);
            if (data == null)
            {
                _userData = SaveGameHelper.defaultData();
            }
            else
            {
                _userData = UserData.Parser.ParseFrom(data);
            }
           
            // GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(_userData);
            return _userData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Load Data error {ex}");
            return null;
        }

    }

    public void SaveData(bool isCloseGame = false)
    {
        try
        {
            if (_userData != null)
            {
                if (GamePlayController.instance != null)
                {
                    GamePlayController.instance.OnSaveGame();
                }

                _userData.Inventory.TotalDiamond = CurrencyModels.instance.Diamonds;
                _userData.MetaData.LastSave = TimeService.instance.GetCurrentTimeStamp();
                _userData.MetaData.LastBeginDayTS = TimeService.instance.GetTimeStampBeginDay();
                _userData.MetaData.MacAddress = SystemInfo.deviceUniqueIdentifier;
                
                var tempUser = _userData.Clone();
                SaveEquipmentLevel(tempUser);
                SaveHeroLevel(tempUser);
                
                if (isCloseGame)
                {
                    ++_userData.MetaData.Revision;
                    //push data to cloud
                    SyncDataToServer((complete) =>
                    {
                        Debug.Log($"SyncDataToServer result: {complete}");
                    });
                }
                // FileManager.SaveFileText("Wtf",JsonConvert.SerializeObject(tempUser));
                // Debug.LogError($"Save game ");
                // Debug.LogError($"{JsonConvert.SerializeObject(tempUser)}");
                // foreach (var VARIABLE in tempUser.Inventory.ListWeaponData)
                // {
                //     if(VARIABLE.Level > 1)
                //         Debug.LogError($"Save weapon {VARIABLE.UniqueID} level {VARIABLE.Level}"); 
                // }
                
                FileManager.SaveFile(SaveFileName, tempUser.ToByteArray());
                FileManager.SaveFileText(SaveFileDebug, tempUser.ToString());
                
                // byte[] data = FileManager.LoadFile(SaveFileName);
                // if (data != null)
                // {
                //     var tempUserData = UserData.Parser.ParseFrom(data);
                //     foreach (var VARIABLE in tempUserData.Inventory.ListWeaponData)
                //     {
                //         if(VARIABLE.Level > 1)
                //         Debug.LogError($"Get weapon {VARIABLE.UniqueID} level {VARIABLE.Level}"); 
                //     }
                // }
                
                _isDataDirty = false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Save game error ${ex}");

        }

    }

    public bool SyncData()
    {
        try
        {
            if (_userData != null)
            {
                bool dataDirty = _userData.SyncData();
                if (dataDirty)
                {
                    SaveData();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sync Data fail {ex}");
            return false;
        }
    }

    public void SetFirstTimeJoinGame()
    {
        _userData.FirstTimeJoinGame();
        //AnalyticsManager.instance.LogEvent(AnalyticsConstant.ANALYTICS_ENUM.NEW_REGISTER_USER);
    }

    public void PostFirstTimeJoinGame()
    {
        AnalyticsManager.instance.LogEvent(AnalyticsConstant.ANALYTICS_ENUM.NEW_REGISTER_USER);
     
        SaveManager.Instance.SetDataDirty();
    }

    public bool CheckIsNewDay()
    {
        bool result = false;
        var beginToday = TimeService.instance.GetTimeStampBeginDay();
        var lastDay = Data.MetaData.LastBeginDayTS;
        if (beginToday - lastDay >= TimeService.DAY_SEC - 1)
        {
            result = true;
        }

        return result;
    }

    public override IEnumerator<float> InitializeCoroutineHandler()
    {
        _userData = LoadData();
        //pull server data here!!!!
        LocalFireData = InitLocalServerData();

        bool waitingTask = true;
        bool lastTaskComplete = false;
        FireUserData ServerData = null;
        PullDataFromServer((success, _serverData) =>
        {
            waitingTask = false;
            lastTaskComplete = success;
            ServerData = _serverData;

        });
        while (waitingTask)
        {
            yield return Timing.WaitForOneFrame;
        }

        //merge Data!!!
        if (ServerData != null)
        {
            waitingTask = true;
            FlowMergeData(ServerData, (success) =>
            {
                waitingTask = false;
            });
            while (waitingTask)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        //continue init Data!!!!
        _isNewDay = CheckIsNewDay();
        yield return Timing.WaitForOneFrame;
        SyncData();

        if (Data.MetaData.FirstTimeJoinGame)
        {
            SetFirstTimeJoinGame();
            //Data.MetaData.FirstTimeJoinGame = false;
        }

        yield return Timing.WaitForOneFrame;

        OnInitializeComplete?.Invoke(true);

    }

    private void OnApplicationQuit()
    {
      
        SaveData(true);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
#if UNITY_EDITOR
            SaveData();
#else
            SaveData(true);
#endif

        }

        // not in battle scene!!!
        TimeService.instance.GetServerTime();
    }

    public void SetDataDirty()
    {
        _isDataDirty = true;
    }

    #region AUTO SAVE 

    public float AutoSaveDuration = TimeService.MIN_SEC * 5;
    public float AutoSaveWeaponDuration = TimeService.MIN_SEC * 20;

    private float _timerAutoSave = 0f;
    private float _timeAutoSaveWeapon = 0f;

    public void UpdateAutoSaveGame(float _deltaTime)
    {
        _timerAutoSave += _deltaTime;
        if (_timerAutoSave >= AutoSaveDuration)
        {
            if (IsDataDirty)
                SaveData();

            _timerAutoSave = 0f;
        }

        // _timeAutoSaveWeapon += _deltaTime;
        // if (_timeAutoSaveWeapon >= AutoSaveWeaponDuration)
        // {
        //     SaveEquipmentLevel();
        //     SaveHeroLevel();
        //     _timeAutoSaveWeapon = 0f;
        // }
    }

    #endregion

    public override void UpdateSystem(float _deltaTime)
    {
        UpdateAutoSaveGame(_deltaTime);
    }

    #region ServerData
    private FireUserData LocalFireData;
    private Action<bool> OnFlowMergeData = null;

    public FireUserData InitLocalServerData()
    {
        FireUserData ServerData = new FireUserData();
        ServerData.UserID = Data.MetaData.UserID;
        //temp get mac address!!! must replace later!!!
        ServerData.FirebaseID = Data.MetaData.MacAddress;
        ServerData.BytesData = Data.ToByteArray();

        return ServerData;
    }

    public void SyncDataToServer(Action<bool> callback)
    {
        if (!SocialLogin.instance.IsEnable())
        {
            callback?.Invoke(false);
            return;
        }

        if (SocialLogin.instance.IsSignedInFireBase())
        {
            LocalFireData.FirebaseID = SocialLogin.instance._firebaseLogin.FirebaseAuthID;
            LocalFireData.Revision = Data.MetaData.Revision;
            LocalFireData.BytesData = Data.ToByteArray();

            FirestoreSystem.instance.PushData("UserData", LocalFireData.FirebaseID, LocalFireData, (success) =>
            {
                callback?.Invoke(success);
            });
        }
        else
        {
            Debug.LogError("Please Sign in Firebase first!!!");
            callback?.Invoke(false);
        }
    }

    public void PullDataFromServer(Action<bool, FireUserData> callback)
    {
        if (!SocialLogin.instance.IsEnable())
        {
            callback?.Invoke(false, null);
            return;
        }

        if (SocialLogin.instance.IsSignedInFireBase())
        {
            FirestoreSystem.instance.TryGetDocument<FireUserData>("UserData", SocialLogin.instance._firebaseLogin.FirebaseAuthID, (success, data) =>
            {
                if (success)
                {
                    Debug.Log($"Get server data response: {data.UserID}");
                    callback?.Invoke(true, data);
                }
                else
                    callback?.Invoke(false, null);
            });
        }
        else
        {
            Debug.LogError("Please Sign in Firebase first!!!");
            callback?.Invoke(false, null);
        }


    }

    public void FlowMergeData(FireUserData serverData, Action<bool> callback)
    {
        Timing.RunCoroutine(FlowMergeDataCoroutine(serverData, callback));
    }

    IEnumerator<float> FlowMergeDataCoroutine(FireUserData serverdata, Action<bool> callback)
    {
        // TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false);
        TopLayerCanvas.instance.ShowHUDLoading(true);
        bool waitingTask = true;
        bool lastTaskSuccess = false;

        //choose Data
        UserData serverUserData = UserData.Parser.ParseFrom(serverdata.BytesData);
        TaskChooseData(serverUserData, (success) =>
         {
             waitingTask = false;
             lastTaskSuccess = success;
         });
        while (waitingTask)
        {
            yield return Timing.WaitForOneFrame;
        }

        //sync Data to Server
        if (lastTaskSuccess)
        {
            waitingTask = true;
            SyncDataToServer((success) =>
            {
                lastTaskSuccess = success;
                waitingTask = false;
            });
            while (waitingTask)
            {
                yield return Timing.WaitForOneFrame;
            }
        }


        callback?.Invoke(lastTaskSuccess);
        //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
        TopLayerCanvas.instance.ShowHUDLoading(false);
    }

    #region Flow Choose Data

    public enum ResolveResult
    {
        NONE,
        CHOOSE_DEVICE,
        CHOOSE_CLOUD,
        CONFLICT
    }

    private ResolveResult CheckResolve(UserData svData)
    {
        ResolveResult result = ResolveResult.NONE;

        //same account
        if (svData.MetaData.FireBaseID == SocialLogin.instance._firebaseLogin.FirebaseAuthID)
        {
            //same device
            if (svData.MetaData.MacAddress == SystemInfo.deviceUniqueIdentifier)
            {
                if (svData.MetaData.Revision > Data.MetaData.Revision)
                    result = ResolveResult.CHOOSE_CLOUD;
                else
                    result = ResolveResult.CHOOSE_DEVICE;
            }
            else //diff devices
            {
                result = ResolveResult.CONFLICT;
            }
        }
        else
            result = ResolveResult.CONFLICT;

        Debug.Log($"Resolve Data result: {result}");
        return result;
    }

    public void TaskChooseData(UserData serverUserData, Action<bool> callback)
    {
        try
        {
            ResolveResult resolveResult = CheckResolve(serverUserData);
            switch (resolveResult)
            {
                case ResolveResult.NONE:
                    break;
                case ResolveResult.CHOOSE_DEVICE:
                    callback?.Invoke(true);
                    break;
                case ResolveResult.CHOOSE_CLOUD:
                    ChooseServerData(serverUserData);
                    callback?.Invoke(true);
                    break;
                case ResolveResult.CONFLICT:
                    ShowPopupMergeConflict(serverUserData, (isChooseLocal) =>
                    {
                        if (isChooseLocal)
                        {
                            SaveData();
                        }
                        else
                        {
                            ChooseServerData(serverUserData);
                        }
                        callback?.Invoke(true);
                    });
                    break;
                default:
                    break;
            }


        }
        catch (Exception ex)
        {
            Debug.LogError($"FlowMergeData error!!!! {ex}");
            callback?.Invoke(false);
        }

    }

    public void ChooseServerData(UserData svUserData)
    {
        //Server data is newer
        if (svUserData != null)
        {
            var backupData = _userData;
            _userData = svUserData;
            SaveData();
            FileManager.SaveFile(BackupSaveFile, backupData.ToByteArray());
        }
    }

    public void ShowPopupMergeConflict(UserData serverUserData, Action<bool> callback)
    {
        ViewConflictData deviceData = new ViewConflictData()
        {
            UserName = Data.MetaData.UserName,
            CampaignLevel = Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel,
            TotalGold = Data.Inventory.TotalGold
        };

        ViewConflictData onCloudData = new ViewConflictData()
        {
            UserName = serverUserData.MetaData.UserName,
            CampaignLevel = serverUserData.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel,
            TotalGold = serverUserData.Inventory.TotalGold
        };

        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_MERGE_CONFLICT, false, null, deviceData, onCloudData, callback);
    }

    #endregion

    /// <summary>
    /// To clear sever data!!! debug only 
    /// </summary>
    /// <param name="callback"></param>
    public void CheatClearData(Action<bool> callback)
    {
        if (SocialLogin.instance.IsSignedInFireBase())
        {
            LocalFireData.FirebaseID = SocialLogin.instance._firebaseLogin.FirebaseAuthID;
            LocalFireData.Revision = Data.MetaData.Revision;
            LocalFireData.BytesData = SaveGameHelper.defaultData().ToByteArray();

            FirestoreSystem.instance.PushData("UserData", LocalFireData.FirebaseID, LocalFireData, (success) =>
            {
                callback?.Invoke(success);
            });
        }
        else
        {
            Debug.LogError("Please Sign in Firebase first!!!");
            callback?.Invoke(false);
        }
    }

    public void ClearLocalData()
    {
        _userData = SaveGameHelper.defaultData();
        SaveManager.Instance.SetDataDirty();
    }

    #endregion


}

