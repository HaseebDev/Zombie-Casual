using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UI.Extensions;
using Gradient = UnityEngine.UI.Extensions.Gradient;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;
using UnityExtensions;
using Random = UnityEngine.Random;


public class MapItemUI : MonoBehaviour
{
    public List<MarkerItemUI> markers;
    public List<Transform> decors;
    public List<Image> roads;
    public Image bg;
    public Gradient lastRoad;

    private List<Image> decorsImage;
    public Gradient bgGradient;
    public MapData MapData;

    public void Load(MapData mapData, int index)
    {
        int startLevel = index * 15 + 1;

        MapData = mapData;
        InitList();

        // bg.color = mapData.;
        bgGradient.vertex2 = mapData.nextBgColor;
        bgGradient.vertex1 = mapData.bgColor;

        lastRoad.vertex2 = mapData.nextRoadColor;
        lastRoad.vertex1 = mapData.roadColor;

        lastRoad.ForceUpdate();
        bgGradient.ForceUpdate();

        // Bind road
        for (int i = 0; i < mapData.roads.Count; i++)
        {
            roads[i].sprite = mapData.roads[i].sprite;
            roads[i].transform.localScale = mapData.roads[i].scale;
            if (i != mapData.roads.Count - 1)
                roads[i].color = mapData.roadColor;
            else
                roads[i].color = Color.white;
        }
        //
        // Bind decor 
        for (int i = 0; i < decors.Count; i++)
        {
            var decorTransform = decors[i];
            var decorImage = decorsImage[i];
        
            if (i < mapData.decors.Count)
            {
                decorTransform.gameObject.SetActive(true);
                var decorData = mapData.decors[i];
        
                decorTransform.localPosition = decorData.localPosition;
                decorTransform.rectTransform().sizeDelta = decorData.size;
                decorImage.sprite = decorData.sprite;
                decorImage.color = mapData.decorColor;
            }
            else
            {
                decorTransform.gameObject.SetActive(false);
            }
        }
        
        int currentLevel = SaveGameHelper.GetMaxCampaignLevel();
        if (currentLevel == 0)
            currentLevel = 1;
        if (currentLevel > 360)
            currentLevel = 360;
        
        // Bind marker
        for (int i = 0; i < markers.Count; i++)
        {
            markers[i].transform.localPosition = mapData.marker[i].localPosition;
            int star = SaveGameHelper.GetStarAtLevel(startLevel);
            markers[i].Load(startLevel++, currentLevel,star);
        }
    }

    private void InitList()
    {
        if (decorsImage == null)
        {
            decorsImage = new List<Image>();
            foreach (var VARIABLE in decors)
            {
                decorsImage.Add(VARIABLE.GetComponent<Image>());
            }
        }
    }

    [Button]
    public void FindMarker()
    {
        markers.Clear();
        var allChild = transform.GetComponentsInChildren<Transform>().ToList();
        var allMarker = allChild.FindAll(x => x.name.Contains("Maker "));
        foreach (var VARIABLE in allMarker)
        {
            markers.Add(VARIABLE.GetComponent<MarkerItemUI>());
        }
    }
}