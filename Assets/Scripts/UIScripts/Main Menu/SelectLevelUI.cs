using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class SelectLevelUI : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _levelText;
    [SerializeField] private StarHelper _starHelper;
    [SerializeField] private Image _lockImage;
    [SerializeField] private Button _button;

    private int _level;
    private Action<int> _onClick;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonEnterClick);
    }

    public void Load(int level, int star, bool isLock)
    {
        _level = level;
        _levelText.text = $"Level {level}";
        _starHelper.Load(star);

        _starHelper.gameObject.SetActive(!isLock);
        _lockImage.gameObject.SetActive(isLock);
        _button.interactable = !isLock;
    }

    public void SetClickCallback(Action<int> callBack)
    {
        _onClick = callBack;
    }

    private void OnButtonEnterClick()
    {
        _onClick?.Invoke(_level);
    }
}