using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 채굴 대상(광산, 채굴기)에 붙는 컴포넌트
/// 플레이어 채굴과 광부 채굴을 명확히 분리
/// </summary>
public class MiningNode : MonoBehaviour
{
    [Header("채굴 설정")]
    [SerializeField] private ItemDataSO mineralData;
    [SerializeField] private float mineInterval = 1f;        // 플레이어
    [SerializeField] private float autoMineInterval = 3f;    // 광부
    [SerializeField] private int maxMineCount = 10;
    [SerializeField] private int currentMineCount = 0;
    [SerializeField] private GameObject[] mineMineral;
    [SerializeField] private float respawnTime = 5f;

    private float baseRespawnTime;
    private float bonusRespawnReduction;

    [SerializeField] private string miningAreaID;

    public bool canMine => currentMineCount < maxMineCount;

    [Header("광부 식별자")]
    [SerializeField] private string minerID;

    [Header("연결 대상")]
    [SerializeField] private ResourceTable resourceTable;
    [SerializeField] private Transform spawnPoint;

    [Header("Guide")]
    [SerializeField] private GuideStepSO mineGuideStep;

    [Header("데이터 연결")]
    [SerializeField] private int sectionIndex;

    private float mineTimer;

    // =========================
    // 기본값 저장
    // =========================

    private float baseMineInterval;
    private float baseAutoMineInterval;

    // =========================
    // 업그레이드 누적값 (분리!)
    // =========================

    private float bonusPlayerMineSpeed;
    private float bonusMinerMineSpeed;

    private int baseMineAmount = 1;
    private int bonusPlayerMineAmount;   // 플레이어 전용
                                         
    private int bonusMinerMineAmount;    // 광부 전용

    private void Awake()
    {
        baseMineInterval = mineInterval;
        baseAutoMineInterval = autoMineInterval;
        baseRespawnTime = respawnTime;
    }

    private void Start()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnPlayerMineSpeedChanged += HandlePlayerMineSpeedChanged;
            UpgradeEffectManager.Instance.OnPlayerMineAmountChanged += HandlePlayerMineAmountChanged;
            UpgradeEffectManager.Instance.OnMinerMineSpeedChanged += HandleMinerMineSpeedChanged;

            UpgradeEffectManager.Instance.OnMiningAreaRespawnTimeChanged += HandleRespawnTimeChanged;
            UpgradeEffectManager.Instance.OnMinerMineAmountChanged += HandleMinerMineAmountChanged;
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnPlayerMineSpeedChanged -= HandlePlayerMineSpeedChanged;
            UpgradeEffectManager.Instance.OnPlayerMineAmountChanged -= HandlePlayerMineAmountChanged;
            UpgradeEffectManager.Instance.OnMinerMineSpeedChanged -= HandleMinerMineSpeedChanged;

            UpgradeEffectManager.Instance.OnMiningAreaRespawnTimeChanged -= HandleRespawnTimeChanged;
            UpgradeEffectManager.Instance.OnMinerMineAmountChanged -= HandleMinerMineAmountChanged;
        }
    }

    // =========================
    // 이벤트 수신
    // =========================

    private void HandlePlayerMineSpeedChanged(float totalBonus)
    {
        bonusPlayerMineSpeed = totalBonus;
    }

    private void HandlePlayerMineAmountChanged(float totalBonus)
    {
        bonusPlayerMineAmount = Mathf.FloorToInt(totalBonus);
    }

    private void HandleMinerMineSpeedChanged(string id, float totalBonus)
    {
        if (id == minerID)
        {
            bonusMinerMineSpeed = totalBonus;
        }
    }

    private void HandleRespawnTimeChanged(string id, float reduction)
    {
        if (id != miningAreaID) return;

        bonusRespawnReduction = reduction;

        respawnTime = Mathf.Max(0.5f, baseRespawnTime - bonusRespawnReduction);

        Debug.Log($"[MiningArea:{id}] 리스폰 시간 → {respawnTime}");
    }

    private void HandleMinerMineAmountChanged(string id, int totalBonus)
    {
        if (id == minerID)
        {
            bonusMinerMineAmount = totalBonus;
        }
    }

    // =========================
    // 플레이어 채굴
    // =========================

    public void TryMine()
    {
        if (!canMine) return;
        if (mineralData == null || resourceTable == null) return;

        float adjustedInterval = baseMineInterval / (1f + bonusPlayerMineSpeed);

        mineTimer += Time.deltaTime;

        if (mineTimer < adjustedInterval)
            return;

        mineTimer = 0f;

        Mine(baseMineAmount + bonusPlayerMineAmount);
    }

    // =========================
    // 광부 채굴
    // =========================

    public void AutoUnitTryMine()
    {
        if (!canMine) return;
        if (mineralData == null || resourceTable == null) return;

        float adjustedInterval = baseAutoMineInterval / (1f + bonusMinerMineSpeed);

        mineTimer += Time.deltaTime;

        if (mineTimer < adjustedInterval)
            return;

        mineTimer = 0f;

        Mine(baseMineAmount + bonusMinerMineAmount);
    }

    // =========================
    // 실제 채굴 처리
    // =========================

    private void Mine(int totalAmount)
    {
        for (int i = 0; i < totalAmount; i++)
        {
            if (currentMineCount >= maxMineCount)
                break;

            GameObject item = PoolManager.instance.Get(
                mineralData.mineralPrefab,
                spawnPoint.position,
                Quaternion.identity);

            item.transform.SetParent(spawnPoint);

            resourceTable.AddResources(item);
            currentMineCount++;

            if (SceneGameDataManager.instance != null)
            {
                SceneGameDataManager.instance.sectionMineralCount[sectionIndex]++;
            }
        }

        if (currentMineCount == maxMineCount)
        {
            foreach (GameObject mineral in mineMineral)
                mineral.SetActive(false);

            StartCoroutine(mineMineralSpawn());
        }

        if (GuideManager.Instance != null &&
            mineGuideStep != null &&
            GuideManager.Instance.IsCurrentStep(mineGuideStep))
        {
            GuideManager.Instance.AddProgress(1);
        }
    }

    private IEnumerator mineMineralSpawn()
    {
        yield return new WaitForSeconds(respawnTime);

        currentMineCount = 0;

        foreach (GameObject mineral in mineMineral)
            mineral.SetActive(true);
    }
}