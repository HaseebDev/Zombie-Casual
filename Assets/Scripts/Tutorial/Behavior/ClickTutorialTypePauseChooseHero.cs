using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public class ClickTutorialTypePauseChooseHero : ClickTutorialTypePause
{
    protected override void BindListener(TutorialButtonID buttonObject)
    {
        var button = buttonObject.GetComponent<Button>();

        if (button != null)
        {
            originButton = button;
            GamePlayController.instance?.SetPauseGameplay(true);

            cloneButton = MasterCanvas.CurrentMasterCanvas.tutorialCanvas.PauseWithButton(button,
                () =>
                {
                    OnExit();
                    GamePlayController.instance?.SetPauseGameplay(false);
                });
            
            cloneButton.GetComponentInParent<ChangeTeamHeroButton>().LoadHeroSprite("HERO_2");
            
            var tutorialBtnId = cloneButton.GetComponent<TutorialButtonID>();
            var focusArrow = tutorialBtnId.focusArrow;
            if (focusArrow != null)
            {
                focusArrow.SetActive(true);
            }
            else
            {
                tutorialBtnId.focusArrow =
                    MasterCanvas.CurrentMasterCanvas.tutorialCanvas.FocusButton(cloneButton, bonusPosition);
            }

            tutorialBtnId.OnActive();

            IsFound = true;
        }
    }
}