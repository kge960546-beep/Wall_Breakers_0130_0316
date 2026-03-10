using UnityEngine;
using System.Collections;
using System.Linq;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("Graph")]
    public UpgradeGraphBuilder graphBuilder;

    [Header("Section Panels (순서대로 Section0~Section4)")]
    [SerializeField] private GameObject[] sectionPanels;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.35f;
    [SerializeField] private float delayBeforeTransition = 0.35f;
    [SerializeField] private float slideDistance = 1200f;

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

        if(SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting 2", Camera.main.transform.position);
        }
    }

    // 패널 닫기
    public void CloseUpgradePanel()
    {
        if (sectionPanels.Length == 0) return;

        sectionPanels[currentSectionIndex].SetActive(false);

        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting 2", Camera.main.transform.position);
        }
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
    public void OpenNextSection(bool showPanel = true)
    {
        if (!IsSectionComplete(currentSectionIndex))
            return;

        int nextIndex = currentSectionIndex + 1;

        if (nextIndex >= sectionPanels.Length)
            return;

        if(showPanel)
        {
            StartCoroutine(TransitionSection(nextIndex));
            SceneGameDataManager.instance.SaveGame();
        }
        else
        {
            sectionPanels[currentSectionIndex].SetActive(false);
            currentSectionIndex = nextIndex;
            sectionPanels[currentSectionIndex].SetActive(false);
            SceneGameDataManager.instance.SaveGame();
        }
    }

    // ===============================
    // 패널 슬라이드 전환
    // ===============================
    private IEnumerator TransitionSection(int nextIndex)
    {
        yield return new WaitForSeconds(delayBeforeTransition);

        GameObject currentPanel = sectionPanels[currentSectionIndex];
        GameObject nextPanel = sectionPanels[nextIndex];

        RectTransform currentRect = currentPanel.GetComponent<RectTransform>();
        RectTransform nextRect = nextPanel.GetComponent<RectTransform>();

        Vector2 originalPos = currentRect.anchoredPosition;

        // =========================
        // 1단계 : 현재 패널 왼쪽으로 이동
        // =========================

        Vector2 exitPos = originalPos + Vector2.left * slideDistance;

        yield return StartCoroutine(
            UITween.MoveUI(currentRect, originalPos, exitPos, transitionDuration)
        );

        currentPanel.SetActive(false);

        // =========================
        // 2단계 : 다음 패널 오른쪽에서 등장
        // =========================

        Vector2 startPos = originalPos + Vector2.right * slideDistance;

        nextRect.anchoredPosition = startPos;
        nextPanel.SetActive(true);

        yield return StartCoroutine(
            UITween.MoveUI(nextRect, startPos, originalPos, transitionDuration)
        );

        currentSectionIndex = nextIndex;
    }
}