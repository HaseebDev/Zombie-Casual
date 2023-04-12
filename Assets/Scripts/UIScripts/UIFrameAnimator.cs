using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIFrameAnimator : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> frames;

    public float framePerSec;

    private Image img;

    private void Awake()
    {
        img = this.GetComponent<Image>();
    }

    private void Update()
    {
        int index = (int)((Time.time * framePerSec) % frames.Count);
        img.sprite = frames[index];
    }
}
