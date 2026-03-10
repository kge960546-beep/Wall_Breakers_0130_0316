using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        UpgradeUIRenewal();
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

            if (SceneGameDataManager.instance != null)
            {
                SceneGameDataManager.instance.currentGold = (int)creditService.credits;
            }

            if (UpgradeEffectManager.Instance != null)
            {
                UpgradeEffectManager.Instance.RecalculateAllEffects();
            }

            if(SFXManager.instance != null)
            {
                SFXManager.instance.PlayOnSFX("Upgrade1", Camera.main.transform.position);
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

        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting", Camera.main.transform.position);
        }
    }

    // =========================
    // Hover Exit
    // =========================

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UpgradeTooltipManager.Instance == null) return;

        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting", Camera.main.transform.position);
        }

        UpgradeTooltipManager.Instance.Hide();


    }

    public void UpgradeUIRenewal()
    {
        if (sourceImage == null) sourceImage = GetComponent<Image>();
        if (runtimeMat == null && sourceImage != null && sourceImage.material != null)
        {
            runtimeMat = Instantiate(sourceImage.material);
            sourceImage.material = runtimeMat;
        }
        if (node == null && graphBuilder != null)
        {
            graphBuilder.DAG.TryGetNode(targetSO.upgradeID, out node);
        }

        if (node != null && node.IsActivated)
        {
            isPurchased = true;

            if (runtimeMat != null)
            {
                runtimeMat.SetFloat("_GrayAmount", 0f);
            }

            foreach (var line in connectedLines)
            {
                if (line != null)
                {
                    line.color = Color.yellow;
                }
            }
        }
        else
        {
            isPurchased = false;
            if (runtimeMat != null)
            {
                runtimeMat.SetFloat("_GrayAmount", 1f);
            }
        }

        if (graphBuilder != null && graphBuilder.DAG.TryGetNode(targetSO.upgradeID, out var currentNode))
        {
            if (currentNode.IsActivated)
            {
                isPurchased = true;

                if (runtimeMat != null)
                    runtimeMat.SetFloat("_GrayAmount", 0f);

                foreach (var line in connectedLines)
                {
                    if (line != null) line.color = Color.yellow;
                }
            }
            else
            {
                isPurchased = false;
                if (runtimeMat != null)
                    runtimeMat.SetFloat("_GrayAmount", 1f);
            }
        }
    }
}