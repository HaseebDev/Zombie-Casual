using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using UnityEngine;

public class AngleUpgradeButton : SoliderUpgradeButton
{
    [SerializeField] private GameObject parentHolder;

    // public override int Level => SaveManager.Instance.Data.Inventory.ManualHero.Level;

    protected override int GetItemMaxLevel()
    {
        return (int) HeroDesign.MaxLevel;
    }

    protected override void OnAddShard(HeroData heroData)
    {
    }

    protected override void OnPromoteHero(HeroData heroData)
    {
    }

    public override long Price
    {
        get
        {
            if (_data != null)
            {
                if (IsDataDirty)
                {
                    _cachedPrice = GetItemPrice(_data.GetHeroLevel() + 1);
                }

                return _cachedPrice;
            }
            else
            {
                return 0;
            }
        }
    }


    public override int Level
    {
        get => _data == null
            ? 0
            : _data.GetHeroLevel();
    }

    protected override void PreInit()
    {
        if (!DesignHelper.IsRequirementAvailable(UnlockRequireId.ANGLE_UPGRADE_BUTTON))
        {
            parentHolder.SetActive(false);
            return;
        }

        base.PreInit();
    }

    public override bool IsAvailableForBuy()
    {
        return CurrencyModels.instance.GetValueFromEnum(currencyType) >= Price;
    }

    public override int GetNextLevel()
    {
        return Data.GetHeroLevel() + 1;
    }

    protected override void UpdateLevelTxt()
    {
        lvlTxtValue.text = $"{Level}";
    }

    protected override void ShowNotEnough()
    {
        MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(CurrencyType.PILL, Price, false);
    }

    public override void UpgradeItem()
    {
        int nextLevel = GetNextLevel();
        if (!IsMaxLevelWith(nextLevel))
        {
            CurrencyModels.instance.AddCurrency(currencyType, -1 * Price);
            Data.LevelUpHero(nextLevel);

            // if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
            // else if (GamePlayController.instance.gameMode == GameMode.IDLE_MODE)
            //     Data.LevelUpHeroIdleMode(nextLevel);

            SaveManager.Instance.SetDataDirty();

            ResetView();

            EventSystemServiceStatic.DispatchAll(EVENT_NAME.HERO_UPGRADED, HeroID);

            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_UPGRADE);
        }
        else
        {
            maxBtn.SetActive(true);
        }
    }

    protected override long GetItemPrice(int level)
    {
        currencyType = CurrencyType.PILL;
        return GetPriceCampaign(level);
    }

    public override bool IsInTeamMember()
    {
        return true;
    }

    public override bool IsLocked()
    {
        return false;
    }
}