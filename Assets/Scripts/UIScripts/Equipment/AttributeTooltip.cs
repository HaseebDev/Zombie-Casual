using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class AttributeTooltip : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI content;

    public void UpdateText(string s)
    {
        content.text = s;
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            gameObject.SetActive(false);
        }
    }
}
