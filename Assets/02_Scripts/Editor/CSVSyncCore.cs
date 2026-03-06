#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CSVSyncCore
{
    public static void Import(CSVSyncEntry entry)
    {
        if (entry.database == null) return;
        if (!File.Exists(entry.csvPath)) return;

        string csv = File.ReadAllText(entry.csvPath);

        CSVParser.Parse(csv, entry.database);
        CSVValidator.Validate(entry.database);

        EditorUtility.SetDirty(entry.database);
        AssetDatabase.SaveAssets();

        Debug.Log($"CSV Import 완료: {entry.csvPath}");
    }

    public static void Export(CSVSyncEntry entry)
    {
        if (entry.database == null) return;

        CSVExporter.Export(entry.database, entry.csvPath);

        Debug.Log($"CSV Export 완료: {entry.csvPath}");
    }
}
#endif