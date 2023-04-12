using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class CanvasContainDialog : BaseParentHUD
{
    [SerializeField] public TutorialCanvas tutorialCanvas;
    [SerializeField] public Transform collectAnimHolder;
    public SpawnCollectAnim spawnCollectAnim;

    protected virtual void Start()
    {
        if ((float) Screen.width / Screen.height < 0.43f)
        {
            GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
    }

    
    public void ShowNotEnoughHUD(CurrencyType type, long cost, bool forceShow = true)
    {
        TopLayerCanvas.instance.ShowNotEnoughHUD(type, cost, forceShow);
    }

    public void ShowRewardSimpleHUD(List<RewardData> rewardDatas, bool autoCollect, bool showCollectAnim = false)
    {
        ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, rewardDatas, autoCollect, showCollectAnim);
    }

    public void ShowNotifyHUD(string text)
    {
        ShowHUD(EnumHUD.HUD_NOTIFY, false, null, text);
        // ShowHUD(EnumHUD.HUD_NOTIFY, false);
    }

    public void ShowPurchaseFail()
    {
        ShowHUD(EnumHUD.HUD_NOTIFY, false, null, LOCALIZE_ID_PREF.PURCHASE_FAIL.AsLocalizeString(), false);
    }

    public void SpawnCollectAnim(Sprite sprite, int num, Transform destination)
    {
        // In game
        if (GamePlayController.instance != null)
            return;

        spawnCollectAnim.SpawnCollectAnimation(sprite, num, destination, collectAnimHolder, Vector3.zero);
    }

    public void SpawnCollectAnim(List<RewardData> rewardDatas, Vector3 position, float delay, int min, int max)
    {
        // In game
        if (GamePlayController.instance != null)
        {
            MasterCanvas.CurrentMasterCanvas.ShowRewardSimpleHUD(rewardDatas, false);
            return;
        }

        foreach (var VARIABLE in rewardDatas)
        {
            int value = (int) VARIABLE._value;
            bool canSpawn = false;
            switch (VARIABLE._type)
            {
                case REWARD_TYPE.GOLD:
                    canSpawn = true;
                    value /= 2000;
                    break;
                case REWARD_TYPE.DIAMOND:
                    canSpawn = true;
                    value /= 150;
                    break;
                case REWARD_TYPE.SCROLL_WEAPON:
                    canSpawn = true;
                    value /= 500;
                    break;
                case REWARD_TYPE.PILL:
                    canSpawn = true;
                    value /= 500;
                    break;
            }

            if (canSpawn)
            {
                ResourceManager.instance.GetRewardSprite(VARIABLE._type, s =>
                {
                    spawnCollectAnim.SpawnCollectAnimation(s,
                        value,
                        CurrencyBar.Instance.GetCurrencyTransform(DesignHelper.ConvertToCurrencyType(VARIABLE)),
                        collectAnimHolder, position, delay, min, max);
                });
            }
            else
            {
                ShowRewardSimpleHUD(rewardDatas, false, true);
                break;
            }
        }
    }
}