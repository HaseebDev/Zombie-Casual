#if UNITY_EDITOR

using UnityEditor;
using UnityExtensions.Editor;

namespace UnityExtensions.Localization.Editor
{
    [CustomEditor(typeof(LocalizedTMPTextProfile), true)]
    class LocalizedTMPTextProfileEditor : BaseEditor<LocalizedTMPTextProfile>
    {
        public override void OnInspectorGUI()
        {
            target.OnGUI();
        }

    }

} // UnityExtensions.Localization.Editor

#endif