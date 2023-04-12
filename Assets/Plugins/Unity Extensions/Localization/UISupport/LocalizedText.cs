using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityExtensions.Localization.Editor;
#endif


namespace UnityExtensions.Localization
{
    /// <summary>
    /// https://bitbucket.org/Unity-Technologies/ui/src/2019.1/UnityEngine.UI/UI/Core/Text.cs
    /// </summary>
    [AddComponentMenu("UI/Localized Text")]
    public partial class LocalizedText : MaskableGraphic, ILayoutElement, ILocalizedContent
    {
        [SerializeField] string _textName;                  // invalid name will not change text
        [SerializeField] bool _richText;
        [SerializeField, Tooltip("If check this, should use a typewriter material too.")]
        bool _typewriter;
        [SerializeField] LocalizedTextProfile _profile;


        int _languageIndex = -1;
        int _contentId = -1;
        string _text = string.Empty;        // never null
        LinkedListNode<LocalizedText> _node;

        int _typewriterCharacterCount = 0;
        bool _verticesDirty = true;


        private TextGenerator _textCache;
        private TextGenerator _textCacheForLayout;

        // We use this flag instead of Unregistering/Registering the callback to avoid allocation.
        bool _disableFontTextureRebuiltCallback = false;


        /// <summary>
        /// The text name of Localized text.
        /// </summary>
        public string textName
        {
            get => _textName;
            set
            {
                if (_contentId >= 0)
                {
                    if (_textName != value)
                    {
                        _textName = value;
                        OnLanguageOrTextNameChanged();

#if UNITY_EDITOR
                        _trackedTextName = _textName;
#endif
                    }
                }
                else _textName = value;
            }
        }


        int ILocalizedContent.languageIndex
        {
            get => _languageIndex;
            set
            {
                _languageIndex = value;
                OnLanguageOrTextNameChanged();
            }
        }


        /// <summary>
        /// The sharing profile used now.
        /// </summary>
        public LocalizedTextProfile profile
        {
            get => _profile;
            set
            {
                if (_profile != value)
                {
                    if (_contentId >= 0)
                    {
                        if (validProfile.Contains(_node)) validProfile.Unregister(_node);
                        else
                        {
                            // If asset was deleted need fix _node here
                            if (_node.List != null) _node.List.Remove(_node);
                            
                        }
                    }

                    _profile = value;

                    if (_contentId >= 0) validProfile.Register(_node);

#if UNITY_EDITOR
                    _trackedProfile = _profile;
#endif
                }
            }
        }


        public LocalizedTextProfile validProfile => _profile ? _profile : LocalizedTextProfile.defaultProfile;


        protected LocalizedText()
        {
            useLegacyMeshGeneration = false;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            cachedTextGenerator.Invalidate();

            if (_node == null) _node = new LinkedListNode<LocalizedText>(this);
            validProfile.Register(_node);

            _contentId = LocalizationManager.AddContent(this);

#if UNITY_EDITOR
            _trackedTextName = _textName;
            _trackedProfile = _profile;
#endif
        }


        protected override void OnDisable()
        {
            LocalizationManager.RemoveContent(_contentId);
            _contentId = -1;

            if (validProfile.Contains(_node)) validProfile.Unregister(_node);
            else
            {
                // If asset was deleted need fix _node here
                if (_node.List != null) _node.List.Remove(_node);
            }

            base.OnDisable();
        }


        void OnLanguageOrTextNameChanged()
        {
            if (_languageIndex >= 0)
            {
                var text = LocalizationManager.GetText(_textName);
                SetText(text);
            }
        }


        /// <summary>
        /// Called by the FontUpdateTracker when the texture associated with a font is modified.
        /// </summary>
        public void OnFontTextureChanged()
        {
            // Only invoke if we are not destroyed.
            if (!this)
                return;

            if (_disableFontTextureRebuiltCallback)
                return;

            cachedTextGenerator.Invalidate();

            if (!IsActive())
                return;

            // this is a bit hacky, but it is currently the
            // cleanest solution....
            // if we detect the font texture has changed and are in a rebuild loop
            // we just regenerate the verts for the new UV's
            if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
                UpdateGeometry();
            else
            {
                SetAllDirty();
            }
        }


        /// <summary>
        /// The cached TextGenerator used when generating visible Text.
        /// </summary>
        public TextGenerator cachedTextGenerator
        {
            get { return _textCache ?? (_textCache = (_text.Length != 0 ? new TextGenerator(_text.Length) : new TextGenerator())); }
        }


        /// <summary>
        /// The cached TextGenerator used when determine Layout
        /// </summary>
        public TextGenerator cachedTextGeneratorForLayout
        {
            get { return _textCacheForLayout ?? (_textCacheForLayout = new TextGenerator()); }
        }


        /// <summary>
        /// Text's texture comes from the font.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (font != null && font.material != null && font.material.mainTexture != null)
                    return font.material.mainTexture;

                if (m_Material != null)
                    return m_Material.mainTexture;

                return base.mainTexture;
            }
        }


        /// <summary>
        /// Text that's being displayed by the Text.
        /// </summary>
        /// <remarks>
        /// This is the string value of a Text component. Use this to read or edit the message displayed in Text.
        /// </remarks>
        public virtual string text
        {
            get => _text;
            set
            {
                if (_text != (value ?? string.Empty))
                {
                    SetText(value);
                }
            }
        }


        void SetText(string text)
        {
            _text = text ?? string.Empty;

            SetVerticesDirty();
            SetLayoutDirty();

#if UNITY_EDITOR
            // Force update in edit mode :(
            if (!Application.isPlaying && isActiveAndEnabled)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this && isActiveAndEnabled)
                    {
                        enabled = false;
                        enabled = true;
                    }
                };
            }
#endif
        }


        public bool richText
        {
            get => _richText;
            set
            {
                if (_richText == value)
                    return;
                _richText = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }


        public bool typewriter
        {
            get => _typewriter;
            set
            {
                if (_typewriter == value)
                    return;
                _typewriter = value;
                SetVerticesDirty();
            }
        }


        /// <summary>
        /// Used for typewriter effect.
        /// </summary>
        public int typewriterCharacterCount => _typewriterCharacterCount;


        /// <summary>
        /// The Font used by the text.
        /// </summary>
        /// <remarks>
        /// This is the font used by the Text component. Use it to alter or return the font from the Text. There are many free fonts available online.
        /// </remarks>
        public Font font => validProfile.font;


        /// <summary>
        /// Should the text be allowed to auto resized.
        /// </summary>
        public bool resizeTextForBestFit => validProfile.bestFit;


        /// <summary>
        /// The minimum size the text is allowed to be.
        /// </summary>
        public int resizeTextMinSize => validProfile.minSize;


        /// <summary>
        /// The maximum size the text is allowed to be. 1 = infinitely large.
        /// </summary>
        public int resizeTextMaxSize => validProfile.maxSize;
        

        /// <summary>
        /// The positioning of the text reliative to its [[RectTransform]].
        /// </summary>
        /// <remarks>
        /// This is the positioning of the Text relative to its RectTransform. You can alter this via script or in the Inspector of a Text component using the buttons in the Alignment section.
        /// </remarks>
        public TextAnchor alignment => validProfile.alignment;


        /// <summary>
        /// Use the extents of glyph geometry to perform horizontal alignment rather than glyph metrics.
        /// </summary>
        /// <remarks>
        /// This can result in better fitting left and right alignment, but may result in incorrect positioning when attempting to overlay multiple fonts (such as a specialized outline font) on top of each other.
        /// </remarks>
        public bool alignByGeometry => validProfile.alignByGeometry;
            

        /// <summary>
        /// The size that the Font should render at. Unit of measure is Points.
        /// </summary>
        /// <remarks>
        /// This is the size of the Font of the Text. Use this to fetch or change the size of the Font. When changing the Font size, remember to take into account the RectTransform of the Text. Larger Font sizes or messages may not fit in certain rectangle sizes and do not show in the Scene.
        /// Note: Point size is not consistent from one font to another.
        /// </remarks>
        public int fontSize => validProfile.fontSize;


        /// <summary>
        /// Horizontal overflow mode.
        /// </summary>
        /// <remarks>
        /// When set to HorizontalWrapMode.Overflow, text can exceed the horizontal boundaries of the Text graphic. When set to HorizontalWrapMode.Wrap, text will be word-wrapped to fit within the boundaries.
        /// </remarks>
        public HorizontalWrapMode horizontalOverflow => validProfile.horizontalOverflow;


        /// <summary>
        /// Vertical overflow mode.
        /// </summary>
        public VerticalWrapMode verticalOverflow => validProfile.verticalOverflow;


        /// <summary>
        /// Line spacing, specified as a factor of font line height. A value of 1 will produce normal line spacing.
        /// </summary>
        public float lineSpacing => validProfile.lineSpacing;


        /// <summary>
        /// Font style used by the Text's text.
        /// </summary>
        public FontStyle fontStyle => validProfile.fontStyle;


        public float typewriterSpeed => validProfile.typewriterSpeed;


        public Vector2 positionOffset => validProfile.positionOffset;


        /// <summary>
        /// Provides information about how fonts are scale to the screen.
        /// </summary>
        /// <remarks>
        /// For dynamic fonts, the value is equivalent to the scale factor of the canvas. For non-dynamic fonts, the value is calculated from the requested text size and the size from the font.
        /// </remarks>
        public float pixelsPerUnit
        {
            get
            {
                var localCanvas = canvas;
                if (!localCanvas)
                    return 1;
                // For dynamic fonts, ensure we use one pixel per pixel on the screen.
                if (!font || font.dynamic)
                    return localCanvas.scaleFactor;
                // For non-dynamic fonts, calculate pixels per unit based on specified font size relative to font object's own font size.
                if (validProfile.fontSize <= 0 || font.fontSize <= 0)
                    return 1;
                return font.fontSize / (float)validProfile.fontSize;
            }
        }


        public override void SetVerticesDirty()
        {
            _verticesDirty = true;
            base.SetVerticesDirty();
        }


        protected override void UpdateGeometry()
        {
            if (font != null)
            {
                base.UpdateGeometry();
                _verticesDirty = false;
            }
        }


        /// <summary>
        /// Convenience function to populate the generation setting for the text.
        /// </summary>
        /// <param name="extents">The extents the text can draw in.</param>
        /// <returns>Generated settings.</returns>
        public TextGenerationSettings GetGenerationSettings(Vector2 extents)
        {
            var settings = new TextGenerationSettings();

            settings.generationExtents = extents;
            if (font != null && font.dynamic)
            {
                settings.fontSize = validProfile.fontSize;
                settings.resizeTextMinSize = validProfile.minSize;
                settings.resizeTextMaxSize = validProfile.maxSize;
            }

            // Other settings
            settings.textAnchor = validProfile.alignment;
            settings.alignByGeometry = validProfile.alignByGeometry;
            settings.scaleFactor = pixelsPerUnit;
            settings.color = color;
            settings.font = font;
            settings.pivot = rectTransform.pivot;
            settings.richText = _richText;
            settings.lineSpacing = validProfile.lineSpacing;
            settings.fontStyle = validProfile.fontStyle;
            settings.resizeTextForBestFit = validProfile.bestFit;
            settings.updateBounds = false;
            settings.horizontalOverflow = validProfile.horizontalOverflow;
            settings.verticalOverflow = validProfile.verticalOverflow;

            return settings;
        }


        readonly UIVertex[] _tempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            _typewriterCharacterCount = 0;

            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            _disableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            int vertCount = verts.Count;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            Vector3 positionOffset = this.positionOffset;

            Vector2 roundingOffset = (verts[0].position + positionOffset) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();

            bool isValidCharacter = false;

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position = unitsPerPixel * (_tempVerts[tempVertsIndex].position + positionOffset);
                    _tempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    _tempVerts[tempVertsIndex].position.y += roundingOffset.y;

                    if (_typewriter)
                    {
                        // 判断是否是有宽度的字符，rich text 标记是没有宽度的，不计入有效字符
                        if (tempVertsIndex == 0)
                        {
                            isValidCharacter = (verts[i].position - verts[i+1].position).sqrMagnitude > 0;
                            if (isValidCharacter) _typewriterCharacterCount++;
                        }

                        if (isValidCharacter)
                        {
                            _tempVerts[tempVertsIndex].position.z = _typewriterCharacterCount + ((tempVertsIndex == 0 || tempVertsIndex == 3) ? -1 : -0.5f);
                        }
                    }
                    
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(_tempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position = unitsPerPixel * (_tempVerts[tempVertsIndex].position + positionOffset);

                    if (_typewriter)
                    {
                        // 判断是否是有宽度的字符，rich text 标记是没有宽度的，不计入有效字符
                        if (tempVertsIndex == 0)
                        {
                            isValidCharacter = (verts[i].position - verts[i + 1].position).sqrMagnitude > 0;
                            if (isValidCharacter) _typewriterCharacterCount++;
                        }

                        if (isValidCharacter)
                        {
                            _tempVerts[tempVertsIndex].position.z = _typewriterCharacterCount + ((tempVertsIndex == 0 || tempVertsIndex == 3) ? -1 : -0.5f);
                        }
                    }

                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(_tempVerts);
                }
            }

            _disableFontTextureRebuiltCallback = false;
        }


        public virtual void CalculateLayoutInputHorizontal() { }

        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth
        {
            get { return 0; }
        }

        public virtual float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(Vector2.zero);
                return cachedTextGeneratorForLayout.GetPreferredWidth(_text, settings) / pixelsPerUnit;
            }
        }

        public virtual float flexibleWidth { get { return -1; } }

        public virtual float minHeight
        {
            get { return 0; }
        }

        public virtual float preferredHeight
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
                return cachedTextGeneratorForLayout.GetPreferredHeight(_text, settings) / pixelsPerUnit;
            }
        }

        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return 0; } }

        public bool isVerticesDirty => _verticesDirty;


#if UNITY_EDITOR

        string _trackedTextName;
        LocalizedTextProfile _trackedProfile;


        protected override void OnValidate()
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

            base.OnValidate();
        }


        public override void OnRebuildRequested()
        {
            cachedTextGenerator.Invalidate();

            base.OnRebuildRequested();
        }


        [ContextMenu("Open Localization Window")]
        void OpenLocalizationWindow()
        {
            LocalizationWindow.ShowWindow();
        }


        [ContextMenu("Open Localization Folder")]
        void OpenLocalizatioFolder()
        {
            LanguagePacker.ShowSourceFolder();
        }

        [ContextMenu("Reload Localization Meta")]
        void ReloadLocalizationMeta()
        {
            LocalizationSettings.instance.ReloadMeta();
        }
#endif

    }
}
