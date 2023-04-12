using System;
using System.Collections;
using System.Collections.Generic;
using Ez.Pooly;
using MEC;
using Spine;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableManager : BaseSystem<AddressableManager>
{
    public static AddressableManager instance;

    private Dictionary<string, GameObject> _dictLoadedObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        instance = this;
    }

    #region AsyncLoad

    public void PreloadFromAddressable<T>(string itemName, Action<T> callback)
    {
        StartCoroutine(TaskLoadObjectAsync<T>(itemName, callback));
    }

    public void LoadFromAddressable(string itemName, Action callback)
    {
        if (Pooly.IsContainKey(itemName))
        {
            callback?.Invoke();
        }
        else
        {
            LoadObjectAsync<GameObject>(itemName, (t) =>
            {
                Pooly.Spawn(t.transform, Vector3.zero, Quaternion.identity);
                Pooly.Despawn(t.transform);
                callback?.Invoke();
            });
        }
    }


    public void SpawnFromAddressable<T>(string itemName, Vector3 position, Quaternion rotation, Transform parent,
        Action<T> callBack)
    {
        if (Pooly.IsContainKey(itemName))
        {
            T result = default(T);
            Transform trf = null;
            Transform resultTransform = Pooly.Spawn(itemName, position, rotation, parent);
            if (resultTransform != null)
                result = resultTransform.GetComponent<T>();
            callBack?.Invoke(result);
        }
        else
        {
            LoadObjectAsync<GameObject>(itemName, (t) =>
            {
                Transform resultTransform = Pooly.Spawn(t.transform, position, rotation, parent);
                callBack?.Invoke(resultTransform.GetComponent<T>());
            });
        }
    }

    public void LoadObjectAsync<T>(string objectKey, Action<T> result, bool saveToDict = false)
    {
        StartCoroutine(TaskLoadObjectAsync<T>(objectKey, result));
    }

    public IEnumerator<float> TaskLoadObjectAsync<T>(string objectKey, Action<T> result, bool saveToDict = false)
    {
        if (saveToDict)
        {
            _dictLoadedObjects.TryGetValue(objectKey, out var go);
            if (go != null)
            {
                var r = go.GetComponent<T>();
                result?.Invoke(r);
                yield break;
            }
        }

        var op = Addressables.LoadAssetAsync<GameObject>(objectKey);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;

        if (saveToDict)
        {
            _dictLoadedObjects.Add(objectKey, op.Result.gameObject);
        }

        if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            result?.Invoke(op.Result.GetComponent<T>());
        }
        else
            result?.Invoke(default(T));
    }

    #endregion

    #region Spawn Async
    public void SpawnObjectAsync<T>(string objectName, Action<T> callback)
    {
        Timing.RunCoroutine(SpawnObjectAsyncCoroutine(objectName, callback));
    }

    IEnumerator<float> SpawnObjectAsyncCoroutine<T>(string objectName, Action<T> callback)
    {
        var op = Addressables.LoadAssetAsync<GameObject>(objectName);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        var obj = GameObject.Instantiate(op.Result);
        obj.transform.SetParent(null);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        var result = obj.GetComponent<T>();

        callback?.Invoke(result);
    }

    #endregion
}