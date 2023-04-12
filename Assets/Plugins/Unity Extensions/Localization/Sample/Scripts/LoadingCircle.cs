using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.Localization.Sample
{
    public class LoadingCircle : MonoBehaviour
    {
        public Image image;
        public float rotateSpeed = 90;
        public float alphaSpeed = 1;


        float direction = -1;


        private void Awake()
        {
            var color = image.color;
            color.a = 0;
            image.color = color;
            enabled = false;
        }


        public void Show()
        {
            enabled = true;
            direction = 1;
        }


        private void Update()
        {
            var color = image.color;
            color.a += alphaSpeed * Time.unscaledDeltaTime * direction;
            image.color = color;

            if (!LocalizationManager.hasTask)
            {
                if (color.a >= 1) direction = -1;
                else if (color.a <= 0)
                {
                    enabled = false;
                }
            }

            transform.Rotate(0, 0, rotateSpeed * Time.unscaledDeltaTime);
        }
    }

}
