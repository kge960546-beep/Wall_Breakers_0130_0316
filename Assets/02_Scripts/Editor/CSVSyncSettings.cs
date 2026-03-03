using System;
using System.Collections.Generic;
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
        return UnityEditor.AssetDatabase.LoadAssetAtPath<CSVSyncSettings>(
            "Assets/Editor/CSV/CSVSyncSettings.asset");
    }
#endif
}

[Serializable]
public class CSVSyncEntry
{
    public string csvPath;
    public ScriptableObject database;
}