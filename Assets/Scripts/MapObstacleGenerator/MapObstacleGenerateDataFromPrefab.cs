using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuickType.MapObstacle;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapObstacleDataElement
{
    public Vector2 scale;
    public Vector3 position;
    public OBSTACLE_TYPE type;
    public List<ObstacleByMapName> prefabByName;

    public MapObstacleDataElement(Vector2 scale, Vector3 position, OBSTACLE_TYPE type)
    {
        this.scale = scale;
        this.position = position;
        this.type = type;
    }
}

[Serializable]
public class ObstacleByMapName
{
    public string prefabName;
    public Vector3 scale;
    public Vector3 rotation;
    public List<MAP_NAME> mapName;

    public ObstacleByMapName(List<MAP_NAME> mapType, string prefabName, Vector3 scale, Vector3 rotation)
    {
        this.mapName = mapType;
        this.prefabName = prefabName;
        this.scale = scale;
        this.rotation = rotation;
    }
}


public class MapObstacleGenerateDataFromPrefab : MonoBehaviour
{
    public MapObstacleDataWrapper Datas;

    [Button]
    public void GenerateData()
    {
        string tagExplode = "explode";
        string tagObstacle = "obstacle";

        Datas = new MapObstacleDataWrapper {DataElements = new List<MapObstacleDataElement>()};
        var allChild = transform.GetComponentsInChildren<Transform>().ToList();

        var holder = allChild.Find(x => x.name.Contains("Obstacles"));

        foreach (Transform child in holder)
        {
            var localScale = child.GetComponent<Vector2Component>();
            OBSTACLE_TYPE type = child.name.Contains(tagExplode) ? OBSTACLE_TYPE.EXPLODE : OBSTACLE_TYPE.BARRIER;
            var rotation = child.rotation;
            var result = new MapObstacleDataElement(new Vector2(localScale.value.x, localScale.value.y), child.position, type);

            // Debug.LogError("WTF " + child.name +"   " + rotation.eulerAngles); 

            // var originRotation = rotation.eulerAngles;
            //
            // if (originRotation != Vector3.zero)
            // {
            //     child.transform.rotation = Quaternion.Euler(Vector3.zero);
            // }

            foreach (Transform child1 in child)
            {
                var byMapName = child1.GetComponent<ObstacleProtectGate>();
                if (byMapName != null)
                {
                    if (result.prefabByName == null)
                        result.prefabByName = new List<ObstacleByMapName>();

                    result.prefabByName.Add(new ObstacleByMapName(byMapName.mapNames, child1.name, child1.localScale,
                        child1.rotation.eulerAngles));
                }

                // if (originRotation != Vector3.zero)
                //     child1.transform.rotation = Quaternion.Euler(originRotation);
            }

            Datas.DataElements.Add(result);
        }
    }
}