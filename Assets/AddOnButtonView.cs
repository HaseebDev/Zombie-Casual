using QuickType.SkillDesign;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using com.datld.data;
using UnityExtensions.Localization;
using Coffee.UIEffects;
using MEC;
using QuickType;
using TMPro;
using static AnalyticsConstant;

public class AddOnButtonView : MonoBehaviour
{
    public Image imgFilled;
    public Image imgIcon;
    public string AddOnID { get; private set; }
    [SerializeField] private ReminderUI _reminderUi;

    public Image grayScaleIcon;

    [Header("Timer")] public RectTransform _rectTimer;
    public Toggle _toggleInUsed;
    public TextMeshProUGUI _txtTimer;

    [Header("Ready use")] public RectTransform _rectReadyUse;
    public Text _txtReadyUse;

    [Header("Num Items")] public LocalizedTMPTextUI txtNumItem;
    public RectTransform _rectNumItem;

    [Header("UIEffects")] public List<UIEffect> listUIEffects;
    public UIShiny _iconShiny;
    public GameObject _unlimitedIcon;

    private SkillDesignElement _Design;
    public SkillDesignElement Design => _Design;
    private BaseCharacterUltimate _SkillInstance;

    private float timerCountDown = 0f;
    private float CountDownDuration = 0f;
    private bool startCountDown = false;
    public bool IsInUsed { get; private set; }
    public bool IsWaitingForUsed { get; private set; }
    public AddOnItem itemData { get; private set; }

    private float timerInUsed = 0f;
    private float timerReadyUsed = 0f;
    private float timerTick = 0f;
    private bool IsCountDown = false;

    private float TimeCastSkill = 0f;

    private int currentCampaignLevel = 0;

    private bool IsAvailable {
        get { return !IsLocked && itemData != null ? itemData.Status == ITEM_STATUS.Available : false; }
    }

    private bool IsLocked;

    private void Awake()
    {
        imgFilled.fillAmount = 0;
        SetIsWaitingForUse(false);
    }

    private void OnEnable()
    {
        ResetItem();
    }

    public virtual void Initialize(SkillDesignElement designData)
    {
        imgFilled.fillAmount = 0;
        imgFilled.gameObject.SetActiveIfNot(false);
        _rectTimer.gameObject.SetActiveIfNot(false);

        _Design = designData;
        itemData = SaveManager.Instance.Data.GetAddOnItem(_Design.SkillId);


        if (!itemData.IsSpecialAddOn())
        {
            EventSystemServiceStatic.AddListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN,
                new Action<string, float, float>(SetCountDown));
        }

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_TIMER,
            new Action<string, bool, long, long>(SetEnableTimer));

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_READY_USE,
            new Action<string, bool, float, long>(SetEnableReadyUse));

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_REACHED_LIMIT,
            new Action<string>(SetReachedLimit));

        AddOnID = _Design.SkillId;
        ResourceManager.instance.GetUltimateSprite(_Design.SkillId, s =>
        {
            imgIcon.sprite = s;
            grayScaleIcon.sprite = s;
        });

        Timing.RunCoroutine(GamePlayController.instance.SpawnAddOnSkill(_Design.SkillId,
            (skillCallback) => { _SkillInstance = skillCallback; }));
        currentCampaignLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel;
        ResetItem();
        int curLevel = currentCampaignLevel;
        IsLocked = curLevel < _Design.UnlockLevel;
        SetGrayScale(IsLocked);
        gameObject.SetActiveIfNot(curLevel >= _Design.AppearLevel);

        _rectNumItem.gameObject.SetActiveIfNot(!itemData.IsSpecialAddOn());
        //DisableReadyUseTimer();

        _iconShiny.Stop();

        _unlimitedIcon.gameObject.SetActiveIfNot(itemData != null && itemData.IsUnlimitedItem);
    }

    public virtual void SetIsWaitingForUse(bool isWaiting)
    {
        IsWaitingForUsed = isWaiting;
    }

    private void SetGrayScaleNotEnough(bool grayScale)
    {
        if (itemData.IsSpecialAddOn())
        {
            foreach (var fx in listUIEffects)
            {
                fx.effectMode = EffectMode.None;
            }

            imgIcon.gameObject.SetActive(true);
            grayScaleIcon.gameObject.SetActive(false);
            return;
        }

        foreach (var fx in listUIEffects)
        {
            fx.effectMode = grayScale ? EffectMode.Grayscale : EffectMode.None;
        }

        imgIcon.gameObject.SetActive(!grayScale);
        grayScaleIcon.gameObject.SetActive(grayScale);
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    private void SetCountDown(string skillID, float timeCastSkill, float countDownDuration)
    {
        if (IsWaitingForUsed)
            return;

        if (skillID == AddOnID && !IsWaitingForUsed && !this.itemData.IsSpecialAddOn())
        {
            CountDownDuration = countDownDuration;
            if (CountDownDuration <= 0)
            {
                SetGrayScale(false);
                imgFilled.fillAmount = 0;
                startCountDown = false;
                IsCountDown = false;
                _iconShiny.Play();
            }
            else
            {
                TimeCastSkill = timeCastSkill;
                startCountDown = true;
                CountDownDuration = countDownDuration;
                timerCountDown = 0f;
                IsCountDown = true;
            }
        }
    }

    private void SetEnableTimer(string skillID, bool enable, long duration, long maxTime)
    {
        if (skillID == AddOnID)
        {
            if (enable)
            {
                SetEnableTimer(duration, maxTime);
            }
            else
            {
                DisableInUsedTimer(duration);
            }
        }
    }

    private void SetEnableReadyUse(string skillID, bool enable, float duration, long totalTime)
    {
        if (skillID == AddOnID)
        {
            _rectReadyUse.gameObject.SetActiveIfNot(true);
            if (enable)
            {
                SetEnableReadyUse(duration, totalTime);
            }
            else
            {
                DisableReadyUseTimer();
            }
        }
    }

    public virtual void UpdateButton(float _deltaTime)
    {
        if (!IsWaitingForUsed && startCountDown)
        {
            if (CountDownDuration == 0)
            {
                SetGrayScale(false);
                imgFilled.fillAmount = 0;
                startCountDown = false;
                IsCountDown = false;
                _iconShiny.Play();
                return;
            }

            SetGrayScale(true);
            timerCountDown = Time.time - TimeCastSkill;
            var percent = 1.0f - (timerCountDown * 1.0f / CountDownDuration * 1.0f);
            imgFilled.gameObject.SetActiveIfNot(true);
            imgFilled.fillAmount = percent;

            if (timerCountDown >= CountDownDuration)
            {
                SetGrayScale(false);
                imgFilled.fillAmount = 0;
                startCountDown = false;
                IsCountDown = false;
                _iconShiny.Play();
            }
        }

        UpdateTimer(Time.unscaledDeltaTime);
        UpdateReadyUse(Time.unscaledDeltaTime);
    }

    public void OnButtonActive()
    {
    }

    private bool busyHUDInfo = false;

    public void OnPointerDown()
    {
        ScaleUp();
        busyHUDInfo = false;
        _unlimitedIcon.gameObject.SetActiveIfNot(itemData != null && itemData.IsUnlimitedItem);
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_BUTTON);

        if (IsWaitingForUsed)
        {
            InGameCanvas.instance.ShowFloatingTextNotify(
                $"Remaining time: {TimeService.FormatTimeSpanShortly(timerReadyUsed)}");
            return;
        }
        else if (IsCountDown)
        {
            return;
        }
        else if (IsLocked)
        {
            InGameCanvas.instance.ShowFloatingTextNotify($"Unlocked at level {_Design.UnlockLevel}");
        }
        else if (!IsInUsed && itemData.Status != ITEM_STATUS.Locked &&
                 (itemData.ItemID == GameConstant.ADD_ON_SPEED_UP ||
                  itemData.ItemID == GameConstant.ADD_ON_AUTO_MANUAL_HERO))
        {
            if (!itemData.IsUnlimitedItem)
                ShowHUDInfo();
            return;
        }
        else if (itemData.ItemID == GameConstant.ADD_ON_AIR_DROP && itemData.ExpiredDuration <= 0 &&
                 itemData.Status == ITEM_STATUS.Available)
        {
            SetIsWaitingForUse(false);
            ShowHUDInfo();
            return;
        }

        _SkillInstance?.PointerDownSkill(Input.mousePosition);
    }

    public virtual void OnPointerUp()
    {
        ScaleDown();

        if (IsCountDown || busyHUDInfo || IsWaitingForUsed)
            return;
        ShowReminder(false);
        bool castSucess = false;

        if (_SkillInstance != null)
            castSucess = _SkillInstance.PointerUpSkill(Input.mousePosition);

        if (castSucess)
        {
            if(_Design.SkillType == SkillType.ADD_ON)
                MissionManager.Instance.TriggerMission(MissionType.USE_ADDON, _Design.SkillId);
            SaveManager.Instance.Data.ConsumeAddOnItem(_Design.SkillId);
            ResetItem();
        }


        Timing.CallDelayed(0.2f, () => { GamePlayController.instance.touchDetector.SetEnableTouch(true); });

        //analytics here!!!!
        if (currentCampaignLevel == 3)
        {
            if (this.AddOnID.Equals(GameConstant.ADD_ON_MEDIC))
            {
                AnalyticsManager.instance.LogEvent(
                    getEventNameByLevel(ANALYTICS_ENUM.TUTORIAL_TOUCH_MEDIC, currentCampaignLevel),
                    new LogEventParam("level", currentCampaignLevel));
            }
            else if (this.AddOnID.EndsWith(GameConstant.ADD_ON_RADIO_CALL))
            {
                AnalyticsManager.instance.LogEvent(
                    getEventNameByLevel(ANALYTICS_ENUM.TUTORIAL_TOUCH_RADIO_CALL, currentCampaignLevel),
                    new LogEventParam("level", currentCampaignLevel));
            }
        }
    }

    public void ResetItem()
    {
        if (itemData != null)
        {
            txtNumItem.text = itemData.ItemCount.ToString();
            SetGrayScaleNotEnough(itemData.ItemCount == 0);
        }
        else
        {
            txtNumItem.text = "0";
        }
    }

    public void CleanUp()
    {
        RemoveAllListeners();
    }

    private void RemoveAllListeners()
    {
        if (!itemData.IsSpecialAddOn())
            EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN,
                new Action<string, float, float>(SetCountDown));

        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_TIMER,
            new Action<string, bool, long, long>(SetEnableTimer));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_READY_USE,
            new Action<string, bool, float, long>(SetEnableReadyUse));

        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_REACHED_LIMIT,
            new Action<string>(SetReachedLimit));
    }

    public void SetGrayScale(bool grayScale)
    {
        //foreach (var fx in listUIEffects)
        //{
        //    fx.effectMode = grayScale ? EffectMode.Grayscale : EffectMode.None;
        //}

        //IsLocked = grayScale;
    }

    public void ResetView()
    {
        int curLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel;
        IsLocked = curLevel < _Design.UnlockLevel;
        SetGrayScale(IsLocked);
        gameObject.SetActiveIfNot(curLevel >= _Design.AppearLevel);
    }

    #region Timer In Used

    private long MaxTimeUse;

    public void SetEnableTimer(long remainTime, long maxTime)
    {
        _toggleInUsed.isOn = true;
        _txtTimer.text = TimeService.FormatTimeSpanShortly(remainTime);
        timerInUsed = remainTime;
        IsInUsed = true;
        _rectTimer.gameObject.SetActiveIfNot(true);
        _txtTimer.gameObject.SetActiveIfNot(!this.itemData.IsUnlimitedItem);
        MaxTimeUse = maxTime;
        //var percentFilled = remainTime * 1.0f / maxTime * 1.0f;
        //imgFilled.fillAmount = percentFilled;
        imgFilled.fillAmount = 1.0f;
        imgFilled.gameObject.SetActiveIfNot(true);
    }

    public void DisableInUsedTimer(long remainTime)
    {
        _toggleInUsed.isOn = false;
        _txtTimer.text = TimeService.FormatTimeSpanShortly(remainTime);
        _txtTimer.gameObject.SetActiveIfNot(!this.itemData.IsUnlimitedItem);
        timerInUsed = 0;
        IsInUsed = false;
        _rectTimer.gameObject.SetActiveIfNot(false);
        //  imgFilled.fillAmount = 0;

        imgFilled.gameObject.SetActiveIfNot(false);
        imgFilled.fillAmount = 0f;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (GamePlayController.instance != null && GamePlayController.instance.IsPausedGame)
            return;

        if (IsInUsed && !this.itemData.IsUnlimitedItem)
        {
            timerInUsed -= deltaTime;

            //var percentFilled = timerInUsed * 1.0f / MaxTimeUse * 1.0f;
            //imgFilled.fillAmount = percentFilled;
            imgFilled.fillAmount = 1.0f;
            timerTick += deltaTime;
            if (timerTick >= 1.0f)
            {
                timerTick = 0f;
                _txtTimer.text = TimeService.FormatTimeSpanShortly(timerInUsed);
            }

            if (timerInUsed < 0)
            {
                timerInUsed = 0;
                DisableInUsedTimer(0);
                SaveManager.Instance.Data.ResetAddOnItem(Design.SkillId);
                IsInUsed = false;
            }
        }
    }

    #endregion

    #region Timer Ready Used

    private float TotalReadyUse = 0f;

    public virtual void SetEnableReadyUse(float remainTime, float totalTime)
    {
        _rectReadyUse.gameObject.SetActiveIfNot(false);
        _txtReadyUse.text = TimeService.FormatTimeSpan(remainTime);
        //timerReadyUsed = remainTime;

        timerReadyUsed = itemData.ReadyTS - TimeService.instance.GetCurrentTimeStamp(true);
        SetIsWaitingForUse(true);
        SetGrayScale(true);
        TotalReadyUse = totalTime;
        this.imgFilled.fillAmount = (timerReadyUsed * 1.0f / TotalReadyUse * 1.0f);
        IsCountDown = false;
    }

    public void DisableReadyUseTimer()
    {
        _rectReadyUse.gameObject.SetActiveIfNot(false);
        SetGrayScale(false);
        IsCountDown = false;
    }

    public void UpdateReadyUse(float deltaTime)
    {
        if (IsWaitingForUsed)
        {
            this.imgFilled.fillAmount = (timerReadyUsed * 1.0f / TotalReadyUse * 1.0f);
            imgFilled.gameObject.SetActiveIfNot(true);
            SetGrayScale(true);
            timerReadyUsed -= deltaTime;
            timerTick += deltaTime;
            if (timerTick >= 1.0f)
            {
                timerTick = 0f;
                _txtReadyUse.text = TimeService.FormatTimeSpan(timerReadyUsed);
            }

            if (timerReadyUsed <= 0)
            {
                timerReadyUsed = 0;
                DisableReadyUseTimer();
                SaveManager.Instance.Data.ResetAddOnItem(Design.SkillId);
                SetIsWaitingForUse(false);
                SetGrayScale(false);
                this.imgFilled.fillAmount = 0f;
            }
        }
    }

    #endregion

    #region HUD INFO

    public void ShowHUDInfo()
    {
        onEarnedAds = false;
        busyHUDInfo = true;
        var des = DesignHelper.GetSkillDesign(itemData.ItemID);
        AddOnInfoData data = new AddOnInfoData();
        data.AddOnID = this.itemData.ItemID;
        data.title = des.GetName();
        data.content = des.GetDescription();
        data.number = (int)itemData.ItemCount;
        InGameCanvas.instance.ShowHUD(EnumHUD.HUD_ADD_ON_INFO, false, null, data, new Action(OnEarnAds),
            new Action(OnUseItem),
            (data.AddOnID == GameConstant.ADD_ON_SPEED_UP || data.AddOnID == GameConstant.ADD_ON_AUTO_MANUAL_HERO));
    }


    private bool onEarnedAds = false;
    public void OnEarnAds()
    {
        if (this.AddOnID == GameConstant.ADD_ON_AIR_DROP)
        {
            AdsManager.instance.ShowAdsReward((complete, amount) =>
            {
                if (complete && !onEarnedAds)
                {
                    GamePlayController.instance.SpawnAirDrop();
                    SaveManager.Instance.Data.AddAddONItem(_Design.SkillId, 1);
                    if (_SkillInstance != null)
                        _SkillInstance.PointerUpSkill(Input.mousePosition, false);
                    SaveManager.Instance.Data.ConsumeAddOnItem(_Design.SkillId, 1, true);
                    SaveManager.Instance.Data.DayTrackingData.TodayEarnAddonAirDrop++;
                    SaveManager.Instance.SetDataDirty();
                    ResetItem();

                    onEarnedAds = true;
                    Timing.CallDelayed(1.0f, () =>
                    {
                        onEarnedAds = false;
                    });
                }
            });
        }
        else
        {
            AdsManager.instance.ShowAdsReward((complete, amount) =>
            {
                if (complete && !onEarnedAds)
                {
                    SaveManager.Instance.Data.AddAddONItem(_Design.SkillId, 1);
                    if (_SkillInstance != null)
                    {
                        _SkillInstance._addOnItem.Status = ITEM_STATUS.Locked;
                        _SkillInstance.PointerUpSkill(Input.mousePosition, false);
                    }

                    SaveManager.Instance.Data.ConsumeAddOnItem(_Design.SkillId, 1, true);
                    ResetItem();

                    onEarnedAds = true;
                    Timing.CallDelayed(1.0f, () =>
                    {
                        onEarnedAds = false;
                    });
                }
            });
        }
    }

    public void OnUseItem()
    {
        _unlimitedIcon.gameObject.SetActiveIfNot(itemData != null && itemData.IsUnlimitedItem);
        if (_SkillInstance != null)
            _SkillInstance.PointerUpSkill(Input.mousePosition, false);
        SaveManager.Instance.Data.ConsumeAddOnItem(_Design.SkillId, 1, true);
        ResetItem();
    }

    #endregion

    #region Reached limit

    private void SetReachedLimit(string addonID)
    {
        //if (this.itemData.ItemID == addonID)
        //{
        //    this.imgFilled.fillAmount = 0f;
        //    IsCountDown = false;
        //    itemData.Status = ITEM_STATUS.Available;
        //    itemData.ExpiredDuration = 0;
        //    SetIsWaitingForUse(false);
        //}
    }

    #endregion

    public void ShowReminder(bool has)
    {
        _reminderUi?.Show(has);
    }

    public void ScaleDown()
    {
        transform.localScale = Vector3.one;
    }

    public void ScaleUp()
    {
        transform.localScale = Vector3.one * 1.2f;
    }
}