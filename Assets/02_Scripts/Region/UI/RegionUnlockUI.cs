using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RegionUnlockUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI regionNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI creditsRequiredText;
    [SerializeField] private Transform itemRequirementsContainer;
    [SerializeField] private GameObject itemRequirementPrefab;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockButtonText;

    private RegionData currentRegionData;
    private RegionUnlockService unlockService;
    private RegionUnlockZone currentZone;

    private void Start()
    {
        Hide();

        if(unlockButton != null)
        {
            unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        }
    }
    public void Show(RegionData regionData, RegionUnlockService service)
    {
        currentRegionData = regionData;
        unlockService = service;

        uiPanel.SetActive(true);
        UpdateDisplay();
    }

    public void Hide()
    {
        uiPanel.SetActive(false);
        currentRegionData = null;
    }

    private void UpdateDisplay()
    {
        if (currentRegionData != null) return;

        // 지역 이름 및 설명
        regionNameText.text = currentRegionData.regionName;
        descriptionText.text = currentRegionData.description;

        // 크레딧 요구 사항
        long requiredCredits = currentRegionData.unlockRequirement.requiredCredits;
        long currentCredits = GameManager.Instance.GetService<CreditService>().credits;

        string creditsColor = currentCredits >= requiredCredits ? "#00FF00" : "#FF0000";
        creditsRequiredText.text = $"Credits: <color={creditsColor}>{NotateNumber.ChangeNumber(currentCredits)}</color> / {NotateNumber.ChangeNumber(requiredCredits)}";

        ClearItemRequirements();

        foreach(var itemReq in currentRegionData.unlockRequirement.requiredItems)
        {
            CreateItemRequirementUI(itemReq);
        }

        bool canUnlock = unlockService.CanUnlockRegion(currentRegionData.regionId, currentRegionData);
        unlockButton.interactable = canUnlock;
        unlockButtonText.text = canUnlock ? "Unlock Region" : "Requirements Not Met";
    }

    private void ClearItemRequirements()
    {
        foreach(Transform child in itemRequirementsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateItemRequirementUI(ItemRequirement itemReq)
    {
        GameObject itemUI = Instantiate(itemRequirementPrefab);

        //아이템 정보 설정(프리팹에 따라 조정)
        TextMeshProUGUI itemText = itemUI.GetComponentInChildren<TextMeshProUGUI>();
        Image itemIcon = itemUI.GetComponentInChildren<Image>();

        var inventoryItem = PlayerInventory.Instance.Items.Find(i => i.itemData == itemReq.itemdata);
        int currentAmount = inventoryItem?.quantity ?? 0;

        string itemColor = currentAmount >= itemReq.requiredAmount ? "#00FF00" : "#FF0000";

        if(itemText != null)
        {
            itemText.text = $"{itemReq.itemdata.itemName}: <color={itemColor}>{currentAmount}</color> / {itemReq.requiredAmount}";
        }

        if(itemIcon != null && itemReq.itemdata.icon != null)
        {
            itemIcon.sprite = itemReq.itemdata.icon;
        }
    }

    private void OnUnlockButtonClicked()
    {
        // RegionUnlockZone을 통해 해금 시도
        RegionUnlockZone zone = FindObjectOfType<RegionUnlockZone>();
        if(zone != null)
        {
            zone.AttempUnlock();
        }
    }

    // 실시간 업데이트 (선택사항)
    private void Update()
    {
        if(uiPanel.activeSelf && currentRegionData != null)
        {
            UpdateDisplay();
        }
    }
}
