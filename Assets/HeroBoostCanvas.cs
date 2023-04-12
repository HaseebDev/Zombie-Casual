using DG.Tweening;
using Ez.Pooly;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroBoostCanvas : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public RectTransform content;

    private bool isInited = false;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        isInited = true;
    }

    public void SetIconBoost(Sprite sprite, Action<BoostItemView> callback)
    {
        AddressableManager.instance.LoadObjectAsync<BoostItemView>(POOLY_PREF.BOOST_VIEW_ICON, (boostItemView) =>
        {
            var boostView = Pooly.Spawn<BoostItemView>(boostItemView.transform, Vector3.zero, Quaternion.identity, content);
            boostView.SetIcon(sprite);
            boostView.transform.localPosition = Vector3.zero;
            boostView.transform.localScale = Vector3.one;
            boostView.transform.localRotation = Quaternion.Euler(Vector3.zero);
            boostView.SetColorAlpha(0);
            boostView._canvasGroup.DOFade(1, .3f).SetEase(Ease.Linear);
            callback?.Invoke(boostView);
        });

    }

    private void Update()
    {
        if (this.canvasGroup != null && GamePlayController.instance != null)
            canvasGroup.transform.rotation = GamePlayController.instance.GetMainCamera().transform.rotation;
        else
            canvasGroup.transform.rotation = Camera.main.transform.rotation;
    }
}
