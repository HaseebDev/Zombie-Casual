using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityExtensions;
using static AnalyticsConstant;

public class HeroSlot : MonoBehaviour
{
    [SerializeField] private Image _highlight;
    [SerializeField] private SoliderUpgradeButton _upgradeButton;
    [SerializeField] private Button _removeButton;
    [SerializeField] private Button _parentButton;
    private int _index;
    private string _heroId;
    public string HeroId => _heroId;

    private RectTransform _rectTransform;
    private Sequence _glowSequence;

    public void ShowAddHeroPanel()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        // Vector3[] objectCornersArrow = new Vector3[4];
        // var rect = _rectTransform.rect;
        // _rectTransform.GetWorldCorners(objectCornersArrow);
        // if (objectCornersArrow[0].x + rect.width / 2f < 0 || objectCornersArrow[3].x - rect.width / 2f > Screen.width)
        // {
        //     return;
        // }


        InGameCanvas.instance._gamePannelView.ShowSelectHeroPanel(OnSelectHero,
            transform.position + new Vector3(0, Utils.ConvertToMatchHeightRatio(-20f)));

        //analytics
        var currentLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
        if (currentLevel <= AnalyticsConstant.MAX_TRACKING_LEVEL)
        {
            AnalyticsManager.instance.LogEvent(ANALYTICS_ENUM.TOUCH_INGAME_HEROSLOT,
                new LogEventParam("level", currentLevel));
        }
    }

    private void OnSelectHero(HeroData heroData)
    {
        // var nextHeroSlot = InGameCanvas.instance._gamePannelView.GetNextAvailableHeroSlot();
        // if (nextHeroSlot != null)
        // {
        //     nextHeroSlot.Load(heroData.UniqueID);
        //     SaveManager.Instance.Data.GameData.TeamSlots[nextHeroSlot._index] = heroData.UniqueID;
        // }
        // else
        // {
        Load(heroData.UniqueID, _index);
        SaveManager.Instance.Data.GameData.TeamSlots[_index] = heroData.UniqueID;
        // }

        // if (CompareWithLastFormation(formation))
        heroData.ItemStatus = ITEM_STATUS.Choosing;
        GamePlayController.instance.gameLevel.UpdateFormation();
    }

    public void OnSelectCheat(string heroID)
    {
        var heroData = SaveManager.Instance.Data.GetHeroData(heroID);
        Load(heroData.UniqueID, _index);
        SaveManager.Instance.Data.GameData.TeamSlots[_index] = heroData.UniqueID;
        // }

        // if (CompareWithLastFormation(formation))
        heroData.ItemStatus = ITEM_STATUS.Choosing;
        GamePlayController.instance.gameLevel.UpdateFormation();
    }


    public void Load(string heroID)
    {
        Load(heroID, _index);
    }

    public void UpdateUltimateBtns(CharacterUltimateButton btn1, CharacterUltimateButton btn2)
    {
        _upgradeButton.UpdateUltimateBtns(btn1, btn2);
    }


    public void Load(string heroID, int index)
    {
        _index = index;
        _heroId = heroID;
        bool hasHero = HasHero();

        _upgradeButton.gameObject.SetActive(hasHero);
        _removeButton.gameObject.SetActive(hasHero);
        _parentButton.enabled = !hasHero;
        _parentButton.gameObject.SetActive(!hasHero);

        if (hasHero)
        {
            _upgradeButton.HeroID = heroID;
            _upgradeButton.InitController();

            InGameCanvas.instance._gamePannelView.ResetSoldiserButtons();
        }
    }

    public void RemoveHero()
    {
        Load(GameConstant.NONE, _index);
        int index = SaveManager.Instance.Data.GameData.TeamSlots.IndexOf(_heroId);
        SaveManager.Instance.Data.GameData.TeamSlots[_index] = GameConstant.NONE;

        // if (CompareWithLastFormation(formation))
        GamePlayController.instance.gameLevel.UpdateFormation();
    }

    public bool HasHero()
    {
        return _heroId != "" && _heroId != GameConstant.NONE;
    }

    public void LockRemoveButton(bool isLock)
    {
        if (HasHero())
            _removeButton.gameObject.SetActive(!isLock);
    }

    public void ShowHighlight(bool isShow)
    {
        _highlight.gameObject.SetActive(isShow && !HasHero());

        if (_glowSequence == null)
        {
            _glowSequence = DOTween.Sequence();
            _glowSequence.Append(_highlight.DOFade(0.2f, 0.5f));
            _glowSequence.Append(_highlight.DOFade(1f, 0.5f));
            // _glowSequence.Append(_highlight.DOFade(0.2f, 0.5f));
            _glowSequence.SetLoops(-1);
        }

        if (isShow && !HasHero())
        {
            _glowSequence.Play();
        }
        else
        {
            _glowSequence.Pause();
        }
    }

    public void UpdateHeroInfo()
    {
        _upgradeButton.ResetView();
    }
}