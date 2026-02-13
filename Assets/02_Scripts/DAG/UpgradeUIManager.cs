using UnityEngine;
using System.Linq;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("Graph")]
    public UpgradeGraphBuilder graphBuilder;

    [Header("UI")]
    public GameObject upgradePanel;   // 업그레이드 패널 오브젝트

    private void Start()
    {
        // 시작 시 패널 닫혀있도록
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    // ===============================
    // 패널 열기
    // ===============================
    public void OpenUpgradePanel()
    {
        if (upgradePanel == null) return;

        upgradePanel.SetActive(true);
        Debug.Log("Upgrade Panel Opened");
    }

    // ===============================
    // 패널 닫기
    // ===============================
    public void CloseUpgradePanel()
    {
        if (upgradePanel == null) return;

        upgradePanel.SetActive(false);
        Debug.Log("Upgrade Panel Closed");
    }

    // ===============================
    // 디버그 로그
    // ===============================
    public void PrintActivatedNodes()
    {
        var activated = graphBuilder.dag.GetActivatedNodes();

        Debug.Log("===== 현재 활성 노드 목록 =====");

        foreach (var node in activated)
        {
            Debug.Log(node.Data.displayName);
        }

        Debug.Log("총 활성 개수 : " + activated.Count);
    }
}
