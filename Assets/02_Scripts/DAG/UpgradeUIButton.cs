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
        if (graphBuilder.DAG.TryGetNode(targetSO.upgradeID, out node) == false)
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
        if (!node.CanActivate()) return;

        // 2. 골드 체크
        if (creditService == null) return;
       
        int cost = targetSO.cost;

        if (creditService.credits < cost) return;

        // 3. 노드 활성화 시도
        bool success = node.Activate();

        if (success)
        {
            // 골드 차감
            creditService.AddCredit(-cost);

            // 재집계 구조 적용
            if (UpgradeEffectManager.Instance != null)
            {
                UpgradeEffectManager.Instance.RecalculateAllEffects();
            }
            else
            {
            }

            //uiManager.PrintActivatedNodes();

            // 버튼 비활성화
            GetComponent<Button>().interactable = false;

            int sectionIndex = node.SectionIndex;

            if (uiManager.IsSectionComplete(sectionIndex))
            {
                uiManager.OpenNextSection();
            }
        }
        else
        {
        }
    }
}
