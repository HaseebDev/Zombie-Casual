using System;
using Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationItemPanelView : MonoBehaviour, IState
{
	private void Start()
	{
		SceneManager.sceneUnloaded += this.SceneManager_SceneUnloaded;
	}

	private void SceneManager_SceneUnloaded(Scene arg0)
	{
		this.Unload();
	}

	public void Load()
	{
		base.gameObject.SetActive(true);
		this.locationCellsController.UpdateCellsData();
	}

	public void Unload()
	{
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		SceneManager.sceneUnloaded -= this.SceneManager_SceneUnloaded;
	}

	[SerializeField]
	private LocationCellsController locationCellsController;
}
