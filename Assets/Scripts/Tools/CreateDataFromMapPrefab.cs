using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

[Serializable]
public class RoadMapData
{
    public Sprite sprite;
    public Vector3 scale;

    public RoadMapData(Sprite sprite, Vector3 scale)
    {
        this.sprite = sprite;
        this.scale = scale;
    }
}

[Serializable]
public class DecorMapData
{
    public Sprite sprite;
    public Vector3 localPosition;
    public Vector2 size;
    
    public DecorMapData(Sprite sprite, Vector3 localPosition, Vector2 size)
    {
        this.sprite = sprite;
        this.localPosition = localPosition;
        this.size = size;
    }
}

[Serializable]
public class LevelMarkMapData
{
    public Vector3 localPosition;

    public LevelMarkMapData(Vector3 localPosition)
    {
        this.localPosition = localPosition;
    }
}

[Serializable]
public class MapData
{
    public string name;
    public List<RoadMapData> roads;
    public List<DecorMapData> decors;
    public List<LevelMarkMapData> marker;
    public Color bgColor;
    public Color roadColor;
    public Color decorColor;
    public Color nextBgColor;
    public Color nextRoadColor;
}


public class CreateDataFromMapPrefab : MonoBehaviour
{
    private string decorHolderName = "DecorsHolder";
    private string markerHolderName = "MarkersHolder";
    public MapData mapData;

#if UNITY_EDITOR
    [Button]
    public void GenerateMapData()
    {
        var allChild = transform.GetComponentsInChildren<Transform>().ToList();

        mapData = new MapData
        {
            name = gameObject.name,
            bgColor = transform.Find("Background").GetComponent<Image>().color,
            roadColor = allChild.Find(x => x.name.Contains("road")).GetComponent<Image>().color,
            decorColor = allChild.Find(x => x.name.Contains("Decor ")).GetComponent<Image>().color,
            roads = new List<RoadMapData>(),
            decors = new List<DecorMapData>(),
            marker = new List<LevelMarkMapData>()
        };

        // Bind road
        var allRoad = allChild.FindAll(x => x.name.Contains("road"));
        int index = 0;
        foreach (var road in allRoad)
        {
            road.name = $"road {index}";
            mapData.roads.Add(new RoadMapData(road.GetComponent<Image>().sprite, road.localScale));
        }

        // Bind decor
        var allDecor = allChild.FindAll(x => x.name.Contains("Decor "));
        foreach (var decor in allDecor)
        {
            mapData.decors.Add(new DecorMapData(decor.GetComponent<Image>().sprite, decor.localPosition,decor.rectTransform().sizeDelta));
        }

        // Bind marker
        var allMaker = allChild.FindAll(x => x.name.Contains("Maker "));
        foreach (var marker in allMaker)
        {
            mapData.marker.Add(new LevelMarkMapData(marker.localPosition));
        }
    }

    [Button]
    public void BringDecorToOutSide()
    {
        Transform decorHolder = transform.Find("Decors");
        if (decorHolder == null)
        {
            if (transform.Find(decorHolderName) == null)
            {
                decorHolder = new GameObject(decorHolderName).GetComponent<Transform>();
                decorHolder.parent = transform; //.SetParent(transform);
            }
        }
        else
        {
            decorHolder.name = decorHolderName;
        }

        decorHolder = transform.Find(decorHolderName);

        var allChild = transform.GetComponentsInChildren<Transform>();
        var moveToDecorHolderChild = new List<Transform>();

        foreach (var child in allChild)
        {
            string childName = child.name;

            if (childName.Contains("Decor") && childName != decorHolderName && child.transform.parent != decorHolder)
            {
                moveToDecorHolderChild.Add(child.transform);
            }
        }

        for (int i = 0; i < moveToDecorHolderChild.Count; i++)
        {
            var decor = moveToDecorHolderChild[i];
            decor.name = $"Decor {i}";
            decor.SetParent(decorHolder);
        }
    }

    [Button]
    public void BringMarkerOutSide()
    {
        Transform markerHolder = transform.Find(markerHolderName);

        if (markerHolder == null)
        {
            markerHolder = new GameObject(markerHolderName).GetComponent<Transform>();
            markerHolder.parent = transform; //.SetParent(transform);
        }

        var allChild = transform.GetComponentsInChildren<Transform>();
        var moveToMakerHolderChild = new List<Transform>();
        foreach (var child in allChild)
        {
            string childName = child.name;

            if (childName.Contains("Maker") && child != markerHolder && child.parent != markerHolder)
            {
                moveToMakerHolderChild.Add(child.transform);
            }
        }

        for (int i = 0; i < moveToMakerHolderChild.Count; i++)
        {
            var child = moveToMakerHolderChild[i];
            child.gameObject.name = $"Maker {i}";
            child.transform.parent = markerHolder;
        }
    }

    [Button]
    public void TurnOfSoc()
    {
        var soc = transform.Find("soc");
        soc.gameObject.SetActive(false);
    }

    [Button]
    public void Reposition()
    {
        var allChild = transform.GetComponentsInChildren<Transform>().ToList();
        var allDecor = allChild.FindAll(x => x.name.Contains("Decor "));
        foreach (var decor in allDecor)
        {
            decor.SetParent(transform);
        }

        var decorHolder = transform.Find(decorHolderName);
        if (decorHolder.rectTransform() == null)
            decorHolder.gameObject.AddComponent<RectTransform>();

        decorHolder.rectTransform().sizeDelta = Vector2.zero;
        decorHolder.rectTransform().anchoredPosition = Vector2.zero;
        foreach (var decor in allDecor)
        {
            decor.SetParent(decorHolder);
        }


        var allMarker = allChild.FindAll(x => x.name.Contains("Maker "));
        foreach (var maker in allMarker)
        {
            maker.SetParent(transform);
        }

        var makerHolder = transform.Find(markerHolderName);
        var rect = makerHolder.rectTransform();
        if (rect == null)
            rect = makerHolder.gameObject.AddComponent<RectTransform>();
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        foreach (var maker in allMarker)
        {
            maker.SetParent(rect);
        }
    }

    [Button]
    public void LoadReference()
    {
        var mapItemUI = GetComponent<MapItemUI>();
        if (mapItemUI == null)
        {
            mapItemUI = gameObject.AddComponent<MapItemUI>();
        }

        // mapItemUI.bg = transform.Find("Background").GetComponent<Image>();


        var allChild = transform.GetComponentsInChildren<Transform>().ToList();
        var allDecor = allChild.FindAll(x => x.name.Contains("Decor "));
        mapItemUI.decors = new List<Transform>();
        foreach (var VARIABLE in allDecor)
        {
            mapItemUI.decors.Add(VARIABLE);
        }

        // var allMarker = allChild.FindAll(x => x.name.Contains("Maker "));
        // mapItemUI.markers = new List<MarkerItemUI>();
        //
        // foreach (var VARIABLE in allMarker)
        // {
        //     mapItemUI.markers.Add(VARIABLE);
        // }

        var allRoad = allChild.FindAll(x => x.name.Contains("road"));
        mapItemUI.roads = new List<Image>();
        foreach (var VARIABLE in allRoad)
        {
            mapItemUI.roads.Add(VARIABLE.GetComponent<Image>());
        }
    }

    [Button]
    public void ChuanHoaNew()
    {
        EditorUtility.SetDirty(this);

        var allChild = transform.GetComponentsInChildren<Transform>().ToList();
        var allRoad = allChild.FindAll(x => x.name.Contains("road"));
        foreach (var VARIABLE in allRoad)
        {
            foreach (Transform child in VARIABLE)
            {
                child.name = "Maker ";
            }
        }

        var decorHolder = transform.Find(decorHolderName);
        foreach (Transform VARIABLE in decorHolder)
        {
            VARIABLE.name = "Decor ";
        }

        BringMarkerOutSide();
        BringDecorToOutSide();
        Reposition();
        GenerateMapData();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
    public Color GetBGColor()
    {
        return transform.Find("Background").GetComponent<Image>().color;
    }

    public Color GetRoadColor()
    {
        var allChild = transform.GetComponentsInChildren<Transform>().ToList();
        var allRoad = allChild.FindAll(x => x.name.Contains("road"));

        return allRoad[0].GetComponent<Image>().color;
    }
}