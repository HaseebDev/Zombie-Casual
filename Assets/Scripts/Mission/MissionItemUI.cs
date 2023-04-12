using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using com.datld.data;
using QuickType;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItemUI : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI progressText;
    public RewardUI rewardUI;
    public Slider progress;
    public Image bg;
    public UIShiny shiny;
    public GameObject claimed;

    public Sprite _normalBG;
    public Sprite _completedBG;
    private MissionData _missionData;
    private MissionDesignElement _missionDesign;
    private RewardData _rewardData;


    public void Load(MissionData missionData)
    {
        _missionData = missionData;
        _missionDesign = DesignHelper.GetMissionDesign(missionData.Id);

        
        var reward = _missionDesign.Rewards[0];
        _rewardData = new RewardData(reward.RewardId, reward.Value);
        rewardUI.Load(_rewardData);

        progress.value = (float) missionData.CurrentStep / _missionDesign.Total;

        int currentStep = missionData.IsComplete? (int)_missionDesign.Total : missionData.CurrentStep;
        
        progressText.text = $"{currentStep}/{_missionDesign.Total}";
        name.text = LocalizeController.GetText(_missionDesign.Description, _missionDesign.Total);

        bg.sprite = missionData.IsComplete ? _completedBG : _normalBG;
        shiny.enabled = missionData.IsComplete && !missionData.IsClaimed;
        // claimed.SetActive(missionData.IsComplete && missionData.IsClaimed);

        // claimed.SetActive(!missionData.IsComplete); 
    }
    
    public void OnClaim()
    {
        if (Time.time - HUDMission._lastClaimTime < 0.2f)
        {
            return;
        }

        if (_missionData.IsComplete && !_missionData.IsClaimed)
        {
            TopLayerCanvas.instance.ShowRewardSimpleHUD(new List<RewardData>() {_rewardData}, true, true);
            _missionData.IsClaimed = true;
            Load(_missionData);
            
            HUDMission._lastClaimTime = Time.time;
            EventSystemServiceStatic.DispatchAll( EVENT_NAME.CLAIM_MISSION);
        }
    }
}