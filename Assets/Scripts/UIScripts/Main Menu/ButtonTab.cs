using System;
using DG.Tweening;
using QuickEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

namespace UIScripts.Main_Menu
{
    public class ButtonTab : MonoBehaviour
    {
        [SerializeField] private Image bg;
        [SerializeField] private Image selectImage;
        [SerializeField] private Image unselectImage;
        [SerializeField] private Sprite selectBG;
        [SerializeField] private Sprite unselectBG;

        private float _originWidth;
        private float _orginHeight;
        private RectTransform selectImageRectTransform;
        private LayoutElement _layoutElement;

        private void Awake()
        {
            _layoutElement = GetComponent<LayoutElement>();
            _orginHeight = transform.rectTransform().rect.height;
            _originWidth = transform.rectTransform().rect.height;
            selectImageRectTransform = selectImage.GetComponent<RectTransform>();
        }

        public void UnSelect()
        {
            bg.sprite = unselectBG;
            selectImage.gameObject.SetActive(false);
            unselectImage.gameObject.SetActive(true);
            transform.rectTransform().SetHeight(_orginHeight);

            selectImage.transform.localScale = Vector3.one;
            _layoutElement.preferredWidth = _originWidth;
            // selectImageRectTransform.anchoredPosition = originPos;
        }

        public void Select()
        {
            bg.sprite = selectBG;
            _layoutElement.preferredWidth = _originWidth + 30f;

            selectImage.gameObject.SetActive(true);
            unselectImage.gameObject.SetActive(false);
            transform.rectTransform().SetHeight(_orginHeight + 33.6f);

            // selectImageRectTransform.DOAnchorPosY(originPos.y + 50, 0.3f);
            selectImage.transform.DOScale(1.05f, 0.3f);
        }
    }
}