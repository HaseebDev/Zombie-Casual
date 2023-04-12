using System;
using UnityEngine;

namespace UnityExtensions.Localization
{
    struct ShaderIDs
    {
        internal static int characterCount = Shader.PropertyToID("_CharacterCount");
    }


    [AddComponentMenu("UI/Typewriter Controller")]
    [RequireComponent(typeof(LocalizedText))]
    [DisallowMultipleComponent]
    public class TypewriterController : ScriptableComponent
    {
        public enum ActionOnLocalizationChange
        {
            None = 0,
            ToBeginning,
            ToEnd,
        }

        [SerializeField] TimeMode _timeMode = TimeMode.Unscaled;
        [SerializeField] ActionOnLocalizationChange _actionOnLocalizationChange = ActionOnLocalizationChange.None;

        bool _playing;
        Material _originalMaterial;
        Material _individualMaterial;
        float _characterCount;


        LocalizedText _target;
        public LocalizedText targetText => _target ? _target : _target = GetComponent<LocalizedText>();


        /// <summary>
        /// Called when a character becomes visible. int: character count.
        /// </summary>
        public event Action<int> onType;


        /// <summary>
        /// Called when stop playing. bool: completed.
        /// </summary>
        public event Action<bool> onStop;


        public bool isPlaying => _playing;


        public void SetCharacterCount(float value)
        {
            _characterCount = value;

            if (targetText.material != _individualMaterial)
            {
                _originalMaterial = targetText.material;
                RuntimeUtilities.Destroy(_individualMaterial);
                if (_originalMaterial) _individualMaterial = Instantiate(_originalMaterial);
                targetText.material = _individualMaterial;
            }

            if (_individualMaterial)
            {
                _individualMaterial.SetFloat(ShaderIDs.characterCount, _characterCount);
            }
        }


        /// <summary>
        /// Start typewriter effect.
        /// </summary>
        public void Play(bool replay = true)
        {
            if (replay)
            {
                SetCharacterCount(0);
            }

            if (!_playing)
            {
                RuntimeUtilities.unitedUpdate += InternalUpdate;
                LocalizationManager.beforeContentsChange += BeforeContentsChange;
                _playing = true;
            }
        }


        /// <summary>
        /// Stop typewriter effect.
        /// </summary>
        public void Stop(bool finish = true)
        {
            if (finish)
            {
                SetCharacterCount(1e7f);
            }

            if (_playing)
            {
                _playing = false;
                RuntimeUtilities.unitedUpdate -= InternalUpdate;
                LocalizationManager.beforeContentsChange -= BeforeContentsChange;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    targetText.material = _originalMaterial;
                }
#endif
            }

            onStop?.Invoke(finish);
        }


        void OnDestroy()
        {
            Stop(false);
        }


        void BeforeContentsChange()
        {
            switch (_actionOnLocalizationChange)
            {
                case ActionOnLocalizationChange.ToBeginning: SetCharacterCount(0f); return;
                case ActionOnLocalizationChange.ToEnd: Stop(); return;
            }
        }


        void InternalUpdate()
        {
            if (targetText.isVerticesDirty) return;

            int _lastCharacterCount = (int)_characterCount;
            SetCharacterCount(_characterCount + targetText.typewriterSpeed * RuntimeUtilities.GetUnitedDeltaTime(_timeMode));
            if (_lastCharacterCount != (int)_characterCount)
            {
                onType?.Invoke(Mathf.Min((int)_characterCount, targetText.typewriterCharacterCount));
                if (_characterCount >= targetText.typewriterCharacterCount) Stop();
            }
        }
    }
}
