using System;
using System.Collections.Generic;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;
using Framework.Utility;
using UnityEngine;

public class LocationCellsController : MonoBehaviour
{
    private void Start()
    {
        this.Inject();
        this.locationModel = GameObject.Find("LocationModel").GetComponent<LocationModel>();
       

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LOADING_FINISHED, new Action(OnLoadingFinished));
    }

    private void OnLevelUp()
    {
        for (int i = 0; i < this.locationCells.Count; i++)
        {
            this.locationCells[i].OnLevelChanged();
        }
    }

    private void OnLoadingFinished()
    {

    }

    public void UpdateCellsData()
    {
        this.locationCells = this.cellsContainer.GetAll<LocationCell>();
        for (int i = 0; i < this.locationCells.Count; i++)
        {
            this.locationCells[i].Init(this.locationModel, LevelModel.instance, CurrencyModels.instance);
        }
    }

    private void Update()
    {
        for (int i = 0; i < this.locationCells.Count; i++)
        {
            this.locationCells[i].OnStarChanged();
        }
    }

    private LocationCell GetCurrentLocationCell(int locationId)
    {
        for (int i = 0; i < this.locationCells.Count; i++)
        {
            if (this.locationCells[i].LocationIsCurrent())
            {
                return this.locationCells[i];
            }
        }
        return null;
    }

    private void OnDestroy()
    {
        this.locationModel = null;
    }

    [SerializeField]
    private List<LocationCell> locationCells;

    [Inject]
    private LocationModel locationModel;


    [SerializeField]
    private Transform cellsContainer;

    private LocationCell currentLocationCell;
}
