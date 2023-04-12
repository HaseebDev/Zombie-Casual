using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UltimateBar : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    [Header("Rect Fill")]
    public Transform _fillParent;
    public Image _preFillImg;
    public Image _FillImg;

    [Header("Rect Complete")]
    public Transform _completeParent;

    private Vector3 _initScale;
    private float MaxPower;

    private float lastFillImage;

    private void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _initScale = transform.localScale;
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void Initialize(int MaxPowerCount)
    {
        _preFillImg.fillAmount = 0;
        _FillImg.fillAmount = 0;
        lastFillImage = 0;
        _fillParent.gameObject.SetActiveIfNot(true);
        _completeParent.gameObject.SetActiveIfNot(false);

        this.MaxPower = MaxPowerCount;
    }

    public void SetPowerValue(float CurrentPower)
    {
        gameObject.SetActiveIfNot(true);
        _fillParent.gameObject.SetActiveIfNot(true);
        _completeParent.gameObject.SetActiveIfNot(false);

        _canvasGroup.alpha = 1;
        float targetPercent = (float)(CurrentPower * 1.0f / MaxPower * 1.0f);
        //_preFillImg.fillAmount = targetPercent;
        _FillImg.fillAmount = targetPercent;
        lastFillImage = targetPercent;


        transform.DOKill();
        transform.localScale = _initScale;
        transform.DOScale(_initScale * 1.1f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);

    }

    public void SetEnable(bool enable)
    {
        if (enable)
        {
            _fillParent.gameObject.SetActiveIfNot(true);
            _completeParent.gameObject.SetActiveIfNot(false);

            transform.DOKill();
            transform.localScale = _initScale;
            gameObject.SetActiveIfNot(true);
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear);
        }
        else
        {
            transform.localScale = _initScale;
            transform.DOKill();
            gameObject.SetActiveIfNot(true);
            _canvasGroup.alpha = 1;
            //gameObject.SetActiveIfNot(false);
            _canvasGroup.DOFade(0, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActiveIfNot(false);
            });

        }
    }
}
