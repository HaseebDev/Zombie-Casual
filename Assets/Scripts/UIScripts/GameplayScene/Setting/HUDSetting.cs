using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDSetting : BaseHUD
{
    [SerializeField] private Button _replayButton;
    [SerializeField] private GameObject _bottomButton;
    [SerializeField] private RectTransform _content;
    
    private MyPopup _myPopup;
    private float _originHeight = 1200;
    private float _expandedHeight = 1800f;

    private bool _isInGameScene = false;
    public static HUDSetting Instance;
    

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        if (args != null)
        {
            _isInGameScene = (bool) args[0];
            _bottomButton.SetActive(_isInGameScene);
            if (_isInGameScene)
            {
                _content.sizeDelta = new Vector2(_content.sizeDelta.x,1620f);
            }
            else
            {
                _content.sizeDelta = new Vector2(_content.sizeDelta.x,1400);
            }
        }
        
        if (_isInGameScene)
            _replayButton.gameObject.SetActive(GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE);
    }

    private void OnEnable()
    {
        GamePlayController.instance?.SetPauseGameplay(true);
    }

    private void OnDisable()
    {
        GamePlayController.instance?.SetPauseGameplay(false);
    }

    public void OnHomeButtonClick()
    {
        GameMaster.instance.BackToMenuScene();
        Hide();
    }

    public void OnRestartButtonClick()
    {
        GamePlayController.instance.RestartCampaignMode();
        Hide();
    }

    public override void ResetLayers()
    {
        base.ResetLayers();
        var settingData = SaveManager.Instance.Data.SettingData;
        _soundToggle.isOn = settingData.IsSfxOn;
        _bgmToggle.isOn = settingData.IsBgmOn;
        _freeStuffToggle.isOn = settingData.IsFreeStuffNotificationOn;
        _rareChestToggle.isOn = settingData.IsRareChestNotificationOn;
        _legendaryChestToggle.isOn = settingData.IsLegendaryChestNotificationOn; 
    }

    public void OnCloseButtonClick()
    {
        if (_myPopup == null)
            _myPopup = GetComponent<MyPopup>();
        _myPopup.Hide();
        OnButtonBack();
    }

    [Header("Notification")] [SerializeField]
    private Toggle _rareChestToggle;

    [SerializeField] private Toggle _legendaryChestToggle;
    [SerializeField] private Toggle _freeStuffToggle;
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Toggle _bgmToggle;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        _soundToggle.onValueChanged.AddListener(AudioSystem.instance.SwitchSoundState);
        _bgmToggle.onValueChanged.AddListener(AudioSystem.instance.SwitchBGMState);
        _freeStuffToggle.onValueChanged.AddListener(NotificationManager.Instance.SwitchFreeStuffNotificationToggle);
        _rareChestToggle.onValueChanged.AddListener(NotificationManager.Instance.SwitchRareChestNotificationToggle);
        _legendaryChestToggle.onValueChanged.AddListener(NotificationManager.Instance
            .SwitchLegendaryChestNotificationToggle);
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    public void OpenMail()
    {
        TopLayerCanvas.instance.ShowHUDMail();
    }

    public void OpenServiceWeb()
    {
        Application.OpenURL("https://www.namnh.com/privacy-policy");
    }
}