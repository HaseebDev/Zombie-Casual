using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Ez.Pooly;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class UIFloatingTextNotify : MonoBehaviour
{
    public LocalizedTMPTextUI _txtContent;
    public CanvasGroup _canvasGroup;
    public RectTransform _rectTransform;

    private float _duration;
    private float _moveHeight;
    public bool DespawnAfterHide = true;

    public void Initialize(string content, int textSize, float moveHeight, float Duration, bool toUpper = true,
        bool trim = true)
    {
        transform.DOKill();
        _rectTransform = GetComponent<RectTransform>();

        if (toUpper)
        {
            content = content.ToUpper();
        }

        if (trim)
        {
            content = content.Trim();
        }

        _txtContent.text = content;
        _txtContent.targetTMPText.fontSize = textSize;
        _duration = Duration;
        _moveHeight = moveHeight;
        transform.localScale = Vector3.one;
        _rectTransform.anchoredPosition = new Vector3(0, 0, 0);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _rectTransform.DOKill();
        _canvasGroup.DOKill();
        transform.DOKill();

        transform.DOLocalMoveY(transform.localPosition.y + _moveHeight, _duration).SetEase(Ease.InOutSine);
        _rectTransform.localScale = Vector3.one * 0.8f;
        _rectTransform.DOScale(1, 0.3f);

        _canvasGroup.alpha = 1;
        _canvasGroup.DOFade(0, _duration).SetDelay(_duration * 0.5f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (DespawnAfterHide)
                    Pooly.Despawn(this.transform);
            });
        // _txtContent.targetTMPText.DOFade(0, _duration * 0.6f).SetDelay(_duration * 0.4f).OnComplete(() =>
        // {
        // Pooly.Despawn(this.transform);
        // });
    }

    public void Hide()
    {
        _rectTransform.DOKill();
        _canvasGroup.DOKill();
        transform.DOKill();
        _canvasGroup.alpha = 0;
        if (DespawnAfterHide)
            Pooly.Despawn(this.transform);
        gameObject.SetActive(false);
    }
}