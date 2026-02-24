using UnityEngine;
using System.Linq;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("Graph")]
    public UpgradeGraphBuilder graphBuilder;

    [Header("Section Panels (순서대로 Section0~Section4)")]
    [SerializeField] private GameObject[] sectionPanels;

    private int currentSectionIndex = 0;

    private void Start()
    {
        for (int i = 0; i < sectionPanels.Length; i++)
        {
            sectionPanels[i].SetActive(false);
        }

        currentSectionIndex = 0;
    }

    // 패널 열기
    public void OpenUpgradePanel()
    {
        if (sectionPanels.Length == 0) return;

        sectionPanels[currentSectionIndex].SetActive(true);
    }

    // 패널 닫기
    public void CloseUpgradePanel()
    {
        if (sectionPanels.Length == 0) return;

        sectionPanels[currentSectionIndex].SetActive(false);
    }

    // ===============================
    // 디버그 로그
    // ===============================
    public void PrintActivatedNodes()
    {
        var activated = graphBuilder.DAG.GetActivatedNodes();

        Debug.Log("===== 현재 활성 노드 목록 =====");

        foreach (var node in activated)
        {
            Debug.Log(node.Data.displayName);
        }

        Debug.Log("총 활성 개수 : " + activated.Count);
    }

    // ===============================
    // 섹션 완료 체크
    // ===============================
    public bool IsSectionComplete(int sectionIndex)
    {
        var nodes = graphBuilder.DAG.Nodes;

        foreach (var node in nodes)
        {
            if (node.SectionIndex == sectionIndex)
            {
                if (!node.IsActivated)
                    return false;
            }
        }

        return true;
    }

    // ===============================
    // 다음 섹션 열기
    // ===============================
    public void OpenNextSection()
    {
        if (!IsSectionComplete(currentSectionIndex))
            return;

        int nextIndex = currentSectionIndex + 1;

        if (nextIndex >= sectionPanels.Length)
        {
            Debug.Log("모든 섹션 완료");
            return;
        }

        // 현재 섹션 비활성화
        sectionPanels[currentSectionIndex].SetActive(false);

        // 다음 섹션 활성화
        sectionPanels[nextIndex].SetActive(true);

        currentSectionIndex = nextIndex;

        Debug.Log("다음 섹션 오픈: " + currentSectionIndex);
    }
}