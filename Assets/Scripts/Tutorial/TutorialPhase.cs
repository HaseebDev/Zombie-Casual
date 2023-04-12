using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

// [ExecuteInEditMode]
public class TutorialPhase : BaseState
{
    private int _currentTutoIndex = 0;
    private bool isComplete = false;
    public List<BaseTutorialBehavior> tutorialSteps;
    public int level = 0;
    public bool isInGame = false;
    private bool _forced = false;

    [HideInInspector] public bool CanSkipNow = false;

    public BaseTutorialBehavior CurrentTutoStep => tutorialSteps[_currentTutoIndex];

    private void StartTutorial(int index)
    {
        SaveManager.Instance.Data.TutorialData.LastTutorialStep = index;
        _currentTutoIndex = index;
        CurrentTutoStep.onExit = NextTutorial;
        CurrentTutoStep.OnEnter();
    }

    private void NextTutorial()
    {
        _currentTutoIndex++;
        if (_currentTutoIndex > tutorialSteps.Count - 1)
        {
            isComplete = true;
            OnExit();
            return;
        }

        StartTutorial(_currentTutoIndex);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        StartTutorial(0);
    }

    private void Update()
    {
        if (!_forced && SaveManager.Instance.Data != null)
        {
            bool passLevelBool = false;
            if (isInGame)
            {
                passLevelBool = GamePlayController.instance != null &&
                                GamePlayController.instance.CurrentLevel >= level;
            }
            else
                passLevelBool = SaveGameHelper.GetCurrentCampaignLevel() >= level;

            if (passLevelBool)
            {
                TutorialManager.instance.ForceToThisPhase(this);
                _forced = true;
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (!isComplete)
        {
            CurrentTutoStep.OnUpdate();
        }
    }

    public void OnChangeHUD()
    {
        if (CanSkipNow && !isComplete)
            OnExit();
    }
    
    private void OnDestroy()
    {
        OnChangeHUD();
    }
}