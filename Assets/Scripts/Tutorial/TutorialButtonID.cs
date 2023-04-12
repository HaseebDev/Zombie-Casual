using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonID : MonoBehaviour
{
    public string id;
    public GameObject focusArrow;

    protected bool _active = false;

    public void OnActive()
    {
        _active = true;
    }
}