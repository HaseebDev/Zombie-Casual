using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseState : MonoBehaviour, ITutorialStep
{
    public Action onEnter;
    public Action onExit;
    public Action onUpdate;

    protected bool isExited = false;

    public virtual void OnEnter()
    {
        isExited = false;
        onEnter?.Invoke();
    }

    public virtual void OnExit()
    {
        if (isExited)
            return;

        onExit?.Invoke();
        isExited = true;
    }

    public virtual void OnUpdate()
    {
        onUpdate?.Invoke();
    }
}