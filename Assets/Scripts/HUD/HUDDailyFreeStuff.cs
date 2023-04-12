using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using DG.Tweening;
using Google.Protobuf.Collections;
using MEC;
using QuickType.FreeStuff;
using QuickType.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class HUDDailyFreeStuff : BaseHUD
{
    [SerializeField] protected Transform _shopItemHolder;
    [SerializeField] protected ShopItemUI _shopItemUiPrefab;

    [SerializeField] private TextMeshProUGUI _resetTimeText;
    [SerializeField] private GameObject _arrowPrefab;

    private List<FreeStuffDesignElement> _freeStuffs;
    private Dictionary<FreeStuffDesignElement, FreeStuffItemUI> _freeStuffUis;
    private ShopData _shopData;

    private bool _isEnable = false;
    private bool _isInitArrow = false;
    private List<FreeStuffItemUI> _items;

    private CoroutineHandle _counterCoroutine;

    public override void Init()
    {
        if (!isInit)
        {
            _shopData = SaveManager.Instance.Data.ShopData;
            _counterCoroutine = Timing.RunCoroutine(CounterTime());

            // Timing.RunCoroutine(CounterTime().CancelWith(gameObject));
            Load();
            InitArrow();
        }

        base.Init();
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        GamePlayController.instance?.SetPauseGameplay(true);
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        GamePlayController.instance?.SetPauseGameplay(false);
    }

    private void OnDestroy()
    {
        Timing.KillCoroutines(_counterCoroutine);
    }

    private void InitArrow()
    {
        if (!_isInitArrow)
        {
            AddArrow();
            _isInitArrow = true;
        }
    }

    public override void ResetLayers()
    {
        base.ResetLayers();
        Init();
        CheckTime();
        CheckCurrentFreeStuffIndex();
    }

    public static bool HasAvailableItem()
    {
        int freeStuffCount = GetFreeStuffCount();
        return SaveManager.Instance.Data.ShopData.CurrentIndexFreeStuff < freeStuffCount;
    }

    public static List<FreeStuffDesignElement> GetFreeStuffs()
    {
        var freeStuff = new List<FreeStuffDesignElement>();
        foreach (var shopDesignElement in DesignManager.instance.freeStuffDesign.FreeStuffDesignElements)
        {
            if (shopDesignElement.Enable)
            {
                freeStuff.Add(shopDesignElement);
            }
        }

        return freeStuff;
    }

    public void ClearOldItems()
    {
        foreach (Transform child in _shopItemHolder)
        {
            Destroy(child.gameObject);
        }
    }

    public void Load()
    {
        ClearOldItems();

        List<FreeStuffDesignElement> designs = GetFreeStuffDesigns();

        _freeStuffs = new List<FreeStuffDesignElement>();
        _freeStuffUis = new Dictionary<FreeStuffDesignElement, FreeStuffItemUI>();
        _items = new List<FreeStuffItemUI>();

        foreach (var shopDesignElement in designs)
        {
            // if (shopDesignElement.Enable)
            // {
            var shopItem = (FreeStuffItemUI) Instantiate(_shopItemUiPrefab, _shopItemHolder);
            shopItem.Load(shopDesignElement);
            shopItem.SetOnPurchaseCallback(OnPurchase);

            _freeStuffs.Add(shopDesignElement);
            _freeStuffUis.Add(shopDesignElement, shopItem);
            _items.Add(shopItem);

            // }
        }

        CheckCurrentFreeStuffIndex();
    }

    private List<FreeStuffDesignElement> GetFreeStuffDesigns()
    {
        List<FreeStuffDesignElement> result = new List<FreeStuffDesignElement>();
        if (_shopData.LastFreeStuffItems.Count == 0)
        {
            Dictionary<string, List<FreeStuffDesignElement>> byIds = DesignHelper.GetDictFreeStuffDesign();

            foreach (var VARIABLE in byIds)
            {
                var random = VARIABLE.Value.PickRandom();
                result.Add(random);
                _shopData.LastFreeStuffItems.Add(random.Id);
            }
        }
        else
        {
            foreach (var id in _shopData.LastFreeStuffItems)
            {
                result.Add(DesignManager.instance.freeStuffDesign.FreeStuffDesignElements.Find(x => x.Id == id));
            }
        }

        return result;
    }

    private void AddArrow()
    {
        if (_arrowPrefab == null)
            return;

        // Vector2 size = grid.cellSize;
        // Vector2 space = grid.spacing;

        DOVirtual.DelayedCall(0.5f, () =>
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if ((i + 1) % 3 != 0 && i != _items.Count - 1)
                {
                    var arrow = Instantiate(_arrowPrefab, _shopItemHolder);
                    Vector3 position = (_items[i].rectTransform().anchoredPosition +
                                        _items[i + 1].rectTransform().anchoredPosition) / 2f;

                    position.y = _items[i].rectTransform().anchoredPosition.y;

                    arrow.rectTransform().anchoredPosition = position;

                    arrow.gameObject.SetActive(true);
                }
            }
        });
    }

    public static int GetAvailableFreeStuffCount()
    {
        return (int) GetFreeStuffCount() - (int) SaveManager.Instance.Data.ShopData.CurrentIndexFreeStuff;
    }

    private IEnumerator<float> CounterTime()
    {
        var resetTime = TimeService.instance.GetTimeStampBeginDay() + TimeService.DAY_SEC -
                        TimeService.instance.GetCurrentTimeStamp();
        while (true)
        {
            resetTime -= 1;

            if (gameObject != null && gameObject.activeInHierarchy && gameObject.activeInHierarchy)
            {
                _resetTimeText.text = $"{TimeService.FormatTimeSpan(resetTime)}";

                CheckTime();
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    private static int GetFreeStuffCount()
    {
        int freeStuffCount = 0;
        foreach (var shopDesignElement in DesignHelper.GetDictFreeStuffDesign())
        {
            freeStuffCount++;
        }

        return freeStuffCount;
    }

    private void CheckTime()
    {
        if (SaveManager.Instance.Data.ShopData.LastDayResetFreeStuff != TimeService.instance.GetCurrentDateTime().DayOfYear)
        {
            ResetShop();
        }
    }

    // Call when reach new day
    private void ResetShop()
    {
        // _shopData
        // = new RepeatedField<string>();

        _shopData.LastFreeStuffItems.Clear();
        _shopData.LastDayResetFreeStuff = TimeService.instance.GetCurrentDateTime().DayOfYear;
        _shopData.CurrentIndexFreeStuff = 0;

        CheckCurrentFreeStuffIndex();
        SaveManager.Instance.SaveData();
    }

    private void UpdateAfterReceive()
    {
        if (_shopData.CurrentIndexFreeStuff < _freeStuffs.Count)
        {
            var justOpen = _freeStuffs[(int) _shopData.CurrentIndexFreeStuff];
            _freeStuffUis[justOpen].CanReceive = true;
        }

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.CHANGE_DAILY_FREE_STUFF, GetAvailableFreeStuffCount());
    }

    public void OnPurchase(CostData costData, List<RewardData> rewardDatas, ShopItemUI shopItemUi,
        Action<bool> callback)
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {
        //        AddReward(rewardDatas);
        //        MasterCanvas.CurrentMasterCanvas.SpawnCollectAnim(rewardDatas, shopItemUi.transform.position, 0, 5,
        //            100);
        //    }

        //    callback?.Invoke(reached);
        //});

        AddReward(rewardDatas);
        MasterCanvas.CurrentMasterCanvas.SpawnCollectAnim(rewardDatas, shopItemUi.transform.position, 0, 5,
            100);


        var justOpen = _freeStuffs[(int) _shopData.CurrentIndexFreeStuff];
        if (justOpen != null)
        {
            _freeStuffUis[justOpen].SoldOut();
        }

        _shopData.CurrentIndexFreeStuff++;
        UpdateAfterReceive();

        callback?.Invoke(true);
    }

    private void CheckCurrentFreeStuffIndex()
    {
        if (_freeStuffs != null)
        {
            for (int i = 0; i < _freeStuffs.Count; i++)
            {
                var shopItem = _freeStuffUis[_freeStuffs[i]];
                shopItem.CanReceive = false;

                if (i < _shopData.CurrentIndexFreeStuff)
                {
                    shopItem.SoldOut();
                }
                else
                {
                    shopItem.Available();
                }
            }

            UpdateAfterReceive();
        }
    }

    public void AddReward(List<RewardData> rewardDatas)
    {
        foreach (var rewardData in rewardDatas)
        {
            SaveManager.Instance.Data.AddReward(rewardData);
        }
    }

    public override void OnButtonBack()
    {
        base.OnButtonBack();
        FindObjectOfType<HUDHomeMenu>()?.ResetLayers();
    }
}