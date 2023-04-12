using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using QuickType.IAPProducts;

[Serializable]
public class IAPItemData
{
    public IAP_TYPE type;
    public string ProductID;
    public float DefaultPrice;
}

public enum IAP_TYPE
{
    NONE,
    CONSUMABLE,
    NON_CONSUMABLE,
    SUBSCRIPTION
}



[CreateAssetMenu(fileName = "RewardResources", menuName = "ScriptableObjects/IAPManagerSO", order = 1)]
public class IAPManagerSO : ScriptableObject
{
    [Header("List IAP Products")]
    public List<IAPItemData> _listIAPProducts;

    public TextAsset _iapProductsJSON;

    [Button("Reset List IAP")]
    public void ResetListIAP()
    {
        var listIAP = IapProducts.FromJson(_iapProductsJSON.text);
        if (listIAP != null)
        {
            _listIAPProducts.Clear();

            foreach (var item in listIAP.IapProductsIapProducts)
            {
                _listIAPProducts.Add(new IAPItemData()
                {
                    type = item.IapType,
                    ProductID = item.ProductId,
                    DefaultPrice = item.Price
                });

            }
        }
    }

}
