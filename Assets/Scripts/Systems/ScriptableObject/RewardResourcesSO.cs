using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RewardResource
{
    public string RewardID;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "RewardResources", menuName = "ScriptableObjects/RewardResourcesSO", order = 1)]
public class RewardResourcesSO : ScriptableObject
{
    public List<RewardResource> rewardResources;
}
