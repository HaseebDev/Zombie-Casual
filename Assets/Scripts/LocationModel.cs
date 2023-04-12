using System;
using Adic;
using Framework;
using Framework.Managers.Event;
using UnityEngine;

public class LocationModel : MonoBehaviour, IDisposable
{
	public void Init()
	{
		this.currentLocation = this.GetLocationNumber();
		this.maxUnlockedLocation = PlayerPrefs.GetInt("mLocation");
	}

	public void LoadLastLocation()
	{
		this.ChangeLocation(this.currentLocation);
	}

	public void UnlockLocation(int locationId)
	{
		if (locationId > this.currentLocation)
		{
			PlayerPrefs.SetInt("mLocation", locationId);
		}
	}

	public void ChangeLocation(int locationId)
	{
		PlayerPrefs.SetInt("cLocation", locationId);
		this.currentLocation = locationId;
		this.levelLoader.LoadLevel(locationId);
		
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.LOCATION_CHANGED);

    }

	private int GetLocationNumber()
	{
		return PlayerPrefs.GetInt("cLocation", 1);
	}

	public void Dispose()
	{
		this.levelLoader = null;
	}

	[Inject]
	private LevelLoader levelLoader;



	public int currentLocation;

	public int maxUnlockedLocation = 1;
}
