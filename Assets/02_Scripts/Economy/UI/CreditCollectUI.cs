using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditCollectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI collectingText;
    [SerializeField] private TextMeshProUGUI remainingText;
    [SerializeField] private TextMeshProUGUI totalCreditsText;

    private CreditService creditService;
    private bool isInitalized = false;

    private void Start()
    {
        Hide();
        InitializeCreditService();
    }
    private void InitializeCreditService()
    {
        if (isInitalized) return;

        if(GameManager.Instance == null)
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
            return;
        }

        creditService = GameManager.Instance.GetService<CreditService>();

        if(creditService != null)
        {
            creditService.OnCreditsChanged += HandleCreditsUpdated;
            isInitalized= true;
            UpdateTotalCredits();
        }
        else
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
        }
    }
    private void OnDestroy()
    {
        if(creditService != null)
        {
            creditService.OnCreditsChanged -= HandleCreditsUpdated;
        }
    }
    private void HandleCreditsUpdated(long newCredits)
    {
        UpdateTotalCredits();
    }
    public void Show()
    {
        uiPanel.SetActive(true);

        if(!isInitalized)
        {
            InitializeCreditService();
        }

    }
    public void Hide()
    {
        uiPanel.SetActive(false);
    }
    public void UpdateDisplay(int collectingAmount, int remainingCount)
    {
        if(collectingAmount > 0)
        {
            collectingText.text = $"Collecting: +{collectingAmount}";
            remainingText.text = $"Remaining: {remainingCount}";
        }
        else
        {
            collectingText.text = "No credits to collect";
            remainingText.text = "";
        }
    }
    public void UpdateTotalCredits()
    {
        if(creditService != null && totalCreditsText != null)
        {
            string formattedCredits = NotateNumber.ChangeNumber(creditService.credits);
            totalCreditsText.text = $"Total: {formattedCredits}";
        }
    }
}