using UnityEngine;
using UnityEngine.Video;

// 광고 영상 정보
[CreateAssetMenu(menuName = "Ads/AdVideoData")]
public class AdVideoData : ScriptableObject
{
    public string videoId;        // 식별용 ID
    public VideoClip videoClip;   // 실제 광고 영상
}
