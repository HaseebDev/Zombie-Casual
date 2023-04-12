using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions.Localization;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;

[ExecuteInEditMode]
public class AutoResizeImage : EditorWindow
{
    public static Object folder;
    
    [MenuItem("EditorUtils/Auto Resize Image")]
    static void Init()
    {
        AutoResizeImage window = (AutoResizeImage) EditorWindow.GetWindow(typeof(AutoResizeImage));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }
    
    private void OnGUI()
    {
        folder = EditorGUILayout.ObjectField("Folder", folder, typeof(Object), true);
        
        if (GUILayout.Button("Resize"))
        {
            StartResize();
        }
        
        if (GUILayout.Button("Get Folder Name"))
        {
            GetFolderName();
        }
        
        if (GUILayout.Button("Get Not Square 4"))
        {
            GetNotSquare4Image();
        }
    }

    public static void GetFolderName()
    {
        string path =  AssetDatabase.GetAssetPath(folder);
        if (folder != null)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (file.Contains(".png") && !file.Contains(".meta"))
                {
                    LoadPNG(file);
                } 
            }  
        } 
    }
    
    public static void LoadPNG(string filePath) {
 
        Texture2D tex = null;
        byte[] fileData;
 
        if (File.Exists(filePath))
        {
            Texture2D tmpTexture = new Texture2D(1,1);
            byte[] tmpBytes = File.ReadAllBytes(filePath);
            tmpTexture.LoadImage(tmpBytes);

            TextureScale.Bilinear(tmpTexture,ConvertToSquare4(tmpTexture.width),ConvertToSquare4(tmpTexture.height));
            
            Debug.LogError(tmpTexture.width + " " + tmpTexture.height);
 
            // Texture2D itemBGTex = itemBGSprite.texture;
            byte[] itemBGBytes = tmpTexture.EncodeToPNG();
            File.WriteAllBytes( filePath , itemBGBytes );
        }
    }

    private static void StartResize()
    {
        
    }

    private static int ConvertToSquare4(int size)
    {
        while (size % 4 != 0)
        {
            size++;
        }

        return size;
    }

    public static void GetNotSquare4Image()
    {
        string result = "";
        
        string path =  AssetDatabase.GetAssetPath(folder);
        if (folder != null)
        {
            foreach (string file in Directory.GetFiles(path,"*.png",SearchOption.AllDirectories))
            {
                if (file.Contains(".png") && !file.Contains(".meta"))
                {
                    Texture2D tmpTexture = new Texture2D(1,1);
                    byte[] tmpBytes = File.ReadAllBytes(file);
                    tmpTexture.LoadImage(tmpBytes);
                    //Debug.LogError(file + " "+ tmpTexture.width + " " + tmpTexture.height);

                    if (tmpTexture.width % 4 != 0 || tmpTexture.height % 4 != 0)
                    {
                        result += file + "\n";
                    }
                    // LoadPNG(file);
                } 
            }  
        }

        Debug.LogError(result);
    }

}

#endif