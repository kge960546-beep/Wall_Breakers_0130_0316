#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class CSVSyncWindow : EditorWindow
{
    CSVSyncSettings settings;

    [MenuItem("Tools/CSV Sync Tool")]
    static void Open()
    {
        GetWindow<CSVSyncWindow>("CSV Sync");
    }

    void OnEnable()
    {
        settings = CSVSyncSettings.Load();
    }

    void OnGUI()
    {
        if (settings == null)
        {
            EditorGUILayout.HelpBox("CSVSyncSettings를 생성하세요.", MessageType.Warning);
            return;
        }

        foreach (var entry in settings.entries)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("CSV", entry.csvPath);
            entry.database = (ScriptableObject)EditorGUILayout.ObjectField(
                "Database", entry.database, typeof(ScriptableObject), false);

            if (GUILayout.Button("Import"))
                CSVSyncCore.Import(entry);

            if (GUILayout.Button("Export"))
                CSVSyncCore.Export(entry);

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Save Settings"))
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif