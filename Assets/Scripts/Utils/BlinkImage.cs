using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlinkImage : MonoBehaviour
{
    [SerializeField] private Image _highlight;
    private Sequence _glowSequence;

    // Start is called before the first frame update
    void Start()
    {
        _glowSequence = DOTween.Sequence();
        _glowSequence.Append(_highlight.DOFade(0.2f, 0.5f));
        _glowSequence.Append(_highlight.DOFade(1f, 0.5f));
        // _glowSequence.Append(_highlight.DOFade(0.2f, 0.5f));
        _glowSequence.SetLoops(-1);
    }
}
