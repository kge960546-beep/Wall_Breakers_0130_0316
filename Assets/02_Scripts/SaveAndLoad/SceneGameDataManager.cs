using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneGameDataManager : MonoBehaviour
{
    public static SceneGameDataManager instance;
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
    public bool[] unlockedSections;
    public int[] sectionFillAmount;

    [Header("NPC업그래이드, 플레이어 업그래이드, 광산 업그래이드")]
    public float playerMoveSpeed;
    public float playerMineSpeed;
    public int playerMaxCarry;
    public float playerMineAmount;
    public float playerRawSellPrice;
    public float processedSellPrice;

    public int processingMaxCapacity;
    public float processorProcessTime;

    public float carrierMoveSpeed;
    public int carrierMaxCarry;
    public bool carrierUnlock;

    public float minerMineSpeed;
    public int minerMineAmount;
    public int minerUnlock;

    public int miningAreaMaxStorage;
    public float miningAreaRespawnTime;
    #endregion
    public List<string> savedAchievements = new List<string>();    

    private bool isPendingLoad = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if(unlockedSections == null || unlockedSections.Length == 0)
            {
                unlockedSections = new bool[5];
            }

            if(sectionFillAmount == null || sectionFillAmount.Length == 0)
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

    public void SaveGame()
    {
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

        data.playerMoveSpeed = this.playerMoveSpeed;
        data.playerMineSpeed = this.playerMineSpeed;
        data.playerMaxCarry = this.playerMaxCarry;
        data.playerMineAmount = this.playerMineAmount;
        data.playerRawSellPrice = this.playerRawSellPrice;
        data.processedSellPrice = this.processedSellPrice;

        data.processingMaxCapacity = this.processingMaxCapacity;
        data.processorProcessTime = this.processorProcessTime;

        data.carrierMoveSpeed = this.carrierMoveSpeed;
        data.carrierMaxCarry = this.carrierMaxCarry;
        data.carrierUnlock = this.carrierUnlock;

        data.minerMineSpeed = this.minerMineSpeed;
        data.minerMineAmount = this.minerMineAmount;
        data.minerUnlock = this.minerUnlock;

        data.miningAreaMaxStorage = this.miningAreaMaxStorage;
        data.miningAreaRespawnTime = this.miningAreaRespawnTime;

        SaveSystem.Save(data);       
        Debug.Log("<color=green>1. 파일 저장 완료</color>");
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

        if(data != null)
        {
            ApplyDataVariable(data);
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

        this.playerMoveSpeed = data.playerMoveSpeed;
        this.playerMineSpeed = data.playerMineSpeed;
        this.playerMaxCarry = data.playerMaxCarry;
        this.playerMineAmount = data.playerMineAmount;
        this.playerRawSellPrice = data.playerRawSellPrice;
        this.processedSellPrice = data.processedSellPrice;

        this.processingMaxCapacity = data.processingMaxCapacity;
        this.processorProcessTime = data.processorProcessTime;

        this.carrierMoveSpeed = data.carrierMoveSpeed;
        this.carrierMaxCarry = data.carrierMaxCarry;
        this.carrierUnlock = data.carrierUnlock;

        this.minerMineSpeed = data.minerMineSpeed;
        this.minerMineAmount = data.minerMineAmount;
        this.minerUnlock = data.minerUnlock;

        this.miningAreaMaxStorage = data.miningAreaMaxStorage;
        this.miningAreaRespawnTime = data.miningAreaRespawnTime;

        this.savedAchievements = data.achievementProgess;
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
        yield return new WaitForEndOfFrame();
        #region 섹션해금
        if (RegionUnlockService.Instance != null)
        {
            RegionUnlockService.Instance.LoadUnlockedRegions();
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
            Debug.Log($"<color=gold>[Load] CreditService 데이터 복구 완료: {currentGold}</color>");
        }       

        RestorePendingCredits();
        #endregion

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

    public void SaveUnlockSection()
    {
        if (RegionUnlockService.Instance != null)
        {
            this.unlockedSections = RegionUnlockService.Instance.GetUnlockedStates(5);
            RegionUnlockService.Instance.SaveUnlockedRegions();
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
}
