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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (creditService == null)
                creditService = GameManager.Instance.GetService<CreditService>();

            if (creditService != null)
            {
                creditService.AddCredit((int)debugCreditAmount);
                Debug.Log($"[CreditDebugSetter] 디버그 골드 지급: +{debugCreditAmount}");
            }
        }
    }

    private void InitializeCreditService()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager NULL");
            Invoke(nameof(InitializeCreditService), 0.1f);
            return;
        }

        creditService = GameManager.Instance.GetService<CreditService>();
        Debug.Log("CreditService = " + creditService);
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