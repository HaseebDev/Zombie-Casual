using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotSwitcher : MonoBehaviour
{
    [SerializeField] private Transform _dotsHolder;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Toggle _dotPrefab;

    private List<Toggle> _dots;

    public void Load(int total, Action<int> callBack)
    {
        if (_dots != null && total == _dots.Count)
            return;

        _dotsHolder.DestroyAllChild();
        _dots = new List<Toggle>();

        for (int i = 0; i < total; i++)
        {
            int index = i;
            var newDot = Instantiate(_dotPrefab, _dotsHolder);
            newDot.group = _toggleGroup;
            newDot.gameObject.SetActive(true);

            newDot.onValueChanged.AddListener((v) =>
            {
                if (v)
                {
                    callBack?.Invoke(index);
                }
            });
            _dots.Add(newDot);
        }
    }

    public void TurnOnDot(int index)
    {
        _dots[index].isOn = true;
    }
}