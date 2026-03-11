using System;
using System.Collections.Generic;
using UnityEngine;

public class RegionUnlockService
{
    public static RegionUnlockService Instance { get; private set; }
    private CreditService creditService;

    private HashSet<int> unlockedRegions = new();

    public event Action<int> OnRegionUnlocked; // regionId

    public RegionUnlockService()
    {
        Instance = this;
        unlockedRegions.Add(1);
        LoadUnlockedRegions();
        Utils.DebugLog("[RegionUnlockService] Service created. Region 1 unlocked by default.");

    }

    public bool IsRegionUnlocked(int regionId)
    {
        return unlockedRegions.Contains(regionId);
    }

    public int GetNextLockedRegionId()
    {
        // 순차적으로 다음 잠긴 지역 찾기
        for (int i = 1; i <= 5; i++)
        {
            if (!unlockedRegions.Contains(i))
            {
                return i;
            }
        }
        return -1; // 모든 지역 해금됨
    }

    public bool CanUnlockRegion(int regionId, RegionData regionData)
    {
        if (IsRegionUnlocked(regionId))
        {
            Utils.DebugLog($"[RegionUnlockService] Region {regionId} already unlocked");
            return false;
        }

        if (regionId > 1 && !IsRegionUnlocked(regionId - 1))
        {
            Utils.DebugLog($"[RegionUnlockService] Previous region {regionId - 1} must be Unlocked first");
            return false;
        }

        creditService = GameManager.Instance.GetService<CreditService>();

        if (creditService.credits < regionData.unlockRequirement.requiredCredits)
        {
            Utils.DebugLog($"[RegionUnlockService] Not enough credits: {creditService.credits}/{regionData.unlockRequirement.requiredCredits}");
            return false;
        }

        foreach (var itemReq in regionData.unlockRequirement.requiredItems)
        {
            var inventoryItem = PlayerInventory.Instance.Items.Find(i => i.itemData == itemReq.itemData);
            int currentAmount = inventoryItem?.quantity ?? 0;

            if (currentAmount < itemReq.requiredAmount)
            {
                Utils.DebugLog($"[RegionUnlockedService] Not enough {itemReq.itemData.itemName}: {currentAmount}/{itemReq.requiredAmount}");
                return false;
            }
        }

        return true;
    }

    public bool TryUnlockRegion(int regionId, RegionData regionData)
    {
        if (!CanUnlockRegion(regionId, regionData))
        {
            return false;
        }

        long creditsToDeduct = regionData.unlockRequirement.requiredCredits;

        if (creditsToDeduct > 0)
        {
            creditService.AddCredit((int)-creditsToDeduct);
            Utils.DebugLog($"[RegionUnlockService] Deducted {creditsToDeduct} credits");
        }

        // 아이템 차감
        foreach (var itemReq in regionData.unlockRequirement.requiredItems)
        {
            bool removed = PlayerInventory.Instance.RemoveItem(itemReq.itemData, itemReq.requiredAmount);
            if (removed)
            {
                Utils.DebugLog($"[RegionUnlockService] Deducted {itemReq.requiredAmount}x {itemReq.itemData.itemName}");
            }
        }

        // 지역 해금
        unlockedRegions.Add(regionId);
        SaveUnlockedRegions();

        OnRegionUnlocked?.Invoke(regionId);

        // =========================
        // Guide Progress
        // =========================
        if (regionId == 2)
        {
            GuideManager.Instance?.AddProgress(GuideActionType.UnlockSection2);
        }

        Utils.DebugLog($"[RegionUnlockService] Region {regionId} unlocked");
        return true;
    }

    public void SaveUnlockedRegions()
    {
        string data = string.Join(",", unlockedRegions);
        PlayerPrefs.SetString("UnlockedRegions", data);
        PlayerPrefs.Save();
        //Debug.Log($"[RegionUnlockService]");
    }

    public void LoadUnlockedRegions()
    {
        unlockedRegions.Clear();

        string data = PlayerPrefs.GetString("UnlockedRegions", "1");
        string[] regionIds = data.Split(',');

        foreach (string id in regionIds)
        {
            if (int.TryParse(id, out int regionId))
            {
                unlockedRegions.Add(regionId);
            }
        }

        Utils.DebugLog($"[RegionUnlockService] Loaded unlocked regions: {data}");
    }

    public bool[] GetUnlockedStates(int totalUnlockSections)
    {
        bool[] states = new bool[totalUnlockSections];
        for (int i = 0; i < totalUnlockSections; i++)
        {
            states[i] = unlockedRegions.Contains(i + 1);
        }
        return states;
    }

    /// <summary>
    /// 디버그 테스트용
    /// </summary>
    public void ResetAllRegions()
    {
        unlockedRegions.Clear();
        unlockedRegions.Add(1); // 첫 지역만 유지
        SaveUnlockedRegions();
        Utils.DebugLog("[RegionUnlockService] All regions reset");
    }
}