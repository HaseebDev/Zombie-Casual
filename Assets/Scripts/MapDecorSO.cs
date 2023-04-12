using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Object = UnityEngine.Object;
using System.Linq;

[Serializable]
public class DecorDataElement
{
    public MAP_NAME mapName;

    [Header("Decor Prefab")]
    public AssetReference DecorPrefab;

    [Header("Map Color")]
    public Color _mapGroundColor;

    [Header("LightSetting")]
    public Color _sourceColor;
    [Range(-10f, 10f)]
    public float _sourceIntensity = 0;
    public Color _equatorColor;
    [Range(-10f, 10f)]
    public float _equatorIntensity = 0;
    public Color _groundColor;
    [Range(-10f, 10f)]
    public float _groundIntensity = 0;

    [Header("Enviroment Refections")]
    [Range(0, 1)]
    public float IntensityMultiplie = 1;

    [Header("Direct Light")]
    public Color _directLightColor;
    public float _directLightIntensity;

    [HideInInspector]
    public int Bounces = 1;

    public DecorDataElement()
    {
        _mapGroundColor = Color.white;
        _sourceColor = Color.white;
        _sourceIntensity = 1.0f;
        _equatorColor = Color.white;
        _equatorIntensity = 1.0f;
        _groundColor = Color.white;
        _groundIntensity = 1.0f;

        IntensityMultiplie = 0.5f;
        _directLightColor = Color.white;
        _directLightIntensity = 0.5f;

        Bounces = 1;

    }


}


[CreateAssetMenu(fileName = "MapDecorSO", menuName = "ScriptableObjects/MapDecorSO", order = 1)]
public class MapDecorSO : ScriptableObject
{
    public List<DecorDataElement> mapDecors;

    public void CreateData()
    {
        //mapDecors = new List<DecorDataElement>();
        //string path = "Assets/Resources_moved/levels/Decors/MapDecors";

        //var allFile =
        //    Directory.GetFiles(path); //, "*.prefab", SearchOption.AllDirectories).ToList().OrderBy(s => s).ToList();
        //foreach (var file in allFile)
        //{
        //    if (file.Contains(".meta"))
        //        continue;

        //    var go = AssetDatabase.LoadAssetAtPath<Object>(file);
        //    string guid = "";
        //    long guidInt = 0;

        //    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(go, out guid,
        //        out guidInt); //,out var guid, out var guidInt);

        //    mapDecors.Add(new DecorDataElement(guid));
        //}
    }

    [Button("Add Missing Map")]
    public void AddMissingMap()
    {
        foreach (MAP_NAME map in Enum.GetValues(typeof(MAP_NAME)))
        {
            if (map == MAP_NAME.NONE)
                continue;
            var exists = mapDecors.FirstOrDefault(x => x.mapName == map);
            if (exists == null)
            {
                var data = new DecorDataElement();
                data.mapName = map;
                mapDecors.Add(data);
            }
        }
        SaveData();
    }


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