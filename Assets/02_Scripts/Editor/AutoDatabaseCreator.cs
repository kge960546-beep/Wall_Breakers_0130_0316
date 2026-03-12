#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AutoDatabaseCreator
{
    private const string DATABASE_FOLDER = "Assets/06_Data/Database/";

    public static ScriptableObject CreateDatabaseIfNeeded(string csvPath)
    {
        string fileName = Path.GetFileNameWithoutExtension(csvPath);
        string dbName = fileName + "Database";

        string dbPath = DATABASE_FOLDER + dbName + ".asset";

        var db = AssetDatabase.LoadAssetAtPath<ScriptableObject>(dbPath);
        if (db != null) return db;

        var type = System.AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == dbName);

        if (type == null)
        {
#if UNITY_EDITOR
            Debug.LogError($"Database 타입 없음: {dbName}");
#endif
            return null;
        }

        Directory.CreateDirectory(DATABASE_FOLDER);

        var newDB = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(newDB, dbPath);
        AssetDatabase.SaveAssets();
#if UNITY_EDITOR
        Debug.Log($"자동 DB 생성: {dbName}");
#endif

        return newDB;
    }
}
#endif