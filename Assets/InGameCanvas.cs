using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;
using UnityEngine.UI;
using UnityEngine.UIElements;
using DG.Tweening;
using QuickType.Shop;
using UnityExtensions;
using Button = UnityEngine.UI.Button;

[Serializable]
public enum INGAME_SHOP
{
    NONE,
    GEM,
    COIN,
    WEAPON_SCROLL,
    POTION
}


public class InGameCanvas : CanvasContainDialog
{
    public static InGameCanvas instance;
    public GamePanelView _gamePannelView;
    public HintTapOnScreen _hintTapOnScreen;
    public CanvasGroup _gameBlackScreenView;

    [Header("CastleHPBar")] public HealthBar _castleHPBarPrefab;
    public Transform _castleHPHolder;

    //public MyPopup shop;
    public ShopInGameView shop;
    public List<BaseShop> shops;

    
    private void Awake()
    {
        instance = this;
        MasterCanvas.CurrentMasterCanvas = this;
        MasterCanvas.IsInGameScene = true;

        Initialize();

        foreach (var shop in shops)
        {
            shop.Load();
        }

        shop.Shops = shops;

        //EnableBlackScreen(false, 0f);
    }

    private void Start()
    {

    }

    public void CleanUp()
    {
        _gamePannelView?.CleanUp();
    }

    //public void ShowFloatingText(Vector3 screenPos, string content, int size, float moveHeight, Color color, float duration = 0.3f)
    //{
    //    UIFloatingText _UIFloatingText = Pooly.Spawn<UIFloatingText>(POOLY_PREF.FLOATING_TEXT, Vector3.zero, Quaternion.identity, transform);
    //    _UIFloatingText.transform.SetParent(transform);
    //    _UIFloatingText.Initialize(content, screenPos, size, color, moveHeight, duration);
    //    _UIFloatingText.Show();
    //}

    #region IDLE Offline Reward

    public void ShowHUDIdleOfflineReward(IdleOfflineRewardData data, Action<bool> Complete)
    {
        ShowHUD(EnumHUD.HUD_IDLE_OFFLINE_REWARD, false, null, data, Complete);
    }

    #endregion

    #region Black Screen

    public void EnableBlackScreen(bool enable, float timeFX = 0.5f)
    {
        if (enable)
        {
            _gameBlackScreenView.alpha = 0;
            _gameBlackScreenView.gameObject.SetActiveIfNot(true);
            _gameBlackScreenView.DOFade(1.0f, timeFX).SetEase(Ease.Linear);
        }
        else
        {
            _gameBlackScreenView.alpha = 1;
            _gameBlackScreenView.gameObject.SetActiveIfNot(true);
            _gameBlackScreenView.DOFade(0f, timeFX).SetEase(Ease.Linear).OnComplete(() =>
            {
                _gameBlackScreenView.gameObject.SetActiveIfNot(false);
            });
        }
    }

    #endregion

    public HealthBar SpawnCastleHPBar(Vector2 screenPos)
    {
        HealthBar result = GameObject.Instantiate<HealthBar>(_castleHPBarPrefab, _castleHPHolder);
        result.transform.position = screenPos - new Vector2(0,200);
        result.transform.localScale = Vector3.one;

        return result;
    }

    public void ShowShop(INGAME_SHOP targetCategory = INGAME_SHOP.NONE)
    {
        shop.Show(targetCategory);
  
    }

    public void ShowShop(int targetCategory)
    {
        ShowShop((INGAME_SHOP)targetCategory);
    }

    public void HighlightShopPack(ShopDesignElement pack)
    {
        shop.HighLightPackage(pack);
    }

    public void HideShop()
    {
        shop.Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!BackButtonManager.Instance.HasPopup())
            {
                if (HUDSetting.Instance == null || !HUDSetting.Instance.gameObject.activeInHierarchy)
                {
                    ShowHUDSetting();
                }
            }
        }
    }

    public void ShowComingSoon()
    {
        MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.COMING_SOON.AsLocalizeString());
    }

    public void ShowHUDSetting()
    {
        MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_SETTING, false, null, true);
    }
}
