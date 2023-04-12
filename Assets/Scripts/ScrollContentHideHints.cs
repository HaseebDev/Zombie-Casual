using System;
using UnityEngine;

public class ScrollContentHideHints : MonoBehaviour
{
	private void Start()
	{
		ShopCategoryManager shopCategoryManager = this.shopCategoryManager;
		shopCategoryManager.OnCategoryChanged = (Action<ShopCategory>)Delegate.Combine(shopCategoryManager.OnCategoryChanged, new Action<ShopCategory>(this.ShopCategoryManager_OnCategoryChanged));
		this.indicatorLeft.SetActive(false);
	}

	private void ShopCategoryManager_OnCategoryChanged(ShopCategory obj)
	{
		this.CheckAndSetIndicators(0f);
	}

	public void OnScrollValueChaned(Vector2 newScrollVal)
	{
		this.CheckAndSetIndicators(newScrollVal.x);
	}

	private void CheckAndSetIndicators(float scrollXRatio)
	{
		if (scrollXRatio >= 0.3f)
		{
			this.indicatorLeft.SetActive(true);
		}
		else
		{
			this.indicatorLeft.SetActive(false);
		}
		if (scrollXRatio >= 1f)
		{
			this.indicatorRight.SetActive(false);
			return;
		}
		this.indicatorRight.SetActive(true);
	}

	[SerializeField]
	private GameObject indicatorLeft;

	[SerializeField]
	private GameObject indicatorRight;

	[SerializeField]
	private ShopCategoryManager shopCategoryManager;
}
