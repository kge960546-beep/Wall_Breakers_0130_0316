using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager instance;
    public List<AchievementSO> achievements;

    private HashSet<string> unlockIDs = new HashSet<string>();

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

    private void ResetAllAchievements()
    {
        foreach (var achievement in achievements)
        {
            if(achievement != null)
                achievement.isUnlocked = false;
        }
    }

    //세이브매니저 호출하는 함수
    public void InitializeAchievements(List<string> savedIDs)
    {
        unlockIDs = new HashSet<string> (savedIDs);

        foreach(var a in achievements)
        {
            a.isUnlocked = unlockIDs.Contains (a.id);
        }
    }

    public List<string> GetUnlockedIds()
    {
        return new List<string> (unlockIDs);
    }

    public void ProgressAchievement(string id, int amount)
    {
        if (unlockIDs.Contains(id)) return;

        AchievementSO achieve = achievements.Find(a => a.id == id);

        if (achieve != null)
        {
            if (amount >= achieve.targetValue)
            {
                UnlockAchievement(achieve);
            }
        }
    }

    public void UnlockAchievement(AchievementSO a)
    {
        if (unlockIDs.Contains(a.id)) return;

        unlockIDs.Add(a.id);
        a.isUnlocked = true;

        SceneGameDataManager.instance.SaveGame();

        Utils.DebugLog($"업적달성: {a.title}");

        //TODO: UI연출 넣기
    }   
}
