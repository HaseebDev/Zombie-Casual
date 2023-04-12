using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ZombieSpawnDefineSO", order = 1)]
public class ZombieSpawnDefineSO : ScriptableObject
{
    [Header("Spawn Distance")]
    public float MinVerticalDistance = 0.3f;
    public float MaxVerticalDistance = 0.6f;

}
