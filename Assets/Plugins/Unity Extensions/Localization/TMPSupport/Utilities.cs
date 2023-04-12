#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityExtensions.Editor;
using TMPro.EditorUtilities;
using TMPro;

namespace UnityExtensions.Localization.Editor
{
    public partial struct Utilities
    {
        public static TextAlignmentOptions TextAlignmentField(Rect rect, GUIContent label, TextAlignmentOptions value)
        {
            using (GUIContentColorScope.New(EditorGUIUtilities.labelNormalColor))
            {
                int hValue = TMP_EditorUtility.GetHorizontalAlignmentGridValue((int)value);
                int vValue = TMP_EditorUtility.GetVerticalAlignmentGridValue((int)value);

                rect = EditorGUI.PrefixLabel(rect, label);
                rect.width = Mathf.Min(rect.width, rect.height * 9f);
                hValue = GUI.Toolbar(rect, hValue, TMP_UIStyleManager.alignContentA, EditorStyles.miniButton);

                rect.y = EditorGUILayout.GetControlRect().y;
                vValue = GUI.Toolbar(rect, vValue, TMP_UIStyleManager.alignContentB, EditorStyles.miniButton);

                return (TextAlignmentOptions)((0x1 << hValue) | (0x100 << vValue));
            }
        }


        public static FontStyles FontStyleField(Rect rect, GUIContent label, FontStyles value)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
            var rect2 = rect;
            rect2.width = (int)Mathf.Min(rect.height * 1.8f, rect.width / 4);

            bool flag = GUI.Toggle(rect2, (value & FontStyles.Bold) != 0, EditorGUIUtilities.TempContent("B", tooltip: "Bold"), EditorStyles.miniButtonLeft);
            if (flag) value |= FontStyles.Bold; else value &= ~FontStyles.Bold;

            rect2.x = rect2.xMax;
            flag = GUI.Toggle(rect2, (value & FontStyles.Italic) != 0, EditorGUIUtilities.TempContent("I", tooltip: "Italic"), EditorStyles.miniButtonMid);
            if (flag) value |= FontStyles.Italic; else value &= ~FontStyles.Italic;

            rect2.x = rect2.xMax;
            flag = GUI.Toggle(rect2, (value & FontStyles.Underline) != 0, EditorGUIUtilities.TempContent("U", tooltip: "Underline"), EditorStyles.miniButtonMid);
            if (flag) value |= FontStyles.Underline; else value &= ~FontStyles.Underline;

            rect2.x = rect2.xMax;
            flag = GUI.Toggle(rect2, (value & FontStyles.Strikethrough) != 0, EditorGUIUtilities.TempContent("S", tooltip: "Strikethrough"), EditorStyles.miniButtonRight);
            if (flag) value |= FontStyles.Strikethrough; else value &= ~FontStyles.Strikethrough;

            int select = -1;
            if ((value & FontStyles.LowerCase) != 0) select = 0;
            else if ((value & FontStyles.UpperCase) != 0) select = 1;
            else if ((value & FontStyles.SmallCaps) != 0) select = 2;

            rect2.y = EditorGUILayout.GetControlRect().y;
            rect2.x = rect.x;

            flag = GUI.Toggle(rect2, select == 0, EditorGUIUtilities.TempContent("ab", tooltip: "LowerCase"), EditorStyles.miniButtonLeft);
            if (flag) select = 0; else if (select == 0) select = -1;

            rect2.x = rect2.xMax;
            flag = GUI.Toggle(rect2, select == 1, EditorGUIUtilities.TempContent("AB", tooltip: "UpperCase"), EditorStyles.miniButtonMid);
            if (flag) select = 1; else if (select == 1) select = -1;

            rect2.x = rect2.xMax;
            flag = GUI.Toggle(rect2, select == 2, EditorGUIUtilities.TempContent("SC", tooltip: "SmallCaps"), EditorStyles.miniButtonRight);
            if (flag) select = 2; else if (select == 2) select = -1;

            value &= ~(FontStyles.LowerCase | FontStyles.UpperCase | FontStyles.SmallCaps);
            if (select == 0) value |= FontStyles.LowerCase;
            else if (select == 1) value |= FontStyles.UpperCase;
            else if (select == 2) value |= FontStyles.SmallCaps;

            return value;
        }


        public static Vector4 MarginsField(Rect rect, GUIContent label, Vector4 value)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
            using (LabelWidthScope.New(rect.height * 0.7f))
            {
                rect.width = (rect.width - 8) / 2;
                var rect2 = rect;
                rect2.x = rect.xMax + 8;

                value.x = EditorGUI.FloatField(rect, "L", value.x);
                value.z = EditorGUI.FloatField(rect2, "R", value.z);

                rect2.y = rect.y = EditorGUILayout.GetControlRect().y;

                value.y = EditorGUI.FloatField(rect, "T", value.y);
                value.w = EditorGUI.FloatField(rect2, "B", value.w);
            }

            return value;
        }

    } // struct Utilities

} // namespace UnityExtensions.Localization.Editor

#endif // UNITY_EDITOR