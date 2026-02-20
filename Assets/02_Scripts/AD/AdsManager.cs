using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// 광고 재생 흐름 담당 매니저
public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Header("광고 영상 목록 (랜덤 선택용)")]
    [SerializeField] private List<AdVideoData> adVideos = new();

    [Header("광고 UI")]
    [SerializeField] private GameObject adPanel;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject closeButton;

    [Header("닫기 버튼 활성화 시간")]
    [SerializeField] private float closeButtonDelay = 10f;

    private AdRewardData pendingReward;
    private Coroutine closeButtonRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 광고 버튼에서 호출
    /// </summary>
    public void RequestRewardAd(AdRewardData rewardData)
    {
        if (rewardData == null)
            return;

        AdVideoData selected = GetRandomAdVideo();
        if (selected == null || selected.videoClip == null)
            return;

        pendingReward = rewardData;

        // UI 초기화
        adPanel.SetActive(true);
        closeButton.SetActive(false);

        // 브금 재생 일시정지
        if (SFXManager.instance != null)
            SFXManager.instance.PauseBGM();

        // 영상 재생
        videoPlayer.Stop();
        videoPlayer.clip = selected.videoClip;
        videoPlayer.Play();

        // 닫기 버튼 타이머 시작
        if (closeButtonRoutine != null)
            StopCoroutine(closeButtonRoutine);

        closeButtonRoutine = StartCoroutine(EnableCloseButtonAfterDelay());
    }

    private IEnumerator EnableCloseButtonAfterDelay()
    {
        yield return new WaitForSeconds(closeButtonDelay);
        closeButton.SetActive(true);
    }

    /// <summary>
    /// 닫기 버튼에서 호출
    /// </summary>
    public void OnClickCloseAd()
    {
        // 영상 종료
        videoPlayer.Stop();

        // UI 닫기
        adPanel.SetActive(false);
        closeButton.SetActive(false);

        // BGM 재개
        if (SFXManager.instance != null)
            SFXManager.instance.ResumeBGM();

        // 보상 지급
        if (pendingReward != null)
        {
            AdRewardManager.Instance.ApplyReward(pendingReward);
            pendingReward = null;
        }
    }

    private AdVideoData GetRandomAdVideo()
    {
        if (adVideos == null || adVideos.Count == 0)
            return null;

        int index = Random.Range(0, adVideos.Count);
        return adVideos[index];
    }
}
