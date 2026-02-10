using UnityEngine;

// 광고보고난뒤 보상 SO
[CreateAssetMenu(menuName = "Ads/AdRewardData")]
public class AdRewardData : ScriptableObject
{
    public AdRewardType rewardType; // 보상 종류
    public float value;             // 증가량
    public float duration;          // 지속 시간 (0이면 즉시 보상, 지속시간 따로 존재하지않음)
}
