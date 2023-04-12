using QuickType;
using QuickType.RewardDesign;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ez.Pooly;
using TMPro;
using Coffee.UIEffects;

public class IdleRewardColumnView : MonoBehaviour
{
    public RectTransform _rectTransform;

    public TextMeshProUGUI txtLevel;
    public List<RectTransform> _listRectContent;
    public RewardItemView rewardPrefab;

    private CustomBattleReward _data;
    private List<RewardItemView> rewardItems = new List<RewardItemView>();

    public Image collectablePannel;
    public Image imgTick;

    public Button btnClaim;
    public UIShiny _effectShiny;

    private string RewardID { get { return $"{GameConstant.IDLE_REWARD_PREFIX}{_data.LevelReward}"; } }
    private bool IsClaimed = false;
    private bool IsClaimable = false;

    private void Awake()
    {
        this._rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(CustomBattleReward data, bool isClaimable, bool isClaimed)
    {
        _data = data;
        IsClaimed = isClaimed;
        IsClaimable = isClaimable;
        txtLevel.text = data.LevelReward.ToString();
        int index = 0;
        if (rewardItems.Count > 0)
        {
            foreach (var item in rewardItems)
            {
                Pooly.Despawn(item.transform);
            }

            rewardItems.Clear();
        }

        var listRewards = SaveManager.Instance.Data.GetListValidIdleRewards(this._data);
        if (listRewards != null && listRewards.Count > 0)
        {
            foreach (var dt in listRewards)
            {
                var rwdItem = Pooly.Spawn<RewardItemView>(rewardPrefab.transform, Vector3.zero, Quaternion.identity, getRectForHex(index));
                rwdItem.transform.localScale = Vector3.one;
                rwdItem.Initialize(dt);
                rewardItems.Add(rwdItem);
                index++;
            }
        }

        ResetView(IsClaimable, IsClaimed);
    }

    public void OnButtonClaim()
    {
        if (!IsClaimed && IsClaimable)
        {
            var listReward = SaveManager.Instance.Data.AddIdleRewards(_data);
            if (listReward == null || listReward.Count <= 0)
            {
                Debug.LogError("Sth went wrong when earn idle reward");
            }
            else
            {
                IsClaimed = true;
                TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, listReward, true, false);
                ResetView(IsClaimable, IsClaimed);
                EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_IDLE_REWARD_ITEM_DATA, _data.LevelReward, true);
            }

        }
    }

    public void ResetView(bool isCollectable, bool isClaimed)
    {
        if (isClaimed)
        {
            collectablePannel.gameObject.SetActiveIfNot(false);
            btnClaim.gameObject.SetActiveIfNot(false);
            imgTick.gameObject.SetActiveIfNot(true);
        }
        else
        {
            btnClaim.gameObject.SetActiveIfNot(true);
            collectablePannel.gameObject.SetActiveIfNot(isCollectable);
            btnClaim.interactable = isCollectable;
            if (!isCollectable)
                _effectShiny.Stop(true);

            imgTick.gameObject.SetActiveIfNot(false);
        }
        //imgTick.gameObject.SetActiveIfNot(isClaimed);
    }

    private RectTransform getRectForHex(int index)
    {
        int div = index % 3; //0,1,2
        int nor = index / 3;
        var targetDiv = div >= 2 ? div - 1 : div;
        return _listRectContent[targetDiv + nor];

        //0 -> 0
        //1,2 -> 1
        //3 -> 2
        //4,5 -> 3
    }

}
