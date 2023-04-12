using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class FireUserData
{
    [FirestoreProperty]
    public string UserID { get; set; }

    [FirestoreProperty]
    public long Revision { get; set; }

    [FirestoreProperty]
    public string FirebaseID { get; set; }

    [FirestoreProperty]
    public byte[] BytesData { get; set; }
}
