#if UNITY_EDITOR
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
        if (settings == null) return;

        foreach (var path in importedAssets)
        {
            if (!path.EndsWith(".csv")) continue;

            var entry = settings.GetEntry(path);
            if (entry == null) continue;

            CSVSyncCore.Import(entry);
        }
    }
}
#endif