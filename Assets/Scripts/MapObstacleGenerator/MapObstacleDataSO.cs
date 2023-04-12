using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;


[Serializable]
public class MapObstacleDataWrapper
{
    public List<MapObstacleDataElement> DataElements;
}

[Serializable]
public class MapObstacleDatas
{
    public string id;
    public MapObstacleDataWrapper datasWrapper;
}

[CreateAssetMenu(fileName = "MapObstacleDataSO", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapObstacleDataSO : ScriptableObject
{
    public List<MapObstacleDatas> Datas;
    public bool useTest;
    public MapObstacleDataWrapper testModel;

    [Button("Save Data")]
    public void SaveData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }


    [Button("Create Data")]
    public void CreateData()
    {
#if UNITY_EDITOR
        string path = "Assets/Resources_moved/MapObstacleDesign/ModelsMap";
        Datas.Clear();

        var allFile = Directory.GetDirectories(path);//, SearchOption.AllDirectories).ToList().OrderBy(s => s).ToList();

        foreach (var mapModel in allFile)
        {
            var subModel = Directory.GetFiles(mapModel);

            foreach (var sub in subModel)
            {
                if (sub.Contains(".meta"))
                    continue;

                MapObstacleDatas result = new MapObstacleDatas { datasWrapper = new MapObstacleDataWrapper() };
                result.id = sub.Split('/').Last().Split('\\').Last().Split('.').First();

                GameObject data = AssetDatabase.LoadAssetAtPath<GameObject>(sub);
                var cpm = data.GetComponent<MapObstacleGenerateDataFromPrefab>();
                cpm.GenerateData();
                result.datasWrapper = cpm.Datas;
                Datas.Add(result);
            }

        }
#endif
    }

    [Button("Create Test Data")]
    public void CreateTestData()
    {
#if UNITY_EDITOR
        string path = "Assets/Resources_moved/MapObstacleDesign/ModelsMapTest";
        var allFile = Directory.GetFiles(path);//, SearchOption.AllDirectories).ToList().OrderBy(s => s).ToList();
        foreach (var sub in allFile)
        {
            if (sub.Contains(".meta"))
                continue;

            GameObject data = AssetDatabase.LoadAssetAtPath<GameObject>(sub);
            var cpm = data.GetComponent<MapObstacleGenerateDataFromPrefab>();
            cpm.GenerateData();
            testModel = cpm.Datas;

        }
#endif

    }
}
