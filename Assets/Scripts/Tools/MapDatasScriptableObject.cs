using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "ScriptableObjects/MapDatas", order = 1)]
public class MapDatasScriptableObject : ScriptableObject
{
    public List<MapData> mapDatas;
    [Button("Save Data")]
    public void SaveData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}