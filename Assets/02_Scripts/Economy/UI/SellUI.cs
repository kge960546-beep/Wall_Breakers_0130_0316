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
            Utils.DebugLogWarning("GameManager not ready yet, retrying...");
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
            Utils.DebugLog("[SellUI] creditService successfully initalized in SellUI");
        }
        else
        {
            Utils.DebugLogError("[SellUI] CreditService not found in SellUI!");
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

    public void UpdateDisplay(ItemDataSO item, float duration, int finalPrice)
    {
        if (item == null)
        {
            itemNameText.text = "판매할 아이템 부족!";
            priceText.text = "";
            itemIcon.enabled = false;
            progressBar.fillAmount = 0f;
            return;
        }

        if (itemIcon != null)
        {
            itemIcon.enabled = true;
            itemIcon.sprite = item.icon;
        }

        int currentQuantity = PlayerInventory.Instance.GetItemCount(item);
        itemNameText.text = $"{item.itemName} x{currentQuantity}";

        // 실제 적용 가격 표시
        string formattedPrice = NotateNumber.ChangeNumber(finalPrice);
        priceText.text = $"+{formattedPrice}";

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
            string formattedCredits = NotateNumber.ChangeNumber(creditService.credits);
            totalCreditsText.text = $"총 골드: {formattedCredits}";
        }
    }
}