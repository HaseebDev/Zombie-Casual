using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public class HUDNoInternet : BaseHUD
{
    private Action onHided;
    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        this.onHided = (Action)(args[0]);
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        onHided?.Invoke();
    }

    public void OnButtonRetry()
    {
        Hide();
     
    }
}
