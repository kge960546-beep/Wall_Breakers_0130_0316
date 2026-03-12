using TMPro;
using UnityEngine;
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

    [Header("UI Settings")]
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2, 0); // UI 위치 오프셋
    [SerializeField] private bool faceCamera = true; // 카메라를 향하게 할지

    private RegionData currentRegionData;
    private RegionUnlockService unlockService;
    private RegionUnlockZone currentZone;
    private Camera mainCamera;

    private void Start()
    {
        Utils.DebugLog("[RegionUnlockUI] Start called");
        ValidateSetup();
        Hide();

        if (unlockButton != null)
        {
            unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        }

        mainCamera = Camera.main;
    }

    private void ValidateSetup()
    {
        if (uiPanel == null)
        {
            Utils.DebugLogError("[RegionUnlockUI] UI Panel is NOT assigned!");
        }
        else
        {
            Utils.DebugLog($"[RegionUnlockUI] UI Panel assigned: {uiPanel.name}");
        }

        if (regionNameText == null)
            Utils.DebugLogWarning("[RegionUnlockUI] RegionNameText is not assigned");
        if (descriptionText == null)
            Utils.DebugLogWarning("[RegionUnlockUI] DescriptionText is not assigned");
        if (creditsRequiredText == null)
            Utils.DebugLogWarning("[RegionUnlockUI] CreditsRequiredText is not assigned");
        if (unlockButton == null)
            Utils.DebugLogWarning("[RegionUnlockUI] UnlockButton is not assigned");
    }

    public void Show(RegionData regionData, RegionUnlockService service, RegionUnlockZone zone, Vector3 worldPosition)
    {
        Utils.DebugLog($"[RegionUnlockUI] Show called for {regionData.regionName} at position {worldPosition}");

        currentRegionData = regionData;
        unlockService = service;
        currentZone = zone;

        // UI 위치 설정
        transform.position = worldPosition + uiOffset;

        uiPanel.SetActive(true);
        Utils.DebugLog($"[RegionUnlockUI] UI Panel activated: {uiPanel.activeSelf}");

        UpdateDisplay();
    }

    public void Hide()
    {
        Utils.DebugLog("[RegionUnlockUI] Hide called");

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        currentRegionData = null;
        currentZone = null;
    }

    private void Update()
    {
        if (uiPanel.activeSelf)
        {
            // 카메라를 향하게 회전
            if (faceCamera && mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                 mainCamera.transform.rotation * Vector3.up);
            }

            // 실시간 업데이트
            if (currentRegionData != null)
            {
                UpdateDisplay();
            }
        }
    }

    private void UpdateDisplay()
    {
        if (currentRegionData == null)
        {
            Utils.DebugLogWarning("[RegionUnlockUI] Cannot update display - currentRegionData is null");
            return;
        }

        // 지역 이름 및 설명
        if (regionNameText != null)
        {
            regionNameText.text = currentRegionData.regionName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = currentRegionData.description;
        }

        // 크레딧 요구사항
        long requiredCredits = currentRegionData.unlockRequirement.requiredCredits;
        CreditService creditService = GameManager.Instance.GetService<CreditService>();
        long currentCredits = creditService != null ? creditService.credits : 0;

        string creditsColor = currentCredits >= requiredCredits ? "#00FF00" : "#FF0000";

        if (creditsRequiredText != null)
        {
            creditsRequiredText.text = $"Credits: <color={creditsColor}>{NotateNumber.ChangeNumber(currentCredits)}</color> / {NotateNumber.ChangeNumber(requiredCredits)}";
        }

        // 아이템 요구사항
        if (itemRequirementsContainer != null && itemRequirementPrefab != null)
        {
            ClearItemRequirements();

            foreach (var itemReq in currentRegionData.unlockRequirement.requiredItems)
            {
                CreateItemRequirementUI(itemReq);
            }
        }

        // 해금 버튼 상태
        if (unlockService != null)
        {
            bool canUnlock = unlockService.CanUnlockRegion(currentRegionData.regionId, currentRegionData);

            if (unlockButton != null)
            {
                unlockButton.interactable = canUnlock;
            }

            if (unlockButtonText != null)
            {
                unlockButtonText.text = canUnlock ? "Unlock Region" : "Requirements Not Yet";
            }
        }
    }

    private void ClearItemRequirements()
    {
        foreach (Transform child in itemRequirementsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateItemRequirementUI(ItemRequirement itemReq)
    {
        GameObject itemUI = Instantiate(itemRequirementPrefab, itemRequirementsContainer);

        TextMeshProUGUI itemText = itemUI.GetComponentInChildren<TextMeshProUGUI>();
        Image itemIcon = itemUI.GetComponentInChildren<Image>();

        var inventoryItem = PlayerInventory.Instance.Items.Find(i => i.itemData == itemReq.itemData);
        int currentAmount = inventoryItem?.quantity ?? 0;

        string itemColor = currentAmount >= itemReq.requiredAmount ? "#00FF00" : "#FF0000";

        if (itemText != null)
        {
            itemText.text = $"{itemReq.itemData.itemName}: <color={itemColor}>{currentAmount}</color> / {itemReq.requiredAmount}";
        }

        if (itemIcon != null && itemReq.itemData.icon != null)
        {
            itemIcon.sprite = itemReq.itemData.icon;
        }
    }

    private void OnUnlockButtonClicked()
    {
        Utils.DebugLog("[RegionUnlockUI] Unlock button clicked");

        if (currentZone != null)
        {
            currentZone.AttemptUnlock();
        }
        else
        {
            Utils.DebugLogError("[RegionUnlockUI] currentZone is null!");
        }
    }
}
