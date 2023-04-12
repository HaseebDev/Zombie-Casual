using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = System.Random;

public class CurveHelper : MonoBehaviour
{
    private float _time = 0;
    private float _moveTime = 0;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private Vector3 _randomPos;
    private Action _callBack;

    private bool _isStarted = false;

    public void CurveMove(Vector3 endPosition, float t, int minus, Action callBack)
    {
        _moveTime = t;
        _endPos = endPosition;
        // _randomPos = new Vector3(UnityEngine.Random.Range(_endPos.x, _startPos.x) / 2,
        // UnityEngine.Random.Range(_endPos.y, _startPos.y));
        // float distanceX = Mathf.Abs(_endPos.x - _startPos.x) / 4;
        // _randomPos = new Vector3((_endPos.x + _startPos.x) / 2 + distanceX * minus,(_endPos.y + _startPos.y) / 2);
        _randomPos = new Vector3(minus == 1 ? _endPos.x : _startPos.x, (_endPos.y + _startPos.y) / 2);

        _isStarted = true;
        _callBack = callBack;
    }

    public Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }

    public Vector3 QuadraticCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Lerp(a, b, t);
        Vector3 p1 = Lerp(b, c, t);
        return Lerp(p0, p1, t);
    }

    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStarted)
        {
            _time += Time.deltaTime;
            var curveTime = _time / _moveTime;
            transform.position = QuadraticCurve(_startPos, _randomPos, _endPos, curveTime);

            if (_time > _moveTime)
            {
                _isStarted = false;
                _callBack?.Invoke();
            }
        }
    }
}