using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatDisableGO : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.CHEAT_DISABLE_OBJECT, new Action<bool>(DisableGO));
    }

    private void DisableGO(bool disable)
    {
        this.gameObject.SetActiveIfNot(!disable);
    }
}
