#if UNITY_EDITOR

using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions.Localization.Editor
{
    [CustomEditor(typeof(TypewriterController), true)]
    public class TypewriterControllerEditor : BaseEditor<TypewriterController>
    {
        SerializedProperty _timeMode;
        SerializedProperty _actionOnLocalizationChange;

        private void OnEnable()
        {
            _timeMode = serializedObject.FindProperty("_timeMode");
            _actionOnLocalizationChange = serializedObject.FindProperty("_actionOnLocalizationChange");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_timeMode);
            EditorGUILayout.PropertyField(_actionOnLocalizationChange);
            serializedObject.ApplyModifiedProperties();

            using (DisabledScope.New(!target.targetText.typewriter || !target.targetText.isActiveAndEnabled))
            {
                if (EditorGUIUtilities.IndentedButton(target.isPlaying ? "Stop" : "Play"))
                {
                    if (target.isPlaying) target.Stop();
                    else target.Play();
                }
            }
        }

    }

} // UnityExtensions.Localization.Editor

#endif