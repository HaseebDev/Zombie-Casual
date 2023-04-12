using System;
using Framework.Interfaces;
using Framework.Utility;
using Framework.Views;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using Ez.Pooly;
using UnityExtensions.Localization;
using Extensions = UnityExtensions.Extensions;
using DG.Tweening;
using QuickEngine.Extensions;
using MEC;

public class GamePanelView : MonoBehaviour, ITickLate, IState
{
    [Header("Reward Value")] public Image iconCurrency;

    [Header("Add On Pannel")] public RectTransform _rectAddOnContent;
    public ReminderUI _addOnReminderUI;

    [Header("Special Item Pannel")] public RectTransform _rectSpecialItems;

    public RectTransform _rectSpaceAdsBanner;
    private GameMode _mode;

    [Header("Change Team")]
    [SerializeField]
    private ChangeTeam2 _changeTeam2;

    [SerializeField] private Transform _heroSlotContent;
    [SerializeField] private Transform _arrow;
    [SerializeField] private RectTransform _heroButtonHolder;
    private HeroSlot[] _heroSlots;
    public AngleUpgradeButton _AngleUpgradeButton;

    [Header("Weapon")] [SerializeField] private ChangeWeapon _changeWeapon;

    [Header("Ultimate")] [SerializeField] private Transform _ultimateHolder;
    private CharacterUltimateButton[] _ultimateButtons;

    [Header("ZomHpBar")] public Slider _zomHPBar;
    public PulseEffect _zomHpBarPulseEffect;

    [Header("Wave/ Level View")] public LocalizedTMPTextUI levelTxt;
    public TextMeshProUGUI txtNextWave;
    public TextMeshProUGUI txtMaxWave;
    public TextMeshProUGUI txtTimerWave;
    public TextMeshProUGUI txtInTimer;

    private int levelShowSpecialAddOn = -1;
    private int levelEnableSpecialAddon = -1;

    private long lastGoldValue = 0;
    private long lastWeaponScrollValue = 0;

    private Tween showBannerDelay;

    private void Start()
    {
        this.levelUpAnimator = this.levelUpBtn.GetComponent<Animator>();

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(ResetInfo));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_GAME_HUD, new Action(PlayAnimGainCurrency));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ON_REMOVE_ADS, new Action(OnRemoveAds));
    }

    private void OnRemoveAds()
    {
        if (SaveManager.Instance.Data.RemoveAds)
        {
            _rectSpaceAdsBanner.gameObject.SetActiveIfNot(false);
            AdsManager.instance.HideAdsBanner(null);
        }
    }

    public void InitByGameMode(GameMode mode)
    {
        _mode = mode;
        SetGoodState();
        iconCurrency.gameObject.SetActive(true);
        _rectSpaceAdsBanner.gameObject.SetActiveIfNot(false);
        switch (mode)
        {
            case GameMode.NONE:
                break;
            case GameMode.CAMPAIGN_MODE:
                EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_CASTLE_HP,
                    new Action<float, float>(SetCastleHPValue));
                ResourceManager.instance.GetCurrencySprite(CurrencyType.GOLD, iconCurrency);
                break;
            case GameMode.IDLE_MODE:
                showBannerDelay = DOVirtual.DelayedCall(60, () =>
                {
                    if (!SaveManager.Instance.Data.RemoveAds)
                    {
                        AdsManager.instance.RequestAdsBanner((successs)=> {
                            if(successs)
                            {
                                AdsManager.instance.ShowAdsBanner();
                                _rectSpaceAdsBanner.gameObject.SetActiveIfNot(true);
                            }
                              
                        });
                    }
                }).SetUpdate(true);

                EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_IDLE_HUD,
                    new Action<int, int>(SetProgressWave));
                ResourceManager.instance.GetCurrencySprite(CurrencyType.TOKEN, iconCurrency);
                var levelWaveData = GamePlayController.instance._waveController._levelWaveData;
                SetProgressWave(levelWaveData.currentWave, levelWaveData.totalWave);
                //temp off ads banner
                //AdsManager.instance.ShowAdsBanner((complete) =>
                //{
                //    _rectSpaceAdsBanner.gameObject.SetActiveIfNot(complete);
                //});
                break;
            default:
                break;
        }

        ResetInfo();


        _heroSlots = _heroSlotContent.GetComponentsInChildren<HeroSlot>();
        _ultimateButtons = _ultimateHolder.GetComponentsInChildren<CharacterUltimateButton>(true);

        // List<string> availableHeroId = new List<string>();
        //sort cards
        // for (int i = 0; i < SaveManager.Instance.Data.GameData.TeamSlots.Count; i++)
        // {
        //     var heroID = SaveManager.Instance.Data.GameData.TeamSlots[i];
        //     if (heroID != null && heroID != GameConstant.NONE)
        //     {
        //         availableHeroId.Add(heroID);
        //     }
        // }

        // for (int i = 0; i < availableHeroId.Count; i++)
        // {
        //     _heroSlots[i].Load(availableHeroId[i], i);
        // }

        // for (int i = availableHeroId.Count; i < _heroSlots.Length; i++)
        // {
        //     _heroSlots[i].Load(GameConstant.NONE, i);
        // }

        int availableHero = 0;
        int ultimateIndex = 0;

        for (int i = 0; i < _heroSlots.Length; i++)
        {
            _heroSlots[i].Load(SaveManager.Instance.Data.GameData.TeamSlots[i], i);
            if (_heroSlots[i].HasHero())
                availableHero++;
            else
            {
                _ultimateButtons[ultimateIndex].gameObject.SetActive(false);
                _ultimateButtons[ultimateIndex + 1].gameObject.SetActive(false);
            }

            _heroSlots[i].UpdateUltimateBtns(_ultimateButtons[ultimateIndex], _ultimateButtons[ultimateIndex + 1]);

            ultimateIndex += 2;
        }

        _changeWeapon.Init();
        _changeWeapon.SetOnUpgradeCallback(OnUpgradeWeapon);
        _AngleUpgradeButton.InitController();
        InGameCanvas.instance._gamePannelView.LockRemoveHero(availableHero < 4);
        ShowHeroSlotHighlight(SaveGameHelper.HasAvailableHero());

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_GP_ZOM_HPBAR,
            new Action<float, int>(DecreaseZomHpBar));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));

        InitAddOnPannel();
        InitSoldierButtons();
    }

    public void LockRemoveHero(bool isLock)
    {
        foreach (var heroSlot in _heroSlots)
        {
            heroSlot.LockRemoveButton(isLock);
        }
    }

    public void ShowSelectHeroPanel(Action<HeroData> callback, Vector3 position)
    {
        _arrow.transform.position = position;
        _changeTeam2.Show(callback);

        CheckOutSide();
    }

    private void UpdateUltimates()
    {
        int ultimateIndex = 0;

        for (int i = 0; i < _heroSlots.Length; i++)
        {
            _heroSlots[i].UpdateUltimateBtns(_ultimateButtons[ultimateIndex], _ultimateButtons[ultimateIndex + 1]);

            if (!_heroSlots[i].HasHero())
            {
                _ultimateButtons[ultimateIndex].gameObject.SetActive(false);
                _ultimateButtons[ultimateIndex + 1].gameObject.SetActive(false);
            }

            ultimateIndex += 2;
        }
    }

    public HeroSlot GetNextAvailableHeroSlot()
    {
        foreach (var heroSlot in _heroSlots)
        {
            if (!heroSlot.HasHero())
            {
                return heroSlot;
            }
        }

        return null;
    }

    private void CheckOutSide()
    {
        // Arrow
        Vector3[] objectCornersArrow = new Vector3[4];
        var arrowRect = Extensions.rectTransform(_arrow);
        arrowRect.GetWorldCorners(objectCornersArrow);

        if (objectCornersArrow[0].x < 0)
        {
            var position1 = arrowRect.position;
            var delta = arrowRect.rect.width / 2f - position1.x;

            position1 = new Vector3(position1.x + Utils.ConvertToMatchWidthRatio(delta),
                position1.y);

            arrowRect.position = position1;
        }

        if (objectCornersArrow[3].x > Screen.width)
        {
            var position1 = arrowRect.position;
            var delta = arrowRect.rect.width / 2f - (Screen.width - position1.x);

            position1 = new Vector3(position1.x - Utils.ConvertToMatchWidthRatio(delta),
                position1.y);

            arrowRect.position = position1;
        }


        _heroButtonHolder.anchoredPosition = new Vector3(0, _heroButtonHolder.anchoredPosition.y);
        Vector3[] objectCorners = new Vector3[4];
        _heroButtonHolder.GetWorldCorners(objectCorners);

        float width = _heroButtonHolder.GetWidth();
        var halfWidth = width / 2f;

        var posTemp = _heroButtonHolder.anchoredPosition;
        if (objectCorners[0].x < 0)
        {
            posTemp = new Vector3(posTemp.x + halfWidth - width / 8f, posTemp.y);
        }
        else if (objectCorners[3].x > Screen.width)
        {
            posTemp = new Vector3(posTemp.x - halfWidth + width / 8f, posTemp.y);
        }
        //
        // if (posTemp.x + contentRect.width / 2f <
        //     arrowRect.position.x + arrowRect.rect.width / 2f)
        // {
        //     posTemp.x += (arrowRect.position.x + Utils.ConvertToMatchWidthRatio(arrowRect.rect.width / 2f)) -
        //                  (posTemp.x + Utils.ConvertToMatchWidthRatio(contentRect.width / 2f));
        // }
        // else if (posTemp.x - contentRect.width / 2f >
        //          arrowRect.position.x - arrowRect.rect.width / 2f)
        // {
        //     posTemp.x -=
        //         Utils.ConvertToMatchWidthRatio(
        //             60f); // posTemp.x - Utils.ConvertToMatchWidthRatio(contentRect.width / 2f) -
        //     // (arrowRect.position.x - Utils.ConvertToMatchWidthRatio(arrowRect.rect.width / 2f));
        // }

        _heroButtonHolder.anchoredPosition = posTemp;
    }

    public void OnUpdateFormation()
    {
        ShowHeroSlotHighlight(SaveGameHelper.HasAvailableHero());
        UpdateUltimates();
        _changeWeapon.OnUpdateFormation();
    }

    public void OnUpgradeWeapon(HeroData heroData)
    {
        HeroSlot slot = _heroSlots.ToList().Find(x => x.HeroId == heroData.UniqueID);
        slot.UpdateHeroInfo();
    }

    public void ResetInfo()
    {
        lastGoldValue = _mode == GameMode.CAMPAIGN_MODE
            ? CurrencyModels.instance.Golds
            : CurrencyModels.instance.Tokens;
        starTxt.text = FBUtils.CurrencyConvert(lastGoldValue);

        lastWeaponScrollValue = CurrencyModels.instance.WeaponScrolls;
        ScrollWeaponTxt.text = FBUtils.CurrencyConvert(lastWeaponScrollValue);
        ShowHeroSlotHighlight(SaveGameHelper.HasAvailableHero());
    }

    public void PlayAnimGainCurrency()
    {
        long currentGold = lastGoldValue;
        long targetValue = _mode == GameMode.CAMPAIGN_MODE
            ? CurrencyModels.instance.Golds
            : CurrencyModels.instance.Tokens;

        DOTween.To(() => currentGold, (x) =>
        {
            currentGold = x;
            starTxt.text = FBUtils.CurrencyConvert(currentGold);
        }, targetValue, 0.5f).SetEase(Ease.Linear).OnComplete(() => { lastGoldValue = currentGold; });


        long targetScrollWeapon = CurrencyModels.instance.WeaponScrolls;
        long currentScrollWeapon = lastWeaponScrollValue;
        DOTween.To(() => currentScrollWeapon, (x) =>
            {
                currentScrollWeapon = x;
                ScrollWeaponTxt.text = FBUtils.CurrencyConvert(currentScrollWeapon);
            }, targetScrollWeapon, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => { lastWeaponScrollValue = currentScrollWeapon; });
    }

    public void SetBadState()
    {
        this.goodProgressVisualisationObj.SetActive(false);
        this.badProgressVisualisationObj.SetActive(true);
        this.currentProgressBarImage.sprite = this.badProgressBarStateSprite;
        base.Invoke("HideStatusIndies", 0.8f);
    }

    public void SetGoodState()
    {
        this.goodProgressVisualisationObj.SetActive(true);
        this.badProgressVisualisationObj.SetActive(false);
        this.currentProgressBarImage.sprite = this.goodProgressBarStateSprite;
        base.Invoke("HideStatusIndies", 0.8f);
    }

    public void HideStatusIndies()
    {
        this.currentProgressBarImage.sprite = this.goodProgressBarStateSprite;
        this.goodProgressVisualisationObj.SetActive(false);
        this.badProgressVisualisationObj.SetActive(false);
    }

    public void StartAttack()
    {
        // this.attackStatusPanelView.Show();
    }

    public void StopAttack()
    {
        // this.attackStatusPanelView.Hide();
    }

    public void ShowLevelUpButton()
    {
        this.levelUpBtn.gameObject.SetActive(true);
    }

    public void HideLevelUpButton()
    {
        this.levelUpBtn.gameObject.SetActive(false);
    }

    public void SetStarTxt(long starValue)
    {
        this.starTxt.text = FBUtils.CurrencyConvert(starValue);
    }

    public void SetScrollWeaponTxt(long scrollWeapon)
    {
        this.ScrollWeaponTxt.text = FBUtils.CurrencyConvert(scrollWeapon);
    }

    public void SetLevelTxt(int levelValue)
    {
        this.levelTxt.text = $"{LOCALIZE_ID_PREF.LEVEL.AsLocalizeString()} {levelValue}";
    }

    public void SetNextLevelTxt(int nexlevelValue)
    {
    }

    public void SetLevelProgress(float progressRatio)
    {
        this.levelProgressBarView.SetNormalizedValue(progressRatio);
    }

    public void TickLate()
    {
        this.onOpenSettingBtnPressed = false;
        this.onOpenChoosePanelBtnPressed = false;
    }

    private void OnDestroy()
    {
        showBannerDelay?.Kill();
        AdsManager.instance.HideAdsBanner(null);

        this.levelUpBtn.onClick.RemoveAllListeners();
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.RESET_GAME_HUD, new Action(ResetInfo));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(ResetInfo));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.UPDATE_CASTLE_HP,
            new Action<float, float>(SetCastleHPValue));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.UPDATE_IDLE_HUD,
            new Action<float, float>(SetCastleHPValue));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ON_REMOVE_ADS, new Action(OnRemoveAds));
    }

    public void Load()
    {
        base.gameObject.SetActive(true);
    }

    public void Unload()
    {
        base.gameObject.SetActive(false);
    }

    [SerializeField] private Button levelUpBtn;

    [SerializeField] private Animator levelUpAnimator;


    [Header("Currency")] [SerializeField] private LocalizedTMPTextUI starTxt;
    [SerializeField] private LocalizedTMPTextUI ScrollWeaponTxt;

    // [SerializeField] private AttackStatusPanelView attackStatusPanelView;

    [SerializeField] private BaseFloatValueView levelProgressBarView;

    [SerializeField] private Sprite goodProgressBarStateSprite;

    [SerializeField] private Sprite badProgressBarStateSprite;

    [SerializeField] private GameObject goodProgressVisualisationObj;

    [SerializeField] private GameObject badProgressVisualisationObj;

    [SerializeField] private GameObject attackIndicator;

    public Image currentProgressBarImage;

    public TextMeshProUGUI txtProgressValue;

    public bool onOpenSettingBtnPressed;

    public bool onOpenChoosePanelBtnPressed;

    public void OnButtonHome()
    {
        GameMaster.instance.BackToMenuScene();
    }

    public void SetCastleHPValue(float percent, float currentHP)
    {
        currentProgressBarImage.fillAmount = percent;
        txtProgressValue.text = Mathf.RoundToInt(currentHP).ToString();
    }

    public void SetProgressWave(int currentWave, int maxWave)
    {
        var percent = currentWave * 1.0f / maxWave * 1.0f;
        if (currentProgressBarImage != null)
            currentProgressBarImage.fillAmount = percent;

        if (txtProgressValue != null)
            txtProgressValue.text = $"{currentWave}/{maxWave}";
    }

    private void Update()
    {
        float _deltaTime = Time.deltaTime;
        UpdateAddOnButton(_deltaTime);
        UpdateLevelWaveInfo(_deltaTime);
        UpdateSoliderButtons(_deltaTime);
    }

    #region ADD ON PANNEL

    private List<AddOnButtonView> _listAddOnButton = new List<AddOnButtonView>();
    public List<AddOnButtonView> ListAddOnButton => _listAddOnButton;

    public void InitAddOnPannel()
    {
        if (_listAddOnButton == null || _listAddOnButton.Count <= 0)
        {
            foreach (var btn in _listAddOnButton)
            {
                Pooly.Despawn(btn.transform);
            }
        }

        _listAddOnButton.Clear();
        var listAddOns = DesignHelper.GetSkillDesignByType(SkillType.ADD_ON);
        if (listAddOns.Count > 0)
        {
            foreach (var ds in listAddOns)
            {
                var btn = Pooly.Spawn<AddOnButtonView>(POOLY_PREF.ADD_ON_BTN_VIEW, Vector3.zero, Quaternion.identity,
                    _rectAddOnContent);
                btn.transform.localScale = Vector3.one;
                btn.Initialize(ds);
                Extensions.rectTransform(btn).sizeDelta = new Vector2(202f, 224f);
                _listAddOnButton.Add(btn);
            }
        }

        var listSpecial = DesignHelper.GetSkillDesignByType(SkillType.SPECIAL_ADD_ON);
        if (listSpecial.Count > 0)
        {
            foreach (var ds in listSpecial)
            {
                AddOnButtonView btn = null;
                if (ds.SkillId == GameConstant.ADD_ON_AIR_DROP)
                {
                    btn = Pooly.Spawn<AddOnAirDropButtonView>(POOLY_PREF.ADD_ON_AIR_DROP_BTN_VIEW, Vector3.zero, Quaternion.identity,
                        _rectSpecialItems);
                }
                else
                {
                    btn = Pooly.Spawn<AddOnButtonView>(POOLY_PREF.ADD_ON_BTN_VIEW, Vector3.zero, Quaternion.identity,
                        _rectSpecialItems);
                }

                btn.transform.localScale = Vector3.one;
                btn.Initialize(ds);
                _listAddOnButton.Add(btn);

            }
        }

        _addOnReminderUI.Load(ReminderManager.HasNewAddOn().Item2.Count);
    }

    public void UpdateAddOnButton(float _deltaTime)
    {
        for (int i = _listAddOnButton.Count - 1; i >= 0; i--)
        {
            _listAddOnButton[i].UpdateButton(_deltaTime);
        }
    }

    public void ResetAddOnPannel()
    {
        foreach (var item in _listAddOnButton)
        {
            item.ResetItem();
        }
    }

    #endregion

    public void OnButtonAirDrop()
    {
        GamePlayController.instance.SpawnAirDrop();
    }

    public void OnButtonAutoManualHero()
    {
    }

    public void OnButtonSpeedUp()
    {
    }

    public void CleanUp()
    {
        for (int i = _listAddOnButton.Count - 1; i >= 0; i--)
        {
            _listAddOnButton[i].CleanUp();
            Pooly.Despawn(_listAddOnButton[i].transform);
        }

        _listAddOnButton.Clear();

        RemoveListeners();
    }

    public void RemoveListeners()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.UPDATE_GP_ZOM_HPBAR,
            new Action<float, int>(DecreaseZomHpBar));
    }

    public void ShowHeroSlotHighlight(bool isShow)
    {
        if(_heroSlots == null)
            return;
        
        foreach (var heroSlot in _heroSlots)
        {
            heroSlot.ShowHighlight(isShow);
        }
    }

    public void OnLevelUp()
    {
        foreach (var btn in _listAddOnButton)
        {
            btn.ResetView();
        }
    }

    #region Zom Hp bar

    public void InitZomHpBar()
    {
        _zomHPBar.interactable = false;
        _zomHPBar.maxValue = 1;
        _zomHPBar.value = 1;
    }

    public void ResetZomHPBar(float totalLevelHp)
    {
        _zomHPBar.interactable = false;
        _zomHPBar.maxValue = totalLevelHp;
        _zomHPBar.value = totalLevelHp;
    }

    public void CleanZomHpBar()
    {
        _zomHPBar.value = 0;
    }

    public void DecreaseZomHpBar(float dmg, int wave)
    {
        //float totalDmg = GamePlayController.instance._waveController.zombieDispenser.GetWaveTotalHp(wave);
        //float percent = dmg * 1.0f / totalDmg * 1.0f;
        //_zomHPBar.value -= percent;

        _zomHPBar.value -= dmg;
        // _zomHpBarPulseEffect.Pulse();
    }

    #endregion

    #region Wave Level View

    private bool countDownWave = false;
    private float timerWave = 0f;

    public void InitLevelWaveInfo(int level, int maxWave)
    {
        SetCampaignModeLevelWaveInfo(level, 1, maxWave);
    }

    public void ResetLevelWaveInfo(int level, int currentWave, int maxWave, float durationNextWave)
    {
        int nextWave = currentWave + 1;
        bool nextIsMax = nextWave >= maxWave;
        bool currentIsMax = currentWave >= maxWave;


        if (GameMaster.instance.currentMode == GameMode.CAMPAIGN_MODE)
        {
            SetCampaignModeLevelWaveInfo(level, currentWave, maxWave);
            timerWave = durationNextWave;
            this.txtTimerWave.gameObject.SetActiveIfNot(!currentIsMax);
            this.txtTimerWave.text = $"{TimeService.FormatTimeSpanShortly(timerWave)}";
            countDownWave = true;
        }
        else if (GameMaster.instance.currentMode == GameMode.IDLE_MODE)
        {
            SetIdleModeIdleLevelWaveInfo(level, currentWave, maxWave);
            this.txtTimerWave.gameObject.SetActiveIfNot(false);
            countDownWave = false;
        }

        //string waveStr = LOCALIZE_ID_PREF.WAVE.AsLocalizeString() + $" {currentWave}/{maxWave}";
        //if (currentWave == 1)
        //{
        //    Timing.CallDelayed(0.5f, () =>
        //    {
        //        InGameCanvas.instance.ShowFloatingTextNotify(waveStr, 50, 200, 2f);
        //    });
        //}
        //else
        //    InGameCanvas.instance.ShowFloatingTextNotify(waveStr, 50, 200, 2f);
    }

    public void SetCampaignModeLevelWaveInfo(int level, int currentWave, int maxWave)
    {
        bool nextIsMax = currentWave + 1 >= maxWave;
        bool currentIsMax = currentWave >= maxWave;
        this.levelTxt.text = $"{LOCALIZE_ID_PREF.LEVEL.AsLocalizeString()} {level}";
        this.txtNextWave.text = nextIsMax ? $"{maxWave}" : $"{currentWave + 1}";
        this.txtMaxWave.text = $"{maxWave}";
        this.txtTimerWave.text = $"";

        this.txtInTimer.gameObject.SetActiveIfNot(!currentIsMax);
    }

    public void SetIdleModeIdleLevelWaveInfo(int level, int currentWave, int maxWave)
    {
        this.levelTxt.text = $"{LOCALIZE_ID_PREF.LEVEL.AsLocalizeString()} {level}";
        this.txtNextWave.text = $"{currentWave}";
        this.txtMaxWave.text = $"{maxWave}";
        this.txtTimerWave.text = $"";
        this.txtInTimer.gameObject.SetActiveIfNot(false);
    }


    private float timerUpdateText = 0f;
    public void UpdateLevelWaveInfo(float _deltaTime)
    {
        if (countDownWave)
        {
            timerUpdateText += _deltaTime;
            timerWave -= _deltaTime;
            if (timerUpdateText >= 1.0f)
            {
                this.txtTimerWave.text = $"{TimeService.FormatTimeSpanShortly(timerWave)}";
                timerUpdateText = 0f;
            }

            if (timerWave <= 0)
            {
                this.txtTimerWave.text = $"";
                countDownWave = false;
            }
        }
    }

    #endregion

    #region Soldier Upgrade Button

    private SoliderUpgradeButton[] listUpgradeButtons;

    public void InitSoldierButtons()
    {
        listUpgradeButtons = FindObjectsOfType<SoliderUpgradeButton>();
    }

    public void ResetSoldiserButtons()
    {
        listUpgradeButtons = FindObjectsOfType<SoliderUpgradeButton>();
    }

    public void UpdateSoliderButtons(float _deltaTime)
    {
        if (listUpgradeButtons == null || listUpgradeButtons.Length <= 0)
            return;

        for (int i = 0; i < listUpgradeButtons.Length; i++)
        {
            listUpgradeButtons[i].UpdateController(_deltaTime);
        }
    }

    #endregion
}