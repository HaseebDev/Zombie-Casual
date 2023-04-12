using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClickTutorialTypeAddHero : ClickTutorialTypePause
{
    public Button prefab;

    protected override void BindListener(TutorialButtonID buttonObject)
    {
        var button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            originButton = button;

            cloneButton = MasterCanvas.CurrentMasterCanvas.tutorialCanvas.PauseWithButton(prefab, () =>
            {
                button.onClick.Invoke();
                OnExit();
                GamePlayController.instance?.SetPauseGameplay(false);
            });

            cloneButton.gameObject.SetActive(true);
            // cloneButton.GetComponentInParent<ChangeTeamHeroButton>()?.LoadHeroSprite();
            
            MasterCanvas.CurrentMasterCanvas.tutorialCanvas.FocusButton(cloneButton, bonusPosition);
            IsFound = true;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        GamePlayController.instance?.SetPauseGameplay(true);

        if (cloneButton != null && originButton != null)
        {
            cloneButton.transform.position =
                originButton.transform.position; //+ new Vector3(0, -10 * Utils.ScreenHeightRatio, 0);
        }
    }
}