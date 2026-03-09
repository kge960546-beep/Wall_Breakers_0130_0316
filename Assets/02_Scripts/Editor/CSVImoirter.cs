#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public class CSVImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        var settings = CSVSyncSettings.Load();

        foreach (var path in importedAssets)
        {
            if (!path.EndsWith(".csv")) continue;

            // 엑셀 임시파일 무시
            if (Path.GetFileName(path).StartsWith("~$"))
                continue;

            var entry = settings?.GetEntry(path);

            if (entry == null)
            {
                // 자동 DB 생성 시도
                var db = AutoDatabaseCreator.CreateDatabaseIfNeeded(path);
                if (db == null) continue;

                entry = new CSVSyncEntry
                {
                    csvPath = path,
                    database = db
                };

                settings.entries.Add(entry);
                EditorUtility.SetDirty(settings);
            }

            CSVSyncCore.Import(entry);
        }
    }
}
#endif