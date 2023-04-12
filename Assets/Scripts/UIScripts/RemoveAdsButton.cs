using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class RemoveAdsButton : MonoBehaviour
{
    private Vector2 _originPos;
    private RectTransform _rectTransform;
    public int LEVEL_UNLOCK_REMOVE_ADS = 5;

    public bool EnableRemoveAds { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        EnableRemoveAds = true;

        var unlockRemoveAds = FirebaseRemoteSystem.instance.GetConfigValue("LEVEL_UNLOCK_REMOVE_ADS");
        if (unlockRemoveAds.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
        {
            LEVEL_UNLOCK_REMOVE_ADS = (int)unlockRemoveAds.LongValue;
        }

        var remoteConfig = FirebaseRemoteSystem.instance.GetConfigValue("ENABLE_REMOVE_ADS");
        if (remoteConfig.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
        {
            EnableRemoveAds = remoteConfig.BooleanValue;
        }

        _rectTransform = transform.rectTransform();
        _originPos = _rectTransform.anchoredPosition;

        OnNewLevel();

        GetComponent<Button>().onClick.AddListener(() =>
        {
            MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_REMOVE_ADS, false);
        });

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(OnNewLevel));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ON_REMOVE_ADS, new Action(OnPurchase));
    }

    private void OnNewLevel()
    {
        if (!EnableRemoveAds)
        {
            gameObject.SetActiveIfNot(false);
            return;
        }

        bool isShow = !SaveManager.Instance.Data.RemoveAds && SaveGameHelper.GetMaxCampaignLevel() >= LEVEL_UNLOCK_REMOVE_ADS;
        // Debug.LogError("IS SHOW " + isShow + " , REMOVE ADS " + SaveManager.Instance.Data.RemoveAds + " , LEVEL " + SaveGameHelper.GetCurrentCampaignLevel());
        gameObject.SetActive(false);

        if (isShow)
        {
            DOVirtual.DelayedCall(1, () =>
            {
                gameObject.SetActive(true);

                _rectTransform.DOKill();
                _rectTransform.anchoredPosition = _originPos + new Vector2(Utils.ConvertToMatchWidthRatio(200), 0);
                _rectTransform.DOAnchorPos(_originPos, 0.35f);
            });
        }

    }


    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ON_REMOVE_ADS, new Action(OnPurchase));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(OnNewLevel));
    }

    private void OnPurchase()
    {
        gameObject.SetActive(!SaveManager.Instance.Data.RemoveAds);
    }
}