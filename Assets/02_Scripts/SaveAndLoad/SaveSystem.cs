using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "Save.json");

    public static void Save(GameData data, bool prettuprint = true)
    {
        string json = JsonUtility.ToJson(data, prettuprint);

        File.WriteAllText(SavePath, json);

        Utils.DebugLog($"[SaveSystem] 파일 저장 완료: {SavePath}");
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

    public static void DeleteSaveData()
    {
        if(File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Utils.DebugLog($"<color=red>[SaveSystem] 세이브 파일 삭제 완료: {SavePath}</color>");
        }
        else
        {
            Utils.DebugLog("[SaveSystem] 삭제할 저장 파일이 없습니다");
        }
    }
}
