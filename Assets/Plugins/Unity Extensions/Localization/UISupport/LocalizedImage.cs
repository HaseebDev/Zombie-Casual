using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.Localization
{
    [System.Serializable]
    public struct LocalizedSprite
    {
        public string languageType;
        public Sprite sprite;
    }


    [AddComponentMenu("UI/Localized Image")]
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public partial class LocalizedImage : LocalizedComponent, ISerializationCallbackReceiver
    {
        [SerializeField] bool _autoSetNativeSise = true;
        [SerializeField] LocalizedSprite[] _spritesArray;    // for serialization

        Image _target;

        Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        public Image targetImage => _target ? _target : _target = GetComponent<Image>();


        public override void UpdateContent()
        {
            UpdateContent(LocalizationManager.languageType);
        }


        public void UpdateContent(string languageType)
        {
            if (_sprites.TryGetValue(languageType, out var sprite))
            {
                targetImage.sprite = sprite;
                if (_autoSetNativeSise) targetImage.SetNativeSize();
            }
        }


        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _sprites.Clear();
            if (!RuntimeUtilities.IsNullOrEmpty(_spritesArray))
            {
                foreach (var item in _spritesArray)
                {
                    _sprites.Add(item.languageType, item.sprite);
                }
            }
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_spritesArray == null || _spritesArray.Length != _sprites.Count)
                _spritesArray = new LocalizedSprite[_sprites.Count];

            int index = 0;
            foreach (var item in _sprites)
            {
                _spritesArray[index++] = new LocalizedSprite { languageType = item.Key, sprite = item.Value };
            }
        }


#if UNITY_EDITOR

        void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += () => { if (this) UpdateContent(LocalizationManager.languageType); };
        }

        public bool TryGetSprite(string languageType, out Sprite sprite)
        {
            return _sprites.TryGetValue(languageType, out sprite);
        }

        public void SetSprite(string languageType, Sprite sprite)
        {
            _sprites[languageType] = sprite;
        }

#endif
    }
}