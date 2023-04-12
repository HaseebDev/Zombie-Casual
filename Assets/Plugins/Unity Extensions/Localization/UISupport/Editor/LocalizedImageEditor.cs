#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions.Localization.Editor
{
    [CustomEditor(typeof(LocalizedImage), true)]
    public class LocalizedImageEditor : BaseEditor<LocalizedImage>
    {
        SerializedProperty _autoSetNativeSise;

        void OnEnable()
        {

            _autoSetNativeSise = serializedObject.FindProperty("_autoSetNativeSise");
        }


        void OnDisable()
        {
            target.UpdateContent(LocalizationManager.languageType);
        }


        public override bool RequiresConstantRepaint()
        {
            return true;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_autoSetNativeSise);
            serializedObject.ApplyModifiedProperties();

            if (!LocalizationManager.isMetaLoaded)
            {
                EditorGUILayout.HelpBox("Need load meta before editing.", MessageType.Warning);
            }

            int mouseHoverIndex = -1;

            for (int i = 0; i < LocalizationManager.languageCount; i++)
            {
                var languageType = LocalizationManager.GetLanguageType(i);
                target.TryGetSprite(languageType, out var sprite);

                using (var scope = ChangeCheckScope.New(target))
                {
                    var rect = EditorGUILayout.GetControlRect();
                    bool mouseIn = rect.Contains(Event.current.mousePosition);

                    using (GUIBackgroundColorScope.New(mouseIn ? new Color(1f, 1f, 0.6f) : GUI.backgroundColor))
                    {
                        var newSprite = EditorGUI.ObjectField(rect, LocalizationSettings.instance.GetLanguageLabel(i), sprite, typeof(Sprite), false);
                        if (scope.changed)
                        {
                            target.SetSprite(languageType, newSprite as Sprite);
                        }
                    }

                    if (mouseIn)
                    {
                        mouseHoverIndex = i;
                        target.UpdateContent(languageType);
                    }
                }
            }

            if (mouseHoverIndex < 0)
            {
                target.UpdateContent(LocalizationManager.languageType);
            }
        }


        [MenuItem("GameObject/UI/Localized Image")]
        static void CreateGameObject(MenuCommand menuCommand)
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<LocalizedImage>();

            var menuOptionsType = typeof(UnityEditor.UI.TextEditor).GetOtherTypeInSameAssembly("UnityEditor.UI.MenuOptions");
            menuOptionsType.InvokeMethod("PlaceUIElementRoot", gameObject, menuCommand);
        }

    } // class LocalizedImageEditor

} // UnityExtensions.Localization.Editor

#endif