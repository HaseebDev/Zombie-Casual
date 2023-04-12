using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTutorialType : BaseTutorialType
{
    public string context = "";

    public override void OnEnter()
    {
        base.OnEnter();
        // MasterCanvas.CurrentMasterCanvas.tutorialCanvas.TutorialCreateTextBox(context,OnExit);
    }
}