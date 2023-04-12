using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class MoveLoopEffect : MonoBehaviour
{
    public float deltaX;

    public float deltaY;

    public float duration;

    public LoopType loopType = LoopType.Yoyo;
    private Vector3 originPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.localPosition;
        transform.DOLocalMove(originPosition + new Vector3(Utils.ConvertToMatchWidthRatio(deltaX),Utils.ConvertToMatchHeightRatio(deltaY)), duration).SetLoops(-1,loopType).SetEase(Ease.Linear);
    }
   
}
