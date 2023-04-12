using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckButton : MonoBehaviour
{
    [SerializeField] private Color checkedColor;
    [SerializeField] private Image targetImage;
    [SerializeField] private bool isChecked = false;

    [SerializeField] private Button.ButtonClickedEvent onChecked = new Button.ButtonClickedEvent();
    [SerializeField] private Button.ButtonClickedEvent onUnChecked = new Button.ButtonClickedEvent();

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            isChecked = !isChecked;
            targetImage.color = isChecked ? checkedColor : Color.white;
            if (isChecked)
            {
                onChecked?.Invoke();
            }
            else
            {
                onUnChecked?.Invoke();
            }
        });
    }

    public void ResetCheck()
    {
        isChecked = false;
        targetImage.color = isChecked ? checkedColor : Color.white;
    }
}