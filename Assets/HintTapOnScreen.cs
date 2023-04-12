using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MEC;

public class HintTapOnScreen : MonoBehaviour
{
    public CanvasGroup _canvasGroup;
    public Animator _animHand;
    public float _animSpeed = 2.0f;

    private void Awake()
    {
        _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();

    }

    public void SetEnable(bool enable)
    {
        if (gameObject.activeInHierarchy == enable)
            return;

        transform.DOKill();
        gameObject.SetActiveIfNot(true);
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.DOFade(enable ? 1 : 0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.SetActiveIfNot(enable);
        });
    }

    public void PlayAnimOnce()
    {
        transform.DOKill();
        transform.localScale = Vector3.one * 1.2f;
        transform.DOScale(1.0f, 0.2f).SetEase(Ease.InOutSine);
    }

    public void PlayAnimLoop()
    {
        SetEnable(true);
        _animHand.gameObject.SetActiveIfNot(true);
        _animHand.speed = _animSpeed;

    }



}
