using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.Localization.Sample
{
    public class FullscreenBlack : MonoBehaviour
    {
        public Image image;
        public float alphaSpeed = 1;


        private void Awake()
        {
            image.color = Color.black;
            enabled = false;
        }


        public void Hide()
        {
            enabled = true;
        }


        private void Update()
        {
            var color = image.color;
            color.a -= alphaSpeed * Time.unscaledDeltaTime;
            image.color = color;
            if (color.a <= 0)
            {
                enabled = false;
                image.enabled = false;
            }
        }
    }

}
