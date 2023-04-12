using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClickTutorialChangeWeaponType : ClickTutorialType
{
    protected override void BindListener(TutorialButtonID buttonObject)
    {
        targetButton = buttonObject.GetComponent<Button>();
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(OnExit);

            Image upgradeImg = buttonObject.GetComponentInParent<WeaponSlot>()._upgradeButton.GetComponent<Image>();
            upgradeImg.raycastTarget = false;                
            Debug.LogError(upgradeImg.name);
                
            targetButton.onClick.AddListener(() =>
            {
                if(buttonObject != null && buttonObject.focusArrow != null)
                    buttonObject.focusArrow.SetActive(false);
                upgradeImg.raycastTarget = true;                
            });
            
            
            if (buttonObject.focusArrow != null)
            {
                buttonObject.focusArrow.SetActive(true);
            }
            else
            {
                // Debug.LogError("THE HELL");

                buttonObject.focusArrow =
                    MasterCanvas.CurrentMasterCanvas.tutorialCanvas.FocusButton(targetButton, bonusPosition);
            }

            buttonObject.OnActive();

            IsFound = true;
        }
    }
}
