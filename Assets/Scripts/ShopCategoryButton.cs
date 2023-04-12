using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopCategoryButton : MonoBehaviour
{
    private void Awake()
    {
        ShopCategoryManager shopCategoryManager = this.shopCategoryManager;
        shopCategoryManager.OnCategoryChanged = (Action<ShopCategory>) Delegate.Combine(
            shopCategoryManager.OnCategoryChanged,
            new Action<ShopCategory>(this.ShopCategoryManager_OnCategoryChanged));
        this.buttonObj.onClick.AddListener(new UnityAction(this.OnShopCategoryBtnClicked));
    }

    private void ShopCategoryManager_OnCategoryChanged(ShopCategory newCategory)
    {
        if (this.buttonShopCategory == newCategory)
        {
            this.ActiveButton();
            return;
        }

        this.DisableButton();
    }

    protected virtual void OnShopCategoryBtnClicked()
    {
        this.shopCategoryManager.ChangeModeBtnClicked(this.buttonShopCategory);
    }

    protected virtual void ActiveButton()
    {
        this.activeState.SetActive(true);
        this.disabledState.SetActive(false);
    }

    protected virtual void DisableButton()
    {
        this.activeState.SetActive(false);
        this.disabledState.SetActive(true);
    }

    protected virtual void OnDestroy()
    {
        ShopCategoryManager shopCategoryManager = this.shopCategoryManager;
        shopCategoryManager.OnCategoryChanged = (Action<ShopCategory>) Delegate.Remove(
            shopCategoryManager.OnCategoryChanged,
            new Action<ShopCategory>(this.ShopCategoryManager_OnCategoryChanged));
        this.buttonObj.onClick.RemoveListener(new UnityAction(this.OnShopCategoryBtnClicked));
    }

    [SerializeField] private ShopCategoryManager shopCategoryManager;

    [SerializeField] private Button buttonObj;

    [SerializeField] private GameObject activeState;

    [SerializeField] private GameObject disabledState;

    [SerializeField] private GameObject lockImg;

    [SerializeField] private ShopCategory buttonShopCategory;
}