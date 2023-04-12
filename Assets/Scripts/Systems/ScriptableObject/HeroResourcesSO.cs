using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using MEC;
#if UNITY_EDITOR
using UnityEditor;

#endif

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HeroResourcesSO", order = 1)]
public class HeroResourcesSO : ScriptableObject
{
    [SerializeField] public List<HeroResource> listHeroResources;

    //public Sprite GetHeroActiveSprite(string heroID)
    //{
    //    Sprite result = null;
    //    var def = listHeroResources.Find(x => x.HeroID == heroID);
    //    if (def != null)
    //        result = def.spActive;
    //    return result;
    //}

    //public Sprite GetHeroInactiveSprite(string heroID)
    //{
    //    Sprite result = null;
    //    var def = listHeroResources.Find(x => x.HeroID == heroID);
    //    if (def != null)
    //        result = def.spInactive;
    //    return result;
    //}

    public string GetHeroName(string heroId)
    {
        string result = "";
        var def = listHeroResources.Find(x => x.HeroID == heroId);
        if (def != null)
            result = def.Name;
        return result;
    }

    //public HeroAnimMachine GetHeroAnim(string heroID)
    //{
    //    HeroAnimMachine result = null;

    //    var def = listHeroResources.Find(x => x.HeroID == heroID);
    //    if (def != null)
    //        result = def.heroAnim;

    //    if (result == null)
    //        Debug.LogError($"Cant GetHeroAnim {heroID}");
    //    return result;
    //}

    public void SpawnHeroAnim(string heroID, Action<HeroAnimMachine> callback)
    {
        Timing.RunCoroutine(SpawnHeroAnimCoroutine(heroID, callback));
    }

    public IEnumerator<float> SpawnHeroAnimCoroutine(string heroID, Action<HeroAnimMachine> callback)
    {
        var def = listHeroResources.Find(x => x.HeroID == heroID);
        if (def != null)
        {
            var op = def.heroAnimAddress.InstantiateAsync();
            while (!op.IsDone)
                yield return Timing.WaitForOneFrame;

            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                Debug.LogError($"SpawnHerAnimCoroutine fail! {heroID}");
            }

            callback?.Invoke(op.Result.GetComponent<HeroAnimMachine>());
            yield break;
        }
        else
        {
            callback?.Invoke(null);
        }



    }

    [Button("SaveData")]
    public void SaveData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}

[Serializable]
public class HeroResource
{
    public string HeroID;
    public string Name;
    public AssetReference spActiveAddress;
    public AssetReference spInActiveAddress;
    //public HeroAnimMachine heroAnim;
    public AssetReference heroAnimAddress;
}