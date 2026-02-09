using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RegionUnlockZone : MonoBehaviour
{
    [Header("Region Settings")]
    [SerializeField] private RegionData regionData;
    [SerializeField] private RegionUnlockUI unlockUI;
    [SerializeField] private RegionGate regionGate;

    private RegionUnlockService unlockService;
    private bool playerInZone = false;

    private void Start()
    {
        ValidateSetUp();
    }

    private void ValidateSetUp()
    {
        if (regionData == null)
        {
            Debug.LogError("[RegionUnlockZone] RegionData is Not assigned!");
        }

        if (unlockUI = null)
        {
            Debug.LogError("[RegionUnlockZone] RegionUnlockUI is Not assigned!");
        }

        if (regionGate = null)
        {
            Debug.LogError("[RegionUnlockZone] RegionGate is Not assigned!");
        }
    }

    private void InitializeService()
    {
        if (GameManager.Instance == null)
        {
            Invoke(nameof(InitializeService), 0.1f);
            return;
        }

        unlockService = GameManager.Instance.GetService<RegionUnlockService>();

        if (unlockService != null)
        {
            unlockService.OnRegionUnlocked += OnRegionUnlocked;

        }
    }

    private void OnRegionUnlocked(int regionId)
    {
        if (regionId == regionData.regionId)
        {
            UpdateGateState();

            if(playerInZone)
            {
                unlockUI.Hide();
            }
        }
    }

    private void UpdateGateState()
    {
        if (unlockService == null || regionGate == null) return;

        bool isUnlocked = unlockService.IsRegionUnlocked(regionData.regionId);
        regionGate.SetGateState(isUnlocked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (playerInZone) return;

            playerInZone = true;
            Debug.Log($"[RegionUnlockZone] Player entered Region {regionData.regionId} unlock zone");

            // 이미 해금된 경우 UI 표시 안 함
            if(unlockService.IsRegionUnlocked(regionData.regionId))
            {
                Debug.Log($"[RegionUnlockZone] Region {regionData.regionId} already unlocked");
                return;
            }

            // UI 표시
            unlockUI.Show(regionData, unlockService);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInZone = false;
            Debug.Log($"[RegionUnlockZone] Player exited Region {regionData.regionId} unlock zone");

            unlockUI.Hide();
        }
    }
    public void AttempUnlock()
    {
        if(unlockService == null || regionData == null)
        {
            Debug.LogError("[RegionUnlockZone] Cannot unlock - missing references!");
            return;
        }

        bool success = unlockService.TryUnlockRegion(regionData.regionId, regionData);

        if(success)
        {
            Debug.Log($"[RegionUnlockZone] Successfully unlocked Region {regionData.regionId}");
        }
        else
        {
            Debug.Log($"[RegionUnlockZone] Failed to unlock Region {regionData.regionId}");
        }
    }

}