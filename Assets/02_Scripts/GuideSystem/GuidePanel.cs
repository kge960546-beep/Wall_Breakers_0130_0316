using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GuidePanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform root;
    [SerializeField] private Image iconImage;

    [SerializeField] private TMP_Text guideTitleText;        // 가이드 명
    [SerializeField] private TMP_Text guideDescriptionText;  // 가이드 설명

    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text countText;

    [Header("Reward UI")]
    [SerializeField] private TMP_Text rewardText;

    [Header("Button")]
    [SerializeField] private Button panelButton;

    [SerializeField] GameObject goldIconPrefab;
    [SerializeField] RectTransform goldTarget;
    [SerializeField] RectTransform rewardGoldIcon;

    [SerializeField] int spawnCount = 8;

    [SerializeField] float scatterRadius = 120f;
    [SerializeField] float scatterDuration = 0.25f;

    [SerializeField] float moveDuration = 0.6f;

    private GuideStepSO currentStep;

    /// <summary>
    /// 가이드 표시
    /// </summary>
    public void Show(GuideStepSO step, int currentCount)
    {
        currentStep = step;

        iconImage.sprite = step.icon;

        guideTitleText.text = step.guideTitle;
        guideDescriptionText.text = step.guideDescription;

        rewardText.text = $"{step.rewardGold} G";

        root.gameObject.SetActive(true);
        root.localScale = Vector3.one;

        UpdateProgress(currentCount);
    }

    /// <summary>
    /// 진행도 갱신
    /// </summary>
    public void UpdateProgress(int currentCount)
    {
        if (currentStep == null) return;

        float ratio = (float)currentCount / currentStep.targetCount;

        fillImage.fillAmount = ratio;
        countText.text = $"{currentCount}/{currentStep.targetCount}";

        UpdateButtonState(currentCount);
    }

    /// <summary>
    /// 버튼 상태 갱신
    /// </summary>
    void UpdateButtonState(int currentCount)
    {
        bool ready = currentCount >= currentStep.targetCount;

        panelButton.interactable = ready;
    }

    /// <summary>
    /// 패널 버튼 클릭 (보상 수령)
    /// </summary>
    public void OnClickPanel()
    {
        Debug.Log("GuidePanel Button Clicked");

        if (!GuideManager.Instance.IsCurrentStepComplete())
        {
            Debug.Log("Guide step not complete");
            return;
        }

        StartCoroutine(RewardSequence());
    }

    /// <summary>
    /// 가이드 숨김
    /// </summary>
    public void Hide()
    {
        root.gameObject.SetActive(false);
    }

    IEnumerator RewardSequence()
    {
        panelButton.interactable = false;

        List<RectTransform> coins = new List<RectTransform>();

        Vector2 start;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root,
            RectTransformUtility.WorldToScreenPoint(null, rewardGoldIcon.position),
            null,
            out start
        );

        Vector2 end;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root,
            RectTransformUtility.WorldToScreenPoint(null, goldTarget.position),
            null,
            out end
        );

        // 코인 생성
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject icon = Instantiate(goldIconPrefab, root);

            RectTransform rect = icon.GetComponent<RectTransform>();
            rect.anchoredPosition = start;

            coins.Add(rect);
        }

        // Scatter 동시에
        List<Vector2> scatterTargets = new List<Vector2>();

        foreach (var coin in coins)
        {
            Vector2 scatter = start + new Vector2(
                Random.Range(-scatterRadius, scatterRadius),
                Random.Range(-scatterRadius, -scatterRadius * 0.3f)
            );

            scatterTargets.Add(scatter);

            StartCoroutine(
                UITween.MoveUI(coin, start, scatter, scatterDuration)
            );
        }

        yield return new WaitForSeconds(scatterDuration);

        // Bezier
        for (int i = 0; i < coins.Count; i++)
        {
            Vector2 scatter = scatterTargets[i];

            Vector2 control = (scatter + end) * 0.5f + Vector2.up * 200f;

            StartCoroutine(
                MoveCoinSequence(coins[i], scatter, control, end)
            );

            yield return new WaitForSeconds(0.08f); // 핵심 (코인 출발 간격)
        }

        yield return new WaitForSeconds(moveDuration);

        foreach (var coin in coins)
            Destroy(coin.gameObject);

        GuideManager.Instance.CompleteCurrentStep();
    }

    IEnumerator MoveCoinSequence(RectTransform coin, Vector2 start, Vector2 control, Vector2 end)
    {
        yield return StartCoroutine(
            UITween.MoveBezierUI(coin, start, control, end, moveDuration)
        );
    }
}