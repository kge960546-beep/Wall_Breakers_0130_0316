using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        if (!GuideManager.Instance.IsCurrentStepComplete())
            return;

        GuideManager.Instance.CompleteCurrentStep();
    }

    /// <summary>
    /// 가이드 숨김
    /// </summary>
    public void Hide()
    {
        root.gameObject.SetActive(false);
    }
}