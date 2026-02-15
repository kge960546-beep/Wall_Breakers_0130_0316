using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "Save.json");

    public static void Save(GameData data, bool prettuprint = true)
    {
        string json = JsonUtility.ToJson(data, prettuprint);

        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveSystem] 파일 저장 완료: {SavePath}");
    }

    public static GameData Load()
    {
        if (!File.Exists(SavePath))
        {            
            Utils.DebugLog("파일 없음");
            return null;
        }

        string json = File.ReadAllText(SavePath);

        GameData data = JsonUtility.FromJson<GameData>(json);

        Utils.DebugLog("불러오기 성공");

        return data;
    }
}
