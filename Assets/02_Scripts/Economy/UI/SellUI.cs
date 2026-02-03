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

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        uiPanel.SetActive(true);
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
        totalCreditsText.text = $"Total Credits: {PlayerInventory.Instance.Credits}";
    }
}