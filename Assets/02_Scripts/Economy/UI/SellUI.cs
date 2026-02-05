using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI totalCreditsText;

    private CreditService creditService;
    private bool isInitalized = false;

    private void Start()
    {
        Hide();
        InitalizeCreditService();
    }
    private void InitalizeCreditService()
    {
        if (isInitalized) return;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager not ready yet, retrying...");
            Invoke(nameof(InitalizeCreditService), 0.1f);
            return;
        }

        // CreditService 가져오기 및 이벤트 구독
        creditService = GameManager.Instance.GetService<CreditService>();
        if( creditService != null )
        {
            creditService.OnCreditsChanged += OnCreditsUpdated;
            isInitalized = true;
            //초기 크레딧 표시
            UpdateTotalCredits();
            Debug.Log("[SellUI] creditService successfully initalized in SellUI");
        }
        else
        {
            Debug.LogError("[SellUI] CreditService not found in SellUI!");
            //재시도
            Invoke(nameof(InitalizeCreditService), 0.1f);
        }

    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if(creditService != null)
        {
            creditService.OnCreditsChanged -= OnCreditsUpdated;
        }
    }

    // CreditService 이벤트 핸들러
    private void OnCreditsUpdated(long newCredits)
    {
        UpdateTotalCredits();
    }
    public void Show()
    {
        uiPanel.SetActive(true);

        // 초기화 되지 않았다면 다시 시도
        if(!isInitalized)
        {
            InitalizeCreditService();
        }
        UpdateTotalCredits();
    }

    public void Hide()
    {
        uiPanel.SetActive(false);
    }

    public void UpdateDisplay(InventoryItem item, float duration)
    {
        if (item == null)
        {
            itemNameText.text = "No items to sell";
            priceText.text = "";
            itemIcon.enabled = false;
            progressBar.fillAmount = 0f;
            return;
        }

        itemIcon.enabled = true;
        itemIcon.sprite = item.itemData.icon;
        itemNameText.text = $"{item.itemData.itemName} x{item.quantity}";
        priceText.text = $"+{item.itemData.sellPrice} Credits";
        progressBar.fillAmount = 0f;
    }

    public void UpdateProgress(float progress)
    {
        progressBar.fillAmount = progress;
    }

    public void UpdateTotalCredits()
    {
        if (creditService != null && totalCreditsText != null)
        {
            totalCreditsText.text = $"Total Credits: {creditService.credits}";
        }
        else if(totalCreditsText != null)
        {
            totalCreditsText.text = "Total Credits: ...";
        }
    }
}