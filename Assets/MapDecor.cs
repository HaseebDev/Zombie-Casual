using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class MapDecor : MonoBehaviour
{
    public List<MeshRenderer> _listBarrierViews;
    public Transform _barrierGO;
    public MeshRenderer groundRenderer;

    public List<ObstacleColorByMap> _listColourByMap;

    public void Initialize(GameMode mode)
    {
        //_barrierGO.gameObject.SetActiveIfNot(mode == GameMode.CAMPAIGN_MODE);
        _barrierGO.gameObject.SetActiveIfNot(true);
    }

    public void SetGroundColor(Color newColor)
    {
        groundRenderer.material.color = newColor;
    }

    public void ResetObjectsColor(MAP_NAME name)
    {
        if (_listColourByMap != null)
        {
            foreach (var item in _listColourByMap)
            {
                item.SetColor(name);
            }
        }
    }

    [Button("Fetch Colour By Map")]
    public void FetchColourByMap()
    {
        _listColourByMap.Clear();
        var allColourByMap = this.gameObject.GetComponentsInChildrenRecursively<ObstacleColorByMap>();
        if (allColourByMap != null && allColourByMap.Count > 0)
        {
            _listColourByMap = allColourByMap;
        }
    }
}