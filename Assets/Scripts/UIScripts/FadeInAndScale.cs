using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FadeInAndScale : MonoBehaviour
{
    public CanvasGroup CanvasGroup;

    private void Start()
    {
        if (CanvasGroup == null)
            CanvasGroup = GetComponent<CanvasGroup>();
        
        CanvasGroup.DOFade(1, 0.1f).SetEase(Ease.Linear);
        transform.DOScale(1.2f, 0.05f).OnComplete(() =>
        {
            transform.DOScale(1, 0.05f);
        }); 
    }
}
