using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 채굴 대상(광산, 채굴기)에 붙는 컴포넌트
/// - 채굴 타이머 관리
/// - 채굴 결과 생성
/// - 채굴 결과를 ResourceTable(창고)에 적재
/// 
/// ※ 주의
/// - 백팩(StackBackPack)과 직접 연동하지 않음
/// - 용량 판단, 상태 전환은 여기서 하지 않음
/// - 취합 테스트용 최소 책임만 가짐
/// </summary>
public class MiningNode : MonoBehaviour
{
    [Header("채굴 설정")]
    [SerializeField] private ItemDataSO mineralData;  // 채굴 결과 SO
    [SerializeField] private float mineInterval = 1f; // 채굴 주기 (초)
    [SerializeField] private float autoMineInterval = 3f; // 자동 채굴 주기 (초)
    [SerializeField] float mineDelay = 5f;           // 채굴 쿨타임(초)
    [SerializeField] int maxMineCount = 10;           // 최대 채굴 가능 횟수
    [SerializeField] int currentMineCount = 0;        // 현재 채굴한 횟수
    [SerializeField] GameObject[] mineMineral;        // 채굴 광물 오브젝트
    [SerializeField] WaitForSeconds wait = new WaitForSeconds(5f);
    public bool canMine => currentMineCount < maxMineCount;

    [Header("연결 대상")]
    [SerializeField] private ResourceTable resourceTable; // 채굴기 옆 창고
    [SerializeField] private Transform spawnPoint;        // 생성 위치

    [Header("Guide")]
    [SerializeField] private GuideStepSO mineGuideStep;   // 흙 채굴 가이드 스텝

    private float mineTimer;

    // =========================
    // 업그레이드 관련 변수
    // =========================

    private float baseMineInterval;
    private float baseAutoMineInterval;
    private float bonusMineSpeed;     // 누적 채굴속도 강화

    private int baseMineAmount = 1;   // 기본 채굴 수량
    private int bonusMineAmount;      // 누적 채굴 수량 강화

    private void Awake()
    {
        baseMineInterval = mineInterval;
        baseAutoMineInterval = autoMineInterval;
    }

    private void OnEnable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnPlayerMineSpeedChanged += HandleMineSpeedChanged;
            UpgradeEffectManager.Instance.OnPlayerMineAmountChanged += HandleMineAmountChanged;
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnPlayerMineSpeedChanged -= HandleMineSpeedChanged;
            UpgradeEffectManager.Instance.OnPlayerMineAmountChanged -= HandleMineAmountChanged;
        }
    }

    private void HandleMineSpeedChanged(float totalBonus)
    {
        bonusMineSpeed = totalBonus;

        // 비율 방식 적용 (속도 증가 = 간격 감소)
        mineInterval = baseMineInterval / (1f + bonusMineSpeed);
        autoMineInterval = baseAutoMineInterval / (1f + bonusMineSpeed);
    }

    private void HandleMineAmountChanged(float totalBonus)
    {
        bonusMineAmount = Mathf.FloorToInt(totalBonus);
    }

    /// <summary>
    /// 채굴 시도
    /// - MiningTrigger / MiningState 등에서 호출
    /// </summary>
    public void TryMine()
    {
        if (!canMine) return;

        if (mineralData == null || resourceTable == null)
            return;

        mineTimer += Time.deltaTime;

        if (mineTimer < mineInterval)
            return;

        mineTimer = 0f;

        Mine();
    }

    public void AutoUnitTryMine()
    {
        if (!canMine) return;

        if (mineralData == null || resourceTable == null)
            return;

        mineTimer += Time.deltaTime;

        if (mineTimer < autoMineInterval)
            return;

        mineTimer = 0f;

        Mine();
    }

    /// <summary>
    /// 실제 채굴 처리
    /// </summary>
    private void Mine()
    {
        if (currentMineCount < maxMineCount)
        {
            int totalAmount = baseMineAmount + bonusMineAmount;

            for (int i = 0; i < totalAmount; i++)
            {
                if (currentMineCount >= maxMineCount)
                    break;

                GameObject item = PoolManager.instance.Get(mineralData.mineralPrefab, spawnPoint.position, Quaternion.identity);
                item.transform.SetParent(spawnPoint);

                resourceTable.AddResources(item);
                currentMineCount++;
            }
        }

        if (currentMineCount == maxMineCount)
        {
            foreach (GameObject mineral in mineMineral)
            {
                mineral.SetActive(false);
            }

            StartCoroutine(mineMineralSpawn());
        }

        if (GuideManager.Instance != null &&
            mineGuideStep != null &&
            GuideManager.Instance.IsCurrentStep(mineGuideStep))
        {
            GuideManager.Instance.AddProgress(1);
        }

#if UNITY_EDITOR
        Debug.Log($"[MiningNode] {mineralData.itemName} 채굴 → ResourceTable 적재");
#endif
    }

    IEnumerator mineMineralSpawn()
    {
        yield return wait;
        currentMineCount = 0;

        foreach (GameObject mineral in mineMineral)
        {
            mineral.SetActive(true);
        }
    }
}
