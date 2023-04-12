using System;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public Vector3 GetRandomPoint(float offsetX = 0f)
    {
        float num = UnityEngine.Random.Range(-0.3f, -0.6f);
        float x = UnityEngine.Random.Range(this.minPoint.transform.position.x + offsetX, this.maxPoint.transform.position.x + offsetX);
        float y = UnityEngine.Random.Range(this.minPoint.transform.position.y, this.maxPoint.transform.position.y);
        float z = UnityEngine.Random.Range(this.minPoint.transform.position.z, this.maxPoint.transform.position.z);
        return new Vector3(x, y, z);
    }

    public Vector3 GetNearestForwardCastlePoint(float offsetX = 0f)
    {
        float x = this.maxPoint.transform.position.x + offsetX;
        float y = UnityEngine.Random.Range(this.minPoint.transform.position.y, this.maxPoint.transform.position.y);
        float z = UnityEngine.Random.Range(this.minPoint.transform.position.z, this.maxPoint.transform.position.z);
        return new Vector3(x, y, z);
    }

    [SerializeField]
    public Transform minPoint;

    [SerializeField]
    public Transform maxPoint;
}
