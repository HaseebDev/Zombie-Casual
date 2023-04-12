using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using com.datld.data;
using DG.Tweening;
using MEC;
using UnityExtensions;

public class HUDShardChar : BaseHUD
{
    public StarHelper _StarHelper;
    public Slider _sliderProgress;
    public TextMeshProUGUI txtProgressValue;
    public Image imgHero;
    public TextMeshProUGUI txtUpgradeBonus;
    public Transform shakeTransform;

    private HeroData _data;
    private int TargetShard;
    private bool IsMaxRank;

    public Button _btnRankUp;

    [SerializeField] private List<Transform> scaleAfterFusion;
    [SerializeField] private FusionResultAttributeUI _maxLevel;
    [SerializeField] private FusionResultAttributeUI _baseDamage;
    [SerializeField] private GameObject after;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private UIShiny _upgradeSuccessShiny;
    [SerializeField] private UIShiny _upgradbleShiny;
    [Header("Char Preview Prefab")] public ShopHeroPreview _heroPreviewPrefab;
    private ShopHeroPreview _heroPreview;
    [SerializeField] private RawImage _heroPreviewRawImg;

    public StarHelper _StarHelperAfter;


    public string GetPassiveSkillText(HeroData data)
    {
        string bonus = "";
        var nextRankDs = DesignHelper.GetHeroDesign(_data.UniqueID, _data.Rank + 1, data.GetHeroLevel());
        if (nextRankDs != null)
        {
            bonus = LocalizeController.GetText(LOCALIZE_ID_PREF.BOOST_HERO_BASE_DMG,
                nextRankDs
                    .RarityDmgMultiply); // $"Boost <color=green><size=80>{nextRankDs.RarityDmgMultiply}%</size></color> ATK to your hero base power";
        }

        return bonus;
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        _data = (HeroData) (args[0]);
        after.SetActive(false);

        txtUpgradeBonus.text = GetPassiveSkillText(_data);
        ResourceManager.instance.GetHeroAvatar(_data.UniqueID, imgHero);
        if (_data != null)
        {
            var maxRank = DesignHelper.GetHeroMaxRank(_data.UniqueID);
            IsMaxRank = _data.Rank >= maxRank;
            _sliderProgress.interactable = false;
            if (!IsMaxRank)
            {
                var nextRankDs = DesignHelper.GetHeroDesign(_data.UniqueID, _data.Rank + 1, _data.GetHeroLevel());
                _sliderProgress.maxValue = nextRankDs.ShardRequire;
                _sliderProgress.value = _data.CurrentShard;
                txtProgressValue.text = $"{_data.CurrentShard}/{nextRankDs.ShardRequire}";
            }
            else
            {
                _sliderProgress.maxValue = 100;
                _sliderProgress.value = 100;
                txtProgressValue.text = LOCALIZE_ID_PREF.MAX.AsLocalizeString();
            }

            _StarHelper.Load(_data.Rank);
        }

        _upgradbleShiny.enabled = IsRankUpReady();
        GamePlayController.instance?.SetPauseGameplay(true);
    }

    public void OnButtonCheatShard()
    {
        if (_data.AddShardHero(5))
        {
            ResetView();
        }
    }

    public void ResetView()
    {
        var maxRank = DesignHelper.GetHeroMaxRank(_data.UniqueID);
        IsMaxRank = _data.Rank >= maxRank;
        var nextRankDs = DesignHelper.GetHeroDesign(_data.UniqueID, _data.Rank + 1, _data.GetHeroLevel());
        _sliderProgress.maxValue = nextRankDs.ShardRequire;
        _sliderProgress.value = _data.CurrentShard;
        txtProgressValue.text = $"{_data.CurrentShard}/{nextRankDs.ShardRequire}";
        _StarHelper.Load(_data.Rank);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_HERO_INFO_VIEW);
    }

    private bool IsRankUpReady()
    {
        var nextRankDs = DesignHelper.GetHeroDesign(_data.UniqueID, _data.Rank + 1, _data.GetHeroLevel());
        if (_data.CurrentShard >= nextRankDs.ShardRequire)
            return true;

        return false;
    }

    private void InitHeroPreview()
    {
        _heroPreview = FindObjectOfType<ShopHeroPreview>();
            
        if (_heroPreview == null)
        {
            _heroPreview = GameObject.Instantiate(_heroPreviewPrefab, null);
            _heroPreview.transform.position = Vector3.one * 999f;
            _heroPreview.gameObject.SetActiveIfNot(true);
        }
                
        _heroPreview.ResetRenderTexture();
        _heroPreview.ResetAnimPreview(_data);
        
        _heroPreviewRawImg.texture = _heroPreview._renderTexture;
        _heroPreviewRawImg.SetAllDirty();
    }

    public void OnButtonRankUp()
    {
        if (IsRankUpReady())
        {
            if (_data.RankUpHero())
            {
                EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_PROMOTE_HERO, _data);
                //ResetView();
                //_StarHelper.PlayAnimSingle(_data.Rank - 1);

                var oldDesign = DesignHelper.GetHeroDesign(_data.UniqueID, _data.Rank - 1, _data.GetHeroLevel());
                var currentDesign = DesignHelper.GetHeroDesign(_data.UniqueID, _data.Rank, _data.GetHeroLevel());
                after.SetActive(true);
                
                InitHeroPreview(); 

                name.text = currentDesign.Name.AsLocalizeString();
                _StarHelperAfter.Load(_data.Rank);
                
                _maxLevel.Load(LOCALIZE_ID_PREF.MAX_LV.AsLocalizeString(), oldDesign.MaxLevel, currentDesign.MaxLevel);
                _baseDamage.Load("Base damage", oldDesign.RarityDmgMultiply, currentDesign.RarityDmgMultiply, true);

                
                Timing.RunCoroutine(Coroutine());
            }
        }
        else
        {
            TopLayerCanvas.instance.ShowFloatingTextNotify(LOCALIZE_ID_PREF.NOT_ENOUGH_SHARD.AsLocalizeString());
        }
    }

    private IEnumerator<float> Coroutine()
    {
        foreach (var VARIABLE in scaleAfterFusion)
        {
            VARIABLE.transform.localScale = Vector3.zero;
        }
        
        yield return Timing.WaitForSeconds(0.5f);
        _StarHelperAfter.PlayAnimSingle(_data.Rank - 1);
        yield return Timing.WaitForSeconds(0.25f);

        shakeTransform.DOKill();
        shakeTransform.rectTransform().anchoredPosition = Vector2.zero;
        shakeTransform.DOShakePosition(0.2f, 40, 50, randomness: 90).SetEase(Ease.OutQuad).SetLoops(1);
        
        
        
        yield return Timing.WaitForSeconds(0.5f);
        _upgradeSuccessShiny.effectPlayer.duration = 0.25f;
        _upgradeSuccessShiny.Play();
        yield return Timing.WaitForSeconds(0.25f);
        _upgradeSuccessShiny.Play();
        yield return Timing.WaitForSeconds(0.25f);

        foreach (var VARIABLE in scaleAfterFusion)
        {
            VARIABLE.transform.DOScale(1, 0.5f);
            yield return Timing.WaitForSeconds(0.5f);
        }
    }

    public override void OnButtonBack()
    {
        if (after.activeInHierarchy)
        {
            HideInstantly();
            _parentHUD.DequeueStacks(refreshLastLayer);
        }
        else
        {
            GetComponent<MyPopup>().Hide();
            Hide();
        }
        
        GamePlayController.instance?.SetPauseGameplay(false);

    }
}