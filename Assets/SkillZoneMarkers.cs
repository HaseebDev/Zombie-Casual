using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillZoneMarkers : MonoBehaviour
{
    public Transform startSkillMarker;
    public Transform endSkillMarker;
    public Transform centerSkillMarker;
    

    public Vector3 GetRandValidPos()
    {
        Vector3 result = Vector3.zero;

        var RandX = UnityEngine.Random.Range(startSkillMarker.position.x, endSkillMarker.position.x);
        var RandZ = UnityEngine.Random.Range(startSkillMarker.position.z, endSkillMarker.position.z);
        var RandY = startSkillMarker.position.y;
        result = new Vector3(RandX, RandY, RandZ);
        return result;
    }
}
