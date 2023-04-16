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
        [SerializeField] private Image icon;
        [SerializeField] private Sprite unselectImage;
        [SerializeField] private Sprite selectImage;
        [SerializeField] private Sprite selectBG;
        [SerializeField] private Sprite unselectBG;
        [SerializeField] private GameObject buttonText;

        private float _originWidth;
        private float _orginHeight;
        //private RectTransform selectImageRectTransform;
        private LayoutElement _layoutElement;

        private void Awake()
        {
            _layoutElement = GetComponent<LayoutElement>();
            _orginHeight = transform.rectTransform().rect.height;
            _originWidth = transform.rectTransform().rect.height;
            //selectImageRectTransform = icon.GetComponent<RectTransform>();
            buttonText.SetActive(false);
        }

        public void UnSelect()
        {
            bg.sprite = unselectBG;
            bg.transform.localScale = Vector3.one;
            icon.transform.DOScale(1f, 0.1f);
            icon.transform.DOMoveY(70, .1f);
            icon.sprite = unselectImage;
            //unselectImage.gameObject.SetActive(true);
            transform.rectTransform().SetHeight(_orginHeight);
            _layoutElement.preferredWidth = _originWidth;
            buttonText.SetActive(false);
            // selectImageRectTransform.anchoredPosition = originPos;
        }

        public void Select()
        {
            bg.sprite = selectBG;
            bg.transform.DOScale(1.2f, 0.15f);
            _layoutElement.preferredWidth = _originWidth + 30f;
            icon.sprite = selectImage;
            //unselectImage.gameObject.SetActive(false);
            transform.rectTransform().SetHeight(_orginHeight + 33.6f);
            buttonText.SetActive(true);
            // selectImageRectTransform.DOAnchorPosY(originPos.y + 50, 0.3f);
            icon.transform.DOMoveY(160, .2f);
            buttonText.transform.DOMoveY(25, .2f);
            icon.transform.DOScale(1.5f, 0.1f);
            buttonText.transform.DOScale(1.2f, 0.1f);
        }
    }
}