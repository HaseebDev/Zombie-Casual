using System;
using System.Collections;
using System.Collections.Generic;
using QuickType.Shop;
using TMPro;
using UnityEngine;

public class LoadIAPButton : MonoBehaviour
{
    public GameObject loadingIcon;
    public GameObject source;

    private TMP_Text _unlockText;
    private string _priceStr;
    private string _productID;
    
    private void StartLoad()
    {
        loadingIcon.SetActive(true);
        source.SetActive(false);
    }

    public void CompleteLoad()
    {
        loadingIcon.SetActive(false);
        source.SetActive(true);
    }


    public void StartLoadCost(string priceStr, string productID, TMP_Text unlockText)
    {
        _priceStr = priceStr;
        _productID = productID;
        _unlockText = unlockText;
        
        if (priceStr == GameConstant.ERROR_IAP_COST)
        {
            StartLoad();
                    
            IAPManager.instance.GetProductPrice(productID, s =>
            {
                if (gameObject == null)
                    return;
                
                if (s != GameConstant.ERROR_IAP_COST)
                {
                    unlockText.text = s;
                    CompleteLoad();
                }
            });
        }
        else
        {
            CompleteLoad();
        }
    }
}
