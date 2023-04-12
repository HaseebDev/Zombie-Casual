using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public bool isClockwise;
    public float speed = 50;

    private float realSpeed = 0;

    private void Start()
    {
        realSpeed = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClockwise)
            transform.Rotate(Vector3.back * realSpeed, Space.Self);
        else
            transform.Rotate(Vector3.forward * realSpeed, Space.Self);
    }
}