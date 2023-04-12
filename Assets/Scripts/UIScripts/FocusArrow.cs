using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FocusArrow : MonoBehaviour
{
    public Image circle1;
    public Image circle2;
    //
    // // Start is called before the first frame update
    // void Start()
    // {
    //     SetDelay(circle1.DOFade(0.2f, 0.75f).SetEase(Ease.InBack));
    //     SetDelay(circle1.transform.DOScale(1, 0.75f));
    //
    //     DOVirtual.DelayedCall(0.2f, () =>
    //     {
    //         SetDelay(circle2.DOFade(0.2f, 0.75f).SetEase(Ease.InBack));
    //         SetDelay(circle2.transform.DOScale(1, 0.75f));
    //     });
    // }
    //
    // private void SetDelay(Tween tween)
    // {
    //     Sequence s = DOTween.Sequence();
    //     s.SetDelay(1f);
    //     s.Append(tween);
    //     s.AppendInterval(1f);
    //     s.SetLoops(-1, LoopType.Restart);
    // }
}