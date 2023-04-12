using System;
using Adic;
using Framework;
using Framework.Common;
using Framework.Controllers.Loader;
using Framework.Interfaces;
using Framework.Managers.Ads;
using Framework.Managers.Event;
using Framework.Managers.Statistics;
using Framework.Views;
using UnityEngine;

public class ContextMain : BaseMainContext
{
    protected override void BindManagers()
    {
        //base.containers[0].Bind<UpdateManager>().ToGameObject().Bind<StatisticSystemController>().ToSingleton<StatisticSystemController>().Bind<IEventManager>().ToSingleton<EventManager>().Bind<BaseAdSystem>().ToSingleton<AppodealAdSystem>().Bind<ISceneLoader>().ToGameObject<DummyLoader>().Bind<LevelLoader>().ToGameObject().Bind<IVibrationSystem>().ToSingleton<TapticVibrationSystem>().Bind<IAudioSystem>().To<AudioSystem>(this.audioSystem);
    }

    protected override void BindControllers()
    {
        //base.containers[0].Bind<MainController>().ToSelf().Bind<RewardedVideoButtonController>().ToSelf().Bind<SoundSwitcherController>().ToSingleton().Bind<VibroSwitcherController>().ToSingleton().Bind<GamePanelController>().ToSelf().Bind<CharacterController>().ToSingleton().Bind<LevelUpPanelController>().ToSelf().Bind<OfflineIncomePanelController>().ToSelf().Bind<GamePlayController>().ToSelf().Bind<SettingPanelController>().ToSelf().Bind<LevelCheatPanelController>().ToSelf();
    }

    protected override void BindModels()
    {
        //base.containers[0].Bind<RewardedVideoButtonModel>().ToSelf().Bind<LevelModel>().ToGameObject().Bind<LikeModel>().ToGameObject().Bind<LevelUpPanelModel>().ToSelf().Bind<OfflineIncomePanelModel>().ToSelf().Bind<LocationModel>().ToGameObject().Bind<CoinsModel>().ToGameObject().Bind<SwitchModel>().ToSingleton<SwitchSoundModel>().As("soundSwitcherModel").Bind<SwitchModel>().ToSingleton<SwitchVibroModel>().As("vibroSwitcherModel");
    }

    protected override void BindConfigs()
    {
        base.containers[0].Bind<GameConfig>().To<GameConfig>(this.gameConfig);
    }

    protected override void BindView()
    {
        base.containers[0].Bind<LoaderView>().To<LoaderView>(this.loaderView).Bind<GamePlayView>().To<GamePlayView>(this.gamePlayView).Bind<GamePanelView>().To<GamePanelView>(this.gamePanelView).Bind<LevelUpPanelView>().To<LevelUpPanelView>(this.levelUpPanelView).Bind<SettingPanelView>().To<OfflineIncomePanelView>(this.offlineIncomePanelView).Bind<LevelCheatPanelView>().To<LevelCheatPanelView>(this.levelCheatPanelView).Bind<LocationItemPanelView>().To<LocationItemPanelView>(this.locationItemPanelView);
    }

    protected override void BindComponents()
    {
        base.containers[0].Bind<UpgradebleObjectsController>().To<UpgradebleObjectsController>(this.upgradebleObjectsController);
    }

    public override void Init()
    {
        base.Init();
        this.starter.Init();
    }

    protected override void BindCommands()
    {
        base.containers[0].RegisterCommand<LevelLoadedCommand>();
    }

    [SerializeField]
    private Starter starter;

    [SerializeField]
    private LoaderView loaderView;

    [SerializeField]
    private GamePlayView gamePlayView;

    [SerializeField]
    private GamePanelView gamePanelView;

    //[SerializeField]
    //private SettingPanelView settingPanelView;

    [SerializeField]
    private OfflineIncomePanelView offlineIncomePanelView;

    [SerializeField]
    private LocationItemPanelView locationItemPanelView;

    [SerializeField]
    private UpgradebleObjectsController upgradebleObjectsController;

    [SerializeField]
    private LevelUpPanelView levelUpPanelView;

    [SerializeField]
    private LevelCheatPanelView levelCheatPanelView;

    [SerializeField]
    private AudioSystem audioSystem;

    [SerializeField]
    private GameConfig gameConfig;
}
