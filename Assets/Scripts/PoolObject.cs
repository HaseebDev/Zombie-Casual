using System;
using UnityEngine;
using Ez.Pooly;

public class PoolObject : MonoBehaviour, IObjectPoolKey
{
    public string GetPoolKey()
    {
        return this.poolKey;
    }

    public virtual void OnAwake()
    {
    }

    protected void Done(float time)
    {
        base.Invoke("Done", time);
    }

    protected void Done()
    {
        // PoolManager.instance.ReturnObjectToQueue(base.gameObject);
        Pooly.Despawn(transform);
    }

    public string poolKey;
}
