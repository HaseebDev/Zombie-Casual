using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public float floatValueY;
    public float duration;

    public bool isLocal = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        // var destination = transform.position + new Vector3(0, floatValueY, 0);
        if (isLocal)
        {
            transform.DOLocalMoveY(transform.position.y + floatValueY, duration).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            transform.DOMoveY(transform.position.y + floatValueY, duration).SetLoops(-1, LoopType.Yoyo);
        }
    }
}