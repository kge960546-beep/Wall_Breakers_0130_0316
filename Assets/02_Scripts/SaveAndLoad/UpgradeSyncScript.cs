using UnityEngine;

public class UpgradeSyncScript : MonoBehaviour
{ 
    public void UpgradeSuccess()
    {
        if(UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.RecalculateAllEffects();
        }

        if(SceneGameDataManager.instance != null)
        {
            SceneGameDataManager.instance.SaveGame();
        }

        Utils.DebugLog("<color=yellow>업그레이드 성공: 수치 갱신 및 데이터 저장 완료</color>");
    }
}
