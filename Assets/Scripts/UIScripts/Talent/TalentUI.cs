using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using DG.Tweening;
using QuickType.Talent;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class TalentUI : MonoBehaviour
{
    [SerializeField] private Image _bg;
    [SerializeField] private Image _icon;
    [SerializeField] private LocalizedTMPTextUI _levelHolder;
    [SerializeField] private LocalizedTMPTextUI _levelText;
    [SerializeField] private LocalizedTMPTextUI _nameText;
    [SerializeField] private GameObject _lockPanel;
    [SerializeField] private GameObject _notUpgradePanel;
    [SerializeField] private Image _selected;

    private Action<TalentDesignElement, TalentUI> _onToggle;
    private TalentDesignElement _talentDesignElement;
    private bool _isInit = false;
    private Color _whiteAlpha70;

    public TalentDesignElement TalentDesignElement => _talentDesignElement;

    private void Init()
    {
        if (!_isInit)
        {
            GetComponent<Button>().onClick.AddListener(() => { _onToggle?.Invoke(_talentDesignElement, this); });
        }

        _whiteAlpha70 = Color.white;
        _whiteAlpha70.a = 0.7f;
        _isInit = true;
    }

    public void SetToggleCallback(Action<TalentDesignElement, TalentUI> callback)
    {
        _onToggle = callback;
    }

    public void Load(TalentDesignElement talentDesign)
    {
        Init();

        _talentDesignElement = talentDesign;
        ResourceManager.instance.GetTalentSprite(talentDesign.ID, s => { _icon.sprite = s; });
        _levelHolder.text = "";
        _levelText.text = "";
        _nameText.textName = talentDesign.Name;
        ResourceManager.instance.GetTalentRankSprite(talentDesign.Rank, s => { _bg.sprite = s; });

        _lockPanel.SetActive(false);
        _notUpgradePanel.SetActive(false);

        ITEM_STATUS talentStatus = SaveGameHelper.GetTalentStatus(talentDesign);
        switch (talentStatus)
        {
            case ITEM_STATUS.Locked:
                _lockPanel.SetActive(true);
                break;
            case ITEM_STATUS.Disable:
                _notUpgradePanel.SetActive(true);
                break;
            case ITEM_STATUS.Available:
                if (SaveGameHelper.GetTalentData(talentDesign).TalentLevel >= talentDesign.MaximumLevel)
                    _levelText.text = LOCALIZE_ID_PREF.MAX.AsLocalizeString(); //  LocalizeController.GetText("MAX");
                else
                {
                    _levelHolder.text = "Lv";
                    _levelText.text = $"{SaveGameHelper.GetTalentData(talentDesign).TalentLevel}";
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RefreshLayer()
    {
        Load(_talentDesignElement);
        Select(false);
    }

    public void Select(bool isSelect)
    {
        _selected.gameObject.SetActive(isSelect);
        _selected.color = _whiteAlpha70;
    }

    public void FinalSelect(Action callback)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_selected.DOFade(0.7f, 0.2f));
        sequence.Append(_selected.DOFade(0, 0.2f));
        sequence.SetLoops(3);
        sequence.OnComplete(() => { callback?.Invoke(); });
    }
}