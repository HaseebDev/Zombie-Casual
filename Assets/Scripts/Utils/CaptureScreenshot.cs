#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class CaptureScreenshot : MonoBehaviour
{
    [MenuItem("Screenshot/Take screenshot #S")]
    static void Screenshot()
    {
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string saveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            unixTime.ToString() + ".png");
        ScreenCapture.CaptureScreenshot(saveFile);
    }
}

#endif