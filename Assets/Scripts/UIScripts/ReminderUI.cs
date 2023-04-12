using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions.Localization;

public class ReminderUI : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _quantityText;

    public void Load(int quantity, bool autoHide = true)
    {
        if (quantity > 9)
            _quantityText.text = "9+";
        else
        {
            _quantityText.text = quantity.ToString();
        }

        gameObject.SetActive(quantity != 0);
    }

    // public void LoadWithoutText()
    // {
    //     _quantityText.gameObject.SetActive(false);
    //     gameObject.SetActive(true);
    // }
    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}