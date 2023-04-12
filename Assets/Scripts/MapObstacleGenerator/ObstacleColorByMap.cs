using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ObstacleColorByMapData
{
    public Color color;
    public MAP_NAME mapName;
}


[RequireComponent(typeof(MeshRenderer))]
public class ObstacleColorByMap : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public List<ObstacleColorByMapData> colors;

    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = this.GetComponent<MeshRenderer>();
    }

    public void SetColor(MAP_NAME name)
    {
        var targetColor = colors.FirstOrDefault(x => x.mapName == name);
        if (targetColor != null)
        {
            this.meshRenderer.material.color = targetColor.color;
        }
    }
}
