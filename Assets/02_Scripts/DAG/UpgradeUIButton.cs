using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIButton : MonoBehaviour
{
    public UpgradeGraphBuilder graphBuilder;
    public UpgradeUIManager uiManager;

    public UpgradeDataSO targetSO;

    private UpgradeGraphNode<UpgradeDataSO> node;
    private CreditService creditService;

    private void Start()
    {
        // 노드 찾기
        if (graphBuilder.dag.TryGetNode(targetSO.upgradeID, out node) == false)
        {
            Debug.LogError("노드 찾기 실패: " + targetSO.upgradeID);
        }

        // CreditService 연결
        if (GameManager.Instance != null)
        {
            creditService = GameManager.Instance.GetService<CreditService>();

            if (creditService == null)
            {
                Debug.LogError("[UpgradeUIButton] CreditService 연결 실패");
            }
        }
    }

    public void OnClickUpgrade()
    {
        if (node == null)
            return;

        // 1. 부모 조건 먼저 체크
        if (!node.CanActivate())
        {
            Debug.Log("활성화 실패 (부모 조건 미충족) : " + node.Data.displayName);
            return;
        }

        // 2. 골드 체크
        if (creditService == null)
        {
            Debug.LogError("[UpgradeUIButton] CreditService 없음");
            return;
        }

        int cost = targetSO.cost;

        if (creditService.credits < cost)
        {
            Debug.Log("활성화 실패 (골드 부족) : " + node.Data.displayName);
            return;
        }

        // 3. 노드 활성화 시도
        bool success = node.Activate();

        if (success)
        {
            Debug.Log("활성화 성공 : " + node.Data.displayName);

            // 골드 차감
            creditService.AddCredit(-cost);
            Debug.Log($"골드 차감 : -{cost}");

            // 재집계 구조 적용
            if (UpgradeEffectManager.Instance != null)
            {
                Debug.Log("RecalculateAllEffects 호출 직전");
                UpgradeEffectManager.Instance.RecalculateAllEffects();
                Debug.Log("RecalculateAllEffects 호출 완료");
            }
            else
            {
                Debug.LogError("[UpgradeUIButton] UpgradeEffectManager 없음");
            }

            uiManager.PrintActivatedNodes();

            // 버튼 비활성화
            GetComponent<Button>().interactable = false;
        }
        else
        {
            Debug.Log("활성화 실패 (내부 조건 오류) : " + node.Data.displayName);
        }
    }
}
