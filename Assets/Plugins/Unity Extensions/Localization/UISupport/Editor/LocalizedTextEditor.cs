#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityExtensions.Editor;

namespace UnityExtensions.Localization.Editor
{
    [CustomEditor(typeof(LocalizedText), true)]
    public class LocalizedTextEditor : GraphicEditor
    {
        SerializedProperty _textName;
        SerializedProperty _richText;
        SerializedProperty _typewriter;
        SerializedProperty _profile;

        protected override void OnEnable()
        {
            base.OnEnable();
            _textName = serializedObject.FindProperty("_textName");
            _richText = serializedObject.FindProperty("_richText");
            _typewriter = serializedObject.FindProperty("_typewriter");
            _profile = serializedObject.FindProperty("_profile");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var color = GUI.backgroundColor;
            if (LocalizationManager.isMetaLoaded && !LocalizationManager.HasText(_textName.stringValue))
            {
                color = new Color(1, 0.7f, 0.7f);
            }

            using (GUIBackgroundColorScope.New(color))
            {
                EditorGUILayout.PropertyField(_textName);
            }

            AppearanceControlsGUI();
            RaycastControlsGUI();
            EditorGUILayout.PropertyField(_richText);
            EditorGUILayout.PropertyField(_typewriter);

            if (_typewriter.boolValue)
            {
                EditorGUILayout.HelpBox("Remember to use a material that supports typewriter effect.", MessageType.None);
            }

            EditorGUILayout.Space();

            var rect = EditorGUILayout.GetControlRect();
            var rect2 = rect;
            rect.width -= rect.height + 10;
            rect2.xMin = rect.xMax + 2;

            EditorGUI.PropertyField(rect, _profile);
            if (GUI.Button(rect2, "+", EditorStyles.miniButton))
            {
                var profile = CreateInstance<LocalizedTextProfile>();
                AssetUtilities.CreateAsset(profile);
                _profile.objectReferenceValue = profile;
            }

            serializedObject.ApplyModifiedProperties();

            // Draw profile contents
            (target as LocalizedText).validProfile.OnGUI();
        }


        [MenuItem("GameObject/UI/Localized Text")]
        static void CreateGameObject(MenuCommand menuCommand)
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<LocalizedText>();

            var menuOptionsType = typeof(UnityEditor.UI.TextEditor).GetOtherTypeInSameAssembly("UnityEditor.UI.MenuOptions");
            menuOptionsType.InvokeMethod("PlaceUIElementRoot", gameObject, menuCommand);
        }

    } // class LocalizedTextEditor

} // UnityExtensions.Localization.Editor

#endif