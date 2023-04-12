using TMPro;
using UnityEngine;

namespace UnityExtensions.Localization
{
    [AddComponentMenu("Mesh/Localized TMP Text Mesh")]
    [RequireComponent(typeof(TextMeshPro))]
    public class LocalizedTMPTextMesh : LocalizedTMPText
    {
#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/3D Object/Localized TMP Text Mesh")]
        static void CreateGameObject()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<LocalizedTMPTextMesh>();
        }

#endif
    }

} // namespace UnityExtensions.Localization