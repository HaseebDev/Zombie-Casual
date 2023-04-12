#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions.Localization.Editor
{
    public partial struct Utilities
    {
        static GUIContent[] _hAlignmentContents =
        {
            EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left_active", "Left Align"),
            EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center_active", "Center Align"),
            EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right_active", "Right Align")
        };


        static GUIContent[] _vAlignmentContents =
        {
            EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top_active", "Top Align"),
            EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center_active", "Middle Align"),
            EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom_active", "Bottom Align")
        };


        public static TextAnchor TextAlignmentField(Rect rect, GUIContent label, TextAnchor value)
        {
            using (GUIContentColorScope.New(EditorGUIUtilities.labelNormalColor))
            {
                EditorGUIUtility.SetIconSize(new Vector2(15, 15));

                rect = EditorGUI.PrefixLabel(rect, label);

                int hValue = ((int)value) % 3;
                rect.width = rect.height * 3.9f;
                hValue = GUI.Toolbar(rect, hValue, _hAlignmentContents, EditorStyles.miniButton);

                int vValue = ((int)value) / 3;
                rect.x = rect.xMax + rect.height * 0.43f;
                vValue = GUI.Toolbar(rect, vValue, _vAlignmentContents, EditorStyles.miniButton);

                EditorGUIUtility.SetIconSize(Vector2.zero);

                return (TextAnchor)(vValue * 3 + hValue);
            }
        }

    } // struct Utilities

} // namespace UnityExtensions.Localization.Editor

#endif // UNITY_EDITOR