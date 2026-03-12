using UnityEngine;

/// <summary>
/// 자동 운반인 컨트롤러
/// - ID 기반 Unlock만 담당
/// </summary>
public class CarrierController : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string targetID;

    [SerializeField] private bool isUnlocked;

    public bool IsUnlocked => isUnlocked;

    private void Start()
    {
        gameObject.SetActive(false);

        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierUnlockChanged += HandleUnlock;
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierUnlockChanged -= HandleUnlock;
        }
    }

    private void HandleUnlock(string id, bool unlocked)
    {
        if (id != targetID) return;

        isUnlocked = unlocked;
        gameObject.SetActive(isUnlocked);
    }
}
