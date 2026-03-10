#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CSVSyncCore
{
    public static void Import(CSVSyncEntry entry)
    {
        if (entry.database == null)
        {
            Debug.LogError("Database가 설정되지 않았습니다.");
            return;
        }

        if (!File.Exists(entry.csvPath))
        {
            Debug.LogError($"CSV 파일이 없습니다: {entry.csvPath}");
            return;
        }

        string csv = File.ReadAllText(entry.csvPath, Encoding.UTF8);

        CSVParser.Parse(csv, entry.database);
        CSVValidator.Validate(entry.database);

        EditorUtility.SetDirty(entry.database);
        AssetDatabase.SaveAssets();

        Debug.Log($"CSV Import 완료: {entry.csvPath}");
    }

    public static void Export(CSVSyncEntry entry)
    {
        if (entry.database == null)
        {
            Debug.LogError("Database가 설정되지 않았습니다.");
            return;
        }

        CSVExporter.Export(entry.database, entry.csvPath);

        Debug.Log($"CSV Export 완료: {entry.csvPath}");
    }
}
#endif