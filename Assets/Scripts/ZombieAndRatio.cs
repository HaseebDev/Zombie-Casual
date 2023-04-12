using System;
using UnityEngine;

[Serializable]
public class ZombieAndRatio
{
    public string zombieID;

    public float spawnRatio;

    public ZombieAndRatio(string zombie, float ratio)
    {
        zombieID = zombie;
        ratio = spawnRatio;

    }
}
