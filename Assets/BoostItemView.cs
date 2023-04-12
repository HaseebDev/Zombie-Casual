using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoostItemView : MonoBehaviour
{
    public Image imgIcon;
    public CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void SetIcon(Sprite _icon)
    {
        imgIcon.sprite = _icon;
    }

    public void SetColorAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
    }
}
