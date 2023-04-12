using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using QuickType;
using UnityEngine.UI;
using Ez.Pooly;

public class HUDLevelPreview : BaseHUD
{
    public TextMeshProUGUI txtTitle;
    [SerializeField] private StarHelper _starHelper;

    public GameObject betweenLine;
    public GridLayoutGroup _gridGroup;
    public RectTransform _rectRewards;
    public RectTransform _rectBonusRewards;
    public TextMeshProUGUI _txtBonusRewards;

    private List<Reward> listRewards;
    private List<Reward> listBonusRewards;
    private List<RewardUI> _listRwdViews;

    public Button _btnPlay;

    private int viewLevel;
    private bool isReplayable;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        listRewards = (List<Reward>) args[0];
        listBonusRewards = (List<Reward>) args[1];
        viewLevel = (int) args[2];
        int star = (int) args[3];
        _starHelper.Load(star);

        txtTitle.text = LOCALIZE_ID_PREF.LEVEL + " " + viewLevel;

        if (_listRwdViews != null && _listRwdViews.Count > 0)
        {
            foreach (var item in _listRwdViews)
            {
                Pooly.Despawn(item.transform);
            }
        }

        _gridGroup.gameObject.SetActiveIfNot(true);
        _listRwdViews = new List<RewardUI>();
        foreach (var data in listRewards)
        {
            RewardUI rwdView = Pooly.Spawn<RewardUI>(POOLY_PREF.REWARD_UI_CAMPAIGN, Vector3.zero,
                Quaternion.identity, _rectRewards);
            rwdView.ShowShining(false);
            rwdView.transform.localScale = Vector3.one;
            rwdView.Load(new RewardData(data.RewardId, data.Value));
            _listRwdViews.Add(rwdView);
        }

        //bonus rewards
        var enableBonus = listBonusRewards != null && listBonusRewards.Count > 0;
        _rectBonusRewards.gameObject.SetActiveIfNot(enableBonus);
        _txtBonusRewards.gameObject.SetActiveIfNot(enableBonus);
        betweenLine.gameObject.SetActiveIfNot(enableBonus);

        string topTitle = null;
        foreach (var data in listBonusRewards)
        {
            RewardUI rwdView = Pooly.Spawn<RewardUI>(POOLY_PREF.REWARD_UI_CAMPAIGN, Vector3.zero,
                Quaternion.identity, _rectBonusRewards);
            rwdView.ShowShining(false);
            rwdView.transform.localScale = Vector3.one;

            if (DesignHelper.ConvertToRewardType(data.RewardId) == REWARD_TYPE.DIAMOND)
            {
                topTitle = LOCALIZE_ID_PREF.THREE_STAR_REWARD.AsLocalizeString();
            }

            rwdView.Load(new RewardData(data.RewardId, data.Value), true, topTitle);
            _listRwdViews.Add(rwdView);

            topTitle = null;
        }

        isReplayable = SaveManager.Instance.Data.IsReplayable(GameMode.CAMPAIGN_MODE, viewLevel);
        _btnPlay.interactable = isReplayable;

        _canvas.sortingOrder = 36;
        DOVirtual.DelayedCall(0.05f, () => { _canvas.sortingOrder = 37; });
    }

    public override void Show(System.Action<bool> showComplete = null, bool addStack = true)
    {
        base.Show((complete) =>
        {
            showComplete?.Invoke(complete);
            if (complete)
            {
                this._gridGroup.enabled = false;
                this._gridGroup.enabled = true;
            }
        }, addStack);
    }

    public void OnButtonPlay()
    {
        if (isReplayable)
            GameMaster.instance.QuickPlayCampaignLevel(viewLevel);
    }
}