using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialStep
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
}