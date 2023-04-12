using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Engine.Extensions;
using QuickEngine.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;

public class TutorialCanvas : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI tutorialText;
    [SerializeField] private GameObject focusPrefab;

    // public void TutorialCreateTextBox(string text, Action callBack)
    // {
    //     tutorialText.text = text;
    //     tutorialText.gameObject.SetActive(true);
    //
    //     tutorialCanvas.onClick.AddListener(() =>
    //     {
    //         callBack?.Invoke();
    //         tutorialText.gameObject.SetActive(false);
    //         tutorialCanvas.gameObject.SetActive(false);
    //     });
    //
    //     tutorialCanvas.gameObject.SetActive(true);
    //     tutorialCanvas.transform.SetAsLastSibling();
    // }

    public GameObject FocusButton(Button button, Vector3 bonusPosition)
    {
        var newFocus = Instantiate(focusPrefab, button.transform);

        newFocus.rectTransform().anchoredPosition = Vector3.zero +
                                                    new Vector3(Utils.ConvertToMatchWidthRatio(bonusPosition.x),
                                                        Utils.ConvertToMatchHeightRatio(bonusPosition.y));

        button.onClick.AddListener(() =>
        {
            // Destroy(scaling);
            // button.transform.localScale = originScale;
            Destroy(newFocus.gameObject);
        });
        
        return newFocus;
    }

    public Button PauseWithButton(Button button, Action callBack)
    {
        var cloneButton = Instantiate(button, transform);
        cloneButton.transform.position = button.transform.position;
        cloneButton.rectTransform().sizeDelta = button.rectTransform().sizeDelta;

        cloneButton.onClick.RemoveAllListeners();
        cloneButton.onClick.AddListener(() =>
        {
            callBack?.Invoke();
            button.onClick.Invoke();
            Destroy(cloneButton.gameObject);
            gameObject.SetActive(false);
        });

        var cloneTrigger = cloneButton.GetComponent<EventTrigger>();
        if (cloneTrigger != null)
        {
            var originTrigger = button.GetComponent<EventTrigger>();
            for (int i = 0; i < originTrigger.triggers.Count; i++)
            {
                cloneTrigger.triggers[i].callback = originTrigger.triggers[i].callback;
                // if (originTrigger.triggers[i].eventID == EventTriggerType.PointerUp)
                // {
                //     cloneButton.onClick.AddListener(() =>
                //     {
                //         originTrigger.triggers[temp].callback?.Invoke(new BaseEventData(EventSystem.current));
                //     });
                // }
            }
        }

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        return cloneButton;
    }
}