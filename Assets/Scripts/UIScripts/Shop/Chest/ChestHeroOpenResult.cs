using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using Doozy.Engine.Events;
using QuickType;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestHeroOpenResult : MonoBehaviour
{
    public enum CHEST_HERO_TYPE
    {
        NONE,
        X1,
        X5
    }

    public Transform spawnPosition;
    public Transform endPosition;
    public Transform holder;

    [SerializeField] private GameObject _guideText;
    [SerializeField] private Button _chestButton;
    [Header("Chest")] [SerializeField] private SkeletonGraphic _chestAnim;
    [SerializeField] private ParticleSystem _openPS;
    [SerializeField] private RewardUI _rewardUi;
    [SerializeField] private GameObject _afterOpen;

    [Header("Continue by key")] [SerializeField]
    private GameObject _continueOpenByKeyButton;

    [SerializeField] private GameObject _continueOpenByIAPButton;
    [SerializeField] private TextMeshProUGUI _keyRemains;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private ReminderUI _keyReminder;

    [Header("X5 Reward")]
    [SerializeField] private List<Transform> listX5RewardHolder;
    [SerializeField] private Transform x5RewardHolder;

    private RewardUI x1Result;
    private List<RewardData> _rewardDatas;
    private List<HeroData> justUnlockedHero;
    private List<RewardUI> listX5RewardUI;

    private CHEST_HERO_TYPE _currentType;

    private void OnEnable()
    {
        CurrencyBar.Instance.HideAll();
        MainMenuTab.Instance.Hide();
    }

    public void OpenChestX1(List<RewardData> rewardDatas)
    {
        MissionManager.Instance.TriggerMission(MissionType.OPEN_CHEST,1);
        _currentType = CHEST_HERO_TYPE.X1;
        _rewardDatas = rewardDatas;
        SetUp();
    }

    public void OpenChestX5(List<RewardData> rewardDatas)
    {
        MissionManager.Instance.TriggerMission(MissionType.OPEN_CHEST,5);

        _currentType = CHEST_HERO_TYPE.X5;
        _rewardDatas = rewardDatas;
        SetUp();
    }

    private void SetUp()
    {
        if (x1Result != null)
        {
            x1Result.gameObject.SetActive(false);
        }

        if (listX5RewardUI != null && listX5RewardUI.Count != 0)
        {
            foreach (var VARIABLE in listX5RewardUI)
            {
               VARIABLE.gameObject.SetActive(false); 
            }
        }

        _afterOpen.SetActive(false);

        gameObject.SetActive(true);
        AddHeroScroll();

        _guideText.SetActive(true);
        PlayDropAnimation();
    }

    private void AddHeroScroll()
    {
        justUnlockedHero = new List<HeroData>();
    
        foreach (var reward in _rewardDatas)
        {
            string heroId = (string) reward._extends;
            HeroData heroData = SaveManager.Instance.Data.GetHeroData(heroId);
            var preStatus = heroData.ItemStatus;
            SaveManager.Instance.Data.AddHeroScroll(heroId, reward._value, false);
            if (preStatus == ITEM_STATUS.Locked && heroData.ItemStatus == ITEM_STATUS.Available)
            {
                justUnlockedHero.Add(heroData);
            }
        }
    }

    public void StartAnimOpen()
    {
        _chestButton.interactable = false;
        // Time.timeScale = 0.1f;
        _chestAnim.AnimationState.SetEmptyAnimation(0, 0);
        _chestAnim.AnimationState.SetAnimation(0, "an_openchest", false);
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_CHEST_OPEN);

        if (_currentType == CHEST_HERO_TYPE.X1)
        {
            DOVirtual.DelayedCall(0.25f, () =>
            {
                _openPS.Play();

                if (x1Result == null)
                    x1Result = Instantiate(_rewardUi, holder);

                x1Result.gameObject.SetActive(true);
                x1Result.transform.localScale = Vector3.one * 0.5f;
                Transform transform1 = x1Result.transform;
                transform1.DOScale(2f, 0.25f);
                transform1.DORotate(new Vector3(0, 0, -360), 0.25f, RotateMode.FastBeyond360);
                transform1.position = spawnPosition.position;
                x1Result.transform.DOMove(endPosition.position, 0.25f).OnComplete(OnCompleteAnimation);
                x1Result.Load(_rewardDatas[0], false);
            });
        }
        else
        {
            DOVirtual.DelayedCall(0.25f, () =>
            {
                _openPS.Play();
                if (listX5RewardUI == null || listX5RewardUI.Count == 0)
                {
                    listX5RewardUI = new List<RewardUI>();
                    for (int i = 0; i < 5; i++)
                    {
                        listX5RewardUI.Add(Instantiate(_rewardUi, x5RewardHolder)); 
                    }
                }
                
                for (int i = 0; i < 5; i++)
                {
                    var result = listX5RewardUI[i];
                    result.gameObject.SetActive(true);
                    result.transform.localScale = Vector3.one * 0.5f;
                    Transform transform1 = result.transform;
                    transform1.DOScale(1.5f, 0.25f);
                    transform1.DORotate(new Vector3(0, 0, -360), 0.25f, RotateMode.FastBeyond360);
                    transform1.position = spawnPosition.position;
                    result.transform.DOMove(listX5RewardHolder[i].transform.position, 0.25f);//.OnComplete(OnCompleteAnimation);
                    result.Load(_rewardDatas[i], false); 
                }

                DOVirtual.DelayedCall(0.25f, () =>
                {
                    OnCompleteAnimation();
                });

            });
        }

        _guideText.SetActive(false);
    }

    private void UpdateContinueButtonStatus()
    {
        _continueOpenByKeyButton.gameObject.SetActive(false);
        _continueOpenByIAPButton.gameObject.SetActive(false);

        if (_currentType == CHEST_HERO_TYPE.X1)
        {
            int keyRemain = (int) SaveManager.Instance.Data.Inventory.TotalHeroChestKey;
            if (keyRemain != 0)
            {
                _continueOpenByKeyButton.gameObject.SetActive(true);
                _keyRemains.text = $"1/{keyRemain}";
                _keyReminder.Load(keyRemain);
            }
            else
            {
                _continueOpenByIAPButton.gameObject.SetActive(true);
                _costText.text = IAPManager.instance.GetProductPrice(IAPConstant.hero_chest_x1);
            }
        }
        else
        {
            _continueOpenByIAPButton.gameObject.SetActive(true);
            _costText.text = IAPManager.instance.GetProductPrice(IAPConstant.hero_chest_x5);
        }
    }

    public void OnButtonBack()
    {
        gameObject.SetActive(false);
        MainMenuTab.Instance?.Show();
        HUDShop.Instance.ResetLayers();
    }

    private void OnCompleteAnimation()
    {
        // Check unlocked hero

        if (justUnlockedHero != null && justUnlockedHero.Count != 0)
        {
            foreach (var hero in justUnlockedHero)
            {
                TopLayerCanvas.instance.ShowHUDForce(EnumHUD.HUD_HERO_UNLOCK, false, null, hero);
            }
        }

        _afterOpen.SetActive(true);
        UpdateContinueButtonStatus();
    }

    private void PlayDropAnimation()
    {
        _chestButton.interactable = false;
        DOVirtual.DelayedCall(0.1f, () => { _chestAnim.AnimationState.SetAnimation(0, "an_appear", false); });
        DOVirtual.DelayedCall(0.5f, () => { _chestButton.interactable = true; });

        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_CHEST_APPEAR);
    }

    public void ContinuePurchaseByKey()
    {
        SaveManager.Instance.Data.Inventory.TotalHeroChestKey--;
        ChestHeroDesignElement chestDesign =
            DesignManager.instance.chestHeroDesign.ChestHeroDesignElements.PickRandom();
        OpenChestX1(chestDesign.GetRewards());
    }

    public void ContinuePurchaseByIAP()
    {
        if (_currentType == CHEST_HERO_TYPE.X1)
        {
            IAPManager.instance.PurchaseIAP(IAPConstant.hero_chest_x1, isSuccess =>
            {
                if (isSuccess)
                {
                    ChestHeroDesignElement chestDesign =
                        DesignManager.instance.chestHeroDesign.ChestHeroDesignElements.PickRandom();
                    OpenChestX1(chestDesign.GetRewards());
                }
            });
        }
        else
        {
            IAPManager.instance.PurchaseIAP(IAPConstant.hero_chest_x5, isSuccess =>
            {
                if (isSuccess)
                {
                    List<RewardData> rewardDatas = new List<RewardData>();

                    for (int i = 0; i < 5; i++)
                    {
                        ChestHeroDesignElement chestDesign =
                            DesignManager.instance.chestHeroDesign.ChestHeroDesignElements.PickRandom();
                        var tepmReward = chestDesign.GetRewards();
                        foreach (var VARIABLE in tepmReward)
                        {
                            rewardDatas.Add(VARIABLE);
                        }
                    }
                    
                    OpenChestX5(rewardDatas);
                    // Open chest x5
                }
            });
        }
    }
}