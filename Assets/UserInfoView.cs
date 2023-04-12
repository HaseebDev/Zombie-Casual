using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityExtensions.Localization;

[Serializable]
public class UserInfoViewData
{
    public string name;
    public int currentLevel;
    public int currentProgress;
    public int maxProgress;
}


public class UserInfoView : MonoBehaviour
{
    public Image imgAvatar;
    public LocalizedTMPTextUI txtName;
    public LocalizedTMPTextUI txtProgress;
    public LocalizedTMPTextUI txtLevel;
    public Slider sliderProgress;

    private void Start()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_USER_INFO_VIEW, new Action<UserInfoViewData>(UpdateHUD));
    }

    public void UpdateHUD(UserInfoViewData data)
    {
        // txtName.text = data.name;
        // txtLevel.text = data.currentLevel.ToString();
        // txtProgress.text = $"{data.currentProgress}/{data.maxProgress}";
        //
        // sliderProgress.interactable = false;
        // sliderProgress.maxValue = data.maxProgress;
        // sliderProgress.value = data.currentProgress;
        //
        // ResourceManager.instance.GetHeroAvatar(GameConstant.MANUAL_HERO, imgAvatar);
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.UPDATE_USER_INFO_VIEW, new Action<UserInfoViewData>(UpdateHUD));
    }
}
