using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;
using UnityEngine;

public class GamePanelController : MonoBehaviour, IDisposable, ITick, ITickLate
{
    private void Start()
    {
        //Load();
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_READY_UP, new Action(OnLevelReadyUp));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.EXP_GROW, new Action(OnExpGrow));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.EXP_FALL, new Action(OnExpFall));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ZOMBIE_IN_ATTACK_ZONE, new Action(OnZombieInAttackZone));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ZOMBIE_LEAVE_ATTACK_ZONE, new Action(OnZombieLeaveAttackZone));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.UPDATE_CURRENCY, new Action(UpdateBattleCurrency));
    }

    public void Load()
    {
        this.gameStateView.Load();
        this.updateManager.AddTo(this);

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_READY_UP, new Action(OnLevelReadyUp));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.EXP_GROW, new Action(OnExpGrow));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.EXP_FALL, new Action(OnExpFall));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ZOMBIE_IN_ATTACK_ZONE, new Action(OnZombieInAttackZone));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ZOMBIE_LEAVE_ATTACK_ZONE, new Action(OnZombieLeaveAttackZone));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_CURRENCY, new Action(UpdateBattleCurrency));


        this.UpdateLevelTxt();
        this.UpdateLevelProgressBar();
        this.UpdateBattleCurrency();
    }

    private void OnExpFall()
    {
        this.gameStateView.SetBadState();
    }

    private void OnExpGrow()
    {
        this.gameStateView.SetGoodState();
    }

    private void OnLevelUp()
    {
        this.UpdateLevelProgressBar();
        this.UpdateLevelTxt();
    }

    private void OnLevelReadyUp()
    {
        this.levelApPanelController.Load();
    }

    private void OnZombieInAttackZone()
    {
        this.gameStateView.StartAttack();
    }

    private void OnZombieLeaveAttackZone()
    {
        this.gameStateView.StopAttack();
    }

    public void UnLoad()
    {
        this.updateManager.RemoveFrom(this);
        this.gameStateView.Unload();

    }

    private void UpdateBattleCurrency()
    {
        this.SetStartValue(GameMaster.instance.currentMode == GameMode.CAMPAIGN_MODE ? CurrencyModels.instance.Golds : CurrencyModels.instance.Tokens);
        this.SetScrollWeaponValue(CurrencyModels.instance.WeaponScrolls);
    }

    private void UpdateLevelTxt()
    {
        LevelProgressData progressData = LevelModel.instance.GetProgressData();
        this.gameStateView.SetLevelTxt(progressData.level);
        this.gameStateView.SetNextLevelTxt(progressData.level + 1);
    }

    private void UpdateLevelProgressBar()
    {
        this.gameStateView.SetLevelProgress(LevelModel.instance.getLevelProgressRatio());
    }

    public void SetStartValue(long star)
    {
        this.gameStateView.SetStarTxt(star);
    }

    public void SetScrollWeaponValue(long scrollWeapon)
    {
        this.gameStateView.SetScrollWeaponTxt(scrollWeapon);
    }

    public void Tick()
    {
        this.UpdateLevelProgressBar();
        this.UpdateBattleCurrency();
        if (this.gameStateView.onOpenChoosePanelBtnPressed)
        {
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.CHOOSE_LOCATION_CLICKED);
            this.locationItemPanelView.Load();
        }
    }

    public void TickLate()
    {
        LevelModel.instance.TickLate();
        CurrencyModels.instance.TickLate();
        this.gameStateView.TickLate();
    }

    public void Dispose()
    {
        this.locationItemPanelView = null;

        this.gameStateView = null;
        this.updateManager = null;
        this.levelApPanelController = null;
        this.vibrationSystem = null;
    }

    public GamePanelView gameStateView;

    public LocationItemPanelView locationItemPanelView;

    public LevelUpPanelController levelApPanelController;


    [Inject]
    private UpdateManager updateManager;

    [Inject]
    private IVibrationSystem vibrationSystem;
}
