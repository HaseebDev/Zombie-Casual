using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class AutoUpdateLayoutGroup : MonoBehaviour
{
    private LayoutGroup _layoutGroup;

    private CoroutineHandle _reloadCoroutine;

    public void Reload()
    {
        if (_layoutGroup == null)
        {
            _layoutGroup = GetComponent<LayoutGroup>();
        }

        _layoutGroup.enabled = false;
        
        Timing.KillCoroutines(_reloadCoroutine);
        _reloadCoroutine = Timing.RunCoroutine(ReloadCoroutine());
    }

    private IEnumerator<float> ReloadCoroutine()
    {
        yield return Timing.WaitForOneFrame;
        _layoutGroup.enabled = true;
    }
}