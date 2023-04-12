using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions.Localization;

public class SelectLevelPanel : MonoBehaviour
{
    [SerializeField] private SelectLevelUI _selectLevelUiPrefab;
    [SerializeField] private Transform _contentHolder;
    [SerializeField] private LocalizedTMPTextUI _locationText;
    private List<string> _locations;

    // Key:Location id
    // Value: T1 - Start level, T2 - End level
    // Example (LOCATION_1, <1,10>)
    private Dictionary<string, Tuple<int, int>> _locationDict;
    private int _currentLocationIndex = 0;
    private string CurrentLocationID => _locations[_currentLocationIndex];
    private bool _isInit = false;
    public bool IsInit => _isInit;

    private void OnEnable()
    {
        // if (_isInit)
        // {
        //     Load();
        // }
    }

    public void Init()
    {
        // _locations = new List<string>();
        // _locationDict = new Dictionary<string, Tuple<int, int>>();
        //
        // string iterLocationID = "";
        // int iterStarLevel = 0;
        // int iterCount = 0;
        //
        // var levelDesigns = DesignManager.instance.LevelCampaignDesign;
        //
        // for (int i = 0; i < levelDesigns.Count; i++)
        // {
        //     var levelDesign = levelDesigns[i];
        //
        //     if (!_locations.Contains(levelDesign.LocationId) || i == levelDesigns.Count - 1)
        //     {
        //         // Save
        //         if (iterLocationID != "")
        //         {
        //             if (i == levelDesigns.Count - 1)
        //                 iterCount++;
        //
        //             _locationDict.Add(iterLocationID,
        //                 new Tuple<int, int>(iterStarLevel, iterStarLevel + iterCount));
        //             iterCount = 0;
        //         }
        //
        //         iterLocationID = levelDesign.LocationId;
        //         iterStarLevel = (int) levelDesign.Level;
        //
        //         _locations.Add(levelDesign.LocationId);
        //     }
        //     else
        //     {
        //         iterCount++;
        //     }
        // }
        //
        // string lastLocation = SaveGameHelper.GetLastLocationId();
        // _currentLocationIndex = _locations.IndexOf(lastLocation);
        // _isInit = true;
    }

    public void Load()
    {
        // _locationText.textName = CurrentLocationID + "_NAME";
        // ShowLevels(CurrentLocationID);
    }

    private void ShowLevels(string locationId)
    {
        // _contentHolder.DestroyAllChild();
        // var levelInfo = _locationDict[locationId];
        // int currentLevelProgress = SaveGameHelper.GetLastCampaignLevel();
        //
        // for (int i = levelInfo.Item1; i <= levelInfo.Item2; i++)
        // {
        //     var levelUi = Instantiate(_selectLevelUiPrefab, _contentHolder);
        //     if (i <= currentLevelProgress)
        //     {
        //         levelUi.Load(i, SaveGameHelper.GetStarAtLevel(i), false);
        //     }
        //     else
        //     {
        //         levelUi.Load(i, 0, true);
        //     }
        //
        //     levelUi.SetClickCallback(EnterLevel);
        // }
    }

    private void EnterLevel(int level)
    {
        // Debug.LogError("Enter level " + level);
        // GameMaster.instance.GoToSceneGame(GameMode.CAMPAIGN_MODE, level);
    }

    public void NextLocation()
    {
        // _currentLocationIndex++;
        // if (_currentLocationIndex >= _locations.Count)
        //     _currentLocationIndex = 0;
        //
        // Load();
    }

    public void PreviousLocation()
    {
        // _currentLocationIndex--;
        // if (_currentLocationIndex < 0)
        //     _currentLocationIndex = _locations.Count - 1;
        //
        // Load();
    }
}