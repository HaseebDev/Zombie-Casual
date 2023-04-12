using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class HUDNotify : BaseHUD
{
    [SerializeField] private LocalizedTMPTextUI _content;
    [SerializeField] private GameObject _title;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        UpdateText((string) args[0]);

        bool showTitle = true; // (bool) args[1];

        if (args.Length > 1)
        {
            showTitle = (bool) args[1];
        }

        _title.SetActive(showTitle);
    }

    public override void Awake()
    {
        base.Awake();
        refreshLastLayer = false;
    }

    public void UpdateText(string s)
    {
        _content.text = s;
    }
}