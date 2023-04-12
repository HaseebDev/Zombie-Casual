using Coffee.UIEffects;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarScreenUI : HealthBar
{
    [Header("HealthBarScreen")]
    public Slider _sliderProgress;

    private bool isPlayingAnim = false;

    private void Start()
    {
        _sliderProgress.interactable = false;
    }

    protected override void SetNormalizedValue(float value, bool withAnim = false)
    {
        if (isPlayingAnim)
        {
            _sliderProgress.DOKill(true);
            _uiShiny?.Stop(true);
            isPlayingAnim = false;
        }

        if (!withAnim)
            _sliderProgress.value = value;
        else
        {
            isPlayingAnim = true;
            _sliderProgress.DOKill();
            _sliderProgress.DOValue(value, 1.0f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _uiShiny?.Stop(true);
                isPlayingAnim = false;
            });
        }
        transform.localRotation = Quaternion.Euler(Vector3.zero);


    }

    public override void PlayAnimSetHP(float targetHP, float maxHP, bool withanim = true)
    {
        if (hpText != null)
        {
            int curHP = -1;
            int.TryParse(hpText.text, out curHP);

            if (curHP > 0)
            {
                var baseTween = DOTween.To(() => curHP, x => hpText.text = x.ToString(), Mathf.RoundToInt(targetHP), 1.0f).SetEase(Ease.Linear);

            }
            else
            {
                hpText.text = Mathf.RoundToInt(targetHP).ToString();

            }

        }

        var curValue = this._sliderProgress.value;
        if (_uiShiny != null)
        {
            _uiShiny.enabled = true;
            _uiShiny.Play(true);
        }

        var targetScale = targetHP * 1.0f / maxHP * 1.0f;
        SetNormalizedValue(targetScale, true);
    }


}