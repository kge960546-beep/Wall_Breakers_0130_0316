using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGameDataManager : MonoBehaviour
{
    public static SceneGameDataManager instance;

    [Header("저장할 데이터들")]
    public int currentGold;
    public int unCollectedGold;
    public int[] sectionMineralCount = new int[5];
    public int[] sectionProcessMineralCount = new int[5];
    public int unCollectedMoney;
    public bool[] unlockedSections;
    public int[] sectionFillAmount;
    public int autoNPCLevel;
    public int playerPowerLevel;

    private bool isPendingLoad = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SaveGame()
    {
        SaveGoldObject();

        GameData data = new GameData();

        data.currentGold = this.currentGold;
        data.unCollectedGold = this.unCollectedGold;

        data.sectionMineralCount = (int[])this.sectionMineralCount.Clone();
        data.sectionProcessMineralCount = (int[])this.sectionProcessMineralCount.Clone();

        data.unCollectedMoney = this.unCollectedMoney;
        data.unlockedSections = (bool[])this.unlockedSections.Clone(); //배열은 복제해서 저장하는게 좋음
        data.sectionFillAmount = (int[])this.sectionFillAmount.Clone();
        data.autoNPCLevel = this.autoNPCLevel;
        data.playerPowerLevel = this.playerPowerLevel;

        SaveSystem.Save(data);
        Debug.Log($"[저장확인] 지갑: {data.currentGold} / 바닥: {data.unCollectedGold}");
        Debug.Log("<color=green>1. 파일 저장 완료</color>");
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.Load();

        if (data != null)
        {
            this.currentGold = data.currentGold;

            this.sectionMineralCount = (int[])data.sectionMineralCount.Clone();
            this.sectionProcessMineralCount = (int[])data.sectionProcessMineralCount.Clone();

            this.unlockedSections = (bool[])data.unlockedSections.Clone();
            this.unCollectedMoney = data.unCollectedMoney;
            this.sectionFillAmount = (int[])data.sectionFillAmount.Clone();
            this.autoNPCLevel = data.autoNPCLevel;
            this.playerPowerLevel = data.playerPowerLevel;

            isPendingLoad = true;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            Debug.Log($"[LoadGame] 파일에서 읽은 데이터 확인 - 섹션1: {data.sectionMineralCount[0]}개");
            Debug.Log($"[LoadGame] 파일에서 읽은 데이터 확인 - 섹션2: {data.sectionMineralCount[1]}개");
            Debug.Log($"[LoadGame] 파일에서 읽은 데이터 확인 - 섹션3: {data.sectionMineralCount[2]}개");
            Debug.Log($"[LoadGame] 파일에서 읽은 데이터 확인 - 섹션4: {data.sectionMineralCount[3]}개");
            Debug.Log($"[LoadGame] 파일에서 읽은 데이터 확인 - 섹션5: {data.sectionMineralCount[4]}개");
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isPendingLoad)
        {
            isPendingLoad = false;
            StartCoroutine(SceneLoadSaveData());
        }
    }

    IEnumerator SceneLoadSaveData()
    {
        yield return new WaitForEndOfFrame();

        Debug.Log($"복구 시작 - 섹션1 데이터: {sectionMineralCount[0]}");
        Debug.Log($"복구 시작 - 섹션2 데이터: {sectionMineralCount[1]}");
        Debug.Log($"복구 시작 - 섹션3 데이터: {sectionMineralCount[2]}");
        Debug.Log($"복구 시작 - 섹션4 데이터: {sectionMineralCount[3]}");
        Debug.Log($"복구 시작 - 섹션5 데이터: {sectionMineralCount[4]}");

        var creditService = GameManager.Instance.GetService<CreditService>();
        if (creditService != null)
        {
            creditService?.SetCredit(currentGold);
            Debug.Log($"<color=gold>[Load] CreditService 데이터 복구 완료: {currentGold}</color>");
        }

        RestorePendingCredits();

        ResourceTableLoad();
        ProcessTableLoad();

        Utils.DebugLog("씬 재시작후 불러오기 완료");
    }
    public void SaveGoldObject()
    {
        int pendingTotal = 0;
        CreditObject[] credits = FindObjectsByType<CreditObject>(FindObjectsSortMode.None);
        foreach (var c in credits) { if (!c.IsCollected) pendingTotal += c.CreditAmount; }
        this.unCollectedGold = pendingTotal;
    }

    public void RestorePendingCredits()
    {
        if (unCollectedGold <= 0) return;

        CreditSpawner spawner = FindObjectOfType<CreditSpawner>();
        if (spawner != null)
        {
            spawner.SpawnCredit(unCollectedGold);
        }
    }

    public void ResourceTableLoad()
    {
        ResourceTable[] tables = FindObjectsByType<ResourceTable>(FindObjectsSortMode.None);
        foreach (ResourceTable table in tables)
        {
            if (table != null)
            {
                int saveAmount = sectionMineralCount[table.SectionIndex];
                table.RebuildStack(saveAmount);
            }
        }
    }

    public void ProcessTableLoad()
    {
        ProcessResource[] processors = FindObjectsByType<ProcessResource>(FindObjectsSortMode.None);

        foreach (var proc in processors)
        {
            int saveAmount = sectionProcessMineralCount[proc.sectionIndex];
            proc.RebuildProcessedStack(saveAmount);
        }

        Utils.DebugLog("모든 테이블 오브젝트 복구 완료");
    }
}
