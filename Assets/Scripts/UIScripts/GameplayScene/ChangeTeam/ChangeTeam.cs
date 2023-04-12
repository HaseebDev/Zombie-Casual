using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using UnityEngine;

public class ChangeTeam : MonoBehaviour
{
    [SerializeField] private Transform _heroHolder;
    [SerializeField] private ChangeTeamHeroButton _changeTeamHeroButtonPrefab;
    [SerializeField] private Dictionary<HeroData, ChangeTeamHeroButton> _dictHeroButtons;
    [SerializeField] private Transform _previewHolder;
    
    private List<ChangeTeamHeroPreview> _previews;
    private List<HeroData> _lastFormation;

    private void Awake()
    {
        _previews = new List<ChangeTeamHeroPreview>();
        foreach (Transform child in _previewHolder)
        {
           _previews.Add(child.GetComponent<ChangeTeamHeroPreview>()); 
        }
        
        foreach (var heroPreview in _previews)
        {
            heroPreview.SetOnRemoveCallback(OnRemoveHero);
        }

        _dictHeroButtons = new Dictionary<HeroData, ChangeTeamHeroButton>();
    }

    private void OnRemoveHero(HeroData heroData)
    {
        // _dictHeroButtons[heroData].SetIsSelected(false);
    }

    public void Load()
    {
        // LoadPreview();
        // foreach (Transform child in _heroHolder)
        // {
        //     Destroy(child.gameObject);
        // }
        //
        // _dictHeroButtons.Clear();
        //
        // var teamSlots = SaveManager.Instance.Data.GameData.TeamSlots;
        // var allHeroes = SaveManager.Instance.Data.Inventory.ListHeroData;
        // foreach (var hero in allHeroes)
        // {
        //     if (hero.IsUnlocked() && hero.UniqueID != "MANUAL_HERO")
        //     {
        //         var heroButton = Instantiate(_changeTeamHeroButtonPrefab, _heroHolder);
        //         heroButton.Load(hero);
        //         heroButton.SetOnClickCallback(OnHeroSelect);
        //         heroButton.SetIsSelected(teamSlots.Contains(hero.UniqueID));
        //         _dictHeroButtons.Add(hero, heroButton);
        //     }
        // }
    }

    private void LoadPreview()
    {
        int index = 0;
        foreach (var heroID in SaveManager.Instance.Data.GameData.TeamSlots)
        {
            if (heroID != GameConstant.NONE)
            {
                var heroData = SaveManager.Instance.Data.Inventory.ListHeroData.First(x => x.UniqueID == heroID);
                _previews[index++].Load(heroData);
            }
            else
            {
                _previews[index++].Clear();
            }
        }

        for (int i = index; i < _previews.Count; i++)
        {
            _previews[i].Clear();
        }
    }

    private void OnHeroSelect(HeroData heroData)
    {
        // foreach (var slot in _previews)
        // {
        //     if (slot.HeroData == null)
        //     {
        //         slot.Load(heroData);
        //         _dictHeroButtons[heroData].SetIsSelected(true);
        //         break;
        //     }
        // }
    }

    public void OnConfirm()
    {
        List<HeroData> formation = new List<HeroData>();
        foreach (var slot in _previews)
        {
            formation.Add(slot.HeroData);
        }

        // if (CompareWithLastFormation(formation))
        // GamePlayController.instance.gameLevel.UpdateFormation(formation);
        // InGameCanvas.instance._gamePannelView.OnUpdateFormation(formation);

        _lastFormation = formation;
    }

    private bool CompareWithLastFormation(List<HeroData> heroDatas)
    {
        if (_lastFormation == null)
            return true;

        for (int i = 0; i < heroDatas.Count; i++)
        {
            var hero1 = heroDatas[i];
            var hero2 = _lastFormation[i];

            if (hero1 == null && hero2 != null ||
                hero1 != null && hero2 == null ||
                hero1.UniqueID != hero2.UniqueID
            )
                return true;
        }

        return false;
    }
}