using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Random = UnityEngine.Random;

public class HUDRewardSimple : BaseHUD
{
    [SerializeField] private RewardUI _rewardUi;
    [SerializeField] private Transform _rewardHolder;
    [SerializeField] private Image _collectAnimPrefab;
    private List<RewardData> _datas;

    public override void ResetLayers()
    {
        base.ResetLayers();
        Timing.CallDelayed(0.1f, () => { GetComponent<Canvas>().sortingOrder = 100; });
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        if (args != null && args.Length > 0)
        {
            CreateReward((List<RewardData>) args[0], (bool) args[1], (bool) args[2]);
        }
    }

    public void CreateReward(List<RewardData> datas, bool autoCollect, bool runCollectAnim = false)
    {
        RemoveOldRewards();

        _datas = datas;
        foreach (var rewardData in datas)
        {
            var rewardUi = Instantiate(_rewardUi, _rewardHolder);
            rewardUi.gameObject.SetActive(true);
            rewardUi.Load(rewardData,false);
            rewardUi.transform.localScale = Vector3.one * 1.3f;

            if (autoCollect)
            {
                SaveManager.Instance.Data.AddReward(rewardData);
                // CurrencyModels.instance.AddCurrency(rewardData.ConvertToCurrencyType(), rewardData._value);
            }
        }
    }

    private void RemoveOldRewards()
    {
        for (int i = 1; i < _rewardHolder.childCount; i++)
        {
            Destroy(_rewardHolder.GetChild(i).gameObject);
        }
    }

    public override void OnButtonBack()
    {
        base.OnButtonBack();
        // foreach (var VARIABLE in _datas)
        // {
        //     switch (VARIABLE._type)
        //     {
        //         case REWARD_TYPE.GOLD:
        //         case REWARD_TYPE.PILL:
        //         case REWARD_TYPE.SCROLL_WEAPON:
        //         case REWARD_TYPE.DIAMOND:
        //             MasterCanvas.CurrentMasterCanvas.SpawnCollectAnim(
        //                 ResourceManager.instance.GetRewardSprite(VARIABLE._type), (int) VARIABLE._value / 10,
        //                 CurrencyBar.Instance.GetCurrencyTransform(DesignHelper.ConvertToCurrencyType(VARIABLE)));
        //             break;
        //     }
        // }
    }
}