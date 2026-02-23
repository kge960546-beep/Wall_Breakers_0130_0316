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

    private int mineAmountBonus = 0;
    public int MineAmountBonus => mineAmountBonus;

    private void Start()
    {
        gameObject.SetActive(false);

        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnMinerUnlockChanged += HandleMinerUnlockChanged;
            UpgradeEffectManager.Instance.OnMinerMineAmountChanged += HandleMinerMineAmountChanged;
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnMinerUnlockChanged -= HandleMinerUnlockChanged;
            UpgradeEffectManager.Instance.OnMinerMineAmountChanged -= HandleMinerMineAmountChanged;
        }
    }

    private void HandleMinerMineAmountChanged(string id, int bonus)
    {
        if (id == minerID)
        {
            mineAmountBonus = bonus;
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
