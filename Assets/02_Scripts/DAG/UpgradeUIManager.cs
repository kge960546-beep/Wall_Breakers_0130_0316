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

    [SerializeField] private CanvasGroup adCanvasGroup; // 광고 UI 입력 차단용

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

        GameObject panel = sectionPanels[currentSectionIndex];
        panel.SetActive(true);

        RectTransform rect = panel.GetComponent<RectTransform>();

        StopAllCoroutines();
        StartCoroutine(OpenPanelAnimation(rect));

        adCanvasGroup.blocksRaycasts = false;
    }

    // 패널 닫기
    public void CloseUpgradePanel()
    {
        if (sectionPanels.Length == 0) return;

        sectionPanels[currentSectionIndex].SetActive(false);

        adCanvasGroup.blocksRaycasts = true;
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

    IEnumerator OpenPanelAnimation(RectTransform rect)
    {
        Vector2 end = rect.anchoredPosition;

        Vector2 start;

        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0: start = new Vector2(0, 900); break;
            case 1: start = new Vector2(0, -900); break;
            case 2: start = new Vector2(-1400, 0); break;
            default: start = new Vector2(1400, 0); break;
        }

        Vector2 control = (start + end) * 0.5f + new Vector2(
            Random.Range(-400f, 400f),
            Random.Range(200f, 600f)
        );

        rect.anchoredPosition = start;
        rect.localScale = Vector3.one * 0.1f;

        yield return StartCoroutine(
            UITween.MoveBezierUI(rect, start, control, end, 0.9f)
        );

        yield return StartCoroutine(
            UITween.Scale(rect, Vector3.one * 0.1f, Vector3.one, 0.25f)
        );
    }
}