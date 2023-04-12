using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using ExcelDataReader;
using UnityExtensions.Localization.Editor;

public class ExcelToAttribute : EditorWindow
{
    public static string scriptPath = "Assets/Scripts/DesignParsers/";

    public static string sheetName = "TalentDesign";
    public static string excelPath;

    public static string NAME_DESIGN = "NAME_DESIGN";
    public static string NAME_WRAPPER = "NAME_WRAPPER";
    public static string NAME_ELEMENT = "NAME_ELEMENT";
    public static string ATTRIBUTES = "ATTRIBUTES";


    [MenuItem("EditorUtils/Excel to attribute")]
    static void Init()
    {
        ExcelToAttribute window = (ExcelToAttribute) EditorWindow.GetWindow(typeof(ExcelToAttribute));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }

    private void OnGUI()
    {
        sheetName = EditorGUILayout.TextField("Sheet name", sheetName);
        excelPath = Path.GetFullPath(
            "Assets/../../Designs/Design.xlsx");

        string format = "\t[JsonProperty(\"{0}\", Required = Required.Always)]\n\tpublic {1} {2}";
        string end = " { get; set; }\n";

        if (GUILayout.Button("GenerateText"))
        {
            string result = "";
            TextAsset defaultDesignAsset = Resources.Load("Utils/DefaultDesign") as TextAsset;
            string defaultDesign = defaultDesignAsset.text;

            ExcelHelper.ReadFile(excelPath, sheet =>
            {
                if (sheet == sheetName)
                {
                    ExcelHelper.ReadLine();
                    ExcelHelper.ReadLine();
                    int fieldCount = ExcelHelper.fieldCount;
                    for (int i = 1; i < fieldCount; i++)
                    {
                        var columnName = ExcelHelper.GetString(i).Split(':');
                        var name = columnName[0].Replace(".", "");
                        var type = columnName[1];
                        if (type == "str")
                            type = "string";

                        result += string.Format(format, name, type, name) + end + "\n";
                    }
                }
            });

            defaultDesign = defaultDesign.Replace(NAME_DESIGN, sheetName.Replace("Design", ""));
            defaultDesign = defaultDesign.Replace(NAME_WRAPPER, sheetName);
            defaultDesign = defaultDesign.Replace(NAME_ELEMENT, sheetName + "Element");
            defaultDesign = defaultDesign.Replace(ATTRIBUTES, result);

            string savePath = Path.GetFullPath(scriptPath);
            string finalPath = savePath + sheetName + ".cs";
            if (Directory.Exists(finalPath))
            {
                Debug.LogError("Already have");
            }
            else
            {
                System.IO.File.WriteAllText(finalPath, defaultDesign);
                System.Diagnostics.Process.Start("explorer.exe", "/select," + finalPath);
            }

            Debug.LogError("Success save to " + finalPath);
        }
    }
}

#endif