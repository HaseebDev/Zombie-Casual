using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    [SerializeField] private string _loadingText = "Grrr";
    [SerializeField] private string _dotText = ".";
    private int currentDotsCount = 0;
    private TextMeshProUGUI walkingText;

    private void Start()
    {
        walkingText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateLoadingText());
    }

    private IEnumerator UpdateLoadingText()
    {
        while (true)
        {
            currentDotsCount++;
            if (currentDotsCount == 4)
                currentDotsCount = 1;

            string dots = "";
            for (int i = 0; i < currentDotsCount; i++)
            {
                dots = dots + " .";
            }

            walkingText.text = _loadingText + dots;
            yield return new WaitForSeconds(0.25f);
        }
    }
}