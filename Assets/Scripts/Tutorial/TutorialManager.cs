using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// [ExecuteInEditMode]
[Serializable]
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance = null;
    public int testPhase = 0;

    public List<TutorialPhase> tutorialPhase;
    private int _currentTutoIndex = 0;
    private bool isComplete = false;
    private EnumHUD _currentHUD;

    public int CurrentTutorialPhaseIndex => _currentTutoIndex;
    public TutorialPhase CurrentTutoPhase => tutorialPhase[_currentTutoIndex];

    private void Awake()
    {
        instance = this;
        tutorialPhase = GetComponentsInChildren<TutorialPhase>().ToList();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.Data != null);
        if (testPhase != 0)
            SaveManager.Instance.Data.TutorialData.LastTutorial = testPhase;

        StartLastTutorial();
    }

    public void StartLastTutorial()
    {
        StartTutorial(SaveManager.Instance.Data.TutorialData.LastTutorial);
    }

    private void Update()
    {
        if (!isComplete)
        {
            CurrentTutoPhase.OnUpdate();
        }
    }

    private void StopAllTutorialBeforeIndex(int index)
    {
        for (int i = 0; i < index; i++)
        {
            tutorialPhase[i].gameObject.SetActive(false);
        }
    }

    private void StartTutorial(int index)
    {
        StopAllTutorialBeforeIndex(index);

        if (index > tutorialPhase.Count - 1)
        {
            //     Debug.LogError("Complete all");
            return;
        }

        _currentTutoIndex = index;
        CurrentTutoPhase.onExit = NextTutorial;
        CurrentTutoPhase.OnEnter();
        //Debug.LogError("Start phase " + CurrentTutoPhase.gameObject.name + " , index " + index);
        SaveCompleteCurrentTutorial();
    }

    private void NextTutorial()
    {
        _currentTutoIndex++;
        SaveCompleteCurrentTutorial();

        if (_currentTutoIndex > tutorialPhase.Count - 1)
        {
            isComplete = true;
            return;
        }

        StartTutorial(_currentTutoIndex);
    }

    private void SaveCompleteCurrentTutorial()
    {
        // Debug.LogError("Save tutorial!!!!!");
        SaveManager.Instance.Data.TutorialData.LastTutorial = _currentTutoIndex;
        //SaveManager.Instance.SaveData();

        SaveManager.Instance.SetDataDirty();
    }

    public void OnShowHUD(EnumHUD hud)
    {
        if (isComplete)
            return;

        if (hud != _currentHUD)
        {
            CurrentTutoPhase.OnChangeHUD();
        }

        _currentHUD = hud;
    }

    public void ForceToThisPhase(TutorialPhase tutorialPhase1)
    {
        if (CurrentTutoPhase == tutorialPhase1)
            return;

        int index = tutorialPhase.IndexOf(tutorialPhase1);
        StartTutorial(index);
    }
}