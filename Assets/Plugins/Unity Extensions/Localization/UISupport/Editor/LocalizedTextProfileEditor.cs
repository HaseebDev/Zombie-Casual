#if UNITY_EDITOR

using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions.Localization.Editor
{
    [CustomEditor(typeof(LocalizedTextProfile), true)]
    public class LocalizedTextProfileEditor : BaseEditor<LocalizedTextProfile>
    {
        public override void OnInspectorGUI()
        {
            target.OnGUI();
        }

    }

} // UnityExtensions.Localization.Editor

#endif