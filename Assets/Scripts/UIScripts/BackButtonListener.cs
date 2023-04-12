using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackButtonListener : MonoBehaviour
{
    public Button referenceBackButton;
    public bool blockBack;
    public UnityEvent onBack = null; // = new Button.ButtonClickedEvent();

    protected virtual void OnEnable()
    {
        BackButtonManager.Instance?.Push(this);
    }

    protected void OnDisable()
    {
        BackButtonManager.Instance?.Pop(this);
    }

    public virtual void OnBack()
    {
        if (blockBack)
        {
            BackButtonManager.Instance.ShowCanGoBackText();
            return;
        }

        if (referenceBackButton != null)
        {
            referenceBackButton.onClick?.Invoke();
            onBack.Invoke();
            return;
        }

        if (onBack == null || onBack.GetPersistentEventCount() == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        onBack.Invoke();
    }
}