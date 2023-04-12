using System;
using System.Collections;
using System.Collections.Generic;
using QuickType;
using UnityEngine;

public class ButtonMission : MonoBehaviour
{
    public GameObject lockImg;
    public ReminderUI reminderUI;
    
    public static int newMission = 0;
    private int _completedMission = 0;
    private bool _unlocked = false;

    private void Awake()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.CLAIM_MISSION, new Action(UpdateReminder));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.CLAIM_MISSION, new Action(UpdateReminder));
    }

    public void ResetLayer()
    {
        // newMission = 0;
        MissionManager.Instance.CheckToReset();
        MissionManager.Instance.CheckLoginMission();

        _unlocked = DesignHelper.IsRequirementAvailable(EnumHUD.HUD_MISSION.ToString());
        lockImg.SetActive(!_unlocked);
        UpdateReminder();
    }

    private void UpdateReminder()
    {
        if (_unlocked)
        {
            _completedMission = 0;
            foreach (var VARIABLE in SaveManager.Instance.Data.MissionManagerData.Daily.MissionDatas)
            {
                if (VARIABLE.IsComplete && !VARIABLE.IsClaimed)
                {
                    _completedMission++;
                }
            }
        
            foreach (var VARIABLE in SaveManager.Instance.Data.MissionManagerData.Weekly.MissionDatas)
            {
                if (VARIABLE.IsComplete && !VARIABLE.IsClaimed)
                {
                    _completedMission++;
                }
            }
        
            reminderUI.Load(newMission + _completedMission); 
        }
        else
        {
            reminderUI.Load(0);
        }
    }
    
    public void OpenMissionHUD()
    {
        var unlockRequire = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_MISSION.ToString());
        if (!unlockRequire.Item1)
        {
            TopLayerCanvas.instance.ShowFloatingTextNotify(LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV, unlockRequire.Item2));
        }
        else
        {
            newMission = 0;
            reminderUI.Load(_completedMission);
            TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_MISSION);
        }
    }
}
