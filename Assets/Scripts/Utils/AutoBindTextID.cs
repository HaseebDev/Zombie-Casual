using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using QuickEngine.Extensions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class AutoBindTextID : EditorWindow
{
    static public GameObject source;
    static public LocalizedTMPTextProfile generalProfile;
    static public bool bindId;
    static public bool forceGeneraProfile;

    [MenuItem("EditorUtils/Search For Components")]
    static void Init()
    {
        AutoBindTextID window = (AutoBindTextID) EditorWindow.GetWindow(typeof(AutoBindTextID));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }


    public static void BindGeneralLayout()
    {
        TextMeshProUGUI[] allFoundScripts = source.GetComponentsInChildren<TextMeshProUGUI>();
        List<string> words = new List<string>();
        List<string> ids = new List<string>();


        foreach (TextMeshProUGUI foundScript in allFoundScripts)
        {
            // If not number
            var localizeUI = foundScript.GetComponent<LocalizedTMPText>();
            if (localizeUI != null)
                localizeUI.profile = generalProfile;
        }

        UnityEditor.EditorUtility.SetDirty(source);
    }

    public static void CreateTextIdForExcel()
    {
        TextMeshProUGUI[] allFoundScripts = source.GetComponentsInChildren<TextMeshProUGUI>();
        List<string> words = new List<string>();
        List<string> ids = new List<string>();


        foreach (TextMeshProUGUI foundScript in allFoundScripts)
        {
            string word = foundScript.text;

            // If not number
            bool isNumeric = int.TryParse(word, out var n);
            if (!isNumeric)
            {
                string id = word.ToUpper();
                word = word.ToLower();
                word = word.FirstCharToUpper();

                id = id.Replace(" ", "_");

                if (!words.Contains(word))
                {
                    words.Add(word);
                }

                if (!ids.Contains(id))
                    ids.Add(id);

                if (bindId)
                {
                    var localizeUI = foundScript.GetComponent<LocalizedTMPText>();
                    if (localizeUI != null)
                    {
                        localizeUI.textName = id;
                        localizeUI.profile = generalProfile;
                    }
                }
            }
        }

        string wordsResult = "";
        string idsResult = "";

        foreach (var ss in words)
        {
            wordsResult += ss + "\n";
        }

        foreach (var ss in ids)
        {
            idsResult += ss + "\n";
        }

        Debug.LogError("WORD \n" + wordsResult);
        Debug.LogError("ID \n" + idsResult);
        UnityEditor.EditorUtility.SetDirty(source);
    }

    private void OnGUI()
    {
        generalProfile =
            EditorGUILayout.ObjectField("TextProfile", generalProfile, typeof(LocalizedTMPTextProfile), true) as
                LocalizedTMPTextProfile;

        if (generalProfile == null)
            generalProfile = Resources.Load("LocalizeResource/LocalizeProfile/General") as LocalizedTMPTextProfile;


        source = EditorGUILayout.ObjectField("Prefab", source, typeof(GameObject), true) as GameObject;
        bindId = EditorGUILayout.Toggle("Bind id", bindId);
        forceGeneraProfile = EditorGUILayout.Toggle("Force use general profile", forceGeneraProfile);

        // if (GUILayout.Button("Check component usage"))
        // {
        //     Text[] allFoundScripts = source.GetComponentsInChildren<Text>();
        //     Debug.LogError(allFoundScripts.Length);
        //     foreach (Text foundScript in allFoundScripts)
        //     {
        //         var go = foundScript.gameObject;
        //         string text = foundScript.text;
        //         DestroyImmediate(foundScript, true);
        //
        //         // var newGo = new GameObject(go.name);
        //
        //         var newTextComp = go.AddComponent<LocalizedTMPTextUI>();
        //         newTextComp.textName = text;
        //         newTextComp.profile = outlineProfile;
        //     }
        //
        //     UnityEditor.EditorUtility.SetDirty(source);
        // }

        if (GUILayout.Button("Get text for excel"))
        {
            CreateTextIdForExcel();
        }

        if (GUILayout.Button("Bind general layout"))
        {
            BindGeneralLayout();
        }
    }

    public void GetText()
    {
        Text[] allFoundScripts = source.GetComponentsInChildren<Text>();
        List<string> s = new List<string>();

        foreach (Text foundScript in allFoundScripts)
        {
            string a = foundScript.text;

            if (!s.Contains(a))
                s.Add(a);
        }

        string re = "";
        foreach (var ss in s)
        {
            re += ss + "\n";
        }

        Debug.LogError(re);
    }
}

// if (GUILayout.Button("Check component usage"))
// {
// Text[] allFoundScripts = Resources.FindObjectsOfTypeAll<Text>();
// List<string> s = new List<string>();
//
//     foreach (Text foundScript in allFoundScripts)
// {
//     //Debug.LogError("Found the script in: " + foundScript.gameObject);
//     string a = foundScript.text;
//     int temp = -9999;
//
//     Int32.TryParse(a, out temp);
//     Debug.LogError(temp);
//     if (temp == 0)
//     {
//         if (!s.Contains(a))
//             s.Add(a);
//     }
//
//     // Select the script in the inspector, if you want to
//     // UnityEditor.Selection.activeGameObject = foundScript.gameObject;
//
//     // You can also change variables on the found script
//     //foundScript.someVariable = 13;
//     // Set dirty forces the inspector to save the change (there may be a better way to do this)
//     // UnityEditor.EditorUtility.SetDirty(foundScript);
// }
//
// string re = "";
//     foreach (var ss in s)
// {
//     re += ss + "\n";
// }
//
// Debug.LogError(re);
// }
#endif