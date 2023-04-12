using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopCategoryList : MonoBehaviour
{
    private void Awake()
    {
        ShopCategoryManager shopCategoryManager = this.shopCategoryManager;
        shopCategoryManager.OnCategoryChanged = (Action<ShopCategory>)Delegate.Combine(shopCategoryManager.OnCategoryChanged, new Action<ShopCategory>(this.ShopCategoryManager_OnCategoryChanged));
    }

    private void ShopCategoryManager_OnCategoryChanged(ShopCategory newShopCategory)
    {
        Timing.RunCoroutine(ShopCategoryManager_OnCategoryChangedCoroutine(newShopCategory));
    }
    
    IEnumerator<float> ShopCategoryManager_OnCategoryChangedCoroutine(ShopCategory newShopCategory)
    {
        if (newShopCategory == this.listShopCategory)
        {
            yield return Timing.WaitForOneFrame;
            gameObject.SetActiveIfNot(true);

        }
        else
        {
            gameObject.SetActiveIfNot(false);
        }
    }

    private void OnDestroy()
    {
        ShopCategoryManager shopCategoryManager = this.shopCategoryManager;
        shopCategoryManager.OnCategoryChanged = (Action<ShopCategory>)Delegate.Remove(shopCategoryManager.OnCategoryChanged, new Action<ShopCategory>(this.ShopCategoryManager_OnCategoryChanged));
    }

    [SerializeField]
    private ShopCategoryManager shopCategoryManager;

    [SerializeField]
    private ShopCategory listShopCategory;
}
