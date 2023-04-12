using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanExitHUD : BaseHUD
{
    public override void OnButtonBack()
    {
        MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_ASK_EXIT, false);
    }
}
