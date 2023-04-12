using TMPro;
using UnityEngine;

namespace UnityExtensions.Localization
{
    [AddComponentMenu("UI/Localized TMP Text Params")]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTMPTextParam : LocalizedTMPTextUI
    {
        private object[] arg = null;

        public override void UpdateContent()
        {
            if (languageIndex >= 0)
            {
                string text = "";

                if (arg != null)
                {
                    string source = LocalizationManager.GetText(textName);
                    if (source != null)
                        text = string.Format(source, arg);
                    else
                        text = "NF: " + textName;
                }
                else
                    text = LocalizationManager.GetText(textName);

                if (text != null)
                {
                    // set text if _textName is valid
                    targetTMPText.text = text;
                }
            }
        }

        public void UpdateParams(params object[] arg)
        {
            this.arg = arg;
            UpdateContent();
        }

        public void UpdateTextNameWithParams(string textname, params object[] arg)
        {
            this.textName = textname;
            this.arg = arg;
            UpdateContent();
        }
    }
} // namespace UnityExtensions.Localization