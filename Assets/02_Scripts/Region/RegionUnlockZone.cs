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
        //Debug.Log($"[RegionUnlockZone] Start - Region {regionData?.regionId}");
        ValidateSetup();
        InitializeService();
        UpdateGateState();
    }

    private void ValidateSetup()
    {
        if (regionData == null)
        {
            Utils.DebugLogError("[RegionUnlockZone] RegionData is NOT assigned!");
        }
        else
        {
            //Debug.Log($"[RegionUnlockZone] RegionData assigned: {regionData.regionName} (ID: {regionData.regionId})");
        }

        if (unlockUI == null)
        {
            Utils.DebugLogError("[RegionUnlockZone] RegionUnlockUI is NOT assigned!");
        }
        else
        {
            //Debug.Log($"[RegionUnlockZone] UnlockUI assigned: {unlockUI.gameObject.name}");
        }

        if (regionGate == null)
        {
            Utils.DebugLogWarning("[RegionUnlockZone] RegionGate is NOT assigned (optional)");
        }

        if (uiSpawnPoint == null)
        {
            Utils.DebugLogWarning("[RegionUnlockZone] UI Spawn Point not assigned, using zone position");
            uiSpawnPoint = transform;
        }
    }

    private void InitializeService()
    {
        if (GameManager.Instance == null)
        {
            Utils.DebugLogWarning("[RegionUnlockZone] GameManager not ready, retrying...");
            Invoke(nameof(InitializeService), 0.1f);
            return;
        }

        unlockService = GameManager.Instance.GetService<RegionUnlockService>();

        if (unlockService != null)
        {
            unlockService.OnRegionUnlocked += OnRegionUnlocked;
            //Debug.Log($"[RegionUnlockZone] ✓ Service initialized for Region {regionData.regionId}");

            // 초기 게이트 상태 설정
            UpdateGateState();
        }
        else
        {
            Utils.DebugLogError("[RegionUnlockZone] Failed to get RegionUnlockService, retrying...");
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
        //Debug.Log($"[RegionUnlockZone] Region {regionId} unlocked event received");

        if (regionId == regionData.regionId)
        {
            //Debug.Log($"[RegionUnlockZone] This region ({regionData.regionId}) was unlocked!");
            
            // 게이트 상태 즉시 업데이트
            UpdateGateState();
            if(SFXManager.instance != null)
            {
                SFXManager.instance.PlayOnSFX("270404__littlerobotsoundfactory__jingle_achievement_00", Camera.main.transform.position);
            }

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
            Utils.DebugLogWarning("[RegionUnlockZone] Cannot update gate - unlockService is null");
            return;
        }

        if (regionGate == null)
        {
            Utils.DebugLogWarning("[RegionUnlockZone] Cannot update gate - regionGate is null");
            return;
        }

        bool isUnlocked = unlockService.IsRegionUnlocked(regionData.regionId);
        //Debug.Log($"[RegionUnlockZone] Region {regionData.regionId} unlock status: {isUnlocked}");

        // 게이트 상태 변경 (unlocked면 open = true)
        regionGate.SetGateState(isUnlocked);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"[RegionUnlockZone] OnTriggerEnter: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            if (playerInZone)
            {
                Utils.DebugLogWarning("[RegionUnlockZone] Player already in zone!");
                return;
            }

            playerInZone = true;
            Utils.DebugLog($"[RegionUnlockZone] ✓ Player ENTERED Region {regionData.regionId} unlock zone");

            if (unlockService == null)
            {
                Utils.DebugLogError("[RegionUnlockZone] unlockService is NULL!");
                return;
            }

            // 이미 해금된 경우 UI 표시 안 함
            if (unlockService.IsRegionUnlocked(regionData.regionId))
            {
                Utils.DebugLog($"[RegionUnlockZone] Region {regionData.regionId} already unlocked - not showing UI");
                return;
            }

            // UI 표시
            if (unlockUI == null)
            {
                Utils.DebugLogError("[RegionUnlockZone] unlockUI is NULL!");
                return;
            }

            Utils.DebugLog($"[RegionUnlockZone] Showing UI at position: {uiSpawnPoint.position}");
            unlockUI.Show(regionData, unlockService, this, uiSpawnPoint.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Utils.DebugLog($"[RegionUnlockZone] OnTriggerExit: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            if (!playerInZone)
            {
                Utils.DebugLogWarning("[RegionUnlockZone] Player was not in zone!");
                return;
            }

            playerInZone = false;
            Utils.DebugLog($"[RegionUnlockZone] ✓ Player EXITED Region {regionData.regionId} unlock zone");

            if (unlockUI != null)
            {
                unlockUI.Hide();
            }
        }
    }

    public void AttemptUnlock()
    {
        Utils.DebugLog($"[RegionUnlockZone] AttemptUnlock called for Region {regionData?.regionId}");

        if (unlockService == null)
        {
            Utils.DebugLogError("[RegionUnlockZone] Cannot unlock - unlockService is null!");
            return;
        }

        if (regionData == null)
        {
            Utils.DebugLogError("[RegionUnlockZone] Cannot unlock - regionData is null!");
            return;
        }

        bool success = unlockService.TryUnlockRegion(regionData.regionId, regionData);

        if (success)
        {
            Utils.DebugLog($"[RegionUnlockZone] ✓✓✓ Successfully unlocked Region {regionData.regionId}!");
        }
        else
        {
            Utils.DebugLogWarning($"[RegionUnlockZone] ✗ Failed to unlock Region {regionData.regionId}");
        }
    }
}