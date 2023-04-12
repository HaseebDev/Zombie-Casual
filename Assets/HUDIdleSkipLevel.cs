using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class IdleSkipLevelData
{
    public int FreeSkipLevel;
    public long FreeSkipToken;

    public long PriceMaxSkip;
    public int MaxSkipLevel;
    public long MaxSkipToken;
}

public class HUDIdleSkipLevel : BaseHUD
{
    public IdleSkipLevelData Data { get; private set; }

    public TextMeshProUGUI _txtFreeSkip;
    public TextMeshProUGUI _txtFreeToken;
    public TextMeshProUGUI _txtMaxSkipLevel;
    public TextMeshProUGUI _txtMaxToken;

    public TextMeshProUGUI _txtPrice;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        Data = (IdleSkipLevelData)args[0];

        var curLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE).CurrentLevel;

        _txtFreeSkip.text = (curLevel + Data.FreeSkipLevel).ToString();
        _txtFreeToken.text = Data.FreeSkipToken.ToString();

        _txtMaxSkipLevel.text = (curLevel + Data.MaxSkipLevel).ToString();
        _txtMaxToken.text = Data.MaxSkipToken.ToString();
        _txtPrice.text = Data.PriceMaxSkip.ToString();
    }

    public override void Show(Action<bool> showComplete = null, bool addStack = true)
    {
        base.Show(showComplete, addStack);
        GamePlayController.instance.SetPauseGameplay(true);
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        GamePlayController.instance.SetPauseGameplay(false);
    }

    public void OnButtonSkipMax()
    {
        if (CurrencyModels.instance.Diamonds  >= Data.PriceMaxSkip)
        {
            if (SaveManager.Instance.Data.DecreaseDiamond(Data.PriceMaxSkip))
            {
                DoSkipIdleLevel(Data.MaxSkipLevel);
            }
        }
        else
        {
            MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(CurrencyType.DIAMOND, Data.PriceMaxSkip);
        }


    }

    public void OnButtonSkipFree()
    {
        AdsManager.instance.ShowAdsReward((complete, amount) =>
        {
            if (complete)
            {
                DoSkipIdleLevel(Data.FreeSkipLevel);
            }
        });
    }

    private void DoSkipIdleLevel(int levelToSkip)
    {
        if (GameMaster.instance.currentMode == GameMode.IDLE_MODE)
        {
            int targetLevel = SaveManager.Instance.Data.GameData.IdleProgress.CurrentLevel + levelToSkip;
            SaveManager.Instance.Data.OverwritePlayerProgress(GameMode.IDLE_MODE, targetLevel);
            SaveManager.Instance.Data.GameData.IdleProgress.CurrentWave = 1;
            SaveManager.Instance.SetDataDirty();

            LevelModel.instance.CurrentLevel = targetLevel;
            LevelModel.instance.CurrentWave = 1;
            GamePlayController.instance.RestartIdleMode();
            Hide();
        }
    }

}
