using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using com.datld.data;
using Ez.Pooly;
using QuickType;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionFinalItemUI : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI progressText;
    public Image bg;
    public UIShiny shiny;
    public List<GameObject> claimed;
    public Slider progress;

    public Sprite _normalBG;
    public Sprite _completedBG;
    public Transform _rewardHolder;

    private MissionDurationDesignElement _design;
    private MissionDurationData _data;
    private List<RewardData> _rewards;


    public void Load(MissionDurationData missionDurationData)
    {
        _data = missionDurationData;
        _rewardHolder.DespawnAllChild();

        string duration = missionDurationData.Duration == 1 ? "daily" : "weekly";
        name.text = $"Complete all {duration} mission";

        bg.sprite = missionDurationData.IsComplete ? _completedBG : _normalBG;
        shiny.enabled = missionDurationData.IsComplete && !missionDurationData.IsClaimed;

        _design = DesignHelper.GetMissionDurationDesign(missionDurationData.Duration,
            SaveGameHelper.GetMaxCampaignLevel());

        _rewards = new List<RewardData>();
        foreach (var VARIABLE in _design.Reward)
        {
            var rewardUi =
                Pooly.Spawn<RewardUI>(POOLY_PREF.REWARD_UI, Vector3.zero, Quaternion.identity, _rewardHolder);
            rewardUi.transform.localScale = Vector3.one;
            var reward = new RewardData(VARIABLE.RewardId, VARIABLE.Value);

            rewardUi.Load(reward);
            _rewards.Add(reward);
        }

        // foreach (var VARIABLE in claimed)
        // {
        //     VARIABLE.SetActive(missionDurationData.IsComplete && missionDurationData.IsClaimed);
        // }

        int completedMission = 0;
        foreach (var VARIABLE in missionDurationData.MissionDatas)
        {
            if (VARIABLE.IsComplete)
            {
                completedMission++;
            }
        }

        progress.value = (float) completedMission / missionDurationData.MissionDatas.Count;
        progressText.text = $"{completedMission}/{missionDurationData.MissionDatas.Count}";
    }

    public void OnClaim()
    {
        if (Time.time - HUDMission._lastClaimTime < 0.2f)
        {
            return;
        }
        
        if (!_data.IsComplete || _data.IsClaimed) return;
        TopLayerCanvas.instance.ShowRewardSimpleHUD(_rewards, true, true);
        _data.IsClaimed = true;
        Load(_data);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.CLAIM_MISSION);
        HUDMission._lastClaimTime = Time.time;
    }
}