using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using DG.Tweening;
using Ez.Pooly;
using QuickEngine.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class HUDHero : BaseHUD
{
    [SerializeField] private Transform _heroButtonHolder;
    [SerializeField] private HeroInfoUIHelper _heroInfoUiHelper;
    [SerializeField] private ScalingUI _shardButton;

    private HeroData _heroData;

    private Dictionary<string, HeroButtonUI> _dictHeroButton = new Dictionary<string, HeroButtonUI>();

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        _heroInfoUiHelper.onUpgrade = OnUpgrade;
        _heroInfoUiHelper.onUnlock = OnUnlock;
    }

    public override void ResetLayers()
    {
        base.ResetLayers();
        _heroInfoUiHelper.ResetLayer();
        LoadHeroButton();
    }

    private void LoadHeroButton()
    {
        _heroButtonHolder.DespawnAllChild();
        // foreach (Transform child in _heroButtonHolder.transform)
        // {
        // Destroy(child.gameObject);
        // }

        //_dictHeroButton = new Dictionary<string, HeroButtonUI>();
        var dictHero = DesignManager.instance.DictHeroDesign;
        var allHeroes = SaveManager.Instance.Data.Inventory.ListHeroData;
        var toggleGroup = _heroButtonHolder.GetComponent<ToggleGroup>();
        var availableHero = new List<HeroButtonUI>();

        HeroButtonUI firstHero = null;
        for (int i = 0; i < allHeroes.Count; i++)
        {
            var hero = allHeroes[i];
            string heroID = hero.UniqueID;
            if (heroID != GameConstant.MANUAL_HERO && heroID != "HERO_DEMO")
            {
                var unlockDesign = DesignHelper.GetUnlockHeroDesignElement(heroID);
                if (hero.ItemStatus == ITEM_STATUS.Available || SaveGameHelper.GetMaxCampaignLevel() >= unlockDesign.LevelToShow)
                {
                    if (hero.ItemStatus == ITEM_STATUS.Available || hero.ItemStatus == ITEM_STATUS.Choosing ||
                        unlockDesign.Available)
                    {
                        var newHeroButton = Pooly.Spawn<HeroButtonUI>(POOLY_PREF.HERO_BUTTON_UI, Vector3.zero,
                            Quaternion.identity,
                            _heroButtonHolder); // Instantiate(_heroButtonPrefab, _heroButtonHolder);
                        newHeroButton.transform.localScale = Vector3.one;
                        newHeroButton.LoadReminderUi(0);
                        newHeroButton.UpdateToggleGroup(toggleGroup);
                        newHeroButton.Load(hero, hero.IsUnlocked());
                        newHeroButton.onSelect = OnSelectHero;

                        if (firstHero == null)
                        {
                            firstHero = newHeroButton;
                        }
                        // if (hero.ItemStatus == ITEM_STATUS.Available || hero.ItemStatus == ITEM_STATUS.Choosing)
                        // {
                        //     newHeroButton.transform.SetAsFirstSibling();
                        //     firstHero = newHeroButton;
                        // }

                        _dictHeroButton.AddOrUpdate(heroID, newHeroButton);
                        availableHero.Add(newHeroButton);
                    }
                }
            }
        }

        if(_heroData == null)
            firstHero.Select();
        else
        {
            _dictHeroButton[_heroData.UniqueID].Select();
        }

        int startIndex = 0;
        foreach (var availableHeroButtonUi in availableHero)
        {
            ITEM_STATUS heroStatus = availableHeroButtonUi.HeroData.ItemStatus;

            if (heroStatus == ITEM_STATUS.Available || heroStatus == ITEM_STATUS.Choosing)
            {
                availableHeroButtonUi.transform.SetSiblingIndex(startIndex);
                startIndex++;
            }
        }


        var newHero = ReminderManager.HasNewHero();
        if (newHero.Item1)
        {
            foreach (var heroData in newHero.Item2)
            {
                if (heroData.UniqueID != GameConstant.MANUAL_HERO)
                {
                    int newQuantity = heroData.HasEnoughShardToUpRank() ? 2 : 1;
                    _dictHeroButton.TryGetValue(heroData.UniqueID, out var heroButton);
                    heroButton?.LoadReminderUi(newQuantity);
                    // _dictHeroButton[heroData.UniqueID].LoadReminderUi(newQuantity);
                }
            }

            ReminderManager.SaveCurrentHeroState();
        }
    }

    private void OnSelectHero(HeroData heroData)
    {
        LoadHeroInfo(heroData);
        int newQuantity = heroData.HasEnoughShardToUpRank() ? 1 : 0;
        _dictHeroButton[heroData.UniqueID].LoadReminderUi(newQuantity);
    }

    private void LoadHeroInfo(HeroData heroData)
    {
        _heroData = heroData;
        _heroInfoUiHelper.Load(_heroData);

        if (heroData.HasEnoughShardToUpRank())
        {
            _shardButton.StartScaling(Color.red);
        }
        else
        {
            _shardButton.Stop();
        }

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_HERO_PREVIEW, heroData);
    }

    private void OnUpgrade(HeroData heroData)
    {
        _dictHeroButton[heroData.UniqueID].Load(heroData, true);
    }

    private void OnUnlock(HeroData heroData)
    {
        _dictHeroButton[heroData.UniqueID].Load(heroData, true);
    }

    // private void Update()
    // {
    //     if (_heroData != null)
    //     {
    //         //Logwin.Log($"[Shard] {_heroData.UniqueID}", $"shard:{_heroData.CurrentShard} - Rank: {_heroData.Rank}");
    //     }
    // }
}