using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScalingUI : MonoBehaviour
{
    public float scaleTo;

    public float duration;

    public bool playAtAwake = false;

    private Vector3 _originScale;

    private Image _image;
    private Color _initColor;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _image = GetComponent<Image>();
        if (_image != null)
        {
            _initColor = _image.color;
        }
        yield return new WaitForSeconds(0.5f);
        if (!playAtAwake)
        {
            StartScaling();
        }
    }

    private void Awake()
    {
        _originScale = transform.localScale;
        if (playAtAwake)
        {
            StartScaling();
        }
    }

    public void Stop()
    {
        transform.DOKill();
        transform.localScale = _originScale;
        if (this._image != null)
        {
            _image.color = _initColor;
        }
    }

    public void StartScaling()
    {
        Stop();
        transform.DOScale(scaleTo, duration).SetLoops(-1, LoopType.Yoyo);
    }

    public void StartScaling(Color _colorChange)
    {
        StartScaling();
        if (_image != null)
        {
            _image.color = _colorChange;
        }
    }


}