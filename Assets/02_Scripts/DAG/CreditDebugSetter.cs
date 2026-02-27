using UnityEngine;

public class CreditDebugSetter : MonoBehaviour
{
    [Header("Debug Credit Setting")]
    [SerializeField] private long debugCreditAmount = 10000;

    private CreditService creditService;

    private void Start()
    {
        InitializeCreditService();
    }

    private void InitializeCreditService()
    {
        if (GameManager.Instance == null)
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
            return;
        }

        creditService = GameManager.Instance.GetService<CreditService>();

        if (creditService != null)
        {
            creditService.SetCredit(debugCreditAmount);
        }
        else
        {
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Set Debug Credit")]
    private void SetDebugCreditManually()
    {
        if (creditService == null)
            creditService = GameManager.Instance.GetService<CreditService>();

        if (creditService != null)
        {
            creditService.SetCredit(debugCreditAmount);
        }
    }
#endif
}
