using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using com.datld.data;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class GunVariant
{
    [Space(10)]
    public string gunID;
    public string gunName;
    public AssetReference gunIconAddress;

    //public Gun gunPrefab;
    public AssetReference gunAddress;
    public OverWriteBullet overwriteBullet;
}

[Serializable]
public class OverWriteBullet
{
    public GameObject gunBullet;
    public float FireForce = 70;
    public AutoDespawnParticles muzzlePar;
    public SFX_ENUM sfxShoot = SFX_ENUM.NONE;
    public float offsetGunMount = 0f;
}


[Serializable]
public class LauncherVariant
{
    public WEAPON_TYPE type;
    public Launcher launcher;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GunResourceSO", order = 1)]
public class GunResourceSO : ScriptableObject
{
    [Header("List Gun")] public List<GunVariant> _listGun;

    [Header("List Launcher")] public List<LauncherVariant> _listLauncher;

    [Button("Save Data")]
    public void SaveData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    public GunVariant GetGunVariant(string gunID)
    {
        GunVariant result = null;
        result = _listGun.FirstOrDefault(x => x.gunID == gunID);
        if (result == null)
        {
            Debug.LogError($"GetGunVariant error {gunID}");
        }

        return result;
    }

    public LauncherVariant GetLauncherVariant(WEAPON_TYPE type)
    {
        LauncherVariant result = null;
        result = _listLauncher.FirstOrDefault(x => x.type == type);
        if (result == null)
        {
            Debug.LogError($"GetGunVariant error {type}");
        }

        return result;
    }
}