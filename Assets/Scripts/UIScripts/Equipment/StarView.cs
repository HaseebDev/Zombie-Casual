using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class StarView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image _imgStar;

    public Action<StarView> OnStarClick;


    public void EnableStar(bool enable, bool withAnim = false)
    {
        if (withAnim)
        {
            _imgStar.gameObject.SetActiveIfNot(true);
            _imgStar.SetColorAlpha(enable ? 0 : 1);
            _imgStar.DOFade(enable ? 1 : 0, 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                if (!enable)
                    _imgStar.gameObject.SetActiveIfNot(false);
            });
        }
        else
        {
            _imgStar.SetColorAlpha(1);
            _imgStar.gameObject.SetActiveIfNot(enable);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnStarClick?.Invoke(this);
    }
}
