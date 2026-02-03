using UnityEngine;

[CreateAssetMenu(menuName = "Guide/Guide Step")]
public class GuideStepSO : ScriptableObject
{
    [Header("UI")]
    public Sprite icon;
    [TextArea]
    public string guideText;

    [Header("Progress")]
    public int targetCount = 1;   // 1이면 건설/완료형, n이면 누적형
}
