using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMiningNode : MonoBehaviour
{
    [Header("채굴 설정")]
    [SerializeField] private ItemDataSO mineralData;  // 채굴 결과 SO
    [SerializeField] private float mineInterval = 3f; // 채굴 주기 (초)
    [SerializeField] float mineDelay = 5f;           // 채굴 쿨타임(초)
    [SerializeField] int maxMineCount = 10;           // 최대 채굴 가능 횟수
    [SerializeField] int currentMineCount = 0;
    [SerializeField] GameObject[] mineMineral;        // 채굴 광물 오브젝트
    public bool canMine => currentMineCount < maxMineCount;

    [Header("연결 대상")]
    [SerializeField] private ResourceTable resourceTable; // 채굴기 옆 창고
    [SerializeField] private Transform spawnPoint;        // 생성 위치

    [Header("Guide")]
    [SerializeField] private GuideStepSO mineGuideStep;   // 흙 채굴 가이드 스텝

    private float mineTimer;

    /// <summary>
    /// 채굴 시도
    /// - MiningTrigger / MiningState 등에서 호출
    /// </summary>
    public void TryMine()
    {
        if (!canMine) return;

        // 필수 참조 체크
        if (mineralData == null || resourceTable == null)
            return;

        mineTimer += Time.deltaTime;

        if (mineTimer < mineInterval)
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
            GameObject item = PoolManager.instance.Get(mineralData.mineralPrefab, spawnPoint.position, Quaternion.identity);
            item.transform.SetParent(spawnPoint);

            // 채굴 결과를 창고(ResourceTable)에 적재
            resourceTable.AddResources(item);
            currentMineCount++;
        }
        if (currentMineCount == maxMineCount)
        {
            foreach (GameObject mineral in mineMineral)
            {
                mineral.SetActive(false);
            }

            StartCoroutine(mineMineralSpawn());
        }

        IEnumerator mineMineralSpawn()
        {
            yield return new WaitForSeconds(mineDelay);
            currentMineCount = 0;

            foreach (GameObject mineral in mineMineral)
            {
                mineral.SetActive(true);
            }
        }

        // 가이드 진행도 증가 (현재 스텝일 때만)
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
}
