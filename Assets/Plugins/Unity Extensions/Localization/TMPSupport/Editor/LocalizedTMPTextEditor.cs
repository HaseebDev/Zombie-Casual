#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions.Localization.Editor
{
    [CustomEditor(typeof(LocalizedTMPText), true)]
    public class LocalizedTMPTextEditor : BaseEditor<LocalizedTMPText>
    {
        SerializedProperty _textName;
        SerializedProperty _profile;
        SerializedProperty _upperCase;
        SerializedProperty _upperCaseEachWord;
        SerializedProperty _insertBefore;
        SerializedProperty _insertAfter;

        private bool _isUpperCase;

        void OnEnable()
        {
            _textName = serializedObject.FindProperty("_textName");
            _profile = serializedObject.FindProperty("_profile");
            _upperCase = serializedObject.FindProperty("_upperCase");
            _upperCaseEachWord = serializedObject.FindProperty("_upperCaseEachWord");
            _insertAfter = serializedObject.FindProperty("_insertAfter");
            _insertBefore = serializedObject.FindProperty("_insertBefore");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool visible = (target.targetTMPText.hideFlags & HideFlags.HideInInspector) == 0;
            if (EditorGUIUtilities.IndentedButton(visible ? "Hide TMP Text" : "Show TMP Text"))
            {
                if (visible) target.targetTMPText.hideFlags |= HideFlags.HideInInspector;
                else target.targetTMPText.hideFlags &= ~HideFlags.HideInInspector;
                EditorUtility.SetDirty(target.targetTMPText);
            }
            
            if (EditorGUIUtilities.IndentedButton("Update"))
            {
                target.UpdateContent();
            }

            var color = GUI.backgroundColor;
            if (LocalizationManager.isMetaLoaded && !LocalizationManager.HasText(_textName.stringValue))
            {
                color = new Color(1, 0.7f, 0.7f);
            }

            using (GUIBackgroundColorScope.New(color))
            {
                EditorGUILayout.PropertyField(_textName);
            }

            EditorGUILayout.PropertyField(_upperCase);
            EditorGUILayout.PropertyField(_upperCaseEachWord);
            EditorGUILayout.PropertyField(_insertBefore);
            EditorGUILayout.PropertyField(_insertAfter);
          

            EditorGUILayout.Space();

            var rect = EditorGUILayout.GetControlRect();
            var rect2 = rect;
            rect.width -= rect.height + 10;
            rect2.xMin = rect.xMax + 2;

            EditorGUI.PropertyField(rect, _profile);
            if (GUI.Button(rect2, "+", EditorStyles.miniButton))
            {
                var profile = CreateInstance<LocalizedTMPTextProfile>();
                AssetUtilities.CreateAsset(profile);
                _profile.objectReferenceValue = profile;
            }

            serializedObject.ApplyModifiedProperties();

            // Draw profile contents
            target.validProfile?.OnGUI();
        }
        
#if UNITY_EDITOR

        [MenuItem("GameObject/UI/Localized TMP Text")]
        static void CreateGameObject(MenuCommand menuCommand)
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<LocalizedTMPTextUI>();

            var menuOptionsType =
                typeof(UnityEditor.UI.TextEditor).GetOtherTypeInSameAssembly("UnityEditor.UI.MenuOptions");
            menuOptionsType.InvokeMethod("PlaceUIElementRoot", gameObject, menuCommand);
        }

#endif
    } // class LocalizedTMPTextEditor
} // UnityExtensions.Localization.Editor

#endif