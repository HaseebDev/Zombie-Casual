using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CurrencyBar : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _holder;
    [SerializeField] public List<CurrencyType> _defaultCurrencys;
    [SerializeField] private CurrencyBarItem _currencyBarItemPrefab;

    private Dictionary<CurrencyType, CurrencyBarItem> _dictCurrencyItem;
    private CurrencyModels _currencyModels;
    public static CurrencyBar Instance;
    public Vector2 defaultSize = new Vector2(219.5f, 60);
    public Vector2 bigSize = new Vector2(250f, 80);


    private CurrencyType[] showingCurrency;

    private void Awake()
    {
        Instance = this;

        _currencyModels = CurrencyModels.instance;
        _currencyModels.Load();
        _currencyModels.AddCallback(OnAddCurrency);

        LoadCurrency();
        _holder.cellSize = bigSize;
    }

    //
    // private void Start()
    // {
    //   
    //     // EventSystemServiceStatic.AddListener(this, EVENT_NAME.UPDATE_CURRENCY,
    //     //     new Action<CurrencyType>(UpdateCurrencyText));
    // }

    private void OnDestroy()
    {
        _currencyModels.RemoveCallback(OnAddCurrency);
    }


    private void LoadCurrency()
    {
        if (_dictCurrencyItem == null)
        {
            _dictCurrencyItem = new Dictionary<CurrencyType, CurrencyBarItem>();

            foreach (CurrencyType currencyType in (CurrencyType[]) Enum.GetValues(typeof(CurrencyType)))
            {
                if (currencyType == CurrencyType.NONE)
                    continue;

                var currencyItem = Instantiate(_currencyBarItemPrefab, _holder.transform);
                currencyItem.name = currencyType.ToString();

                currencyItem.Load(currencyType, AddCurrency);
                currencyItem.UpdateText(CurrencyModels.instance.GetValueFromEnum(currencyType));
                _dictCurrencyItem.Add(currencyType, currencyItem);
            }

            ResetDefaultUI();
        }
    }

    private void ShowFull()
    {
        LoadCurrency();
        gameObject.SetActive(true);

        foreach (var item in _dictCurrencyItem)
        {
            if (item.Key != CurrencyType.PILL && item.Key != CurrencyType.TOKEN)
                item.Value.gameObject.SetActive(true);
        }
    }

    public void HideAll()
    {
        gameObject.SetActive(false);
    }

    public void ShowAll()
    {
        gameObject.SetActive(true);
    }

    public void Hide(CurrencyType type)
    {
        _dictCurrencyItem[type].gameObject.SetActive(false);
    }

    public void ResetDefaultUI()
    {
        foreach (var item in _dictCurrencyItem)
        {
            item.Value.gameObject.SetActive(_defaultCurrencys.Contains(item.Key));
        }
    }

    public void AddCurrency(CurrencyType type)
    {
        // int value = 100000;
        ShopType shopType = ShopType.GOLD;


        switch (type)
        {
            case CurrencyType.GOLD:
                shopType = ShopType.GOLD;
                // _currencyModels.Golds += value;
                break;
            case CurrencyType.DIAMOND:
                shopType = ShopType.DIAMOND;
                // _currencyModels.Diamonds += value;
                break;
            case CurrencyType.TOKEN:
                // _currencyModels.Tokens += value;
                break;
            case CurrencyType.PILL:
                shopType = ShopType.POTION;
                // _currencyModels.Pills += value;
                break;
            case CurrencyType.WEAPON_SCROLL:
                shopType = ShopType.WEAPON_COIN;
                // _currencyModels.WeaponScrolls += value;
                break;
            case CurrencyType.ARMOUR_SCROLL:
                // _currencyModels.ArmourScrolls += value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        Action callback = () => HUDShop.Instance.SnapToShopType(shopType);
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_SHOP, true, null, callback);
    }


    private void OnAddCurrency(CurrencyType type, long totalValue)
    {
        _dictCurrencyItem[type].UpdateText(totalValue);
    }

    public void OnShowHUD(EnumHUD hud)
    {
        _holder.cellSize = defaultSize;

        switch (hud)
        {
            case EnumHUD.NONE:
                break;
            case EnumHUD.HUD_LOADING_GAME:
                break;
            case EnumHUD.HUD_LOADING:
                break;
            case EnumHUD.HUD_IDLE_OFFLINE_REWARD:
                break;
            case EnumHUD.HUD_FUSION_RESULT:
            case EnumHUD.HUD_SHARD_CHAR:
            case EnumHUD.HUD_STAR_REWARD:
                HideAll();
                break;
            case EnumHUD.HUD_SHOP:
                ShowCurrencys(CurrencyType.GOLD, CurrencyType.DIAMOND, CurrencyType.WEAPON_SCROLL, CurrencyType.PILL);
                break;
            case EnumHUD.HUD_EQUIPMENT:
                ShowCurrencys(CurrencyType.GOLD, CurrencyType.WEAPON_SCROLL);
                break;
            case EnumHUD.HUD_TALENT:
                ShowCurrencys(CurrencyType.GOLD, CurrencyType.DIAMOND, CurrencyType.PILL);
                break;
            case EnumHUD.HUD_HOME:
                ShowCurrencys(CurrencyType.GOLD, CurrencyType.DIAMOND);
                _holder.cellSize = bigSize;
                break;
            case EnumHUD.HUD_HERO:
                ShowCurrencys(CurrencyType.GOLD, CurrencyType.DIAMOND);
                break;
            case EnumHUD.HUD_FUSION:
                ShowCurrencys(CurrencyType.GOLD);
                break;
        }
    }

    public void ShowCurrencys(params CurrencyType[] types)
    {
        gameObject.SetActive(true);
        showingCurrency = types;

        foreach (var item in _dictCurrencyItem)
        {
            if (item.Value != null)
                item.Value.gameObject.SetActive(types.Contains(item.Key));
        }
    }

    public Transform GetCurrencyTransform(CurrencyType currencyType)
    {
        return _dictCurrencyItem[currencyType].transform;
    }
}