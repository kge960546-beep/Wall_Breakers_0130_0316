using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager instance;
    public List<AchievementSO> achievements;

    [SerializeField] private HashSet<string> unlockIDs = new HashSet<string>();

    private System.Action<AchievementSO> onAchievementUnlocked;

    public void SubscribeonAchievementUnlocked(Action<AchievementSO> action)
    {
        onAchievementUnlocked += action;
    }
    public void UnsubscribeonAchievementUnlocked(Action<AchievementSO> action)
    {
        onAchievementUnlocked -= action;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            ResetAllAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //게임 시작시 메모리 상의 데이터 초기화
    private void ResetAllAchievements()
    {
        foreach (var achievement in achievements)
        {
            if (achievement != null)
                achievement.isUnlocked = false;
        }
    }

    //세이브매니저 호출하는 함수로 업적 달성 저장을 불러오기
    public void InitializeAchievements(List<string> savedIDs)
    {
        unlockIDs = new HashSet<string>(savedIDs);

        foreach (var a in achievements)
        {
            a.isUnlocked = unlockIDs.Contains(a.id);
        }
    }

    //세이브 데이터용 함수로 HashSet내부 데이터를 외부에서 읽을 수 있게 변환하는 함수
    public List<string> GetUnlockedIds()
    {
        return new List<string>(unlockIDs);
    }

    //업적 달성조건에 부합하면 달성함수 호출
    public void ProgressAchievement(string id, int amount)
    {     
        foreach(var achieve in achievements)
        {
            if (achieve == null) continue;

            if(achieve.id.Contains(id) && !unlockIDs.Contains(achieve.id))
            {

                if (amount >= achieve.targetValue)
                {
                    UnlockAchievement(achieve);
                }
            }
        }
    }

    //업적 달성시 UI갱신
    public void UnlockAchievement(AchievementSO a)
    {
        if (unlockIDs.Contains(a.id)) return;

        unlockIDs.Add(a.id);
        a.isUnlocked = true;

        if(SceneGameDataManager.instance != null)
            SceneGameDataManager.instance.SaveGame();

        Utils.DebugLog($"업적달성: {a.title}");

        onAchievementUnlocked?.Invoke(a);
    }

    public void ResetAllSO()
    {
        unlockIDs.Clear();
        foreach (var a in achievements)
        {
            if (a != null)
            {
                a.isUnlocked = false;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(a);
#endif
            }
        }
    }
}
