#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using DG.Tweening;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Content;
using Debug = UnityEngine.Debug;

public static class EditorMenuItems
{
    private const string BASE_MENU_PATH = "EditorUtils/Open Directory/";
    private const string SCENE_PATH = "EditorUtils/Scene/";

    [MenuItem("EditorUtils/Loading Scene")]
    private static void OpenLoadingScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        EditorSceneManager.OpenScene(pathOfFirstScene);
    }

    [MenuItem("EditorUtils/Main Menu Scene")]
    private static void OpenMenuScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[1].path;
        EditorSceneManager.OpenScene(pathOfFirstScene);
    }

    [MenuItem("EditorUtils/Game Scene")]
    private static void OpenGameScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[2].path;
        EditorSceneManager.OpenScene(pathOfFirstScene);
    }

    [MenuItem(BASE_MENU_PATH + "TemporaryCachePath")]
    private static void OpenTemporaryCachePath()
    {
        Process.Start(Application.temporaryCachePath);
    }

    [MenuItem(BASE_MENU_PATH + "PersistentDataPath")]
    private static void OpenPersistentDataPath()
    {
        Process.Start(Application.persistentDataPath);
    }

    [MenuItem("EditorUtils/Clear save data")]
    private static void ClearSave()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.*");

        foreach (FileInfo f in info)
        {
            File.Delete(f.FullName);
        }

        // ClearPlayerPrefs();
    }

    [MenuItem(BASE_MENU_PATH + "Clear PlayerPrefs")]
    private static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("EditorUtils/" + "PASS KEYSTORE")]
    private static void PassKeyStore()
    {
        string keyStorePassPath = Path.GetFullPath(
            "Assets/../../Key/ReleaseKey/keystore.txt");
        string keyStorePath = Path.GetFullPath(
            "Assets/../../Key/ReleaseKey/idle-zombie-release.keystore");
        
        string pass = File.ReadAllText(keyStorePassPath);

        PlayerSettings.Android.keystoreName = keyStorePath;
        PlayerSettings.Android.keystorePass = pass;
        PlayerSettings.Android.keyaliasName = "pine-entertainment";
        PlayerSettings.Android.keyaliasPass = pass;
    }
    
    [MenuItem("EditorUtils/" + "BUILD APK")]
    private static void BuildAPK()
    {
        string path = PreBuildSetup();
        InitializeBuildAssetBundle();
        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.None);
        ShowExplorer(path);
    }
    
    
    [MenuItem("EditorUtils/" + "BUILD AND RUN APK")]
    private static void BuildAndRunAPK()
    {
        string path = PreBuildSetup();
        InitializeBuildAssetBundle();
        // Build player.
        // BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(new BuildPlayerOptions());
        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.AutoRunPlayer);
        ShowExplorer(path);
    }
    
    static string PreBuildSetup()
    {
        PassKeyStore();

        // EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));

        string name = $"{Application.productName}-v{Application.version}";
        string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", name, "apk");
        
        return path;
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    static void ShowExplorer(string itemPath)
    {
        itemPath = itemPath.Replace(@"/", @"\"); // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
    
    // [InitializeOnLoadMethod]
    private static void InitializeBuildAssetBundle()
    {
        BuildPlayerHandler();
        // BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }
    
    private static void BuildPlayerHandler()
    {
        if (EditorUtility.DisplayDialog("Build with Addressables",
            "Do you want to build a clean addressables before export?",
            "Build with Addressables", "Skip"))
        {
            PreExport();
        }
    }
    
    static public void PreExport()
    {
        Debug.Log("BuildAddressablesProcessor.PreExport start");
        AddressableAssetSettings.CleanPlayerContent(
            AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("BuildAddressablesProcessor.PreExport done");
    }
}
#endif