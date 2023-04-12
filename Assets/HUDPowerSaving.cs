using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using MEC;

public class HUDPowerSaving : BaseHUD
{
    public Image _bgImg;
    public TextMeshProUGUI _txtTitle;

    private Action<bool> OnHide;
    private CoroutineHandle coroutineAnim;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        _bgImg.SetColorAlpha(1);
        OnHide = (Action<bool>)args[0];
        if (coroutineAnim != null)
        {
            Timing.KillCoroutines(coroutineAnim);
        }
        coroutineAnim = Timing.RunCoroutine(PlayTitleAnim());

    }

    IEnumerator<float> PlayTitleAnim()
    {
        _txtTitle.SetColorAlpha(0);
        while (true)
        {
            _txtTitle.DOFade(1f, 1f).SetEase(Ease.Linear);
            yield return Timing.WaitForSeconds(1.2f);
            _txtTitle.DOFade(0f, 1f).SetEase(Ease.Linear);
            yield return Timing.WaitForSeconds(1.2f);
        }
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        if (coroutineAnim != null)
        {
            Timing.KillCoroutines(coroutineAnim);
        }
        OnHide?.Invoke(true);
    }

    public void FadeIn()
    {
        _bgImg.DOFade(0.75f, 0.5f).SetEase(Ease.Linear);
    }
}


