using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

public class BaseTutorialType : BaseState
{
    public bool CanSkipInThisStep = false;

    protected virtual void OnShow()
    {
        if (CanSkipInThisStep)
        {
            GetComponentInParent<TutorialPhase>().CanSkipNow = true;
        }
    }
}