using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonRestartStep : TutorialButtonID
{
    public int tutorialPhase = 0;
    public GameObject target;

    private bool _isTargetVisible = false;

    private void Update()
    {
        if (TutorialManager.instance.CurrentTutorialPhaseIndex == tutorialPhase)
        {
            if (_active && target != null && !_isTargetVisible)
            {
                if (target.activeInHierarchy)
                {
                    TutorialManager.instance.StartLastTutorial();
                }
            }
        }
        // else
        // {
        //     enabled = false;
        // }


        _isTargetVisible = target.activeInHierarchy;
    }
}