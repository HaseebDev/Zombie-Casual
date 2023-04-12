using System.Collections;
using System.Collections.Generic;
using Ez.Pooly;
using QuickType;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;

public class HomeLevelProgressItem : MonoBehaviour
{
    [SerializeField] private Image _bg;
    [SerializeField] private LocalizedTMPTextUI _levelText;

    [SerializeField] private GameObject _reward;
    [SerializeField] private Transform _rewardHolder;
    [SerializeField] private RewardUI _rewardUiPrefab;

    private RewardUI _rewardUi;

    public void Load(int level, Color color, CustomBattleReward rewardData, bool hasReward = false)
    {
        _levelText.text = level.ToString();
        _bg.color = color;

        _reward.SetActive(hasReward);
        if (rewardData != null && hasReward)
        {
            var reward = rewardData.Rewards[0];

            // RewardItemView rwdView = Pooly.Spawn<RewardItemView>(POOLY_PREF.REWARD_ITEM_VIEW, Vector3.zero,
            // Quaternion.identity, _rewardHolder);
            if (_rewardUi == null)
                _rewardUi = Instantiate(_rewardUiPrefab, _rewardHolder);

            _rewardUi.transform.localScale = Vector3.one;

            var rectTrans = _rewardUi.rectTransform();
            rectTrans.anchorMax = rectTrans.anchorMin = rectTrans.pivot = Vector2.one * 0.5f;
            rectTrans.anchoredPosition = Vector2.zero; //+ new Vector2(0,Utils.ConvertToMatchHeightRatio(15));

            _rewardUi.Load(new RewardData(reward.RewardId, 0));
        }
    }
}