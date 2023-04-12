using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using DG.Tweening;
using MEC;
using QuickType.Chest;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class HUDShop : CanExitHUD
{
    [SerializeField] private AssetReference locationPackShopPrefab;
    [SerializeField] private AssetReference chestHeroShopPrefab;
    [SerializeField] private AssetReference chestShopPrefab;
    [SerializeField] private AssetReference diamondShopPrefab;
    [SerializeField] private AssetReference goldPrefab;
    [SerializeField] private AssetReference weaponScrollShopPrefab;
    [SerializeField] private AssetReference potionShopPrefab;
    [SerializeField] private AssetReference openChestResultPrefab;
    [SerializeField] private AssetReference openChestx10ResultPrefab;
    [SerializeField] private AssetReference openChestHeroResultPrefab;

    [SerializeField] private Transform content;
    [SerializeField] private Transform bottomHolder;
    public GameObject loadingIcon;

    private ShopHeroChest _chestHeroShop;
    private ChestShop _chestShop;
    private BaseShop _diamondShop;
    private BaseShop _goldShop;
    private BaseShop _weaponScrollShop;
    private BaseShop _potionShop;
    private LocationPackShop _locationPackShop;

    [SerializeField] private ScrollRect _masterScrollRect;

    public static HUDShop Instance = null;
    private float _lastSnapTime = -10f;

    private bool _isLoadAll = false;
    private Action showCompleteCallback;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        if (args != null)
        {
            showCompleteCallback = (Action) args[0];
        }
    }

    private IEnumerator<float> SnapCoroutine(ShopType shopType)
    {
        // Debug.LogError("PRE SNAP TO " + shopType);

        while (!_isLoadAll)
        {
            yield return Timing.WaitForOneFrame;
        }

        // yield return Timing.WaitForOneFrame;
        RectTransform target = null;

        switch (shopType)
        {
            case ShopType.DIAMOND:
                target = _diamondShop.GetComponent<RectTransform>();
                break;
            case ShopType.GOLD:
                target = _goldShop.GetComponent<RectTransform>();
                break;
            case ShopType.WEAPON_COIN:
                target = _weaponScrollShop.GetComponent<RectTransform>();
                break;
            case ShopType.POTION:
                target = _potionShop.GetComponent<RectTransform>();
                break;
            case ShopType.CHEST:
                target = _chestShop.GetComponent<RectTransform>();
                break;
            case ShopType.FREE_STUFF:
                yield break;
            //target = _freeStuffShop.GetComponent<RectTransform>();
            //break;
            default:
                throw new ArgumentOutOfRangeException(nameof(shopType), shopType, null);
        }

        if (target != null)
        {
            if (gameObject.activeInHierarchy)
            {
                // Debug.LogError("START SNAP TO " + shopType);
                StartCoroutine(_masterScrollRect.BringChildIntoView(target, 100));
            }
        }
    }

    public void SnapToShopType(ShopType shopType)
    {
        if (Time.time - _lastSnapTime > 0.5f)
        {
            _lastSnapTime = Time.time;
            
            // Debug.LogError("START MOVE"); 
            Timing.RunCoroutine(SnapCoroutine(shopType));
        }
    }

    public override void Init()
    {
        if (!isInit)
        {
        }

        base.Init();
    }

    public void Init(Action callback)
    {
        if (!isInit)
        {
            Timing.RunCoroutine(InitAsync(callback));
        }
        else
        {
            callback?.Invoke();
            DOVirtual.DelayedCall(0.1f, () =>
                {
                    showCompleteCallback?.Invoke();
                    showCompleteCallback = null;
                }
            );
        }

        base.Init();
    }

    IEnumerator<float> InitAsync(Action callback)
    {
        AsyncOperationHandle<GameObject> op;

        op = locationPackShopPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _locationPackShop = op.Result.GetComponentInChildren<LocationPackShop>();
        _locationPackShop.Load();
        // op.Result.SetActive(false);

        yield return Timing.WaitForOneFrame;

        op = chestShopPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _chestShop = op.Result.GetComponent<ChestShop>();
        _chestShop.Init();
        _chestShop.Load(true);

        yield return Timing.WaitForOneFrame;

        op = chestHeroShopPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _chestHeroShop = op.Result.GetComponent<ShopHeroChest>();
        bool isUnlocked = DesignHelper.GetUnlockRequirementLevel(UnlockRequireId.SHOP_HERO_CHEST).Item1;
        if(!isUnlocked)
            _chestHeroShop.gameObject.SetActive(false);
        
        yield return Timing.WaitForOneFrame;

        
        op = diamondShopPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _diamondShop = op.Result.GetComponent<DiamondShop>();

        op = goldPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _goldShop = op.Result.GetComponent<BaseShop>();

        op = weaponScrollShopPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _weaponScrollShop = op.Result.GetComponent<BaseShop>();

        op = potionShopPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _potionShop = op.Result.GetComponent<BaseShop>();

        yield return Timing.WaitForOneFrame;
      

        _isLoadAll = true;

        op = openChestResultPrefab.InstantiateAsync(transform);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _chestShop._chestOpenResultUi = op.Result.GetComponent<ChestOpenResult>();

        op = openChestx10ResultPrefab.InstantiateAsync(transform);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _chestShop._chestOpenTenResult = op.Result.GetComponent<ChestOpenTenResult>();

        op = openChestHeroResultPrefab.InstantiateAsync(transform);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _chestHeroShop.openResult = op.Result.GetComponent<ChestHeroOpenResult>();
        
        bottomHolder.SetParent(content);
        loadingIcon.SetActive(false);


        // _freeStuffShop.Init();
        yield return Timing.WaitUntilDone(_diamondShop.Load());
        yield return Timing.WaitUntilDone(_goldShop.Load());
        yield return Timing.WaitUntilDone(_weaponScrollShop.Load());
        yield return Timing.WaitUntilDone(_potionShop.Load());
        
        showCompleteCallback?.Invoke();
        showCompleteCallback = null;
        // _goldShop.Load();
        // _weaponScrollShop.Load();
        // _potionShop.Load();

        callback?.Invoke();


        // yield return Timing.WaitForSeconds(0.1f);

        // Dont active shop
    }

    public override void ResetLayers()
    {
        base.ResetLayers();
        _chestShop?.HideAllResultPanel();

        Init(() =>
        {
            // _freeStuffShop.CheckForUpdate();
            _chestHeroShop?.ResetLayer();
            _diamondShop?.ResetLayer();
            _goldShop?.ResetLayer();
            _weaponScrollShop?.ResetLayer();

            _chestShop?.Load();
            _locationPackShop?.Load();
            _currentActiveShining?.ActiveShiny(false);

            DOVirtual.DelayedCall(2f, ReminderManager.SaveCurrentShopState);
        });
    }

    private ShopItemUI _currentActiveShining;

    public void HighLightPackage(ShopDesignElement shopDesignElement)
    {
        if (_currentActiveShining != null)
        {
            if (shopDesignElement.Id == _currentActiveShining.ShopDesignElement.Id)
            {
                _currentActiveShining?.ActiveShiny(true);
                return;
            }
        }

        var allShopItems = GetComponentsInChildren<ShopItemUI>().ToList();

        _currentActiveShining = allShopItems.Find(x => x.ShopDesignElement.Id == shopDesignElement.Id);
        _currentActiveShining?.ActiveShiny(true);
    }

    private bool _isFirstTime = true;

    public void FocusOnHighlight()
    {
        if (_locationPackShop.gameObject.activeInHierarchy)
        {
            if (ReminderManager.HasNewShopItem() > 0)
            {
                SnapToShopType(ShopType.CHEST);
            }

            _isFirstTime = false;
            return;
        }

        if (_isFirstTime)
        {
            _isFirstTime = false;
            return;
        }

        if (ReminderManager.HasNewShopItem() > 0)
        {
            SnapToShopType(ShopType.CHEST);
        }
    }
}