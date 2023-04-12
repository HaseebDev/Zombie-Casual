using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;

public class BaseSystem<T> : MonoBehaviour
{
    public bool IsInited { get; protected set; }
    public Action<bool> OnInitializeComplete;

    public virtual void Initialize(params object[] pars)
    {
        IsInited = false;
    }

    public virtual void InitializeCoroutine(Action<bool> callback)
    {
        OnInitializeComplete = callback;
        Timing.RunCoroutine(InitializeCoroutineHandler());
    }

    public virtual IEnumerator<float> InitializeCoroutineHandler()
    {
        yield return Timing.WaitForOneFrame;
    }

    public virtual void UpdateSystem(float _deltaTime)
    {

    }

}
