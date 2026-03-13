using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance;

    [Header("Guide Steps (순서 중요)")]
    [SerializeField] private List<GuideStepSO> guideSteps;

    [Header("UI")]
    [SerializeField] private GuidePanel guidePanel;

    private int currentIndex;

    // 각 GuideStep의 진행도를 저장
    private Dictionary<GuideStepSO, int> progressMap = new();

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

        // 모든 가이드 진행도 초기화
        foreach (var step in guideSteps)
        {
            if (!progressMap.ContainsKey(step))
                progressMap.Add(step, 0);
        }

        guidePanel.Show(CurrentStep, progressMap[CurrentStep]);
    }

    /// <summary>
    /// 외부 시스템에서 호출
    /// 예 : 채굴 / 제작 / 판매 / 강화
    /// </summary>
    public void AddProgress(GuideActionType type, int amount = 1)
    {
        // 가이드가 이미 끝난 경우
        if (currentIndex >= guideSteps.Count)
            return;

        foreach (var step in guideSteps)
        {
            if (step.actionType != type)
                continue;

            int current = progressMap[step];
            current += amount;
            current = Mathf.Min(current, step.targetCount);

            progressMap[step] = current;
        }

        // 안전하게 CurrentStep 접근
        GuideStepSO stepSO = guideSteps[currentIndex];

        // 현재 스텝이면 UI 업데이트
        if (stepSO.actionType == type)
        {
            guidePanel.UpdateProgress(progressMap[stepSO]);
        }
    }

    /// <summary>
    /// 보상 수령 후 호출
    /// </summary>
    public void CompleteCurrentStep()
    {
        GuideStepSO step = CurrentStep;

        if (progressMap[step] < step.targetCount)
            return;

        // 보상 지급
        GiveReward(step);

        currentIndex++;

        if (currentIndex >= guideSteps.Count)
        {
            guidePanel.Hide();
            return;
        }

        GuideStepSO nextStep = CurrentStep;

        guidePanel.Show(nextStep, progressMap[nextStep]);

        if(SceneGameDataManager.instance != null)
        {
            SceneGameDataManager.instance.SaveGame();
            Utils.DebugLog($"가이드 {currentIndex}단계 진입 저장완료");
        }

        guidePanel.UpdateProgress(progressMap[nextStep]);
    }

    void GiveReward(GuideStepSO step)
    {
        if (step.rewardGold <= 0) return;

        var creditService = GameManager.Instance.GetService<CreditService>();

        if (creditService != null)
        {
            creditService.AddCredit(step.rewardGold);
        }
    }

    // 현재 스텝 체크
    public bool IsCurrentStep(GuideStepSO step)
    {
        if (currentIndex < 0 || currentIndex >= guideSteps.Count)
            return false;

        return guideSteps[currentIndex] == step;
    }

    // 현재 진행도 반환
    public int GetCurrentProgress()
    {
        return progressMap[CurrentStep];
    }

    // 현재 목표 달성 여부
    public bool IsCurrentStepComplete()
    {
        return progressMap[CurrentStep] >= CurrentStep.targetCount;
    }

    // 가이드 끝났는지 여부
    public bool IsGuideFinished()
    {
        return currentIndex >= guideSteps.Count;
    }

    //가이드 현재 상황 저장, 불러오기, 초기화
    public GuideSaveData GetSaveData()
    {
        GuideSaveData data = new GuideSaveData();
        data.currentIndex = this.currentIndex;
        data.progressList = new List<int>();

        foreach(var step in guideSteps)
        {
            int value = progressMap.ContainsKey(step) ? progressMap[step] : 0;
            data.progressList.Add(progressMap[step]);
        }    
        return data;
    }
    public void LoadGuideSaveData(int savedIndex, List<int> savedProgress)
    {
        this.currentIndex = savedIndex;     
        progressMap.Clear();

        for(int i = 0; i < guideSteps.Count; i++)
        {
            int progress = (savedProgress != null && i < savedProgress.Count) ? savedProgress[i] : 0;
            progressMap.Add(guideSteps[i], progress);
        }

        if(!IsGuideFinished())
        {
            guidePanel.Show(CurrentStep, progressMap[CurrentStep]);
        }
        else
        {
            guidePanel.Hide();
        }
    }

    public void ResetGuideData()
    {
        currentIndex = 0;
        progressMap.Clear();

        foreach(var step in guideSteps)
        {
            progressMap.Add(step, 0);
        }

        guidePanel.Show(CurrentStep, 0);
    }
}