using Coffee.UIEffects;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Color colorHit = Color.red;
    public Color colorNormal = Color.green;
    public UIShiny _uiShiny;

    private PulseEffect _pulseEffect;

    private CanvasGroup _canvasGoup;

    // private Vector3 initScale;
    [SerializeField] private Image fillImg;
    private void Awake()
    {
        _canvasGoup = GetComponent<CanvasGroup>();
        _pulseEffect = GetComponent<PulseEffect>();
        // initScale = transform.localScale;
        // fillImg = this.progressLine.GetComponent<Image>();
    }

    public void EnableHPBar(bool enable, bool playFX = true)
    {
        if (!_canvasGoup)
            _canvasGoup = GetComponent<CanvasGroup>();

        if (playFX)
        {
            gameObject.SetActiveIfNot(true);
            _canvasGoup.DOFade(enable ? 1 : 0, 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                if (!enable)
                    gameObject.SetActiveIfNot(false);
            });
        }
        else
        {
            _canvasGoup.alpha = 1;
            gameObject.SetActiveIfNot(enable);
        }

        UpdateHPBarRotation();
    }

    public void SetHp(float currentHp, float maxHp)
    {
        if (hpText != null)
            this.hpText.text = Mathf.RoundToInt(currentHp).ToString();
        this.SetNormalizedValue(currentHp / maxHp);
    }

    protected virtual void SetNormalizedValue(float value, bool withAnim = false)
    {
        if (withAnim)
        {
            this.progressLine.DOScaleX(value, 1.0f).SetEase(Ease.InOutSine)
      .OnComplete(() => { _uiShiny?.Stop(true); });
            if (fillImg != null)
                fillImg.DOFillAmount(value, 1.0f).SetEase(Ease.InOutSine);
        }
        else 
        {
            if (Double.IsNaN(value))
                return;

            this.progressLine.localScale = new Vector3(value, 1f, 1f);
            if (fillImg != null)
                fillImg.fillAmount = value;
        }

    }

    public virtual void PlayAnimSetHP(float targetHP, float maxHP, bool withAnim = true)
    {
        if (hpText != null)
            this.hpText.text = targetHP + " / " + maxHP;

        var curValue = this.progressLine.localScale.x;
        if (_uiShiny != null)
        {
            _uiShiny.enabled = true;
            _uiShiny.Play(true);
        }

        var targetScale = targetHP * 1.0f / maxHP * 1.0f;
        SetNormalizedValue(targetScale, withAnim);
    }

    public void playAnimGetHit()
    {
        _pulseEffect?.Pulse();
        //fillImg.DOKill();
        //fillImg.color = colorNormal;
        //fillImg.DOColor(colorHit, 0.2f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
    }

    [SerializeField] protected Transform progressLine;

    [SerializeField] protected TextMeshProUGUI hpText;

    public void UpdateHPBarRotation()
    {
        if (GamePlayController.instance != null)
            transform.rotation = GamePlayController.instance.GetMainCamera().transform.rotation;
        else
            transform.rotation = Camera.main.transform.rotation;
    }

    public virtual void Update()
    {


    }
}