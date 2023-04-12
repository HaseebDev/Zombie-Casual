using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using MEC;
using TMPro;
using UnityEditor;

public class HUDGameLoading : BaseHUD
{
    // public GameObject zombieWalking;
    // public Slider _loadingProgress;
    // public Text txtUserID;
    public TextMeshProUGUI versionText;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);

        // _loadingProgress.value = 0;
        // _loadingProgress.maxValue = 100;
        // txtUserID.gameObject.SetActiveIfNot(false);
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_USER_INFO_ID,
            new Action<string>(UpdateUserInfoID));

        versionText.text = $"v{Application.version}";

// #if UNITY_ANDROID
//         //, build: {PlayerSettings.Android.bundleVersionCode}";
// #elif UNITY_IOS
//         versionText.text = $"v{Application.version}, build: {PlayerSettings.iOS.buildNumber}";
// #endif
    }

   

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.UPDATE_USER_INFO_ID,
            new Action<string>(UpdateUserInfoID));
    }

    // private void OnDisable()
    // {
    //     if (zombieWalking != null)
    //     {
    //         zombieWalking.SetActive(false);
    //     }
    // }
    //
    // private void OnEnable()
    // {
    //     if (zombieWalking != null)
    //     {
    //         zombieWalking.SetActive(true);
    //     }
    // }

    public void UpdateUserInfoID(string userID)
    {
        // txtUserID.gameObject.SetActiveIfNot(true);
        // txtUserID.text = string.IsNullOrEmpty(userID) ? "" : $"UID: {userID}";
    }

    public void SetLoadingPercent(float percent)
    {
        // _loadingProgress.DOValue(percent, 0.1f).SetEase(Ease.Linear);
    }

    public override void OnButtonBack()
    {
        BackButtonManager.Instance.ShowCanGoBackText(true);
    }
}