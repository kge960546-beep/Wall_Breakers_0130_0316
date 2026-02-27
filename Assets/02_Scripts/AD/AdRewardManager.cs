using System.Collections;
using UnityEngine;

// 광고 보상 관리 담당 매니저
public class AdRewardManager : MonoBehaviour
{
    public static AdRewardManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ApplyReward(AdRewardData data)
    {
        if (data == null)
            return;

        StartCoroutine(ApplyRoutine(data));
    }

    private IEnumerator ApplyRoutine(AdRewardData data)
    {
        IAdReward reward = CreateReward(data);
        if (reward == null)
            yield break;

        reward.Apply();

        if (data.duration > 0)
        {
            yield return new WaitForSeconds(data.duration);
            reward.Revert();
        }
    }

    private IAdReward CreateReward(AdRewardData data)
    {
        switch (data.rewardType)
        {
            case AdRewardType.IncreaseBackpackCapacity:
                return new BackpackCapacityReward((int)data.value);

            case AdRewardType.IncreaseMoveSpeed:
                return new MoveSpeedReward(data.value);

            case AdRewardType.AddGold:
                return new GoldReward((int)data.value);

            default:
                return null;
        }
    }

}
