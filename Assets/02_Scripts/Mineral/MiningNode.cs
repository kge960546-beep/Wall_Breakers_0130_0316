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
    [SerializeField] private ItemDataSO mineralData;
    [SerializeField] private float mineInterval = 1f;
    [SerializeField] private float autoMineInterval = 3f;
    [SerializeField] float mineDelay = 5f;
    [SerializeField] int maxMineCount = 10;
    [SerializeField] int currentMineCount = 0;
    [SerializeField] GameObject[] mineMineral;
    [SerializeField] new WaitForSeconds wait = new WaitForSeconds(5f);
    public bool canMine => currentMineCount < maxMineCount;

    [Header("연결 대상")]
    [SerializeField] private ResourceTable resourceTable;
    [SerializeField] private Transform spawnPoint;

    [Header("Guide")]
    [SerializeField] private GuideStepSO mineGuideStep;

    [Header("데이터 연결")]
    [SerializeField] int sectionIndex;

    private float mineTimer;

    // =========================
    // 업그레이드 관련 변수
    // =========================

    private float baseMineInterval;
    private float baseAutoMineInterval;
    private float bonusMineSpeed;

    private int baseMineAmount = 1;
    private int bonusMineAmount;

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

        mineInterval = baseMineInterval / (1f + bonusMineSpeed);
        autoMineInterval = baseAutoMineInterval / (1f + bonusMineSpeed);
    }

    private void HandleMineAmountChanged(float totalBonus)
    {
        bonusMineAmount = Mathf.FloorToInt(totalBonus);
    }

    public void TryMine()
    {
        if (!canMine) return;
        if (mineralData == null || resourceTable == null) return;

        mineTimer += Time.deltaTime;

        if (mineTimer < mineInterval)
            return;

        mineTimer = 0f;
        Mine();
    }

    public void AutoUnitTryMine()
    {
        if (!canMine) return;
        if (mineralData == null || resourceTable == null) return;

        mineTimer += Time.deltaTime;

        if (mineTimer < autoMineInterval)
            return;

        mineTimer = 0f;
        Mine();
    }

    private void Mine()
    {
        if (currentMineCount < maxMineCount)
        {
            int totalAmount = baseMineAmount + bonusMineAmount;

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

#if UNITY_EDITOR
        Debug.Log($"[MiningNode] {mineralData.itemName} 채굴 → ResourceTable 적재");
#endif
    }

    IEnumerator mineMineralSpawn()
    {
        yield return wait;
        currentMineCount = 0;

        foreach (GameObject mineral in mineMineral)
            mineral.SetActive(true);
    }
}
