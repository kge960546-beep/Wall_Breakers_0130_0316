#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AutoDatabaseCreator
{
    private const string DATABASE_FOLDER = "Assets/GameData/";

    public static ScriptableObject CreateDatabaseIfNeeded(string csvPath)
    {
        string fileName = Path.GetFileNameWithoutExtension(csvPath);
        string dbName = fileName + "Database";

        string dbPath = DATABASE_FOLDER + dbName + ".asset";

        var db = AssetDatabase.LoadAssetAtPath<ScriptableObject>(dbPath);
        if (db != null)
            return db;

        // 타입 찾기
        var type = System.Type.GetType(dbName);

        if (type == null)
        {
            Debug.LogError($"Database 타입을 찾을 수 없습니다: {dbName}");
            return null;
        }

        ScriptableObject newDB = ScriptableObject.CreateInstance(type);

        Directory.CreateDirectory(DATABASE_FOLDER);

        AssetDatabase.CreateAsset(newDB, dbPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"자동 DB 생성: {dbName}");

        return newDB;
    }
}
#endif