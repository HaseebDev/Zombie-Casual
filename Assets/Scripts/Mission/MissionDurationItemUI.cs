using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using Ez.Pooly;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;

public class MissionDurationItemUI : MonoBehaviour
{
    public GameObject activeImage;
    public Transform missionsHolder;
    public TextMeshProUGUI resetTimeText;
    public ReminderUI reminderUI;
    
    protected MissionDurationData _missionDurationData;

    private void Awake()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.CLAIM_MISSION, new Action(UpdateReminder));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.CLAIM_MISSION, new Action(UpdateReminder));
    }

    public virtual void Init()
    {
        
    }
    
    public void ResetLayer()
    {
        missionsHolder.DespawnAllChild();

        foreach (var VARIABLE in _missionDurationData.MissionDatas)
        {
            if(VARIABLE.IsComplete && VARIABLE.IsClaimed)
                continue;
            
            var missionItemUI = Pooly.Spawn<MissionItemUI>(POOLY_PREF.MISSION_ITEM_UI, Vector3.zero,
                Quaternion.identity, missionsHolder);

            missionItemUI.transform.localScale = Vector3.one;
            missionItemUI.Load(VARIABLE);
            
            if(VARIABLE.IsComplete)
                missionItemUI.transform.SetAsFirstSibling();
        }
        
        UpdateTimer();
        CreateFinalMission();
        UpdateReminder();
    }

    public virtual void CreateFinalMission()
    {
        if(_missionDurationData.IsComplete && _missionDurationData.IsClaimed)
            return;
        
        var missionFinalItemUI = Pooly.Spawn<MissionFinalItemUI>(POOLY_PREF.MISSION_FINAL_ITEM_UI, Vector3.zero,
            Quaternion.identity, missionsHolder);

        missionFinalItemUI.transform.localScale = Vector3.one;
        missionFinalItemUI.Load(_missionDurationData); 
    }

    public virtual void UpdateTimer()
    {
        
    }

    public virtual void UpdateReminder()
    {
        int _completedMission = 0;
        
        foreach (var VARIABLE in _missionDurationData.MissionDatas)
        {
            if (VARIABLE.IsComplete && !VARIABLE.IsClaimed)
            {
                _completedMission++;
            }
        }
        
        reminderUI.Load(_completedMission); 
    }

    public void Show()
    {
        gameObject.SetActive(true);
        activeImage.gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        activeImage.gameObject.SetActive(false);
    }
}
