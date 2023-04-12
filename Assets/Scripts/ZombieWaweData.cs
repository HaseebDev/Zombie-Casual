
using QuickType.ZombieRatio;
using System;
using System.Collections.Generic;

[Serializable]
public struct ZombieWaweData
{
    public int minCountOfZombie;
    public float hpMultiplier;
    public float dmgMultiplier;
    public List<SubwaveDispenser> _listSubwaveDispenser;
    public List<Ratio> zombiesAndRatio;
}

[Serializable]
public struct SubwaveDispenser
{
    public int totalPercent;
    public float delaySpawn;
}

