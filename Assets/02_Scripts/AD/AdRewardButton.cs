using UnityEngine;

// 광고 버튼 (광고 요청만 담당)
public class AdRewardButton : MonoBehaviour
{
    [SerializeField] private AdRewardData rewardData;

    public void OnClickAd()
    {
        if (rewardData == null)
            return;

        AdsManager.Instance.RequestRewardAd(rewardData);

        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting", Camera.main.transform.position);
        }
    }
}
