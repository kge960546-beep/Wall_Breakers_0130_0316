using UnityEngine;

public static class Utils
{
    //디버그로그
    public static void DebugLog(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    //디버그 경고
    public static void DebugLogWarning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }

    //디버그 에러
    public static void DebugLogError(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
    }
}

//예제 사용법
public class UtilsExample : MonoBehaviour
{
    void Start()
    {
        Utils.DebugLog("UtilsExample Start method called.");
        Utils.DebugLogWarning("This is a debug warning message.");
        Utils.DebugLogError("This is a debug error message.");
    }
}
