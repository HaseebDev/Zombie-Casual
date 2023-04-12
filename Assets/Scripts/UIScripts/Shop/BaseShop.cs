using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using QuickType.Shop;
using UnityEngine;

public enum ShopType
{
    DIAMOND,
    GOLD,
    WEAPON_COIN,
    POTION,
    CHEST,
    FREE_STUFF
}

public class BaseShop : MonoBehaviour
{
    [SerializeField] protected ShopType ShopType;
    [SerializeField] protected Transform _shopItemHolder;
    [SerializeField] protected ShopItemUI _shopItemUiPrefab;
    [SerializeField] protected Sprite _bgSprite;

    protected List<ShopItemUI> _shopItemUis;

    public virtual void ResetLayer()
    {
        if (_shopItemUis != null && _shopItemUis.Count > 0)
        {   
            int index = 0;
            foreach (var shopDesignElement in DesignManager.instance.shopDesign.ShopDesignElement)
            {
                if (shopDesignElement.Id.Contains(ShopType.ToString()))
                {
                    shopDesignElement.ResetData();
                    _shopItemUis[index++].Load(shopDesignElement);
                }
            }
        }
    }

    protected virtual IEnumerator<float> LoadShop()
    {
        ClearOldItems();
        float scale = 1;

        foreach (var shopDesignElement in DesignManager.instance.shopDesign.ShopDesignElement)
        {
            if (shopDesignElement.Id.Contains(ShopType.ToString()))
            {
                var shopItem = Instantiate(_shopItemUiPrefab, _shopItemHolder);
                shopDesignElement.ResetData();
                shopItem.Load(shopDesignElement);
                shopItem.SetOnPurchaseCallback(OnPurchase);
                shopItem.SetScale(scale);
                if (_bgSprite != null)
                    shopItem.SetBg(_bgSprite);
                scale += 0.1f;

                _shopItemUis.Add(shopItem);
                yield return Timing.WaitForOneFrame;
            }
        }

    }

    public virtual CoroutineHandle Load()
    {
        return Timing.RunCoroutine(LoadShop());
    }

    public void ClearOldItems()
    {
        _shopItemUis = new List<ShopItemUI>();

        foreach (Transform child in _shopItemHolder)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddReward(List<RewardData> rewardDatas)
    {
        foreach (var rewardData in rewardDatas)
        {
            SaveManager.Instance.Data.AddReward(rewardData);
        }
    }

    public virtual void OnPurchase(CostData costData, List<RewardData> rewardDatas, ShopItemUI shopItemUi,
        Action<bool> callback = null)
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {
               
        //    }

        //    callback?.Invoke(reached);
        //});

        AddReward(rewardDatas);

        // foreach (var rw in rewardDatas)
        // {
        //     Debug.LogError(rw._type);
        // }
        // MasterCanvas.CurrentMasterCanvas.ShowRewardSimpleHUD(rewardDatas, false, true);
        MasterCanvas.CurrentMasterCanvas.SpawnCollectAnim(rewardDatas, shopItemUi.transform.position, 0, 5,
            100);
        callback?.Invoke(true);
    }
}