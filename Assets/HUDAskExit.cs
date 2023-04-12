using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HUDAskExit : BaseHUD
{
    public void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}