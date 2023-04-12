using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] private Image _skillIcon;
    [SerializeField] private Image _skillBg;
    [SerializeField] private Image _bg;

    [SerializeField] private Image _lock;
    [SerializeField] private Image _info;

    [SerializeField] private Color _lockColor;
    private bool _isLock = false;
    public bool IsLock => _isLock;

    private int _unlockRank = 0;
    public int UnlockRank => _unlockRank;
    public string SkillID;

    private Action<SkillUI> _onClick;

    public void SetLock(bool isLock, int unlockRank = 0)
    {
        _lock.gameObject.SetActive(isLock);
        _info.gameObject.SetActive(!isLock);

        Color color = isLock ? _lockColor : Color.white;
        _skillIcon.color = color;
        _bg.color = color;
        _skillBg.color = color;

        _isLock = isLock;
        _unlockRank = unlockRank;
    }

    public void Load(string skillID)
    {
        SkillID = skillID;
        var fullInfo = ResourceManager.instance.GetFullUltimateSprite(skillID);
        if (fullInfo != null)
        {
            ResourceManager.instance.GetSprite(fullInfo.spriteAddress, s =>
            {
                _skillIcon.sprite = s;
            });

            ResourceManager.instance.GetSprite(fullInfo.bgAddress, s =>
            {
                if ( _skillBg != null && _skillBg.gameObject != null)
                {
                    _skillBg.gameObject.SetActive(s != null);
                    _skillBg.sprite = s;  
                }
            }); 
        }
        
        var skillDesign = DesignHelper.GetSkillDesign(skillID);
    }

    public void SetOnClickCallback(Action<SkillUI> callback)
    {
        _onClick = callback;
    }

    public void OnButtonClick()
    {
        _onClick?.Invoke(this);
    }
}
