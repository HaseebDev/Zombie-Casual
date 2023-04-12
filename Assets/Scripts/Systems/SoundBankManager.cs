using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class SoundBankManager : BaseSystem<SoundBankManager>
{
    public Dictionary<BGM_ENUM, BGMDef> _dictBGM { get; private set; }
    public Dictionary<SFX_ENUM, SFXDef> _dictSFX { get; private set; }

    public override IEnumerator<float> InitializeCoroutineHandler()
    {
        _dictSFX = new Dictionary<SFX_ENUM, SFXDef>();
        _dictBGM = new Dictionary<BGM_ENUM, BGMDef>();

        if (ResourceManager.instance._soundBank.ListBGM != null)
        {
            foreach (var bgm in ResourceManager.instance._soundBank.ListBGM)
            {
                if (!_dictBGM.ContainsKey(bgm._bgm))
                    _dictBGM.Add(bgm._bgm, bgm);
            }
        }

        yield return Timing.WaitForOneFrame;

        if (ResourceManager.instance._soundBank.ListSFX != null)
        {
            foreach (var sfx in ResourceManager.instance._soundBank.ListSFX)
            {
                if (!_dictSFX.ContainsKey(sfx._sfx))
                    _dictSFX.Add(sfx._sfx, sfx);
            }
        }

        this.OnInitializeComplete?.Invoke(true);
    }
    
    
}
