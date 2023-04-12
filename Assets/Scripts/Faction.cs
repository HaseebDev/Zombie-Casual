using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactionType
{
    NONE,
    HERO,
    ZOMBIE
}


public class Faction : MonoBehaviour
{
    public FactionType factionType = FactionType.NONE;
}