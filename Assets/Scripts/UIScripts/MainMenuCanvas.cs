using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : CanvasContainDialog
{
    public static MainMenuCanvas instance;
    [SerializeField] private MainMenuTab _tab;
    [SerializeField] private CurrencyBar _currencyBar;
    public CurrencyBar CurrencyBar => _currencyBar;
    public MainMenuTab MainMenuTab => _tab;

    private void Awake()
    {
        instance = this;
        MasterCanvas.CurrentMasterCanvas = this;
        MasterCanvas.IsInGameScene = false;
    }

    protected override void Start()
    {
        base.Start();

        onShowHUD += _currencyBar.OnShowHUD;
        onShowHUD += _tab.OnShowHUD;
        if (TutorialManager.instance != null)
            onShowHUD += TutorialManager.instance.OnShowHUD;
        onResetLayer += _currencyBar.OnShowHUD;
        onResetLayer += _tab.OnShowHUD;

        //Initialize();
        onResetLayer += ReminderManager.OnResetLayer;
        // Initialize();
    }

    private void OnDestroy()
    {
        onShowHUD -= _currencyBar.OnShowHUD;
        onShowHUD -= _tab.OnShowHUD;
        onResetLayer -= _currencyBar.OnShowHUD;
        onResetLayer -= _tab.OnShowHUD;
    }

    public void HideTabAndCurrencyBar()
    {
        _tab.gameObject.SetActive(false);
        _currencyBar.gameObject.SetActive(false);
    }

    public void ShowTabAndCurrencyBar()
    {
        if (_tab != null && _currencyBar != null)
        {
            _tab.gameObject.SetActive(true);
            _currencyBar.gameObject.SetActive(true);
        }
    }

    public override void ShowHUD(EnumHUD _type, bool hideCurrentHUD = true, Action<bool> ShowComplete = null,
        params object[] args)
    {
        if (CanOpenNewHUD())
        {
            base.ShowHUD(_type, hideCurrentHUD, ShowComplete, args);
            _lastOpenHUDTime = Time.time;
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKey("z"))
    //    {
    //       MainMenuTab.OnButtonShop(); 
    //    }else if (Input.GetKey("x"))
    //    {
    //        MainMenuTab.OnButtonHero(); 
    //    }
    //    else if (Input.GetKey("c"))
    //    {
    //        MainMenuTab.OnHomeButton();
    //    }else if (Input.GetKey("v"))
    //    {
    //        MainMenuTab.OnButtonResearch();
    //    }
    //}

    public override void Initialize()
    {
        base.Initialize();
        ShowHUD(EnumHUD.HUD_HOME, false);
        //check show dialog popup
        Timing.CallDelayed(1.0f, () =>
        {
            CheckAndShowPopupDialog();
        });

    }

    public void ShowTab(bool isShow)
    {
        if (_tab != null && _tab.gameObject != null)
            _tab.gameObject.SetActive(isShow);
    }

    public void ShowCurrency(bool isShow)
    {
        if (_currencyBar != null && _currencyBar.gameObject != null)
            _currencyBar.gameObject.SetActive(isShow);
    }

    public void HideAllHUD(EnumHUD hud)
    {
        foreach (var VARIABLE in _dictHUD)
        {
            if (VARIABLE.Key != hud)
                VARIABLE.Value.HideInstantly();
        }
    }

    #region Rating
    public static int RATING_RESHOW_DURATION = 21;

    public void CheckAndShowPopupDialog()
    {
        if (SaveManager.Instance.Data.DayTrackingData.IsDoneRating)
            return;

        int level = SaveGameHelper.GetCurrentCampaignLevel();
        var popupRatingUnlocked = DesignHelper.GetUnlockRequirement("POPUP_RATING");
        if (level >= popupRatingUnlocked.LevelUnlock)
        {
            if (SaveManager.Instance.Data.DayTrackingData.LastCountShowRating <= 0)
            {
                TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_RATING, false);
                SaveManager.Instance.Data.DayTrackingData.LastCountShowRating = RATING_RESHOW_DURATION;
            }
            else
            {
                SaveManager.Instance.Data.DayTrackingData.LastCountShowRating--;
            }
        }

        SaveManager.Instance.SetDataDirty();
    }

    #endregion
}