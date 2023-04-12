using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Ez.Pooly;
using UnityEngine.UI;
using TMPro;

public class UIFloatingText : MonoBehaviour
{
    public TextMeshProUGUI _txtContent;

    public RectTransform _rectTransform;
    private float _duration;
    private float _moveHeight;

    public void Initialize(string content, Vector2 screenPos, int textSize, Color _color, float moveHeight, float Duration)
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        _txtContent.text = content;

        _txtContent.fontSize = textSize;
        _color.a = 1;
        _txtContent.color = _color;
        _duration = Duration;
        _moveHeight = moveHeight;
        transform.localScale = Vector3.one;
        _rectTransform.position = screenPos;
    }

    public void Show()
    {
        gameObject.SetActiveIfNot(true);
        transform.DOKill();
        _txtContent.DOKill();

        _txtContent.transform.localScale = Vector3.one;
        transform.DOLocalMoveY(transform.localPosition.y + _moveHeight, _duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            Pooly.Despawn(this.transform);
        });
        _txtContent.transform.DOScale(0.2f, _duration * 0.6f).SetDelay(_duration * 0.4f).SetEase(Ease.Linear);
        _txtContent.DOFade(0, _duration * 0.6f).SetDelay(_duration * 0.4f);
    }
}
