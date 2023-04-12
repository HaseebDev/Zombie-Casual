using UnityEngine;
using System.IO;

public interface ISaveGame<T>
{
    bool EnableAutoSave { get; }
    bool IsDataDirty { get; }

    T Data { get; }
    T LoadData();
    bool SyncData();
    void SaveData(bool isCloseGame = false);
}

public static class FileManager
{
    /// <summary>
    /// Load File
    /// </summary>
    /// <typeparam name="T">Data Model Type</typeparam>
    /// <param name="filename">File Name</param>
    /// <returns>Instance</returns>
    public static byte[] LoadFile(string filename)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);
        //Debug.LogError($"Load file as {filePath}");
        byte[] output = null;

        if (File.Exists(filePath))
        {
            byte[] dataAsBytes = File.ReadAllBytes(filePath);

            output = dataAsBytes;
        }


        return output;
    }

    /// <summary>
    /// Save File
    /// </summary>
    /// <typeparam name="T">Model Type</typeparam>
    /// <param name="filename">File Name</param>
    /// <param name="content">Model Content</param>
    public static void SaveFile(string filename, byte[] content)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllBytes(filePath, content);
       // Debug.Log($"Save file as {filePath}");
    }

    public static void SaveFileText(string filename, string content)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(filePath, content);
      //  Debug.Log($"Save file text as {filePath}");
    }

    public static string[] LoadFileLines(string filename)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);
        //Debug.LogError($"Load file as {filePath}");
        string[] output = null;

        if (File.Exists(filePath))
        {
            string[] dataAsLines = File.ReadAllLines(filePath);

            output = dataAsLines;
        }
        return output;
    }

}


