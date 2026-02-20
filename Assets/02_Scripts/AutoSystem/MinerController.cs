using UnityEngine;

/// <summary>
/// ÀÚµ¿ Ã¤±¼ ±¤ºÎ ÄÁÆ®·Ñ·¯
/// </summary>
public class MinerController : MonoBehaviour
{
    [Header("±¤ºÎ ¼³Á¤")]
    [SerializeField] private int minerIndex;

    [Header("±¤ºÎ ID")]
    [SerializeField] private string minerID;

    public string MinerID => minerID;

    private void Start()
    {
        gameObject.SetActive(false);

        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnMinerUnlockChanged += HandleMinerUnlockChanged;
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnMinerUnlockChanged -= HandleMinerUnlockChanged;
        }
    }

    private void HandleMinerUnlockChanged(int totalUnlockedCount)
    {
        if (minerIndex <= totalUnlockedCount)
        {
            gameObject.SetActive(true);
        }
    }
}
