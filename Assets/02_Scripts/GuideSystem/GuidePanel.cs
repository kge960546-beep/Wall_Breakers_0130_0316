using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GuidePanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform root;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text guideText;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text countText;

    private GuideStepSO currentStep;

    /// <summary>
    /// 가이드 표시
    /// </summary>
    public void Show(GuideStepSO step, int currentCount)
    {
        currentStep = step;

        iconImage.sprite = step.icon;
        guideText.text = step.guideText;

        root.gameObject.SetActive(true);

        // DOTween 사용 시
        // root.localScale = Vector3.zero;
        // root.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        // DOTween 미사용 기본 처리
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
    }

    /// <summary>
    /// 가이드 완료 연출
    /// </summary>
    public void PlayCompleteAnimation(Action onComplete)
    {
        // DOTween 사용 시
        // root.DOScale(0f, 0.25f)
        //     .SetEase(Ease.InBack)
        //     .OnComplete(() => onComplete?.Invoke());

        // DOTween 미사용 기본 처리
        root.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    /// <summary>
    /// 가이드 숨김
    /// </summary>
    public void Hide()
    {
        root.gameObject.SetActive(false);
    }
}
