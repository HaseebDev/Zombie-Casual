using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandlers : MonoBehaviour
{
    public Action OnShootEvent;
    public Action OnShootEvent02;
    public Action OnFinishShoot;

    public void shootEvent()
    {
        // Debug.LogError($"Shoot event fired!!! ");
        OnShootEvent?.Invoke();
    }

    public void eventFinishShoot()
    {
        OnFinishShoot?.Invoke();
    }

    public void shootEvent02()
    {
        OnShootEvent02?.Invoke();
    }
}
