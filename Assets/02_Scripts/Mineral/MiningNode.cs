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
    [SerializeField] private MineralSO mineralData;   // 채굴 결과 SO
    [SerializeField] private float mineInterval = 1f; // 채굴 주기 (초)

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
        // 채굴 결과 생성
        GameObject item = PoolManager.instance.Get(mineralData.muneralPrefab, spawnPoint.position, Quaternion.identity);
        ResourcesManager.instance.ChangeAmount(mineralData, 1);
        item.transform.SetParent(spawnPoint);

        // 채굴 결과를 창고(ResourceTable)에 적재
        resourceTable.AddResources(item);

        // 가이드 진행도 증가 (현재 스텝일 때만)
        if (GuideManager.Instance != null &&
            mineGuideStep != null &&
            GuideManager.Instance.IsCurrentStep(mineGuideStep))
        {
            GuideManager.Instance.AddProgress(1);
        }

#if UNITY_EDITOR
        Debug.Log($"[MiningNode] {mineralData.mineralName} 채굴 → ResourceTable 적재");
#endif
    }
}
