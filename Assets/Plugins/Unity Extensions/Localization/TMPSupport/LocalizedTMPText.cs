using System.Collections.Generic;
using QuickEngine.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

namespace UnityExtensions.Localization
{
    //[AddComponentMenu("UI/Localized TMP Text")]
    //[RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public abstract partial class LocalizedTMPText : LocalizedComponent
    {
        [SerializeField] private string _insertBefore;
        [SerializeField] private string _insertAfter;
        [SerializeField] bool _upperCase = false;
        [SerializeField] bool _upperCaseEachWord = false;
        [SerializeField] string _textName; // invalid name will not change text
        [SerializeField] LocalizedTMPTextProfile _profile;

        TMP_Text _target;
        LinkedListNode<TMP_Text> _node;

        public TMP_Text targetTMPText => _target ? _target : _target = GetComponent<TMP_Text>();

        public LocalizedTMPTextProfile validProfile =>
            _profile; // ?  _profile : LocalizedTMPTextProfile.defaultProfile;

        public string textName {
            get => _textName;
            set {
                if (managed)
                {
                    if (_textName != value)
                    {
                        _textName = value;
                        UpdateContent();

#if UNITY_EDITOR
                        _trackedTextName = _textName;
#endif
                    }
                }
                else _textName = value;
            }
        }


        public LocalizedTMPTextProfile profile {
            get => _profile;
            set {
                if (_profile != value)
                {
                    if (managed)
                    {
                        if (validProfile != null && validProfile.Contains(_node)) validProfile.Unregister(_node);
                        else
                        {
                            // If asset was deleted need fix _node here
                            if (_node.List != null) _node.List.Remove(_node);
                        }
                    }

                    _profile = value;

                    if (managed) validProfile?.Register(_node);

#if UNITY_EDITOR
                    _trackedProfile = _profile;
#endif
                }
            }
        }


        public override void UpdateContent()
        {
            if (languageIndex >= 0)
            {
                var text = LocalizationManager.GetText(_textName);
                if (text != null)
                {
// #if UNITY_EDITOR
                    text = PreHandleText(text);
                    // set text if _textName is valid
                    targetTMPText.text = text;
// #endif
                }
            }
        }


        protected override void OnEnable()
        {
            if (_node == null) _node = new LinkedListNode<TMP_Text>(targetTMPText);
            validProfile?.Register(_node);

            base.OnEnable();

#if UNITY_EDITOR
            _trackedTextName = _textName;
            _trackedProfile = _profile;
#endif
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            if (validProfile != null && validProfile.Contains(_node)) validProfile.Unregister(_node);
            else
            {
                // If asset was deleted need fix _node here
                if (_node.List != null) _node.List.Remove(_node);
            }
        }


#if UNITY_EDITOR
        string _trackedTextName;
        LocalizedTMPTextProfile _trackedProfile;


        void OnDestroy()
        {
            targetTMPText.hideFlags &= ~HideFlags.HideInInspector;
        }


        void OnValidate()
        {
            if (_trackedProfile != _profile)
            {
                var newProfile = _profile;
                _profile = _trackedProfile;
                profile = newProfile;
            }

            if (_trackedTextName != _textName)
            {
                var newName = _textName;
                _textName = _trackedTextName;
                textName = newName;
            }
        }
#endif
        
        protected string PreHandleText(string source)
        {
            string text = source;

            if (_upperCase)
                text = text.ToUpper();
            else if (_upperCaseEachWord)
            {
                text = text.ToTitleCase();
            }

            if (!_insertBefore.IsNullOrEmpty())
                text = _insertBefore + text;
            if (!_insertAfter.IsNullOrEmpty())
                text += _insertAfter;

            return text;
        }
    }
} // namespace UnityExtensions.Localization