using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTeamHeroPreview : MonoBehaviour
{
    [SerializeField] private Button _removeButton;
    [SerializeField] private Image _heroIcon;

    public HeroData HeroData;
    private Action<HeroData> _onRemove;

    private void Awake()
    {
        _removeButton.onClick.AddListener(() =>
        {
            _onRemove?.Invoke(HeroData);
            Clear();
        });
    }

    public void Load(HeroData heroData)
    {
        HeroData = heroData;
        _heroIcon.gameObject.SetActive(true);
        _removeButton.gameObject.SetActive(true);
        ResourceManager.instance.GetHeroAvatar(HeroData.UniqueID,_heroIcon);
        
    }

    public void Clear()
    {
        _heroIcon.gameObject.SetActive(false);
        _removeButton.gameObject.SetActive(false);
        HeroData = null;
    }

    public void SetOnRemoveCallback(Action<HeroData> callback)
    {
        _onRemove = callback;
    }
}