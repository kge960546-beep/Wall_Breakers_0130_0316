using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "CSV/CSV Sync Settings")]
public class CSVSyncSettings : ScriptableObject
{
    public List<CSVSyncEntry> entries = new();

    public CSVSyncEntry GetEntry(string csvPath)
    {
        return entries.Find(e => e.csvPath == csvPath);
    }

#if UNITY_EDITOR
    public static CSVSyncSettings Load()
    {
        string folder = "Assets/02_Scripts/Editor";
        string path = folder + "/CSVSyncSettings.asset";

        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "EditorDefaultResources");
        }

        var settings = AssetDatabase.LoadAssetAtPath<CSVSyncSettings>(path);

        if (settings == null)
        {
            settings = CreateInstance<CSVSyncSettings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();

            Debug.Log("CSVSyncSettings »ý¼º");
        }

        return settings;
    }
#endif
}

[Serializable]
public class CSVSyncEntry
{
    public string csvPath;
    public ScriptableObject database;
}