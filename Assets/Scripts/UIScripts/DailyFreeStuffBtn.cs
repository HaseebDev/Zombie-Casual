using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class DailyFreeStuffBtn : MonoBehaviour
{
    public ReminderUI reminderUI;

    private Vector2 _originPos;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = transform.rectTransform();
        _originPos = _rectTransform.anchoredPosition;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_DAILY_FREE_STUFF, false);
        });

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.CHANGE_DAILY_FREE_STUFF,
            new Action<int>(OnChangeFreeStuff));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.CHANGE_DAILY_FREE_STUFF,
            new Action<int>(OnChangeFreeStuff));
    }

    private void Start()
    {
        bool isShow = HUDDailyFreeStuff.HasAvailableItem();
        int freeStuffCount = HUDDailyFreeStuff.GetAvailableFreeStuffCount();
        reminderUI.Load(freeStuffCount);
        if (isShow)
        {
            gameObject.SetActive(DesignHelper.IsRequirementAvailable(EnumHUD.HUD_SHOP.ToString()));
            // DOVirtual.DelayedCall(1, () =>
            // {
            //     gameObject.SetActive(true);
            //
            //     _rectTransform.DOKill();
            //     _rectTransform.anchoredPosition = _originPos + new Vector2(Utils.ConvertToMatchWidthRatio(200), 0);
            //     _rectTransform.DOAnchorPos(_originPos, 0.35f);
            // });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnChangeFreeStuff(int remain)
    {
        reminderUI.Load(remain);
        
        if (remain == 0)
        {
            gameObject.SetActive(false);
        }
    }
}