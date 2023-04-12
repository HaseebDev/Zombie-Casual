using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
using UnityExtensions.Localization.Editor;
using TMPro.EditorUtilities;
#endif

namespace UnityExtensions.Localization
{
    [Serializable]
    public class TMPTextSettings
    {
        public TMP_FontAsset fontAsset;
        public TMP_SpriteAsset spriteAsset;

        public FontStyles fontStyle;

        public float fontSize;
        public bool autoFontSize;
        public float minFontSize;
        public float maxFontSize;
        public float characterWidthAdjustment;
        public float lineSpacingAdjustment;

        public float characterSpacing;
        public float wordSpacing;
        public float lineSpacing;
        public float paragraphSpacing;
        public bool kerning;

        public TextAlignmentOptions alignment;
        public float wordWrappingRatios;

        public bool wordWrapping;
        public TextOverflowModes overflow;
        public int pageToDisplay;
        public TMP_Text linkedText;

        public Vector4 margins;

        public string languageType;
        public int overrides;


        public const int FontAssetBit = 0;
        public const int SpriteAssetBit = 1;
        public const int FontStyleBit = 2;
        public const int FontSizeBit = 3;
        public const int CharacterSpacingBit = 4;
        public const int WordSpacingBit = 5;
        public const int LineSpacingBit = 6;
        public const int ParagraphSpacingBit = 7;
        public const int KerningBit = 8;
        public const int AlignmentBit = 9;
        public const int WordWrappingBit = 10;
        public const int OverflowBit = 11;
        public const int MarginsBit = 12;


        public bool hasOverride => overrides != 0;

        public static TMPTextSettings NewDefault()
        {
            return new TMPTextSettings
            {
                languageType = string.Empty,

                fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"),
                spriteAsset = null,
                fontStyle = FontStyles.Normal,
                fontSize = 32,
                autoFontSize = false,
                minFontSize = 16,
                maxFontSize = 64,
                characterWidthAdjustment = 0,
                lineSpacingAdjustment = 0,
                characterSpacing = 0,
                wordSpacing = 0,
                lineSpacing = 0,
                paragraphSpacing = 0,
                kerning = false,
                alignment = TextAlignmentOptions.TopLeft,
                wordWrappingRatios = 0.5f,
                wordWrapping = true,
                overflow = TextOverflowModes.Overflow,
                pageToDisplay = 1,
                linkedText = null,
                margins = default
            };
        }

        public TMPTextSettings Clone()
        {
            return MemberwiseClone() as TMPTextSettings;
        }
    }


    [CreateAssetMenu(menuName = "Localization/Localized TMP Text Profile")]
    public partial class LocalizedTMPTextProfile : ScriptableAsset, ISerializationCallbackReceiver
    {
        [SerializeField] TMPTextSettings _defaultSettings;
        [SerializeField] TMPTextSettings[] _languageSettingsArray;    // for serialization


        Dictionary<string, TMPTextSettings> _languageSettings = new Dictionary<string, TMPTextSettings>();

        TMPTextSettings _currentSettings;      // never null

        System.Collections.Generic.LinkedList<TMP_Text> _textList = new System.Collections.Generic.LinkedList<TMP_Text>();


        TMPTextSettings this[int bit] => (_currentSettings.overrides & (1 << bit)) == 0 ? _defaultSettings : _currentSettings;


        public bool Contains(LinkedListNode<TMP_Text> node)
        {
            return node.List == _textList;
        }


        public void Register(LinkedListNode<TMP_Text> node)
        {
            if (_textList.Count == 0)
            {
                UpdateSettings();
                LocalizationManager.beforeContentsChange += BeforeContentsChange;
            }

            ApplySettings(node.Value);
            _textList.AddLast(node);
        }


        public void Unregister(LinkedListNode<TMP_Text> node)
        {
            _textList.Remove(node);

            if (_textList.Count == 0)
            {
                LocalizationManager.beforeContentsChange -= BeforeContentsChange;
            }
        }


        void ApplySettings(TMP_Text target)
        {
            target.font = this[TMPTextSettings.FontAssetBit].fontAsset;
            // target.spriteAsset = this[TMPTextSettings.SpriteAssetBit].spriteAsset;
            // target.fontStyle = this[TMPTextSettings.FontStyleBit].fontStyle;
            // target.fontSize = this[TMPTextSettings.FontSizeBit].fontSize;
            // target.enableAutoSizing = this[TMPTextSettings.FontSizeBit].autoFontSize;
            // target.fontSizeMin = this[TMPTextSettings.FontSizeBit].minFontSize;
            // target.fontSizeMax = this[TMPTextSettings.FontSizeBit].maxFontSize;
            // target.characterWidthAdjustment = this[TMPTextSettings.FontSizeBit].characterWidthAdjustment;
            // target.lineSpacingAdjustment = this[TMPTextSettings.FontSizeBit].lineSpacingAdjustment;
            // target.characterSpacing = this[TMPTextSettings.CharacterSpacingBit].characterSpacing;
            // target.wordSpacing = this[TMPTextSettings.WordSpacingBit].wordSpacing;
            // target.lineSpacing = this[TMPTextSettings.LineSpacingBit].lineSpacing;
            // target.paragraphSpacing = this[TMPTextSettings.ParagraphSpacingBit].paragraphSpacing;
            // target.enableKerning = this[TMPTextSettings.KerningBit].kerning;
            // target.alignment = this[TMPTextSettings.AlignmentBit].alignment;
            // target.wordWrappingRatios = this[TMPTextSettings.AlignmentBit].wordWrappingRatios;
            // target.enableWordWrapping = this[TMPTextSettings.WordWrappingBit].wordWrapping;
            // target.overflowMode = this[TMPTextSettings.OverflowBit].overflow;
            // target.pageToDisplay = this[TMPTextSettings.OverflowBit].pageToDisplay;
            target.linkedTextComponent = this[TMPTextSettings.OverflowBit].linkedText;
            // target.margin = this[TMPTextSettings.MarginsBit].margins;

#if UNITY_EDITOR
            // Force update in edit mode :(
            if (!Application.isPlaying && target != null &&  target.isActiveAndEnabled)
            {
                EditorApplication.delayCall += () =>
                {
                    if (target && target.isActiveAndEnabled)
                    {
                        target.enabled = false;
                        target.enabled = true;
                    }
                };
            }
#endif
        }


        void UpdateSettings()
        {
            if (!_languageSettings.TryGetValue(LocalizationManager.languageType, out _currentSettings))
            {
                // ensure current is not null
                _currentSettings = _defaultSettings;
            }
        }


        void BeforeContentsChange()
        {
            var oldSettings = _currentSettings;
            UpdateSettings();

#if !UNITY_EDITOR
            if (oldSettings != _currentSettings)
#endif
            {
                foreach (var item in _textList)
                {
                    ApplySettings(item);
                }
            }
        }


        void Reset()
        {
            // ensure default is not null
            _defaultSettings = TMPTextSettings.NewDefault();
            _currentSettings = _defaultSettings;
        }


        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _languageSettings.Clear();
            if (!RuntimeUtilities.IsNullOrEmpty(_languageSettingsArray))
            {
                foreach (var item in _languageSettingsArray)
                {
                    _languageSettings.Add(item.languageType, item);
                }
            }
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_languageSettingsArray == null || _languageSettingsArray.Length != _languageSettings.Count)
                _languageSettingsArray = new TMPTextSettings[_languageSettings.Count];

            int index = 0;
            foreach (var item in _languageSettings.Values)
            {
                _languageSettingsArray[index++] = item;
            }
        }


        static LocalizedTMPTextProfile _defaultProfile;
        public static LocalizedTMPTextProfile defaultProfile
        {
            get
            {
                if (_defaultProfile == null)
                {
                    _defaultProfile = CreateInstance<LocalizedTMPTextProfile>();
                    _defaultProfile.Reset();
                }
                return _defaultProfile;
            }
        }


#if UNITY_EDITOR

        void OnValidate()
        {
            BeforeContentsChange();
        }


        TMPTextSettings DoOverrideToggle(int bit, out Rect rect2)
        {
            var rect = EditorGUILayout.GetControlRect();
            rect2 = rect;
            rect.width = rect.height;
            rect2.xMin = rect.xMax;

            bool newOverride = EditorGUI.ToggleLeft(rect, EditorGUIUtilities.TempContent(tooltip:"Override this for current language."), _currentSettings.overrides.GetBit(bit));
            if (newOverride != _currentSettings.overrides.GetBit(bit))
            {
                Undo.RecordObject(this, name);

                if (newOverride)
                {
                    if (!_currentSettings.hasOverride) // 实际上 _currentSettings 此时就是 _defaultSettings
                    {
                        // 尚没有语言独立设置, 需要新建
                        _currentSettings = _defaultSettings.Clone();
                        _currentSettings.languageType = LocalizationManager.languageType;
                        _languageSettings.Add(_currentSettings.languageType, _currentSettings);
                    }
                    _currentSettings.overrides.SetBit1(bit);
                }
                else
                {
                    _currentSettings.overrides.SetBit0(bit);
                    if (!_currentSettings.hasOverride)
                    {
                        // 已经失去了所有覆写，删除此设置
                        _languageSettings.Remove(_currentSettings.languageType);
                        _currentSettings = _defaultSettings;
                    }
                }
            }

            return newOverride ? _currentSettings : _defaultSettings;
        }


        public void OnGUI()
        {
            if (_defaultProfile != this && _textList.Count == 0)
            {
                EditorGUILayout.HelpBox("This profile is unused now.", MessageType.None);
                return;
            }

            using (VerticalLayoutScope.New(EditorStyles.helpBox))
            {
                EditorGUILayout.GetControlRect(false, 2);
                LocalizationSettings.instance.DrawLanguageSelection();
                EditorGUILayout.GetControlRect(false, 2);

                using (DisabledScope.New(_defaultProfile == this || string.IsNullOrEmpty(LocalizationManager.languageType)))
                {
                    using (LabelWidthScope.New(EditorGUIUtility.labelWidth - EditorGUIUtility.singleLineHeight - 4))
                    {
                        using (var scopeOutside = ChangeCheckScope.New())
                        {
                            // Font
                            var settings = DoOverrideToggle(TMPTextSettings.FontAssetBit, out Rect rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.ObjectField(rect, "Font Asset", settings.fontAsset, typeof(TMP_FontAsset), false);
                                if (scope.changed) settings.fontAsset = newValue as TMP_FontAsset;
                            }

                            // SpriteAsset
                            settings = DoOverrideToggle(TMPTextSettings.SpriteAssetBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.ObjectField(rect, "Sprite Asset", settings.spriteAsset, typeof(TMP_SpriteAsset), false);
                                if (scope.changed) settings.spriteAsset = newValue as TMP_SpriteAsset;
                            }

                            // FontStyle
                            settings = DoOverrideToggle(TMPTextSettings.FontStyleBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = Utilities.FontStyleField(rect, EditorGUIUtilities.TempContent("Font Style"), settings.fontStyle);
                                if (scope.changed) settings.fontStyle = newValue;
                            }

                            // FontSize
                            settings = DoOverrideToggle(TMPTextSettings.FontSizeBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                using (DisabledScope.New(settings.autoFontSize))
                                {
                                    var newValue = EditorGUI.FloatField(rect, "Font Size", settings.fontSize);
                                    if (scope.changed) settings.fontSize = Mathf.Max(newValue, 0);
                                }
                            }

                            using (IndentLevelScope.New())
                            {
                                // AutoFontSize
                                rect = EditorGUILayout.GetControlRect();
                                rect.xMin += rect.height;
                                using (var scope = ChangeCheckScope.New(this))
                                {
                                    var newValue = EditorGUI.Toggle(rect, "Auto Size", settings.autoFontSize);
                                    if (scope.changed) settings.autoFontSize = newValue;
                                }

                                if (settings.autoFontSize)
                                {
                                    using (IndentLevelScope.New())
                                    {
                                        // MinSize
                                        rect = EditorGUILayout.GetControlRect();
                                        rect.xMin += rect.height;
                                        using (var scope = ChangeCheckScope.New(this))
                                        {
                                            var newValue = EditorGUI.FloatField(rect, "Min Size", settings.minFontSize);
                                            if (scope.changed) settings.minFontSize = Mathf.Max(newValue, 0);
                                        }

                                        // MaxSize
                                        rect = EditorGUILayout.GetControlRect();
                                        rect.xMin += rect.height;
                                        using (var scope = ChangeCheckScope.New(this))
                                        {
                                            var newValue = EditorGUI.FloatField(rect, "Max Size", settings.maxFontSize);
                                            if (scope.changed) settings.maxFontSize = Mathf.Max(newValue, settings.minFontSize);
                                        }

                                        // WidthAdjustment
                                        rect = EditorGUILayout.GetControlRect();
                                        rect.xMin += rect.height;
                                        using (var scope = ChangeCheckScope.New(this))
                                        {
                                            var newValue = EditorGUI.Slider(rect, "Width Adjustment", settings.characterWidthAdjustment, 0, 50);
                                            if (scope.changed) settings.characterWidthAdjustment = newValue;
                                        }

                                        // LineAdjustment
                                        rect = EditorGUILayout.GetControlRect();
                                        rect.xMin += rect.height;
                                        using (var scope = ChangeCheckScope.New(this))
                                        {
                                            var newValue = EditorGUI.FloatField(rect, "Line Adjustment", settings.lineSpacingAdjustment);
                                            if (scope.changed) settings.lineSpacingAdjustment = Mathf.Min(newValue, 0);
                                        }
                                    }
                                }
                            }

                            // CharacterSpacing
                            settings = DoOverrideToggle(TMPTextSettings.CharacterSpacingBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.FloatField(rect, "Character Spacing", settings.characterSpacing);
                                if (scope.changed) settings.characterSpacing = newValue;
                            }

                            // WordSpacing
                            settings = DoOverrideToggle(TMPTextSettings.WordSpacingBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.FloatField(rect, "Word Spacing", settings.wordSpacing);
                                if (scope.changed) settings.wordSpacing = newValue;
                            }

                            // LineSpacing
                            settings = DoOverrideToggle(TMPTextSettings.LineSpacingBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.FloatField(rect, "Line Spacing", settings.lineSpacing);
                                if (scope.changed) settings.lineSpacing = newValue;
                            }

                            // ParagraphSpacing
                            settings = DoOverrideToggle(TMPTextSettings.ParagraphSpacingBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.FloatField(rect, "Paragraph Spacing", settings.paragraphSpacing);
                                if (scope.changed) settings.paragraphSpacing = newValue;
                            }

                            // Kerning
                            settings = DoOverrideToggle(TMPTextSettings.KerningBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.Toggle(rect, "Kerning", settings.kerning);
                                if (scope.changed) settings.kerning = newValue;
                            }

                            // Alignment
                            settings = DoOverrideToggle(TMPTextSettings.AlignmentBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = Utilities.TextAlignmentField(rect, EditorGUIUtilities.TempContent("Alignment"), settings.alignment);
                                if (scope.changed) settings.alignment = newValue;
                            }

                            var hAlignment = TMP_EditorUtility.GetHorizontalAlignmentGridValue((int)settings.alignment);
                            if (hAlignment == 3 || hAlignment == 4)
                            {
                                using (IndentLevelScope.New())
                                {
                                    // WordWrappingRatios
                                    rect = EditorGUILayout.GetControlRect();
                                    rect.xMin += rect.height;
                                    using (var scope = ChangeCheckScope.New(this))
                                    {
                                        var newValue = EditorGUI.Slider(rect, "Distrubution", settings.wordWrappingRatios, 0, 1);
                                        if (scope.changed) settings.wordWrappingRatios = newValue;
                                    }
                                }
                            }

                            // HorizontalOverflow
                            settings = DoOverrideToggle(TMPTextSettings.WordWrappingBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.Toggle(rect, "Wrapping", settings.wordWrapping);
                                if (scope.changed) settings.wordWrapping = newValue;
                            }

                            // VerticalOverflow
                            settings = DoOverrideToggle(TMPTextSettings.OverflowBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.EnumPopup(rect, "Overflow", settings.overflow);
                                if (scope.changed) settings.overflow = (TextOverflowModes)newValue;
                            }

                            if (settings.overflow == TextOverflowModes.Page)
                            {
                                using (IndentLevelScope.New())
                                {
                                    // OverflowPage
                                    rect = EditorGUILayout.GetControlRect();
                                    rect.xMin += rect.height;
                                    using (var scope = ChangeCheckScope.New(this))
                                    {
                                        var newValue = EditorGUI.IntField(rect, " ", settings.pageToDisplay);
                                        if (scope.changed) settings.pageToDisplay = newValue;
                                    }
                                }
                            }
                            else if (settings.overflow == TextOverflowModes.Linked)
                            {
                                using (IndentLevelScope.New())
                                {
                                    // OverflowLinked
                                    rect = EditorGUILayout.GetControlRect();
                                    rect.xMin += rect.height;
                                    using (var scope = ChangeCheckScope.New(this))
                                    {
                                        var newValue = EditorGUI.ObjectField(rect, " ", settings.linkedText, typeof(TMP_Text), true);
                                        if (scope.changed) settings.linkedText = newValue as TMP_Text;
                                    }
                                }
                            }

                            // Margins
                            settings = DoOverrideToggle(TMPTextSettings.MarginsBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = Utilities.MarginsField(rect, EditorGUIUtilities.TempContent("Margins"), settings.margins);
                                if (scope.changed) settings.margins = newValue;
                            }

                            if (scopeOutside.changed)
                            {
                                EditorUtility.SetDirty(this);
                                OnValidate();
                            }
                        }
                    }
                }
            }
        }

#endif

    } // class LocalizedTMPTextProfile

} // namespace UnityExtensions.Localization