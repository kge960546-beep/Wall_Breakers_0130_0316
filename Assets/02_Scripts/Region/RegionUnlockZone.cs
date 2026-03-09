using UnityEngine;

public class RegionUnlockZone : MonoBehaviour
{
    [Header("Region Settings")]
    [SerializeField] private RegionData regionData;
    [SerializeField] private RegionUnlockUI unlockUI;
    [SerializeField] private RegionGate regionGate;

    [Header("UI Position")]
    [SerializeField] private Transform uiSpawnPoint; // UI 표시 위치

    private RegionUnlockService unlockService;
    private bool playerInZone = false;

    private void Start()
    {
        Debug.Log($"[RegionUnlockZone] Start - Region {regionData?.regionId}");
        ValidateSetup();
        InitializeService();
        UpdateGateState();
    }

    private void ValidateSetup()
    {
        if (regionData == null)
        {
            Debug.LogError("[RegionUnlockZone] RegionData is NOT assigned!");
        }
        else
        {
            Debug.Log($"[RegionUnlockZone] RegionData assigned: {regionData.regionName} (ID: {regionData.regionId})");
        }

        if (unlockUI == null)
        {
            Debug.LogError("[RegionUnlockZone] RegionUnlockUI is NOT assigned!");
        }
        else
        {
            Debug.Log($"[RegionUnlockZone] UnlockUI assigned: {unlockUI.gameObject.name}");
        }

        if (regionGate == null)
        {
            Debug.LogWarning("[RegionUnlockZone] RegionGate is NOT assigned (optional)");
        }

        if (uiSpawnPoint == null)
        {
            Debug.LogWarning("[RegionUnlockZone] UI Spawn Point not assigned, using zone position");
            uiSpawnPoint = transform;
        }
    }

    private void InitializeService()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[RegionUnlockZone] GameManager not ready, retrying...");
            Invoke(nameof(InitializeService), 0.1f);
            return;
        }

        unlockService = GameManager.Instance.GetService<RegionUnlockService>();

        if (unlockService != null)
        {
            unlockService.OnRegionUnlocked += OnRegionUnlocked;
            Debug.Log($"[RegionUnlockZone] ✓ Service initialized for Region {regionData.regionId}");

            // 초기 게이트 상태 설정
            UpdateGateState();
        }
        else
        {
            Debug.LogError("[RegionUnlockZone] Failed to get RegionUnlockService, retrying...");
            Invoke(nameof(InitializeService), 0.1f);
        }
    }

    private void OnDestroy()
    {
        if (unlockService != null)
        {
            unlockService.OnRegionUnlocked -= OnRegionUnlocked;
        }
    }

    private void OnRegionUnlocked(int regionId)
    {
        Debug.Log($"[RegionUnlockZone] Region {regionId} unlocked event received");

        if (regionId == regionData.regionId)
        {
            Debug.Log($"[RegionUnlockZone] This region ({regionData.regionId}) was unlocked!");
            
            // 게이트 상태 즉시 업데이트
            UpdateGateState();

            if (playerInZone)
            {
                unlockUI.Hide();
            }
        }
    }

    public void UpdateGateState()
    {
        if (unlockService == null)
        {
            Debug.LogWarning("[RegionUnlockZone] Cannot update gate - unlockService is null");
            return;
        }

        if (regionGate == null)
        {
            Debug.LogWarning("[RegionUnlockZone] Cannot update gate - regionGate is null");
            return;
        }

        bool isUnlocked = unlockService.IsRegionUnlocked(regionData.regionId);
        Debug.Log($"[RegionUnlockZone] Region {regionData.regionId} unlock status: {isUnlocked}");

        // 게이트 상태 변경 (unlocked면 open = true)
        regionGate.SetGateState(isUnlocked);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[RegionUnlockZone] OnTriggerEnter: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            if (playerInZone)
            {
                Debug.LogWarning("[RegionUnlockZone] Player already in zone!");
                return;
            }

            playerInZone = true;
            Debug.Log($"[RegionUnlockZone] ✓ Player ENTERED Region {regionData.regionId} unlock zone");

            if (unlockService == null)
            {
                Debug.LogError("[RegionUnlockZone] unlockService is NULL!");
                return;
            }

            // 이미 해금된 경우 UI 표시 안 함
            if (unlockService.IsRegionUnlocked(regionData.regionId))
            {
                Debug.Log($"[RegionUnlockZone] Region {regionData.regionId} already unlocked - not showing UI");
                return;
            }

            // UI 표시
            if (unlockUI == null)
            {
                Debug.LogError("[RegionUnlockZone] unlockUI is NULL!");
                return;
            }

            Debug.Log($"[RegionUnlockZone] Showing UI at position: {uiSpawnPoint.position}");
            unlockUI.Show(regionData, unlockService, this, uiSpawnPoint.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[RegionUnlockZone] OnTriggerExit: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            if (!playerInZone)
            {
                Debug.LogWarning("[RegionUnlockZone] Player was not in zone!");
                return;
            }

            playerInZone = false;
            Debug.Log($"[RegionUnlockZone] ✓ Player EXITED Region {regionData.regionId} unlock zone");

            if (unlockUI != null)
            {
                unlockUI.Hide();
            }
        }
    }

    public void AttemptUnlock()
    {
        Debug.Log($"[RegionUnlockZone] AttemptUnlock called for Region {regionData?.regionId}");

        if (unlockService == null)
        {
            Debug.LogError("[RegionUnlockZone] Cannot unlock - unlockService is null!");
            return;
        }

        if (regionData == null)
        {
            Debug.LogError("[RegionUnlockZone] Cannot unlock - regionData is null!");
            return;
        }

        bool success = unlockService.TryUnlockRegion(regionData.regionId, regionData);

        if (success)
        {
            Debug.Log($"[RegionUnlockZone] ✓✓✓ Successfully unlocked Region {regionData.regionId}!");
        }
        else
        {
            Debug.LogWarning($"[RegionUnlockZone] ✗ Failed to unlock Region {regionData.regionId}");
        }
    }
}