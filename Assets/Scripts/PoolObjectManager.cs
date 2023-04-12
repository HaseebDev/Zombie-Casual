using System;
using UnityEngine;

public class PoolObjectManager : MonoBehaviour
{
    private void Start()
    {
        PoolManager.instance.CreatePool(this.indicatorId, this.indiPrefab, this.indicatorPoolSize);
    }
    public void CreatePoolObjectOnRandomPosInRadius(Vector3 _pos, float _radius)
    {
        Vector3 pos = new Vector3(_pos.x + UnityEngine.Random.value * _radius, _pos.y + UnityEngine.Random.value * _radius);
        this.CreatePoolObjectOnPos(pos);
    }

    public void CreatePoolObjectOnPos(Vector3 _pos)
    {
        this.poolObject = PoolManager.instance.GetObject(this.indicatorId, _pos, Quaternion.identity);
    }

    [SerializeField]
    private GameObject indiPrefab;

    [SerializeField]
    private int indicatorPoolSize;

    protected GameObject poolObject;

    [SerializeField]
    protected string indicatorId;
}
