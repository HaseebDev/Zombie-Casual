using System;
using UnityEngine;

public class UpgradebleObjectsController : MonoBehaviour
{
    public void LoadUpgradebleObjects()
    {
        this.upgradebleObjectViews = Resources.FindObjectsOfTypeAll<UpgradebleObjectView>();
        this.OnUpgradebleObjectsLoaded();
    }

    public GameObject GetUpgradebleObject(string _objectId)
    {
        for (int i = 0; i < this.upgradebleObjectViews.Length; i++)
        {
            if (this.upgradebleObjectViews[i].upgardebleValueGameObjectId == _objectId)
            {
                return this.upgradebleObjectViews[i].gameObject;
            }
        }
        throw new ArgumentNullException(string.Format("Upgradeble object with id {0} not found!", _objectId));
    }

    [SerializeField]
    private UpgradebleObjectView[] upgradebleObjectViews;

    public Action OnUpgradebleObjectsLoaded = delegate ()
    {
    };
}
