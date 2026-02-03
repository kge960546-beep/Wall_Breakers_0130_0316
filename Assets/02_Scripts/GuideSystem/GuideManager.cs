using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance;

    [Header("Guide Steps (순서 중요)")]
    [SerializeField] private List<GuideStepSO> guideSteps;

    [Header("UI")]
    [SerializeField] private GuidePanel guidePanel;

    private int currentIndex;
    private int currentCount;

    private GuideStepSO CurrentStep => guideSteps[currentIndex];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartGuide();
    }

    void StartGuide()
    {
        currentIndex = 0;
        currentCount = 0;
        guidePanel.Show(CurrentStep, currentCount);
    }

    /// <summary>
    /// 외부 시스템에서 호출 (채굴, 수확, 건설 등)
    /// </summary>
    public void AddProgress(int amount = 1)
    {
        currentCount += amount;
        currentCount = Mathf.Min(currentCount, CurrentStep.targetCount);

        guidePanel.UpdateProgress(currentCount);

        if (currentCount >= CurrentStep.targetCount)
        {
            CompleteGuide();
        }
    }

    void CompleteGuide()
    {
        guidePanel.PlayCompleteAnimation(() =>
        {
            currentIndex++;

            if (currentIndex >= guideSteps.Count)
            {
                guidePanel.Hide();
                return;
            }

            currentCount = 0;
            guidePanel.Show(CurrentStep, currentCount);
        });
    }

    // 현재 스텝 몇단계인지 파악하는 메서드
    public bool IsCurrentStep(GuideStepSO step)
    {
        if (currentIndex < 0 || currentIndex >= guideSteps.Count)
            return false;

        return guideSteps[currentIndex] == step;
    }

}
