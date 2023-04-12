using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using SRF;
using UnityEngine;

public class ChangeTeam2 : MonoBehaviour
{
    [SerializeField] private Transform _heroHolder;
    [SerializeField] private ChangeTeamHeroButton _changeTeamHeroButtonPrefab;
    private Action<HeroData> _callBack;

    public void Show(Action<HeroData> callback)
    {
        _callBack = callback;
        gameObject.SetActive(true);
        Load();
    }

    public void Load()
    {
        var teamSlots = SaveManager.Instance.Data.GameData.TeamSlots;
        var allHeroes = SaveManager.Instance.Data.Inventory.ListHeroData;
        var availableHero = new List<ChangeTeamHeroButton>();

        _heroHolder.DestroyChildren();
        for (int i = 0; i < allHeroes.Count; i++)
        {
            var hero = allHeroes[i];
            string heroID = hero.UniqueID;
            var unlockDesign = DesignHelper.GetUnlockHeroDesignElement(heroID);

            if (hero.UniqueID != GameConstant.MANUAL_HERO && hero.UniqueID != "HERO_DEMO")
            {
                if (hero.ItemStatus != ITEM_STATUS.Choosing && unlockDesign.Available &&
                    (SaveGameHelper.GetMaxCampaignLevel() >= unlockDesign.LevelToShow || hero.ItemStatus == ITEM_STATUS.Available))
                {
                    var heroButton = Instantiate(_changeTeamHeroButtonPrefab, _heroHolder);
                    heroButton.Load(hero);
                    heroButton.SetOnClickCallback(OnHeroSelect);
                    heroButton.SetOnBackCallback(Load);
                    availableHero.Add(heroButton);
                }
            }
        }

        int startIndex = 0;
        foreach (var heroButton in availableHero)
        {
            if (heroButton.HeroData.ItemStatus == ITEM_STATUS.Available)
            {
                heroButton.transform.SetSiblingIndex(startIndex);
                startIndex++;
            }
        }
    }

    private void OnHeroSelect(HeroData heroData)
    {
        _callBack?.Invoke(heroData);
        gameObject.SetActive(false);
    }
}