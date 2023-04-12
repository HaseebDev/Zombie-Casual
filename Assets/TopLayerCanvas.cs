using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Engine.UI;
using Ez.Pooly;
using UnityEngine;
using UnityEngine.UI;

public class TopLayerCanvas : BaseParentHUD
{
    public static TopLayerCanvas instance;

    public Image SplashScreen;
    public UIView _uiView;

    
    private int _counter = 0;
    private CurrencyType _lastType = CurrencyType.ARMOUR_SCROLL;
    
    private void Awake()
    {
        instance = this;
        Initialize();
        _hudGameLoading?.PreInit(EnumHUD.HUD_LOADING_GAME, this);

        EnableSimuateAdsBanner(false);
    }

    public HUDGameLoading _hudGameLoading;

    public override void Initialize()
    {
        base.Initialize();
        this.SplashScreen.gameObject.SetActiveIfNot(false);
    }

    public override void ShowFloatingTextNotify(string content, int size = 50, float moveHeight = 200,
        float duration = 0.7f, bool toUpper = true, bool trim = true)
    {
        base.ShowFloatingTextNotify(content, size, moveHeight, duration, toUpper, trim);
        _uIFloatingText.GetComponent<Canvas>().overrideSorting = false;
    }

    #region Simulate Ads banner

    public RectTransform _rectAdsBanner;

    public void EnableSimuateAdsBanner(bool enable)
    {
        if (_rectAdsBanner != null)
            _rectAdsBanner.gameObject.SetActiveIfNot(enable);
    }

    public void ShowHUDMail()
    {
        ShowHUD(EnumHUD.HUD_MAIL, false);
    }

    #endregion

    public void EnableSplashScreen(bool enable)
    {
        this.SplashScreen.gameObject.SetActiveIfNot(true);
        if (enable)
        {
            this.SplashScreen.SetColorAlpha(0);
            this.SplashScreen.DOFade(1.0f, 0.3f).SetEase(Ease.Linear);
        }
        else
        {

            this.SplashScreen.SetColorAlpha(1);
            this.SplashScreen.DOFade(0f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                this.SplashScreen.gameObject.SetActiveIfNot(false);
            });
        }
    }


    public void ShowHUDLoading(bool enable)
    {
        if (enable)
        {
            _uiView.transform.SetAsLastSibling();
            if (_uiView.IsShowing)
                _uiView.Hide(true);

            _uiView.Show();
        }
        else
        {
            // if (_uiView.IsShowing)
            if (_uiView.gameObject.activeInHierarchy)
                _uiView.Hide(true);
        }
    }
    
    public void ShowRewardSimpleHUD(List<RewardData> rewardDatas, bool autoCollect, bool showCollectAnim = false)
    {
        ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, rewardDatas, autoCollect, showCollectAnim);
    }
    
    public void ShowNotEnoughHUD(CurrencyType type, long cost, bool forceShow)
    {
        if (gameObject == null)
            return;
        
        if (type != _lastType)
        {
            _counter = 0;
            _lastType = type;
        }

        _counter++;
        if (_counter == 5 || forceShow)
        {
            switch (type)
            {
                case CurrencyType.GOLD:
                case CurrencyType.DIAMOND:
                case CurrencyType.PILL:
                case CurrencyType.WEAPON_SCROLL:
                    ShowHUD(EnumHUD.HUD_NOT_ENOUGH, false, null, type, cost);
                    break;
                default:
                    ShowFloatingTextNotify(LOCALIZE_ID_PREF.NOT_ENOUGH_RESOURCES.AsLocalizeString());
                    break;
            } 
            
            _counter = 0;
        }
        else
        {
            ShowFloatingTextNotify(LOCALIZE_ID_PREF.NOT_ENOUGH_RESOURCES.AsLocalizeString());
        }
    }
}