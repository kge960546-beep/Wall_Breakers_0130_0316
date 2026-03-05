using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    }

    public void OnClickUpgrade()
    {
        if (node == null)
            return;

        // 부모 조건 체크
        if (!node.CanActivate()) return;

        if (creditService == null) return;

        int cost = targetSO.cost;

        if (creditService.credits < cost) return;

        bool success = node.Activate();

        if (success)
        {
            creditService.AddCredit(-cost);

            if (UpgradeEffectManager.Instance != null)
            {
                UpgradeEffectManager.Instance.RecalculateAllEffects();
            }

            GetComponent<Button>().interactable = false;

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