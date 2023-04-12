using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AssetReferencePoolySO")]
public class AssetReferencePoolySO : ScriptableObject
{
    public List<AssetReference> assetReferences;
}
