using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClickTutorialType : BaseTutorialType
{
    public string buttonID = "";

    public Vector3 bonusPosition;
    // public bool resetIfFail = false;

    protected Button targetButton;

    protected bool isFound = false;

    protected bool IsFound
    {
        get { return isFound; }

        set
        {
            isFound = value;
            if (value)
                OnShow();
        }
    }

    protected bool onShowed = false;
    protected List<TutorialButtonID> allButtonId;

    public override void OnEnter()
    {
        base.OnEnter();
        ResetState();
        DOVirtual.DelayedCall(0.1f, () => { onShowed = true; });
    }

    private void ResetState()
    {
        onShowed = false;
        isFound = false;
    }

    public override void OnUpdate()
    {
        if (IsFound || !onShowed)
            return;

        CheckAndShow();
    }

    private TutorialButtonID FindButtonID()
    {
        allButtonId = FindObjectsOfType<TutorialButtonID>().ToList();
        var buttonObject = allButtonId.Find(x => x.id == buttonID);
        return buttonObject;
    }

    protected virtual void BindListener(TutorialButtonID buttonObject)
    {
        targetButton = buttonObject.GetComponent<Button>();
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(OnExit);

            if (buttonObject.focusArrow != null)
            {
                buttonObject.focusArrow.SetActive(true);
                targetButton.onClick.AddListener(() => { buttonObject.focusArrow.SetActive(false); });
            }
            else
            {
                buttonObject.focusArrow =
                    MasterCanvas.CurrentMasterCanvas.tutorialCanvas.FocusButton(targetButton, bonusPosition);
            }

            buttonObject.OnActive();

            IsFound = true;
        }
    }

    public virtual void CheckAndShow()
    {
        var buttonObject = FindButtonID();
        if (buttonObject == null)
            return;

        BindListener(buttonObject);
    }
}