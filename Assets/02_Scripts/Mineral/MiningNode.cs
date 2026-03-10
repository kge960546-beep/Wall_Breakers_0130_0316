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

    [Header("UI Reference")]
    [SerializeField] private MiningUI miningUI;

    [Header("Occupancy")]
    private GameObject currentMiner; // 현재 채굴 중인 대상
    [Header("VFX")]
    [SerializeField] private MiningEffectController effectController;
    [SerializeField] private Transform mineralPoint;

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

    private void HandlePlayerMineSpeedChanged(int section, float totalBonus)
    {
        if (section != sectionIndex) return;

        bonusPlayerMineSpeed = totalBonus;
    }

    private void HandlePlayerMineAmountChanged(int section, float totalBonus)
    {
        if (section != sectionIndex) return;

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

    public bool TryMine(GameObject miner)
    {
        // [1] 점유권 체크: 주인이 없으면 등록, 주인이 내가 아니면 즉시 차단
        if (currentMiner == null)
        {
            currentMiner = miner;
            mineTimer = 0f; // 주인이 새로 바뀌었으므로 타이머 리셋
        }

        if (currentMiner != miner) return false;

        // [2] 채굴 가능 상태 체크
        if (!canMine || mineralData == null || resourceTable == null)
        {
            if (miningUI != null) miningUI.CloseUI();
            // 자원 고갈 상태라면 점유 해제 (Mine 함수에서도 처리하지만 이중 방어)
            if (currentMiner == miner) currentMiner = null;
            return false;
        }

        // [3] 속도 보정 및 타이머 진행
        float adjustedInterval = baseMineInterval / (1f + bonusPlayerMineSpeed);
        mineTimer += Time.deltaTime;

        // [4] UI 업데이트
        if (miningUI != null)
        {
            miningUI.OpenUI(mineralData.icon, baseMineAmount + bonusPlayerMineAmount);
            miningUI.UpdateAmount(baseMineAmount + bonusPlayerMineAmount);
            miningUI.UpdateProgress(Mathf.Clamp01(mineTimer / adjustedInterval));
        }

        // [5] 타이머 체크 및 실제 채굴 실행
        if (mineTimer >= adjustedInterval)
        {
            mineTimer = 0f;
            Mine(baseMineAmount + bonusPlayerMineAmount);
        }

        return true;
    }

    // =========================
    // 광부 채굴
    // =========================

    public bool AutoUnitTryMine(GameObject miner)
    {
        // [1] 점유권 체크: 주인이 없으면 등록, 주인이 내가 아니면 즉시 차단
        if (currentMiner == null)
        {
            currentMiner = miner;
            mineTimer = 0f; // 주인이 새로 바뀌었으므로 타이머 리셋
        }

        if (currentMiner != miner) return false;

        // [2] 채굴 가능 상태 체크
        if (!canMine || mineralData == null || resourceTable == null)
        {
            if (miningUI != null) miningUI.CloseUI();
            if (currentMiner == miner) currentMiner = null;
            return false;
        }

        // [3] 광부 전용 속도 보정 및 타이머 진행
        float adjustedInterval = baseAutoMineInterval / (1f + bonusMinerMineSpeed);
        mineTimer += Time.deltaTime;

        // [4] UI 업데이트
        if (miningUI != null)
        {
            miningUI.OpenUI(mineralData.icon, baseMineAmount + bonusMinerMineAmount);
            miningUI.UpdateAmount(baseMineAmount + bonusMinerMineAmount);
            miningUI.UpdateProgress(Mathf.Clamp01(mineTimer / adjustedInterval));
        }

        // [5] 타이머 체크 및 실제 채굴 실행
        if (mineTimer >= adjustedInterval)
        {
            mineTimer = 0f;
            Mine(baseMineAmount + bonusMinerMineAmount);
        }

        return true;
    }

    // =========================
    // 실제 채굴 처리
    // =========================

    private void Mine(int totalAmount)
    {
        Debug.Log("Mine called at time: " + Time.time);

        // 채굴 타격 이펙트 추가
        if (effectController != null)
        {
            effectController.PlayHit(mineralPoint.position);
        }
        SFXManager.instance.PlayOnSFX("mineralMiner", transform.position);

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

                string sectionAchievementID = $"Section_{sectionIndex}_Mine";
                int currentSectionTotal = SceneGameDataManager.instance.sectionMineralCount[sectionIndex];
                AchievementsManager.instance.ProgressAchievement(sectionAchievementID, currentSectionTotal);
            }
        }

        if (currentMineCount == maxMineCount)
        {
            // 1. 점유 상태 해제
            currentMiner = null;

            // 2. 채굴 타이머 초기화
            mineTimer = 0f;

            // 자원이 다 소모되었으므로 채굴 UI를 즉시 닫기
            if (miningUI != null)
            {
                miningUI.CloseUI();
            }
            // 광물 파괴 이펙트 추가
            if(effectController != null)
            {
                effectController.PlayDestroy(mineralPoint.position);
            }
            SFXManager.instance.PlayOnSFX("Break2", transform.position);

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

    private void OnTriggerExit(Collider other)
    {
        // 나가는 객체가 '현재 점유자'인 경우에만 점유권을 해제
        if (other.gameObject == currentMiner)
        {
            currentMiner = null;
            mineTimer = 0f;

            if (miningUI != null)
            {
                miningUI.CloseUI();
            }
        }
    }
}