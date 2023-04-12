using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

public class MyPopup : MonoBehaviour
{
    [SerializeField] private Image _blackBG;
    [SerializeField] private UIView _uiView;

    [Button]
    public void UpdateReference()
    {
        _uiView = GetComponent<UIView>();
        foreach (var VARIABLE in GetComponentsInChildren<Image>())
        {
            if (VARIABLE.gameObject.name == "BlackBG")
            {
                _blackBG = VARIABLE;
                var eventtrigger = _blackBG.GetComponent<EventTrigger>();
                if (eventtrigger == null)
                    eventtrigger = _blackBG.gameObject.AddComponent<EventTrigger>();

                // Button closeBtn = null;
                // foreach (var iter in GetComponentsInChildren<Button>())
                // {
                //     if (iter.gameObject.name.Contains("CloseButton"))
                //     {
                //         closeBtn = iter;
                //         break;
                //     }
                // }

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                // entry.callback.AddListener((data) => { closeBtn.OnSubmit(data); });

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

    // private void Awake()
    // {
    //     _uiView.OnVisibilityChanged.AddListener(v =>
    //     {
    //         Color color = _blackBG.color;
    //         color.a = v;
    //         _blackBG.color = color;
    //     });
    // }

    public void Hide()
    {
        _blackBG.gameObject.SetActive(false);
        _uiView.Hide();
    }

    // private void OnDestroy()
    // {
    //     _blackBG.gameObject.SetActive(true);
    // }

    private void OnEnable()
    {
        _blackBG.gameObject.SetActive(true);
        _uiView.Show(true);
    }

    public void OnShow()
    {
        _uiView.CancelAutoHide();
        _blackBG.gameObject.SetActive(true);
    }
}