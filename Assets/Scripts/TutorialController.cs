using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private void Awake()
    {
        this.tutStepNumb = PlayerPrefs.GetInt("tutstep");

        //cheat skip tutorial
        PlayerPrefs.SetInt("tutstep", 1);
        return;

        if (PlayerPrefs.GetInt("tutactivestatus", 1) == 0)
        {
            return;
        }
        if (this.tutStepNumb <= this.tutSteps.Length)
        {
            this.tutSteps[this.tutStepNumb].SetActive(true);
        }
    }

    private void Start()
    {
        this.Inject();
    }
    

    public void HideCurrentStep()
    {
        this.tutSteps[this.tutStepNumb].SetActive(false);
    }

    public void PrepareState(int _step)
    {
        this.SetStep(_step);
        this.HideCurrentStep();
    }

    public void ShowStep(int _step)
    {
        this.tutSteps[_step].SetActive(true);
    }

    public void SetStep(int _step)
    {
        this.tutSteps[_step - 1].SetActive(false);
        UnityEngine.Debug.Log(string.Format("PREV STEP {0} CURRENT STEP {1} MAX STE {2}", _step - 1, _step, this.tutSteps.Length));
        if (this.tutStepNumb >= this.tutSteps.Length)
        {
            return;
        }
        this.tutSteps[_step - 1].SetActive(false);
        PlayerPrefs.SetInt("tutstep", _step);
        if (_step <= this.tutSteps.Length)
        {
            this.tutSteps[_step].SetActive(true);
        }
        else
        {
            this.tutSteps[_step - 1].SetActive(true);
        }
        this.tutStepNumb = _step;
    }

    public int tutStepNumb;

    [SerializeField]
    private GameObject[] tutSteps;

    [SerializeField]
    private GameObject tapTooltip;



    [SerializeField]
    public T1TFinger t1TFinger;

    [SerializeField]
    public t1TSprite t1TSprite;
}
