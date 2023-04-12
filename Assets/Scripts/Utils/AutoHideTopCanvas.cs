using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHideTopCanvas : MonoBehaviour
{
    public bool HideTab = true;
    public bool HideCurrencyBar = true;

    private void OnEnable()
    {
        if (HideTab)
            MainMenuCanvas.instance?.ShowTab(false);
        if (HideCurrencyBar)
            MainMenuCanvas.instance?.ShowCurrency(false);
    }

    private void OnDisable()
    {
        if (HideTab)
            MainMenuCanvas.instance?.ShowTab(true);
        if (HideCurrencyBar)
            MainMenuCanvas.instance?.ShowCurrency(true);
    }
}