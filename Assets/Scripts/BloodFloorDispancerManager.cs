using System;
using UnityEngine;

public class BloodFloorDispancerManager : PoolObjectManager
{
	public void CreateBloodEffectOnFloor(float x, float z, float _radius)
	{
		//Vector3 pos = new Vector3(x + _radius, this.floorYCord, z + _radius);
		//base.CreatePoolObjectOnPos(pos);
	}

	private float floorYCord = 0.014f;
}
