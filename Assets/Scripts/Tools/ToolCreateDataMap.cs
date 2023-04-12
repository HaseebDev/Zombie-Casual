using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityExtensions;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
public class ToolCreateDataMap : EditorWindow
{
    public static Object folder;
    public static Object mapDataScriptableObject;

    [MenuItem("EditorUtils/Open Create Map Data Tool")]
    static void Init()
    {
        ToolCreateDataMap window = (ToolCreateDataMap)EditorWindow.GetWindow(typeof(ToolCreateDataMap));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }

    private void OnGUI()
    {
        // folder = EditorGUILayout.ObjectField("Folder", folder, typeof(Object), true);
        mapDataScriptableObject = EditorGUILayout.ObjectField("MapData ScriptableObject ", mapDataScriptableObject,
            typeof(MapDatasScriptableObject), true);

        if (GUILayout.Button("Standardized"))
        {
            Standardized();
        }

        if (GUILayout.Button("BindToSO"))
        {
            BindMapDataToSO();
        }
    }

    public void Standardized()
    {
        // string path = AssetDatabase.GetAssetPath(folder);
        string path = "Assets/Resources_moved/GameData/Map";

        foreach (string file in Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories))
        {
            GameObject datas = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            var mapDataGenerateHelper = datas.GetComponent<CreateDataFromMapPrefab>();
            if (mapDataGenerateHelper == null)
                mapDataGenerateHelper = datas.AddComponent<CreateDataFromMapPrefab>();

            Debug.LogError("Standardized " + datas.name);
            mapDataGenerateHelper.BringDecorToOutSide();
        }
    }

    public void BindMapDataToSO()
    {
        string path = "Assets/Resources_moved/GameData/Map";
        MapDatasScriptableObject mapData = (MapDatasScriptableObject)mapDataScriptableObject;
        mapData.mapDatas.Clear();

        var allFile = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories).ToList().OrderBy(s => s).ToList();

        List<Tuple<string, string>> tempList = new List<Tuple<string, string>>();
        foreach (var VARIABLE in allFile)
        {
            string[] temp = VARIABLE.Split('_');
            string number = temp[3].Split('(')[0];
            number.Replace("(", "");

            string number1 = temp[2].Replace("Map", "");
            // Debug.LogError(number1+number); 
            
 
            tempList.Add(new Tuple<string, string>(number1+number,VARIABLE));
        }

        
        tempList = tempList.OrderBy(s => Int32.Parse(s.Item1)).ToList();
        for (int i = 0; i < tempList.Count; i++)
        {
            string file = tempList[i].Item2;
            
            GameObject datas = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            GameObject nextDatas = null;
            if (i <= tempList.Count - 2)
            {
                nextDatas = AssetDatabase.LoadAssetAtPath<GameObject>(tempList[i + 1].Item2);
            }
            
            
            var mapDataGenerateHelper = datas.GetComponent<CreateDataFromMapPrefab>();
            var NextMapDataGenerateHelper = nextDatas?.GetComponent<CreateDataFromMapPrefab>();
            
            if (mapDataGenerateHelper != null)
            {
                mapDataGenerateHelper.GenerateMapData();
                if (NextMapDataGenerateHelper != null)
                {
                    Color bgColor = NextMapDataGenerateHelper.GetBGColor();
                    mapDataGenerateHelper.mapData.nextBgColor = bgColor;
                    mapDataGenerateHelper.mapData.nextRoadColor = NextMapDataGenerateHelper.GetRoadColor();
                }
                else
                {
                    mapDataGenerateHelper.mapData.nextBgColor = mapDataGenerateHelper.mapData.bgColor;
                    mapDataGenerateHelper.mapData.nextRoadColor = mapDataGenerateHelper.mapData.roadColor;
            
                }
            
            
                mapData.mapDatas.Add(mapDataGenerateHelper.mapData); 
            } 
        }


        // for (int i = 0; i < allFile.Count; i++)
        // {
        //     string file = allFile[i];
        //     Debug.LogError(file);
        //
        //
        //     GameObject datas = AssetDatabase.LoadAssetAtPath<GameObject>(file);
        //     GameObject nextDatas = null;
        //     if (i <= allFile.Count - 2)
        //     {
        //         nextDatas = AssetDatabase.LoadAssetAtPath<GameObject>(allFile[i + 1]);
        //     }
        //
        //
        //     var mapDataGenerateHelper = datas.GetComponent<CreateDataFromMapPrefab>();
        //     var NextMapDataGenerateHelper = nextDatas?.GetComponent<CreateDataFromMapPrefab>();
        //
        //     if (mapDataGenerateHelper != null)
        //     {
        //         mapDataGenerateHelper.GenerateMapData();
        //         if (NextMapDataGenerateHelper != null)
        //         {
        //             Color bgColor = NextMapDataGenerateHelper.GetBGColor();
        //             mapDataGenerateHelper.mapData.nextBgColor = bgColor;
        //             mapDataGenerateHelper.mapData.nextRoadColor = NextMapDataGenerateHelper.GetRoadColor();
        //         }
        //         else
        //         {
        //             mapDataGenerateHelper.mapData.nextBgColor = mapDataGenerateHelper.mapData.bgColor;
        //             mapDataGenerateHelper.mapData.nextRoadColor = mapDataGenerateHelper.mapData.roadColor;
        //
        //         }
        //
        //
        //         mapData.mapDatas.Add(mapDataGenerateHelper.mapData);
        //     }
        //
        // }


    }
}
#endif