using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Lists;

public class HUDSelectCampaignLevel : BaseHUD
{
    public static HUDSelectCampaignLevel Instance;

    [SerializeField] private MapUIListAdapter _listAdapter;
    public MapDatasScriptableObject mapDatas;
    public Image topBG;
    public Image belowBG;
    public Image fakeBarTop;
    public Image fakeBarBelow;
    public GameObject Flag;
    public LocalizedTMPTextUI mapName;

    private List<MapData> mapDataClone;
    private List<MapData> mapDataCloneNonReverse;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void OnButtonBack()
    {
        // base.OnButtonBack();       
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_HOME);
        //FindObjectOfType<MiniGameController>().gameObject.SetActive(true);
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);

        if (!isInit)
        {
            if (_listAdapter.IsInitialized)
            {
                Init();
            }
            else
            {
                _listAdapter.Initialized += Init;
            }
        }

        if (isInit)
        {
            AutoSnap();
        }
    }

    private void AutoSnap()
    {
        int currentIndex = SaveGameHelper.GetMaxCampaignLevel();
        int snapToIndex = mapDataClone.Count - (currentIndex / 15 + 1);

        if (snapToIndex < 0)
        {
            _listAdapter.ScrollTo(0);
        }
        else
        {
            _listAdapter.ScrollTo(snapToIndex, -0.2f);
        }
    }

    public override void Init()
    {
        //DespawnChill();

        if (_listAdapter.Data.Count != 0)
            _listAdapter.Data.RemoveItemsFromStart(_listAdapter.Data.Count);


        mapDataClone = new List<MapData>();
        mapDataCloneNonReverse = new List<MapData>();

        foreach (var VARIABLE in mapDatas.mapDatas)
        {
            mapDataCloneNonReverse.Add(VARIABLE);
            mapDataClone.Add(VARIABLE);
        }

        mapDataClone.Reverse();
        topBG.color = mapDataClone[0].bgColor;
        belowBG.color = mapDataClone.Last().bgColor;
        fakeBarTop.color = mapDataClone[0].roadColor;
        fakeBarBelow.color = mapDataClone.Last().roadColor;

        _listAdapter.Data.InsertItems(0, mapDataClone);
        AutoSnap();

        base.Init();
    }

    private void Update()
    {
        var _mapItemUis = _listAdapter._VisibleItems;

        if (_mapItemUis != null && _mapItemUis.Count != 0)
        {
            float distanceMax = 99999;
            MapData mapData = null;

            foreach (var VARIABLE in _mapItemUis)
            {
                float distance = Vector3.Distance(VARIABLE.root.position, Vector3.zero);
                if (distance < distanceMax)
                {
                    distanceMax = distance;
                    mapData = VARIABLE.mapItemUI.MapData;
                }

            }

            int mapIndex = mapDataCloneNonReverse.IndexOf(mapData);
            mapIndex /= 2;
            mapName.textName = $"LOCATION_{mapIndex + 1}_NAME";
        }

    }
}