using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExtensions;

public class ClickTutorialAddon : BaseTutorialBehavior
{
    public string addOnID;

    // private bool createdFakeButton = false;
    //
    // public override void OnUpdate()
    // {
    //     base.OnUpdate();
    //
    //     if (createdFakeButton)
    //         return;
    //
    //     var allAddonsButton = FindObjectsOfType<AddOnButtonView>().ToList();
    //     foreach (var adonBtn in allAddonsButton)
    //     {
    //         if (adonBtn.Design.SkillId == addOnID)
    //         {
    //             EventTrigger eventTrigger = adonBtn.GetComponent<EventTrigger>();
    //
    //             var button = adonBtn.gameObject.AddComponent<Button>();
    //             var buttonId = adonBtn.gameObject.AddComponent<TutorialButtonID>();
    //             buttonId.id = addOnID;
    //
    //
    //             var skillData = SaveManager.Instance.Data.GetAddOnItem(addOnID);
    //             if (skillData.ItemCount == 0)
    //             {
    //                 skillData.ItemCount += 1;
    //                 adonBtn.ResetItem();
    //                 Debug.LogError("Add ma");
    //             }
    //
    //             createdFakeButton = true;
    //             return;
    //         }
    //     }
    // } 

    public override void OnEnter()
    {
        DOVirtual.DelayedCall(0.2f, () =>
        {
            var allAddonsButton = FindObjectsOfType<AddOnButtonView>().ToList();
            foreach (var adonBtn in allAddonsButton)
            {
                if (adonBtn.Design.SkillId == addOnID)
                {
                    // EventTrigger eventTrigger = adonBtn.GetComponent<EventTrigger>();
                    // var button = adonBtn.gameObject.AddComponent<Button>();
                    var buttonId = adonBtn.gameObject.AddComponent<TutorialButtonID>();
                    buttonId.id = addOnID;


                    var skillData = SaveManager.Instance.Data.GetAddOnItem(addOnID);
                    if (skillData.ItemCount == 0)
                    {
                        skillData.ItemCount += 1;
                        adonBtn.ResetItem();
                    }
                }
            }

            base.OnEnter();
        });
    }
}