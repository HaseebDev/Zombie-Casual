using TMPro;
using UnityEngine;

namespace UnityExtensions.Localization
{
    [AddComponentMenu("UI/Localized TMP Text")]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTMPTextUI : LocalizedTMPText
    {
        public string text {
            get { return targetTMPText.text; }
            set {
#if UNITY_EDITOR
                targetTMPText.text = PreHandleText(value);
#else
                   targetTMPText.text = value;
#endif
            }
        }
    }
} // namespace UnityExtensions.Localization