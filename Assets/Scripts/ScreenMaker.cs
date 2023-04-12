using System;
using UnityEngine;

public class ScreenMaker : MonoBehaviour
{
	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown("s"))
		{
			this.i++;
			ScreenCapture.CaptureScreenshot("Screenshot" + this.i.ToString() + ".png");
			UnityEngine.Debug.Log("ScreenShotDone");
		}
	}

	private int i;
}
