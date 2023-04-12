using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Ads;
using Framework.Managers.Statistics;
using UnityEngine;

public class LevelUpPanelController : MonoBehaviour, IState, IDisposable, ITick, ITickLate
{
    public void Load()
    {
        this.LevelUpPanelModel.SetCurrentLevelAndStarsValues(LevelModel.instance.CurrentLevel, CurrencyModels.instance.Golds);
        this.levelUpStarReward = this.LevelUpPanelModel.GetRewardValue();
        this.LevelApPanelView.Load();
        this.SetLevelUpRewardTextField();
        this.rewardedVideoButtonController.Load(this.LevelApPanelView.GetRewardedVideoButtonView());
        this.updateManager.AddTo(this);
    }

    public void Unload()
    {
        this.rewardedVideoButtonController.Unload();
        LevelModel.instance.LevelUp();
        this.LevelApPanelView.Unload();
        this.updateManager.RemoveFrom(this);
        this.statisticSystem.SendEvent("Level Reached: " + LevelModel.instance.CurrentLevel.ToString());
    }

    private void SetLevelUpRewardTextField()
    {
        this.LevelApPanelView.setBonusLikesTxt(this.levelUpStarReward);
    }

    public void Tick()
    {
        this.rewardedVideoButtonController.Tick();
        if (this.rewardedVideoButtonController.OnRewardedVideoFinishedFailled)
        {
            this.Unload();
        }
        if (this.rewardedVideoButtonController.OnRewardedVideoFinishedSucces)
        {
            CurrencyModels.instance.Golds += this.LevelUpPanelModel.GetExtraRewardValue();
            this.statisticSystem.SendEvent("LevelUP: REWARD FinishedSucces");
            this.Unload();
        }
        if (this.LevelApPanelView.OnCollectBtnClicked)
        {
            CurrencyModels.instance.Golds += this.levelUpStarReward;
            this.vibrationSystem.Vibrate();
            if (LevelModel.instance.CurrentLevel >= 5)
            {
                this.baseAdSystem.ShowInterstetial(delegate (bool obj)
                {
                    this.statisticSystem.SendEvent("LevelUP: Interstetial Showed");
                });
            }
            this.Unload();
        }
        if (this.LevelApPanelView.OnDoubleCollectBtnClicked)
        {
            this.vibrationSystem.Vibrate();
        }
    }

    public void TickLate()
    {
        this.rewardedVideoButtonController.TickLate();
        this.LevelApPanelView.TickLate();
    }

    public void Dispose()
    {
        this.LevelApPanelView = null;

        this.baseAdSystem = null;
        this.rewardedVideoButtonController = null;
        this.LevelUpPanelModel = null;
        this.updateManager = null;
        this.vibrationSystem = null;
        this.statisticSystem = null;
    }

    public LevelUpPanelView LevelApPanelView;

    [Inject]
    private LevelUpPanelModel LevelUpPanelModel;

    [Inject]
    private RewardedVideoButtonController rewardedVideoButtonController;

    [Inject]
    private UpdateManager updateManager;

    [Inject]
    private BaseAdSystem baseAdSystem;

    [Inject]
    private IVibrationSystem vibrationSystem;

    [Inject]
    private StatisticSystemController statisticSystem;

    private long levelUpStarReward;
}
