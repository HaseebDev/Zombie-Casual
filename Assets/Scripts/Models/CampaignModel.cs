using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

[Serializable]
public class CampaignModel
{
    public CastleHealth castleHealth;
    public float TotalArmour { get; private set; }

    private bool BlockTooMuchCall = false;

    public bool AttackBasement(float dmg)
    {
        if (castleHealth.CurrentHp <= 0)
            DefeatLevel();

        if (!BlockTooMuchCall)
        {
            BlockTooMuchCall = true;
            DispatchUI();
            Timing.CallDelayed(0.5f, () =>
            {
                BlockTooMuchCall = false;
            });
        }

        return true;
    }

    public void DispatchUI()
    {
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CASTLE_HP, castleHealth.Percent, castleHealth.CurrentHp);
    }

    public void UpdateData()
    {
        BlockTooMuchCall = false;
        DispatchUI();
    }

    public void ResetMaxHP()
    {
        castleHealth.startHp = SaveManager.Instance.Data.CalcTotalCastleHP();
        BlockTooMuchCall = false;
    }

    public void ResetData(float percentHP = 1f)
    {
        ResetMaxHP();
        castleHealth.ResetHealth();
        TotalArmour = SaveManager.Instance.Data.CalcTotalCastleArmour();

        if (percentHP < 1.0f)
        {
            GamePlayController.instance.campaignData.AttackBasement(castleHealth.startHp * (1.0f - percentHP));
        }
        BlockTooMuchCall = false;
        DispatchUI();
    }

    public void DefeatLevel()
    {
        EventSystemServiceStatic.Dispatch(GamePlayController.instance, EVENT_NAME.CASTLE_DEFEATED);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.LEVEL_FAILED);
    }


}