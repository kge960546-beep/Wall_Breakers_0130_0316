using UnityEngine;

[CreateAssetMenu(menuName = "Guide/Guide Step")]
public class GuideStepSO : ScriptableObject
{
    [Header("UI")]
    public Sprite icon;

    [Header("Guide Title")]
    public string guideTitle;   // 가이드 명

    [TextArea]
    public string guideDescription;  // 가이드 설명

    [Header("Guide Action")]
    public GuideActionType actionType;

    [Header("Progress")]
    public int targetCount = 1;

    [Header("Reward")]
    public int rewardGold = 0;
}