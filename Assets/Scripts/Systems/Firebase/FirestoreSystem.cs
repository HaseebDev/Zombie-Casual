using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class SampleSubData
{
    [FirestoreProperty]
    public long Donate { get; set; }
}

public class FirestoreSystem : BaseSystem<FirestoreSystem>
{
    public static FirestoreSystem instance;
    public FirebaseFirestore db { get; private set; }

    public bool EnableFireStore {
        get {
#if UNITY_EDITOR
            return false;
#else
            return true;
#endif
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);
        db = FirebaseFirestore.DefaultInstance;
    }

    public void GetCollectionData<T>(string collectionName, Action<List<T>> callBack)
    {
        CollectionReference usersRef = db.Collection(collectionName);
        usersRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            List<T> listResult = new List<T>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                try
                {
                    Debug.Log($"Document ID ${document.Id}");
                    T element = document.ConvertTo<T>();
                    listResult.Add(element);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Firestore] GetData error {ex}");
                }

            }

            callBack?.Invoke(listResult);
            Debug.Log("Read all data from the users collection.");
        });

    }

    public void GetCollectionDataOrderBy<T>(string collectionName, string orderBy, int limit, Action<List<T>> callBack)
    {
        CollectionReference usersRef = db.Collection(collectionName);
        Query query = usersRef.OrderBy(orderBy).Limit(limit);
        query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            List<T> listResult = new List<T>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                try
                {
                    Debug.Log($"Document ID ${document.Id}");
                    T element = document.ConvertTo<T>();
                    listResult.Add(element);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Firestore] GetData error {ex}");
                }

            }

            callBack?.Invoke(listResult);
            Debug.Log("Read all data from the users collection.");
        });
    }

    public void TryGetDocument<T>(string collectionName, string documentID, Action<bool, T> callback)
    {
        if (!EnableFireStore)
        {
            callback?.Invoke(false, default(T));
            return;
        }

        DocumentReference docRef = db.Collection(collectionName).Document(documentID);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                T result = snapshot.ConvertTo<T>();
                callback?.Invoke(true, result);
            }
            else
            {
                callback?.Invoke(false, default(T));
            }

        });
    }

    public void PushData<T>(string collectionName, string documentID, T data, Action<bool> complete)
    {
        if (!EnableFireStore)
            return;

        DocumentReference docRef = db.Collection(collectionName).Document(documentID);
        docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            Debug.Log($"Added data to the {documentID} document in the {collectionName} collection.");
            complete?.Invoke(task.IsCompleted);
        });
    }

}
