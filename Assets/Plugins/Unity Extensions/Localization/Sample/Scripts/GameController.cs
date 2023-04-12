using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.Localization.Sample
{
    public class GameController : StateMachineComponent<State>
    {
        public FullscreenBlack fullScreenBlack;
        public Dropdown dropdown;
        public LoadingCircle loadingCircle;
        public TypewriterController typewriter;
        public StoryPlayer storyPlayer;


        State _initializingState;
        State _playingState;


        /// <summary>
        /// Get the most user-friendly language your game provided
        /// </summary>
        static string GetDefaultLanguageType()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Japanese:
                    return "ja-JP";

                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    return "zh-CN";

                default:
                    return "en-US";
            }
        }


        private void Awake()
        {
            InitInitializingState();
            InitPlayingState();

            currentState = _initializingState;
        }


        void InitInitializingState()
        {
            _initializingState = new State();

            _initializingState.onEnter += () =>
            {
                Time.timeScale = 0;

                // init localization system
                LocalizationManager.LoadMetaAsync();

                // In a real game, you may want to use user saved data instead of GetDefaultLanguageType.
                // GetDefaultLanguageType is usually used for the situation you can not find user saved data.
                // Also, you need load all localized assets (fonts & textures) before loading a language.
                // HIGHLY RECOMMEND using Addressables to manager your localized assets.
                // In this sample, for simplicity, we skip all assets managerment, let unity load them all before running our scripts.
                var languageType = GetDefaultLanguageType();

                LocalizationManager.LoadLanguageAsync(languageType, _ => currentState = _playingState);

                LocalizationManager.taskCompleted += _ => { if (typewriter) typewriter.Play(); };
            };

            _initializingState.onExit += () =>
            {
                // Reset dropdown UI control
                void ResetDropDown()
                {
                    var options = new List<string>();
                    for (int i = 0; i < LocalizationManager.languageCount; i++)
                    {
                        options.Add(LocalizationManager.GetLanguageAttribute(i, "LanguageName"));
                    }
                    dropdown.ClearOptions();
                    dropdown.AddOptions(options);
                    dropdown.value = LocalizationManager.languageIndex;
                }

                ResetDropDown();
                dropdown.onValueChanged.AddListener(SetLanguage);

#if UNITY_EDITOR
                // Respond editor operations
                LocalizationManager.taskCompleted += (param) =>
                {
                    if (param.type == TaskType.LoadMeta) ResetDropDown();
                    if (param.type == TaskType.LoadLanguage) dropdown.value = LocalizationManager.languageIndex;
                };
#endif
                // hide fullscreenblack image
                fullScreenBlack.Hide();
            };
        }


        /// <summary>
        /// Change language when user select an item in dropdown list
        /// </summary>
        void SetLanguage(int index)
        {
            if (index != LocalizationManager.languageIndex)
            {
                LocalizationManager.LoadLanguageAsync(index);

                // you can save LocalizationManager.GetLanguageType(index) to user data here

                loadingCircle.Show();
            }
        }


        void InitPlayingState()
        {
            _playingState = new State();

            _playingState.onEnter += () =>
            {
                Time.timeScale = 1;
                if (storyPlayer) storyPlayer.Play();
            };
        }

    }
}