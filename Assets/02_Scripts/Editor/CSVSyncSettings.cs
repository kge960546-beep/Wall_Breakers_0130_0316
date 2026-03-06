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
        string rootFolder = "Assets/02_Scripts";
        string folder = rootFolder + "/Editor";
        string path = folder + "/CSVSyncSettings.asset";

        // 1. 폴더 생성
        if (!AssetDatabase.IsValidFolder(rootFolder))
        {
            AssetDatabase.CreateFolder("Assets", "02_Scripts");
            AssetDatabase.Refresh();
        }

        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder(rootFolder, "Editor");
            AssetDatabase.Refresh();
        }

        // 2. 다시 로드
        var settings = AssetDatabase.LoadAssetAtPath<CSVSyncSettings>(path);

        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<CSVSyncSettings>();

            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("CSVSyncSettings 자동 생성 완료");
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