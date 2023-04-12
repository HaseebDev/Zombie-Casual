using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
using UnityExtensions.Localization.Editor;
#endif

namespace UnityExtensions.Localization
{
    [Serializable]
    public class TextSettings
    {
        public Font font;
        public FontStyle fontStyle;
        public int fontSize;
        public float lineSpacing;
        public TextAnchor alignment;
        public bool alignByGeometry;
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow;
        public bool bestFit;
        public int minSize;
        public int maxSize;
        public float typewriterSpeed;
        public Vector2 positionOffset;

        public string languageType;
        public int overrides;

        public const int FontBit = 0;
        public const int FontStyleBit = 1;
        public const int FontSizeBit = 2;
        public const int LineSpacingBit = 3;
        public const int AlignmentBit = 4;
        public const int AlignByGeometryBit = 5;
        public const int HorizontalOverflowBit = 6;
        public const int VerticalOverflowBit = 7;
        public const int TypewriterSpeedBit = 8;
        public const int PositionOffsetBit = 9;

        public bool hasOverride => overrides != 0;

        public static TextSettings NewDefault()
        {
            return new TextSettings
            {
                languageType = string.Empty,

                font = Resources.GetBuiltinResource<Font>("Arial.ttf"),
                fontStyle = FontStyle.Normal,
                fontSize = 16,
                lineSpacing = 1f,
                alignment = TextAnchor.UpperLeft,
                alignByGeometry = false,
                horizontalOverflow = HorizontalWrapMode.Wrap,
                verticalOverflow = VerticalWrapMode.Overflow,
                bestFit = false,
                minSize = 8,
                maxSize = 48,
                typewriterSpeed = 30,
                positionOffset = default,
            };
        }

        public TextSettings Clone()
        {
            return MemberwiseClone() as TextSettings;
        }

        public void ValidateSize()
        {
            fontSize = Mathf.Clamp(fontSize, 0, 300);
            minSize = Mathf.Clamp(minSize, 0, fontSize);
            maxSize = Mathf.Clamp(maxSize, fontSize, 300);
        }
    }


    [CreateAssetMenu(menuName = "Localization/Localized Text Profile")]
    public partial class LocalizedTextProfile : ScriptableAsset, ISerializationCallbackReceiver
    {
        [SerializeField] TextSettings _defaultSettings;
        [SerializeField] TextSettings[] _languageSettingsArray;    // for serialization


        Dictionary<string, TextSettings> _languageSettings = new Dictionary<string, TextSettings>();

        TextSettings _currentSettings;      // never null

        System.Collections.Generic.LinkedList<LocalizedText> _textList = new System.Collections.Generic.LinkedList<LocalizedText>();


        TextSettings this[int bit] => (_currentSettings.overrides & (1 << bit)) == 0 ? _defaultSettings : _currentSettings;


        public Font font => this[TextSettings.FontBit].font;
        public FontStyle fontStyle => this[TextSettings.FontStyleBit].fontStyle;
        public int fontSize => this[TextSettings.FontSizeBit].fontSize;
        public float lineSpacing => this[TextSettings.LineSpacingBit].lineSpacing;
        public TextAnchor alignment => this[TextSettings.AlignmentBit].alignment;
        public bool alignByGeometry => this[TextSettings.AlignByGeometryBit].alignByGeometry;
        public HorizontalWrapMode horizontalOverflow => this[TextSettings.HorizontalOverflowBit].horizontalOverflow;
        public VerticalWrapMode verticalOverflow => this[TextSettings.VerticalOverflowBit].verticalOverflow;
        public bool bestFit => this[TextSettings.FontSizeBit].bestFit;
        public int minSize => this[TextSettings.FontSizeBit].minSize;
        public int maxSize => this[TextSettings.FontSizeBit].maxSize;
        public float typewriterSpeed => this[TextSettings.TypewriterSpeedBit].typewriterSpeed;
        public Vector2 positionOffset => this[TextSettings.PositionOffsetBit].positionOffset;


        public bool Contains(LinkedListNode<LocalizedText> node)
        {
            return node.List == _textList;
        }


        public void Register(LinkedListNode<LocalizedText> node)
        {
            if (_textList.Count == 0)
            {
                UpdateSettings();
                LocalizationManager.beforeContentsChange += BeforeContentsChange;
                Font.textureRebuilt += FontTextureRebuilt;
            }

            node.Value.SetAllDirty();
            _textList.AddLast(node);
        }


        public void Unregister(LinkedListNode<LocalizedText> node)
        {
            _textList.Remove(node);

            if (_textList.Count == 0)
            {
                Font.textureRebuilt -= FontTextureRebuilt;
                LocalizationManager.beforeContentsChange -= BeforeContentsChange;
            }
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
                    item.SetAllDirty();
                }
            }
        }


        void FontTextureRebuilt(Font f)
        {
            if (font == f)
            {
                foreach (var node in _textList)
                {
                    node.OnFontTextureChanged();
                }
            }
        }


        void Reset()
        {
            // ensure default is not null
            _defaultSettings = TextSettings.NewDefault();
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
                _languageSettingsArray = new TextSettings[_languageSettings.Count];

            int index = 0;
            foreach (var item in _languageSettings.Values)
            {
                _languageSettingsArray[index++] = item;
            }
        }


        static LocalizedTextProfile _defaultProfile;
        internal static LocalizedTextProfile defaultProfile
        {
            get
            {
                if (!_defaultProfile)
                {
                    _defaultProfile = CreateInstance<LocalizedTextProfile>();
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


        TextSettings DoOverrideToggle(int bit, out Rect rect2)
        {
            var rect = EditorGUILayout.GetControlRect();
            rect2 = rect;
            rect.width = rect.height;
            rect2.xMin = rect.xMax;

            bool newOverride = EditorGUI.ToggleLeft(rect, EditorGUIUtilities.TempContent(tooltip: "Override this for current language."), _currentSettings.overrides.GetBit(bit));
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
                            var settings = DoOverrideToggle(TextSettings.FontBit, out Rect rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.ObjectField(rect, "Font", settings.font, typeof(Font), false);
                                if (scope.changed) settings.font = newValue as Font;
                            }

                            // FontStyle
                            settings = DoOverrideToggle(TextSettings.FontStyleBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.EnumPopup(rect, "Font Style", settings.fontStyle);
                                if (scope.changed) settings.fontStyle = (FontStyle)newValue;
                            }

                            // FontSize
                            settings = DoOverrideToggle(TextSettings.FontSizeBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.IntField(rect, "Font Size", settings.fontSize);
                                if (scope.changed)
                                {
                                    settings.fontSize = newValue;
                                    settings.ValidateSize();
                                }
                            }

                            using (IndentLevelScope.New())
                            {
                                // BestFit
                                rect = EditorGUILayout.GetControlRect();
                                rect.xMin += rect.height;
                                using (var scope = ChangeCheckScope.New(this))
                                {
                                    var newValue = EditorGUI.Toggle(rect, "Best Fit", settings.bestFit);
                                    if (scope.changed) settings.bestFit = newValue;
                                }

                                if (settings.bestFit)
                                {
                                    using (IndentLevelScope.New())
                                    {
                                        // MinSize
                                        rect = EditorGUILayout.GetControlRect();
                                        rect.xMin += rect.height;
                                        using (var scope = ChangeCheckScope.New(this))
                                        {
                                            var newValue = EditorGUI.IntField(rect, "Min Size", settings.minSize);
                                            if (scope.changed)
                                            {
                                                settings.minSize = newValue;
                                                settings.ValidateSize();
                                            }
                                        }

                                        // MaxSize
                                        rect = EditorGUILayout.GetControlRect();
                                        rect.xMin += rect.height;
                                        using (var scope = ChangeCheckScope.New(this))
                                        {
                                            var newValue = EditorGUI.IntField(rect, "Max Size", settings.maxSize);
                                            if (scope.changed)
                                            {
                                                settings.maxSize = newValue;
                                                settings.ValidateSize();
                                            }
                                        }
                                    }
                                }
                            }

                            // LineSpacing
                            settings = DoOverrideToggle(TextSettings.LineSpacingBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.FloatField(rect, "Line Spacing", settings.lineSpacing);
                                if (scope.changed) settings.lineSpacing = newValue;
                            }

                            // Alignment
                            settings = DoOverrideToggle(TextSettings.AlignmentBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = Utilities.TextAlignmentField(rect, EditorGUIUtilities.TempContent("Alignment"), settings.alignment);
                                if (scope.changed) settings.alignment = newValue;
                            }

                            // AlignByGeometry
                            settings = DoOverrideToggle(TextSettings.AlignByGeometryBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.Toggle(rect, "Align by Geometry", settings.alignByGeometry);
                                if (scope.changed) settings.alignByGeometry = newValue;
                            }

                            // HorizontalOverflow
                            settings = DoOverrideToggle(TextSettings.HorizontalOverflowBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.EnumPopup(rect, "Horizontal Overflow", settings.horizontalOverflow);
                                if (scope.changed) settings.horizontalOverflow = (HorizontalWrapMode)newValue;
                            }

                            // VerticalOverflow
                            settings = DoOverrideToggle(TextSettings.VerticalOverflowBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.EnumPopup(rect, "Vertical Overflow", settings.verticalOverflow);
                                if (scope.changed) settings.verticalOverflow = (VerticalWrapMode)newValue;
                            }

                            // PositionOffset
                            settings = DoOverrideToggle(TextSettings.PositionOffsetBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {

                                var newValue = EditorGUIUtilities.SingleLineVector2Field(rect, settings.positionOffset, EditorGUIUtilities.TempContent("Position Offset"));
                                if (scope.changed) settings.positionOffset = newValue;
                            }

                            // TypewriterSpeed
                            settings = DoOverrideToggle(TextSettings.TypewriterSpeedBit, out rect);
                            using (var scope = ChangeCheckScope.New(this))
                            {
                                var newValue = EditorGUI.FloatField(rect, "Typewriter Speed", settings.typewriterSpeed);
                                if (scope.changed) settings.typewriterSpeed = Mathf.Max(newValue, 0);
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

    } // class LocalizedTextProfile

} // namespace UnityExtensions.Localization