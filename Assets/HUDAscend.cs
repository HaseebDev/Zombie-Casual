using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ez.Pooly;
using DG.Tweening;
using MEC;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Lists;
using System;
using System.Linq;
using com.datld.data;
using QuickType;

public struct HUDAscendData
{
    public int currentLevel;
    public int maxLevel;
    public long totalPillEarn;
    public long totalTokenReturn;
    public long totalTimePlayed;
    public long resetTimeStamp;
}

public class HUDAscend : BaseHUD
{
    [Header("Rect Rewards")]
    public RectTransform RectRewards;
    public CanvasGroup RectInfo;

    public TextMeshProUGUI txtCurrentLevel;
    public TextMeshProUGUI txtHighestLevel;
    public TextMeshProUGUI txtTimePlayed;
    public IdleRewardAdapter _listAdapter;

    public TextMeshProUGUI txtRemainTime;

    [Header("Rewards")]
    public Image _imgToken;
    public Image _imgPill;
    public TextMeshProUGUI txtPillEarn;
    public TextMeshProUGUI txtTokenReturn;
    public Slider progressBar;
    public ScrollRect _scrollRect;

    public Button buttonAscend;
    public Button buttonAscendFromInfo;
    public IdleRewardColumnView rewardColumPrefab;
    public RectTransform _rectRewards;

    public HorizontalLayoutGroup _horizontalLayerRewards;

    [Header("Loading Icon")]
    public RectTransform _loadingIcon;

    private HUDAscendData _data;
    private int maxRewardLevel = -1;

    private int nearestBeforeNode = -1;
    private int nearestAfterNode = -1;
    private int levelDiff = 0;
    private int levelGap = 0;
    private float widthColumnView = 0f;

    private float timerRemain = 0f;

    private List<CustomBattleReward> _listRewards;

    private List<IdleRewardColumnView> _listColumRewards = new List<IdleRewardColumnView>();

    private void Start()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_IDLE_REWARD_ITEM_DATA, new Action<int, bool>(UpdateAdapterItemData));

    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        RectRewards.gameObject.SetActiveIfNot(true);
        RectInfo.gameObject.SetActiveIfNot(false);
        base.PreInit(type, _parent, args);
        Timing.RunCoroutine(InitCoroutine(type, _parent, args));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.RESET_IDLE_REWARD_ITEM_DATA, new Action<int, bool>(UpdateAdapterItemData));
    }

    private void UpdateAdapterItemData(int levelReward, bool isClaimed)
    {
        var data = _listAdapter.Data.List.FirstOrDefault(x => x.data.LevelReward == levelReward);
        if (data != null)
        {
            data.isClaimed = isClaimed;
        }

    }

    public void ResetListAdapter(PlayProgress idleProgress)
    {
        foreach (var d in _listAdapter.Data)
        {
            var isClaimed = SaveManager.Instance.Data.CheckIsClaimedRewardIdleMode(idleProgress, d.data);
            var isCollectable = _data.maxLevel >= d.data.LevelReward;
            d.isClaimable = isClaimed;
            d.isClaimable = isCollectable;
        }

        _listAdapter.Refresh();
    }

    IEnumerator<float> InitCoroutine(EnumHUD type, IParentHud _parent, params object[] args)
    {
        yield return Timing.WaitForOneFrame;
        _data = (HUDAscendData)args[0];
        _loadingIcon.gameObject.SetActiveIfNot(false);
        progressBar.maxValue = 1;
        progressBar.value = 0;
        nearestAfterNode = nearestBeforeNode = -1;
        levelGap = 0;
        widthColumnView = this.rewardColumPrefab.GetComponent<RectTransform>().sizeDelta.x;


        var idleProgress = SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE);
        _listRewards = DesignManager.instance.CustomIdleRewardDesign.Rewards;
        if (!_listAdapter.IsInitialized)
        {
            _listAdapter.Init();
        }


        if (_listAdapter.Data.Count != 0)
            _listAdapter.Data.RemoveItemsFromStart(_listAdapter.Data.Count);

        int spawnIndex = 0;
        levelDiff = this._data.currentLevel;
        foreach (var item in _listRewards)
        {
            var isClaimed = SaveManager.Instance.Data.CheckIsClaimedRewardIdleMode(idleProgress, item);
            var isCollectable = _data.maxLevel >= item.LevelReward;
            _listAdapter.Data.InsertOneAtEnd(new IdleRewardColumnModel()
            {
                data = item,
                isClaimable = isCollectable,
                isClaimed = isClaimed
            });

            if (this._data.currentLevel >= item.LevelReward)
            {
                nearestBeforeNode = spawnIndex;
                levelDiff = _data.currentLevel - item.LevelReward;
            }

            if (levelGap == 0 && item.LevelReward > this._data.currentLevel)
            {
                if (nearestBeforeNode > 0)
                    levelGap = item.LevelReward - _listRewards[nearestBeforeNode].LevelReward;
                else
                    levelGap = item.LevelReward;

                nearestAfterNode = spawnIndex;
            }

            spawnIndex++;
        }

        yield return Timing.WaitForOneFrame;

        ResourceManager.instance.GetCurrencySprite(CurrencyType.TOKEN, _imgToken, (sp) =>
         {
             _horizontalLayerRewards.SetLayoutHorizontal();

         });
        ResourceManager.instance.GetCurrencySprite(CurrencyType.PILL, _imgPill, (sp) =>
         {
             _horizontalLayerRewards.SetLayoutHorizontal();
         });
        maxRewardLevel = (int)_listRewards[_listRewards.Count - 1].LevelReward;
        // listRewards[listRewards.Count - 1].LevelReward;
        ResetInfo(_data, maxRewardLevel);
    }

    public void ResetInfo(HUDAscendData _data, int maxLevelReward)
    {
        var diffTS = _data.resetTimeStamp - TimeService.instance.GetCurrentTimeStamp();
        if (diffTS < 0)
        {
            Debug.LogError("Reset session IDle mode");
            DoResetIdleMode();
        }

        txtCurrentLevel.text = $"{_data.currentLevel}";
        txtHighestLevel.text = $"{_data.maxLevel}";

        long totalTimePlayed = SaveManager.Instance.Data.MetaData.IsUnlockIdleMode ? _data.totalTimePlayed : 0;
        txtTimePlayed.text = $"{TimeService.FormatTimeSpanShortly(totalTimePlayed)}";
        txtPillEarn.text = $"{_data.totalPillEarn}";
        txtTokenReturn.text = $"{_data.totalTokenReturn}";

        txtRemainTime.text = $"{TimeService.FormatTimeSpan(diffTS)}";
        buttonAscend.interactable = buttonAscendFromInfo.interactable = _data.currentLevel > 1;
        ResetListAdapter(SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE));

        _horizontalLayerRewards.SetLayoutHorizontal();
    }

    public void OnButtonAscend()
    {
        if (DesignManager.instance.baseAscendRewardDesign == null)
        {
            // TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING);
            TopLayerCanvas.instance.ShowHUDLoading(true);
            Timing.RunCoroutine(DesignManager.instance.LoadBaseAscendDesign((design) =>
            {
                // TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
                TopLayerCanvas.instance.ShowHUDLoading(false);
                DoAscend();
            }));
        }
        else
        {
            DoAscend();
        }

    }

    public void DoAscend()
    {
        List<RewardData> listRewards = SaveManager.Instance.Data.AscendIdlePlayProgress(DesignManager.instance.baseAscendRewardDesign);
        if (listRewards == null || listRewards.Count <= 0)
        {
            Debug.LogError("AscendIdlePlayProgress error!!!!!");
        }
        else
        {
            //OnButtonContinuePlay();
            TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, listRewards, false, false);
            _data.currentLevel = 1;
            _data.totalPillEarn = 0;
            _data.totalTokenReturn = 0;
            ResetDataWhenAscend();
            ResetInfo(_data, maxRewardLevel);
            _listAdapter.ScrollTo(0);

        }
    }

    public void ResetDataWhenAscend()
    {
        var idleProgress = SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE);
        progressBar.maxValue = 1;
        progressBar.value = 0;
        nearestAfterNode = nearestBeforeNode = -1;
        levelGap = 0;
        int spawnIndex = 0;
        levelDiff = this._data.currentLevel;
        foreach (var item in _listRewards)
        {
            var isClaimed = SaveManager.Instance.Data.CheckIsClaimedRewardIdleMode(idleProgress, item);
            var isCollectable = _data.maxLevel >= item.LevelReward;
            if (this._data.currentLevel >= item.LevelReward)
            {
                nearestBeforeNode = spawnIndex;
                levelDiff = _data.currentLevel - item.LevelReward;
            }

            if (levelGap == 0 && item.LevelReward > this._data.currentLevel)
            {
                if (nearestBeforeNode > 0)
                    levelGap = item.LevelReward - _listRewards[nearestBeforeNode].LevelReward;
                else
                    levelGap = item.LevelReward;

                nearestAfterNode = spawnIndex;
            }

            spawnIndex++;
        }
    }

    public void OnButtonContinuePlay()
    {
        GameMaster.instance.GoToSceneGame(GameMode.IDLE_MODE);
    }

    private void Update()
    {
        if (_listAdapter.IsInitialized)
        {
            var curIndex = _listAdapter._VisibleItems[0].ItemIndex;

            var visibleBefore = _listAdapter.GetItemViewsHolderIfVisible(nearestBeforeNode);
            var visibleAfter = _listAdapter.GetItemViewsHolderIfVisible(nearestAfterNode);
            if (visibleBefore != null)
            {
                var localPos = visibleBefore.idleRewardColumnView.transform.localPosition;
                float _value = CalcProgressValue(localPos, levelGap, levelDiff);
                progressBar.value = _value;
            }
            else if (visibleAfter != null)
            {
                var localPos = visibleAfter.idleRewardColumnView.transform.localPosition;
                float _value = CalcProgressValue(localPos, levelGap, levelDiff, false);
                progressBar.value = _value;
            }
            else
            {
                progressBar.value = curIndex > nearestAfterNode ? 0f : 1f;
            }

            timerRemain += Time.deltaTime;
            if (timerRemain >= 1.0f)
            {
                timerRemain = 0f;
                var diffTS = _data.resetTimeStamp - TimeService.instance.GetCurrentTimeStamp();
                txtRemainTime.text = $"{TimeService.FormatTimeSpan(diffTS)}";
                if (diffTS < 0)
                {
                    Debug.LogError("Reset session IDle mode");
                    DoResetIdleMode();
                }
            }

        }
    }

    public float CalcProgressValue(Vector3 localPos, int levelGap, int levelDiff, bool targetNodeBerfore = true)
    {
        float result = 0f;
        var curPos = localPos;
        float targetMultiply = targetNodeBerfore ? 1 : -1;
        float extraSize = 0;
        if (levelGap > 0 && targetNodeBerfore)
            extraSize = (widthColumnView / levelGap * 1.0f) * levelDiff;
        else if (levelGap > 0 && !targetNodeBerfore)
            extraSize = (widthColumnView / (levelGap * 2.0f)) * (levelGap - levelDiff);

        result = (curPos.x + targetMultiply * extraSize) / Screen.width * 1.0f;

        return result;

    }

    public void OnButtonInfo()
    {
        EnableRectInfo(true);
    }

    public void CloseRectInfo()
    {
        EnableRectInfo(false);
    }

    public void EnableRectInfo(bool enable)
    {
        RectInfo.gameObject.SetActiveIfNot(true);
        if (enable)
        {
            RectInfo.alpha = 0;
            RectInfo.DOFade(1, .3f).SetEase(Ease.Linear);
        }
        else
        {
            RectInfo.alpha = 1;
            RectInfo.DOFade(0, .3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                RectInfo.gameObject.SetActiveIfNot(false);
            });
        }

    }

    public void DoResetIdleMode()
    {
        if (SaveManager.Instance.Data.CheckAndResetIdleMode())
            Hide();
    }
}
