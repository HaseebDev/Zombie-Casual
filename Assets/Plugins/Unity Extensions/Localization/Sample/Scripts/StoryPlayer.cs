using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.Localization.Sample
{
    public class StoryPlayer : MonoBehaviour
    {
        public int dialogueCount;
        public Button button;
        public LocalizedText locationText;
        public LocalizedText speakerText;
        public TypewriterController dialogueTypewriter;
        public GameObject nextIcon;

        int _mainDialogueIndex = -1;
        int _subDialogueIndex = -1;

        const string prefix = "Dialogue";

        public void Play()
        {
            dialogueTypewriter.onStop += completed => nextIcon.SetActive(completed);
            button.onClick.AddListener(OnClick);

            ToNextMainDialogue();
        }

        void OnClick()
        {
            if (dialogueTypewriter.isPlaying) dialogueTypewriter.Stop();
            else ToNextSubDialogue();
        }

        void ToNextMainDialogue()
        {
            _mainDialogueIndex++;
            _mainDialogueIndex %= dialogueCount;

            locationText.textName = LocalizationManager.GetTextAttribute($"{prefix}{_mainDialogueIndex}.Location");

            _subDialogueIndex = -1;
            ToNextSubDialogue();
        }

        void ToNextSubDialogue()
        {
            _subDialogueIndex++;

            string name = $"{prefix}{_mainDialogueIndex}.{_subDialogueIndex}";

            if (LocalizationManager.HasText(name))
            {
                speakerText.textName = LocalizationManager.GetTextAttribute(name);
                dialogueTypewriter.targetText.textName = name;
                dialogueTypewriter.Play();
            }
            else ToNextMainDialogue();

            nextIcon.SetActive(false);
        }
    }
}