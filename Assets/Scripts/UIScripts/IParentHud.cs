using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IParentHud
{
    void Initialize();

    void ShowHUD(EnumHUD _type, bool hideCurrentHUD, Action<bool> ShowComplete = null, params object[] args);
    BaseHUD HideHUD(EnumHUD _type, Action<bool> HideComplete = null, bool hideInstantly = false);

    Stack<EnumHUD> MenuStacks { get; }
    void EnqueueStacks(EnumHUD hud);
    void DequeueStacks(bool refresh);
}