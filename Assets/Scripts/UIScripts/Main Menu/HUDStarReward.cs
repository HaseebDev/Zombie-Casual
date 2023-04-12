using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.TheFallenGames.OSA.Core;
using CustomListView.StarReward;
using DG.Tweening;
using Ez.Pooly;
using QuickEngine.Extensions;
using QuickType.StarReward;
using Spine;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;

public class HUDStarReward : BaseHUD
{
    [SerializeField] private StarRewardListAdapater _listAdapter;
    [SerializeField] private RectTransform _starBar;
    [SerializeField] private float _fillTime = 0.5f;

    [SerializeField] private GameObject _buyBPButton;
    [SerializeField] private GameObject _activatedBP;

    private List<StarRewardDesignElement> _listStarRewardDesignElements;
    private Dictionary<StarRewardDesignElement, StarRewardItemViewHolder> _uiDictionary;
    private int _currentStar;
    private Tuple<int, int> _nearMileStone;
    private bool _isRuningStarBarAnim = false;
    private bool _isHideTab = false;

    public void OnClickBuyBP()
    {
        BattlePassManager.BuyBP((isSuccess) =>
        {
            if (isSuccess)
            {
                Load();
                UpdateRewardState();
            }
            else
            {
                MasterCanvas.CurrentMasterCanvas.ShowPurchaseFail();
            }
        });
    }

    private void OnEnable()
    {
        CurrencyBar.Instance.HideAll();
        MainMenuTab.Instance.Hide();

        StartCoroutine(CheckToSnap());
    }

    private IEnumerator CheckToSnap()
    {
        yield return new WaitUntil(() => isInit);
        _currentStar = SaveGameHelper.GetTotalStar();
        _nearMileStone = FindNearStarMileStone();

        Load(true);
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

        _isHideTab = true;
        // StartCoroutine(CheckToSnap());
    }

    public override void Init()
    {
        _uiDictionary = new Dictionary<StarRewardDesignElement, StarRewardItemViewHolder>();
        _listStarRewardDesignElements = DesignManager.instance.starRewardDesign.StarRewardDesignElements;

        //DespawnChill();

        if (_listAdapter.Data.Count != 0)
            _listAdapter.Data.RemoveItemsFromStart(_listAdapter.Data.Count);

        for (int i = 0; i < _listStarRewardDesignElements.Count; i++)
        {
            _listAdapter.Data.InsertOneAtEnd(new StarRewardItemModel(i, _listStarRewardDesignElements[i],
                OnClickDefaultReward, OnClickBPReward));
        }

        base.Init();
    }

    // public override void ResetLayers()
    // {
    //     base.ResetLayers();
    //     
    //     _currentStar = SaveGameHelper.GetTotalStar();
    //     _nearMileStone = FindNearStarMileStone();
    //     
    //     Load(true);
    // }

    public override void OnButtonBack()
    {
        _isHideTab = false;
        MainMenuTab.Instance.Show();
        CurrencyBar.Instance.ShowAll();
        base.OnButtonBack();
    }

    // private void DespawnChill()
    // {
    //     for (int i = _contentHolder.childCount - 1; i >= 1; i--)
    //     {
    //         Pooly.Despawn(_contentHolder.GetChild(i));
    //     }
    // }

    private StarRewardDesignElement GetNextMileStone(int currentTotalStar)
    {
        var nextMileStone = _listStarRewardDesignElements.FirstOrDefault(x => x.Star > currentTotalStar);

        if (nextMileStone == null)
            nextMileStone = _listStarRewardDesignElements.GetLastItem();
        return nextMileStone;
    }

    public void Load(bool runAnim = false)
    {
        _buyBPButton.SetActive(!BattlePassManager.HasBattlePass);
        _activatedBP.SetActive(BattlePassManager.HasBattlePass);

        // _starText.text = $"{currentTotalStar}/{GetNextMileStone(currentTotalStar).Star}";

        // if (runAnim)
        // {
        //     SaveManager.Instance.Data.StarData.LastTotalStarInHUDHome = currentTotalStar;
        // }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            UpdateRewardState();
            int nearCanClaim = FindNearCanClaim();
            int nextMileStone = _nearMileStone.Item1;
            int result = nearCanClaim != -1 ? nearCanClaim : nextMileStone;
            _listAdapter.ScrollTo(result);

            DOVirtual.DelayedCall(0.1f, () =>
            {
                if (runAnim)
                    UpdateStarBar();
            });
        });
    }

    private void UpdateRewardState()
    {
        var claimedDefault = SaveManager.Instance.Data.StarData.ClaimedDefault;
        var claimedBP = SaveManager.Instance.Data.StarData.ClaimedBattlePass;

        int index = 0;
        foreach (var rewardUi in _listAdapter.Data)
        {
            int star = rewardUi.designElement.Star;
            bool canReceive = star <= _currentStar;

            rewardUi.SetLock(!canReceive);

            if (claimedDefault.Contains(star))
            {
                rewardUi.SetClaim(true);
            }

            if (claimedBP.Contains(star))
            {
                rewardUi.SetClaim(false);
            }

            StarRewardItemViewHolder holder = _listAdapter.GetItemViewsHolderIfVisible(index);
            holder?.UpdateView(rewardUi);
            index++;
        }
    }

    public static int GetCanClaimRewardCount()
    {
        int currentTotalStar = SaveGameHelper.GetTotalStar();
        var claimedDefault = SaveManager.Instance.Data.StarData.ClaimedDefault;
        var claimedBP = SaveManager.Instance.Data.StarData.ClaimedBattlePass;

        int totalNotClaim = 0;

        foreach (var designElement in DesignManager.instance.starRewardDesign.StarRewardDesignElements)
        {
            int star = designElement.Star;
            bool canReceive = star <= currentTotalStar;

            if (canReceive && !claimedDefault.Contains(star))
                totalNotClaim++;

            if (canReceive && BattlePassManager.HasBattlePass && !claimedBP.Contains(star))
                totalNotClaim++;
        }

        return totalNotClaim;
    }

    public void UpdateStarBar()
    {
        if (_currentStar != SaveManager.Instance.Data.StarData.LastTotalStarInHUDHome)
        {
            _isRuningStarBarAnim = true;
            _starBar.anchoredPosition = Vector2.zero;

            _starBar.DOAnchorPosY(GetNextStarBarPosition().y, _fillTime).OnComplete(() =>
            {
                _isRuningStarBarAnim = false;
            });
        }

        SaveManager.Instance.Data.StarData.LastTotalStarInHUDHome = _currentStar;

        // float percent = (float) currentTotalStar / _totalStar;
        // _starBar.value = (float) _lastTotalStar / _totalStar;
        //
        // if (!Mathf.Approximately(percent, _starBar.value))
        // {
        //     _starBar.DOValue(percent, _fillTime);
        // }
    }

    private void OnClickDefaultReward(StarRewardDesignElement element, RewardData rewardData)
    {
        // Debug.LogError($"Default, star {element.Star}");

        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {

        //    }
        //});

        MainMenuCanvas.instance.ShowRewardSimpleHUD(new List<RewardData> {rewardData}, true);
        int index = DesignManager.instance.starRewardDesign.StarRewardDesignElements.IndexOf(element);
        _listAdapter.Data[index].SetClaim(true);
        _listAdapter.GetItemViewsHolderIfVisible(index).UpdateView(_listAdapter.Data[index]);

        SaveManager.Instance.Data.StarData.ClaimedDefault.Add(element.Star);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.CLAIM_STAR_REWARD);
    }

    private void OnClickBPReward(StarRewardDesignElement element, RewardData rewardData)
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {

        //    }
        //});

        // Debug.LogError($"BP, star {element.Star}");
        MainMenuCanvas.instance.ShowRewardSimpleHUD(new List<RewardData> {rewardData}, true);

        int index = DesignManager.instance.starRewardDesign.StarRewardDesignElements.IndexOf(element);
        _listAdapter.Data[index].SetClaim(false);
        _listAdapter.GetItemViewsHolderIfVisible(index).UpdateView(_listAdapter.Data[index]);

        // _uiDictionary[element].SetClaim(false);
        SaveManager.Instance.Data.StarData.ClaimedBattlePass.Add(element.Star);
    }

    private Tuple<int, int> FindNearStarMileStone()
    {
        var designs = DesignManager.instance.starRewardDesign.StarRewardDesignElements;

        for (int i = 0; i < designs.Count - 1; i++)
        {
            if (designs[i].Star <= _currentStar && designs[i + 1].Star >= _currentStar)
            {
                return new Tuple<int, int>(i, i + 1);
            }
        }

        if (_currentStar > designs.GetLastItem().Star)
        {
            return new Tuple<int, int>(designs.Count - 2, designs.Count - 1);
        }

        if (_currentStar < designs[0].Star)
        {
            return new Tuple<int, int>(0, 0);
        }

        return null;
    }

    private int FindNearCanClaim()
    {
        int index = 0;
        var claimedDefault = SaveManager.Instance.Data.StarData.ClaimedDefault;
        var claimedBP = SaveManager.Instance.Data.StarData.ClaimedBattlePass;

        foreach (var rewardUi in _listAdapter.Data)
        {
            int star = rewardUi.designElement.Star;
            bool canReceive = star <= _currentStar;

            if (canReceive)
            {
                if (!claimedDefault.Contains(star) || (BattlePassManager.HasBattlePass && !claimedBP.Contains(star)))
                {
                    return index;
                }
            }

            index++;
        }

        return -1;
    }

    private void Update()
    {
        if (_isHideTab)
        {
            MainMenuTab.Instance.Hide();
            CurrencyBar.Instance.HideAll();
        }


        if (_isRuningStarBarAnim)
            return;


        if (_nearMileStone == null)
        {
            // _starBar.anchoredPosition = new Vector2(0,-1951);
            // _starBar.value = 0;
        }
        else
        {
            _starBar.anchoredPosition = GetNextStarBarPosition();

            // Debug.LogError("Star " + _currentStar);
            // Debug.LogError("Before " + before.root.transform.position);
            // Debug.LogError("After " + after.root.transform.position);
        }
    }

    private Vector2 GetNextStarBarPosition()
    {
        var before = _listAdapter.GetItemViewsHolderIfVisible(_nearMileStone.Item1);
        var after = _listAdapter.GetItemViewsHolderIfVisible(_nearMileStone.Item2);

        if (before != null && after != null)
        {
            var designs = DesignManager.instance.starRewardDesign.StarRewardDesignElements;
            var deltaStar = designs[_nearMileStone.Item2].Star - designs[_nearMileStone.Item1].Star;
            var deltaStarCore = _currentStar - designs[_nearMileStone.Item1].Star;

            var deltaPosition = after.root.anchoredPosition.y -
                                before.root.anchoredPosition.y;

            return new Vector2(0,
                before.root.anchoredPosition.y - deltaStarCore * Mathf.Abs(deltaPosition) / deltaStar);
        }
        else
        {
            var all = _listAdapter.gameObject.GetComponentsInChildren<StarRewardUI>().ToList();
            var min = all.Min(x => x.StarRewardDesignElement.Star);
            var max = all.Max(x => x.StarRewardDesignElement.Star);
            if (_currentStar < min)
            {
                return Vector2.zero;
            }

            if (_currentStar > max)
            {
                return new Vector2(0, -_starBar.rect.height);
            }
        }

        return Vector2.zero;
    }

    public void CleanUp()
    {
        // foreach (var VARIABLE in _starRewardUis)
        // {
        //     Pooly.Despawn(VARIABLE.transform);
        // }
    }
}