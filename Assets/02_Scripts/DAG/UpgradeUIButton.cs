using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Collections;

public class UpgradeUIButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public UpgradeGraphBuilder graphBuilder;
    public UpgradeUIManager uiManager;

    public UpgradeDataSO targetSO;

    private UpgradeGraphNode<UpgradeDataSO> node;
    private CreditService creditService;

    private Image sourceImage;

    private Material runtimeMat;

    private bool isPurchased = false;
    
    [SerializeField] private Image[] connectedLines;



    private void Start()
    {
        sourceImage = GetComponent<Image>();

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

        // 머티리얼 인스턴스 생성
        runtimeMat = Instantiate(sourceImage.material);
        sourceImage.material = runtimeMat;

        // 처음 상태 = 흑백
        runtimeMat.SetFloat("_GrayAmount", 1f);
    }

    public void OnClickUpgrade()
    {
        if (isPurchased) return;

        if (node == null)
            return;

        if (!node.CanActivate()) return;

        if (creditService == null) return;

        int cost = targetSO.cost;

        if (creditService.credits < cost) return;

        bool success = node.Activate();

        if (success)
        {
            isPurchased = true;

            creditService.AddCredit(-cost);

            if (UpgradeEffectManager.Instance != null)
            {
                UpgradeEffectManager.Instance.RecalculateAllEffects();
            }

            // =========================
            // 연결된 선 색 변경
            // =========================

            foreach (var line in connectedLines)
            {
                if (line == null) continue;

                line.color = Color.yellow;
            }

            // =========================
            // 노드 컬러 전환
            // =========================

            runtimeMat.SetFloat("_GrayAmount", 0f);

            // =========================
            // 섹션 완료 체크
            // =========================

            int sectionIndex = node.SectionIndex;

            if (uiManager.IsSectionComplete(sectionIndex))
            {
                uiManager.OpenNextSection();
            }
        }
    }

    // =========================
    // Hover Enter
    // =========================

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UpgradeTooltipManager.Instance == null) return;

        UpgradeTooltipManager.Instance.Show(
            sourceImage.sprite,
            targetSO.displayName,
            targetSO.description,
            targetSO.cost
        );
    }

    // =========================
    // Hover Exit
    // =========================

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UpgradeTooltipManager.Instance == null) return;

        UpgradeTooltipManager.Instance.Hide();
    }
}