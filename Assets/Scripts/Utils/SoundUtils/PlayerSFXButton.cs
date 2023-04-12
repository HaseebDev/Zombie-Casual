using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayerSFXButton : MonoBehaviour
{
    public SFX_ENUM SoundTap = SFX_ENUM.SFX_BUTTON;

    private Button button;
    private void Awake()
    {
        button = this.GetComponent<Button>();
    }
    private void Start()
    {
        if (!button)
            button = this.GetComponent<Button>();

        button?.onClick.AddListener(PlaySound);
    }

    public void PlaySound()
    {
        AudioSystem.instance.PlaySFX(SoundTap);
    }
}
