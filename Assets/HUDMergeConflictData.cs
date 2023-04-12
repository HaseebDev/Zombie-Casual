using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DataChoosing
{
    NONE,
    THIS_DEVICE,
    ON_CLOUD
}

public class HUDMergeConflictData : BaseHUD
{
    [Header("Conflict View")]
    public ProgressConflictView viewThisDevice;
    public ProgressConflictView viewOnCloud;

    private DataChoosing currentChoosing;
    private Action<bool> OnChooseThisDevice;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        currentChoosing = DataChoosing.NONE;
        var thisDeviceData = (ViewConflictData)(args[0]);
        var onCloudData = (ViewConflictData)(args[1]);
        OnChooseThisDevice = (Action<bool>)(args[2]);

        thisDeviceData.UserName = "THIS DEVICE";
        viewThisDevice.Initialize(thisDeviceData, (chooseDevice) =>
        {
            if (currentChoosing == DataChoosing.NONE)
            {
                currentChoosing = DataChoosing.THIS_DEVICE;
                OnChooseThisDevice?.Invoke(true);

                Hide();
            }

        });

        onCloudData.UserName = "CLOUD";
        viewOnCloud.Initialize(onCloudData, (chooseCloud) =>
         {
             if (currentChoosing == DataChoosing.NONE)
             {
                 currentChoosing = DataChoosing.ON_CLOUD;
                 OnChooseThisDevice?.Invoke(false);

                 Hide();
             }

         });
    }
}
