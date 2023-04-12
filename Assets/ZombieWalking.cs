using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWalking : MonoBehaviour
{
    public RenderTexture _renderTexture;
    public Camera _walkingCamera;

    public void ResetZombieWalking()
    {
        if (_renderTexture != null)
        {
            _walkingCamera.targetTexture = _renderTexture;
        }
        else
        {
            Debug.LogError("_renderTexture is null!!!");
        }
    }

}
