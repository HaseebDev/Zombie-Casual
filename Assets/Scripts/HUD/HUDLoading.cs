using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDLoading : BaseHUD
{
    [SerializeField] private GameObject _zombieWalking;
    [SerializeField] private GameObject _loadingCircle;

    [Header("Zombie loading")]
    public RawImage zombieRawImg;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        bool showZombie = false;
        if (args != null && args.Length > 0 && args[0] != null)
            showZombie = (bool)args[0];

        _zombieWalking.gameObject.SetActive(showZombie);
        _loadingCircle.gameObject.SetActive(!showZombie);
    }

    public override void Show(Action<bool> showComplete = null, bool addStack = true)
    {
        base.Show(showComplete, addStack);

        if (GameMaster.instance != null)
            GameMaster.instance.zombieWalking.ResetZombieWalking();
        else
            Debug.LogError("Game master is null!!");

        if (zombieRawImg != null)
        {
            zombieRawImg.texture = GameMaster.instance.zombieWalking._renderTexture;
            zombieRawImg.SetAllDirty();
        }
        else
        {
            Debug.LogError("zombieRawImg is null!!!");
        }

    }

    public override void OnButtonBack()
    {
        BackButtonManager.Instance.ShowCanGoBackText(true);
    }
}