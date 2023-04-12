using System;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using com.datld.talent;
using DG.Tweening;
using Google.Protobuf.Collections;
using QuickType.Talent;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;
using Random = System.Random;

public class HUDResearch : CanExitHUD
{
    [SerializeField] private GridLayoutGroup researchLayout;

    [SerializeField] private TalentUI _talentUiPrefab;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private LocalizedTMPTextUI _upgradeCostText;
    [SerializeField] private LocalizedTMPTextParam _nextUpgradeText;
    [SerializeField] private TalenUpgradeResult _upgradeResult;

    private RepeatedField<TalentData> _talentDatas; //  = SaveManager.Instance.Data.Inventory.ListTalentData;
    private List<TalentDesignElement> _talentDesignElements; //  = SaveManager.Instance.Data.Inventory.ListTalentData;
    private List<TalentUI> _talentUis;

    [Header("Tooltip")] [SerializeField] private TalentTooltip _tooltip;
    [SerializeField] private Transform _tooltipArrow;
    [SerializeField] private Transform _tooltipBox;

    private bool _isRandoming = false;
    private bool _isHided = false;

    private Sequence _randomSequence = null;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.LogError();

        if ((float) Screen.width / Screen.height > 0.7f)
        {
            researchLayout.transform.localScale = Vector3.one * 0.7f;
        }
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        _isHided = false;
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        _isHided = true;
    }

    public override void Init()
    {
        if (!isInit)
        {
            _talentDesignElements = DesignManager.instance.talentDesign.TalentDesignElements;
            _talentDatas = SaveManager.Instance.Data.Inventory.ListTalentData;
            _talentUis = new List<TalentUI>();

            int index = 0;
            foreach (var talent in _talentDesignElements)
            {
                var newTalent = Instantiate(_talentUiPrefab, researchLayout.transform);
                newTalent.SetToggleCallback(OnTalentToggle);
                newTalent.Load(talent);
                _talentUis.Add(newTalent);

                index++;
            }
        }

        base.Init();
    }

    private void CheckUnlockTalent()
    {
        var tempTalenDataList = _talentDatas.ToList();

        foreach (var designElement in _talentDesignElements)
        {
            TalentData data = null;
            if (_talentDatas != null && _talentDatas.Count > 0)
            {
                data = tempTalenDataList.Find(x => x.TalentID == designElement.ID);
            }

            if (data == null) // Don't have yet
            {
                // Debug.LogError(designElement.SpecialUnlock + " " + designElement.CampaignUnlockLevel + " " +
                //                SaveGameHelper.GetLastCampaignLevel());
                if (designElement.SpecialUnlock == DesignHelper.DEFAULT_NULL &&
                    designElement.CampaignUnlockLevel <= SaveGameHelper.GetMaxCampaignLevel() // Unlock by level
                ) // Unlock by IAP
                {
                    void CreateNewTalent()
                    {
                        var newTalentData = new TalentData();
                        newTalentData.TalentID = designElement.ID;
                        newTalentData.Status = ITEM_STATUS.Disable;
                        newTalentData.TalentLevel = 0;
                        newTalentData.TalentValue = 0;

                        _talentDatas.Add(newTalentData);
                    }

                    if (designElement.SpecialUnlock == "IAP" && AchieveManager.HasPurchaseIAP())
                    {
                        CreateNewTalent();
                    }
                    else
                    {
                        CreateNewTalent();
                    }

                    // Debug.LogError("Unlock");
                }
            }
        }

        foreach (var talentUI in _talentUis)
        {
            talentUI.RefreshLayer();
        }

        SaveManager.Instance.SaveData();
    }

    public override void ResetLayers()
    {
        Init();
        CheckUnlockTalent();
        CheckUpgradeButtonStatus();
        _upgradeResult.gameObject.SetActive(false);

        if (_isRandoming)
        {
            _randomSequence?.Kill();
            _randomSequence = null;
            _isRandoming = false;
        }

        base.ResetLayers();
    }

    private void CheckUpgradeButtonStatus()
    {
        int currentTalentLevel = SaveManager.Instance.Data.Inventory.TalentLevel;
        int currentCampaginLevel = SaveGameHelper.GetMaxCampaignLevel();

        if (currentTalentLevel > DesignManager.instance.talentUpgradeDesign.TalentUpgradeDesignElements.Count - 1)
        {
            _nextUpgradeText.textName = "MAXIMUM_UPGRADE";
            _upgradeButton.gameObject.SetActive(false);
        }
        else
        {
            var talentUpgradeElement = DesignHelper.GetTalentUpgradeDesign(currentTalentLevel);
            _upgradeButton.gameObject.SetActive(true);
            _upgradeCostText.text = "x" + talentUpgradeElement.Potion;
            _upgradeButton.interactable = true;

            if (currentCampaginLevel >= talentUpgradeElement.LevelCampaign)
            {
                _nextUpgradeText.UpdateTextNameWithParams("REMAIN",
                    DesignHelper.GetTotalTalentUpgradeByLevel(currentCampaginLevel) -
                    SaveManager.Instance.Data.Inventory.TalentLevel);

                _upgradeButton.interactable = true;
            }
            else
            {
                _nextUpgradeText.UpdateTextNameWithParams("NEXT_UPGRADE_LEVEL", talentUpgradeElement.LevelCampaign);
                _upgradeButton.interactable = false;
            }
        }
    }

    public void ShowTooltip(string name, string description, Transform position)
    {
        _tooltip.UpdateText(name, description);
        _tooltip.Show();

        // _tooltipArrow.transform.position =
        //     position.position -
        //     new Vector3(0, Utils.ConvertToMatchHeightRatio(_tooltipArrow.rectTransform().rect.height / 2), 0);
        //
        // Vector3 newBoxPosition =
        //     new Vector3(_tooltipBox.transform.position.x,
        //         _tooltipArrow.transform.position.y, 0);
        // _tooltipBox.transform.position = newBoxPosition;

        _tooltip.Show();

        _tooltipBox.transform.parent = _tooltipArrow;

        _tooltipArrow.transform.position =
            position.position -
            new Vector3(0, Utils.ConvertToMatchHeightRatio(_tooltipArrow.rectTransform().rect.height / 2), 0);

        _tooltipBox.transform.parent = _tooltip.transform;
        _tooltipBox.rectTransform().anchoredPosition = new Vector2(0, _tooltipBox.rectTransform().anchoredPosition.y);
    }

    public void OnTalentToggle(TalentDesignElement talentDesign, TalentUI talentUi)
    {
        var status = SaveGameHelper.GetTalentStatus(talentDesign);
        string description = "";
        string name = LocalizeController.GetText(talentDesign.Name);

        switch (status)
        {
            case ITEM_STATUS.Locked:
                if (talentDesign.CampaignUnlockLevel != 0)
                {
                    description = LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV,
                        talentDesign.CampaignUnlockLevel);
                }
                else if (talentDesign.SpecialUnlock != DesignHelper.DEFAULT_NULL)
                {
                    description =
                        LOCALIZE_ID_PREF.NOT_AVAILABLE_NOW
                            .AsLocalizeString(); //ocalizeController.GetText("IAP_LOCK_TALENT");
                }

                break;

            case ITEM_STATUS.Available:
            case ITEM_STATUS.Disable:
                description = LocalizeController.GetText(talentDesign.Description);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        ShowTooltip(name, description, talentUi.transform);
    }


    public void Upgrade()
    {
        if (_isRandoming)
            return;

        int currentTalentLevel = SaveManager.Instance.Data.Inventory.TalentLevel;
        var talentUpgradeElement = DesignHelper.GetTalentUpgradeDesign(currentTalentLevel);
        int cost = talentUpgradeElement.Potion;
        // int cost = 0;

        if (CurrencyModels.instance.IsEnough(CurrencyType.PILL, cost))
        {
            //NetworkDetector.instance.checkInternetConnection((reached) =>
            //{
            //    if (reached)
            //    {

            //    }
            //});

            TalentData talentData = null;

            // Find not upgraded talent
            List<TalentData> notUpgradeTalent = new List<TalentData>();
            foreach (var talentDt in _talentDatas)
            {
                if (talentDt.TalentLevel == 0)
                {
                    notUpgradeTalent.Add(talentDt);
                }
            }

            if (notUpgradeTalent.Count > 0)
            {
                talentData = notUpgradeTalent.PickRandom();
            }

            // Find not max level talent
            if (talentData == null)
            {
                List<TalentData> notMaxLevelTalent = new List<TalentData>();
                foreach (var talent in _talentDatas)
                {
                    var design = DesignHelper.GetTalentDesign(talent);
                    if (talent.TalentLevel < design.MaximumLevel)
                    {
                        notMaxLevelTalent.Add(talent);
                    }
                }

                if (notMaxLevelTalent.Count > 0)
                {
                    talentData = notMaxLevelTalent.PickRandom();
                }
            }

            // Result
            if (talentData != null)
            {
                UpgradeTalent(talentData);
                CurrencyModels.instance.AddCurrency(CurrencyType.PILL, -cost);

                CheckUpgradeButtonStatus();
                DoUpgradeVisual(talentData);
            }
        }
        else
        {
            MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.PILL, cost);
        }
    }

    private void UpgradeTalent(TalentData talentData)
    {
        talentData.TalentLevel++;
        talentData.Status = ITEM_STATUS.Available;
        talentData.ResetTalenData();

        // Debug.LogError(talentData.TalentID);
        // float oldValue = talentData.TalentValue;
        // Debug.LogError("Old value + " + oldValue + " NEW value " + talentData.TalentValue);

        SaveManager.Instance.Data.Inventory.TalentLevel++;
        SaveManager.Instance.SaveData();
        TalentManager.UpdateFromSave();
    }


    private void DoUpgradeVisual(TalentData data)
    {
        TalentUI final = _talentUis.Find(x => x.TalentDesignElement.ID == data.TalentID);
        _isRandoming = true;

        Action finalAnimation = () =>
        {
            if (!_isHided)
            {
                final.Select(true);
                final.FinalSelect(() =>
                {
                    if (!_isHided)
                    {
                        ResetLayers();
                        _upgradeResult.Load(data);
                    }
                });
            }
        };

        if (_talentDatas.Count == 1)
        {
            finalAnimation.Invoke();
            return;
        }

        int step = _talentDatas.Count * 2;
        float delayEachTurn = 0.1f;
        float deltaEachTurn = 0.0025f;

        List<TalentUI> randomUI = new List<TalentUI>();
        var listAvailableTalentData = _talentDatas.ToList();

        foreach (var talent in _talentUis)
        {
            if (listAvailableTalentData.Find(x => x.TalentID == talent.TalentDesignElement.ID) != null)
            {
                randomUI.Add(talent);
            }
        }

        TalentUI currentTalentUi = null;
        _randomSequence = DOTween.Sequence();

        for (int i = 0; i < step; i++)
        {
            int temp = i;
            delayEachTurn -= deltaEachTurn;

            _randomSequence.Append(DOVirtual.DelayedCall(delayEachTurn, () =>
            {
                currentTalentUi?.Select(false);
                currentTalentUi = randomUI.PickRandom();
                currentTalentUi?.Select(true);

                if (temp == step - 1)
                {
                    DOVirtual.DelayedCall(delayEachTurn, () =>
                    {
                        currentTalentUi?.Select(false);
                        finalAnimation.Invoke();
                    });
                }
            }));
        }

        _randomSequence.Play();
    }
}