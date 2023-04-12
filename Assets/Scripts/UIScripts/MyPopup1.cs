using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

public class MyPopup1 : MonoBehaviour
{
    [SerializeField] private Image _blackBG;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    [Button]
    public void UpdateReference()
    {
        foreach (var VARIABLE in GetComponentsInChildren<Image>())
        {
            if (VARIABLE.gameObject.name == "BlackBG")
            {
                _blackBG = VARIABLE;
                var eventtrigger = _blackBG.GetComponent<EventTrigger>();
                if (eventtrigger == null)
                    eventtrigger = _blackBG.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;

                eventtrigger.triggers.Add(entry);
                break;
            }
        }
    }

    [Button]
    public void MakeBlackBGOutSideNotch()
    {
        _blackBG.rectTransform().localScale = Vector3.one * 1.2f;
    }
   
    public void Hide()
    {
        _blackBG.gameObject.SetActive(false);
        _canvasGroup.DOFade(0, 0.3f);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1.1f, 0.1f));
        sequence.Append(transform.DOScale(0f, 0.2f));
        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        // transform.DOScale(0, 0.3f).OnComplete(()=>
        // {
        // });
    }

    private void OnDestroy()
    {
        _blackBG.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        _blackBG.gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
        transform.localScale = Vector3.one;
        // transform.DOScale(1, 0.4f);
    }
}