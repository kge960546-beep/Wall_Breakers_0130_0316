using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneGameDataManager : MonoBehaviour
{
    public static SceneGameDataManager instance;

    public bool isRefreshing = false;

    #region 저장 변수
    //GameData(디스크에 저장될 파일의 규격) SceneGameDataManager(RAM에 실시간으로 바뀌는 값)이라 분리했습니다.
    [Header("현재 가지고있는 골드, 생성된 골드 오브젝트")]
    public int currentGold;
    public int unCollectedGold;

    [Header("각 섹션별 채굴되는 자원 갯수, 섹션별 가공품 갯수")]
    public int[] sectionMineralCount = new int[5];
    public int[] sectionProcessMineralCount = new int[5];

    [Header("판매후 스폰된 재화 오브젝트 갯수")]
    public int unCollectedMoney;

    [Header("섹션해금유무, 섹션해금에 들어간 자원수량")]
    public List<int> unlockedRegionList = new List<int>();
    public bool[] unlockedSections;
    public int[] sectionFillAmount;

    [Header("업그래이드 리스트")]
    public List<string> unlockedUpgradeNodeIds = new List<string>();
    public List<string> savedAchievements = new List<string>();

    [Header("가이드 퀘스트")]
    public int guideCurrentIndex;
    public List<int> guideSaveData = new List<int>();
    #endregion

    private bool isPendingLoad = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (unlockedSections == null || unlockedSections.Length == 0)
            {
                unlockedSections = new bool[5];
            }

            if (sectionFillAmount == null || sectionFillAmount.Length == 0)
            {
                sectionFillAmount = new int[100];
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        AutoLoadOnStart();
    }
    private void OnDisable()
    {
        SaveGame();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveGame();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [ContextMenu("Delete All Save Data")]
    public void ClearAllSaveData()
    {
        SaveSystem.DeleteSaveData();
        PlayerPrefs.DeleteAll();

        this.currentGold = 0;
        this.unCollectedGold = 0;
        this.unCollectedMoney = 0;

        if (sectionMineralCount != null)
        {
            for (int i = 0; i < sectionMineralCount.Length; i++) sectionMineralCount[i] = 0;
        }

        if (sectionProcessMineralCount != null)
        {
            for (int i = 0; i < sectionProcessMineralCount.Length; i++) sectionProcessMineralCount[i] = 0;
        }

        this.unlockedRegionList.Clear();
        this.unlockedRegionList.Add(1);

        if (unlockedSections != null)
        {
            for (int i = 0; i < unlockedSections.Length; i++)
            {
                unlockedSections[i] = false;
            }
        }

        if (sectionFillAmount != null)
        {
            for (int i = 0; i < sectionFillAmount.Length; i++) sectionFillAmount[i] = 0;
        }

        if (RegionUnlockService.Instance != null)
        {
            RegionUnlockService.Instance.ManagerLink(this.unlockedRegionList);            
        }

        this.unlockedUpgradeNodeIds.Clear();
        this.savedAchievements.Clear();

        var creditService = GameManager.Instance?.GetService<CreditService>();
        if (creditService != null)
        {
            creditService.SetCredit(0);
        }

        if (Application.isPlaying)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        if (AchievementsManager.instance != null)
        {
            AchievementsManager.instance.ResetAllSO();
        }

        this.guideCurrentIndex = 0;
        this.guideSaveData.Clear();
        if(GuideManager.Instance != null)
        {
            GuideManager.Instance.ResetGuideData();
        }

        Utils.DebugLog("데이터 초기화 후 씬 재시작함");
    }

    public void SaveGame()
    {
        if (isPendingLoad || isRefreshing) return;

        SaveGoldObject();
        SaveUnlockSection();

        GameData data = new GameData();

        data.currentGold = this.currentGold;
        data.unCollectedGold = this.unCollectedGold;
        data.sectionMineralCount = (int[])this.sectionMineralCount.Clone();
        data.sectionProcessMineralCount = (int[])this.sectionProcessMineralCount.Clone();
        data.unCollectedMoney = this.unCollectedMoney;
        data.unlockedSections = (bool[])this.unlockedSections.Clone(); //배열은 복제해서 저장하는게 좋음
        data.sectionFillAmount = (int[])this.sectionFillAmount.Clone();        
        
        data.achievementProgess = AchievementsManager.instance.GetUnlockedIds();

        data.unlockedRegionList = new List<int>(this.unlockedRegionList);

        if(GuideManager.Instance != null)
        {
            var guideData = GuideManager.Instance.GetSaveData();
            this.guideCurrentIndex = guideData.currentIndex;
            this.guideSaveData = guideData.progressList;
        }

        data.guideCurrentIndex = this.guideCurrentIndex;
        data.guideSaveData = new List<int>(this.guideSaveData);

        if (UpgradeGraphBuilder.instance != null)
        {
            unlockedUpgradeNodeIds.Clear();

            var activatedNodes = UpgradeGraphBuilder.instance.DAG.GetActivatedNodes();
            foreach (var node in activatedNodes)
            {
                var upgradeData = node.Data as UpgradeDataSO;
                if (upgradeData != null)
                {
                    unlockedUpgradeNodeIds.Add(upgradeData.upgradeID);
                }
            }
            data.unlockedUpgradeNodeIds = new List<string>(this.unlockedUpgradeNodeIds);
        }

        SaveSystem.Save(data);
        Utils.DebugLog("<color=green>1. 파일 저장 완료</color>");
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.Load();

        if (data != null)
        {
            ApplyDataVariable(data);
            isPendingLoad = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void AutoLoadOnStart()
    {
        GameData data = SaveSystem.Load();

        if (data != null)
        {
            ApplyDataVariable(data);
            StartCoroutine(SceneLoadSaveData());
        }
        else if(PlayerPrefs.HasKey("UnlockedRegions"))
        {
            string oldData = PlayerPrefs.GetString("UnlockedRegions", "1");
            string[] ids = oldData.Split(',');

            this.unlockedRegionList.Clear();
            foreach(var id in ids)
            {
                if (int.TryParse(id, out int rest))
                    this.unlockedRegionList.Add(rest);
            }

            PlayerPrefs.DeleteKey("UnlockedRegions");
            PlayerPrefs.Save();

            SaveGame();
            StartCoroutine(SceneLoadSaveData());
        }
    }

    void ApplyDataVariable(GameData data)
    {
        this.currentGold = data.currentGold;

        this.sectionMineralCount = (int[])data.sectionMineralCount.Clone();
        this.sectionProcessMineralCount = (int[])data.sectionProcessMineralCount.Clone();

        this.unlockedSections = (bool[])data.unlockedSections.Clone();
        this.unCollectedMoney = data.unCollectedMoney;
        this.sectionFillAmount = (int[])data.sectionFillAmount.Clone();

        this.savedAchievements = data.achievementProgess;

        if(data.unlockedUpgradeNodeIds != null)
        {
            this.unlockedUpgradeNodeIds = new List<string>(data.unlockedUpgradeNodeIds);
        }
        
        this.guideCurrentIndex = data.guideCurrentIndex;
        if(data.guideSaveData != null)
        {
            this.guideSaveData = new List<int>(data.guideSaveData);
        }        

        if(data.unlockedRegionList != null)
        {
            this.unlockedRegionList = new List<int>(data.unlockedRegionList);
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupButtonUI();

        if (isPendingLoad)
        {
            isPendingLoad = false;
            StartCoroutine(SceneLoadSaveData());
        }
    }


    void SetupButtonUI()
    {
        GameObject saveBtn = GameObject.Find("SaveButton");
        if (saveBtn != null)
        {
            Button saveButton = saveBtn.GetComponent<Button>();
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(() => SceneGameDataManager.instance.SaveGame());
        }

        GameObject loadBtn = GameObject.Find("LoadButton");
        if (loadBtn != null)
        {
            Button loadButton = loadBtn.GetComponent<Button>();
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => SceneGameDataManager.instance.LoadGame());
        }
    }

    IEnumerator SceneLoadSaveData()
    {
        isRefreshing = true;

        yield return new WaitForEndOfFrame();
        try
        {
            #region 섹션해금
            if (RegionUnlockService.Instance != null)
            {
                RegionUnlockService.Instance.ManagerLink(this.unlockedRegionList);                
            }

            RegionUnlockZone[] allzones = FindObjectsByType<RegionUnlockZone>(FindObjectsSortMode.None);
            foreach (var zone in allzones)
            {
                zone.UpdateGateState();
            }
            #endregion

            #region 업적
            if (AchievementsManager.instance != null && savedAchievements != null)
            {
                AchievementsManager.instance.InitializeAchievements(savedAchievements);
            }
            #endregion

            #region 판매 크래딧
            var creditService = GameManager.Instance.GetService<CreditService>();
            if (creditService != null)
            {
                creditService?.SetCredit(currentGold);
                Utils.DebugLog($"<color=gold>[Load] CreditService 데이터 복구 완료: {currentGold}</color>");
            }

            RestorePendingCredits();
            #endregion
            #region 업그래이드
            if (UpgradeGraphBuilder.instance != null && unlockedUpgradeNodeIds != null)
            {
                var dag = UpgradeGraphBuilder.instance.DAG;
                foreach (string id in unlockedUpgradeNodeIds)
                {
                    if (dag.TryGetNode(id, out var node))
                    {
                        node.Activate();
                    }
                }

                UpgradeUIManager uiMgr = FindAnyObjectByType<UpgradeUIManager>();
                if (uiMgr != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (uiMgr.IsSectionComplete(i))
                        {
                            uiMgr.OpenNextSection(false);
                        }
                    }
                }

                if (UpgradeEffectManager.Instance != null)
                {
                    UpgradeEffectManager.Instance.RecalculateAllEffects();
                }
            }

            UpgradeUILoad();
            #endregion

            if(GuideManager.Instance != null)
            {
                GuideManager.Instance.LoadGuideSaveData(this.guideCurrentIndex, this.guideSaveData);
            }
            ResourceTableLoad();
            ProcessTableLoad();
        }
        catch (System.Exception e)
        {
            Utils.DebugLogError($"[LoadError] 복구 중 에러 발생: {e.Message}");
        }
        finally
        {
            isRefreshing = false;
            Utils.DebugLog("데이터 복구 프로세스 종료");
        }

        Utils.DebugLog("씬 재시작후 불러오기 완료");
        SaveGame();
    }

    public void SaveGoldObject()
    {
        int pendingTotal = 0;
        CreditObject[] credits = FindObjectsByType<CreditObject>(FindObjectsSortMode.None);
        foreach (var c in credits) { if (!c.IsCollected) pendingTotal += c.CreditAmount; }
        this.unCollectedGold = pendingTotal;
    }

    public void SaveUnlockSection()
    {
        if (RegionUnlockService.Instance != null)
        {
            this.unlockedSections = RegionUnlockService.Instance.GetUnlockedStates(5);
             this.unlockedRegionList =RegionUnlockService.Instance.GetUnlockList();            
        }
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

    public void UpgradeUILoad()
    {
        UpgradeUIButton[] allUpgradeButton = Resources.FindObjectsOfTypeAll<UpgradeUIButton>();
        foreach (var btn in allUpgradeButton)
        {
            if (btn.gameObject.scene.name != null)
            {
                btn.UpgradeUIRenewal();
            }
        }
    }
}
