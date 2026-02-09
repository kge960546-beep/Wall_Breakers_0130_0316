using System;
using System.Collections.Generic;
using UnityEngine;

public class RegionUnlockService
{
    private CreditService creditService;

    private HashSet<int> unlockedRegions = new();

    public event Action<int> OnRegionUnlocked; // regionId

    public RegionUnlockService()
    {
        unlockedRegions.Add(1);
        LoadUnlockedRegions();
    }

    private void Awake()
    {
        creditService = GameManager.Instance.GetService<CreditService>();
    }

    public bool IsRegionUnlocked(int regionId)
    {
        return unlockedRegions.Contains(regionId);
    }

    public int GetNextLockedRegionId()
    {
        // 순차적으로 다음 잠긴 지역 찾기
        for(int i = 1; i<=5; i++)
        {
            if(!unlockedRegions.Contains(i))
            {
                return i;
            }
        }
        return -1; // 모든 지역 해금됨
    }

    public bool CanUnlockRegion(int regionId, RegionData regionData)
    {
        if(IsRegionUnlocked(regionId))
        {
            Debug.Log($"[RegionUnlockService] Region {regionId} already unlocked");
            return false;
        }

        if(regionId > 1 && !IsRegionUnlocked(regionId - 1))
        {
            Debug.Log($"[RegionUnlockService] Previous region {regionId - 1} must be Unlocked first");
            return false;
        }

        if(creditService.credits < regionData.unlockRequirement.requiredCredits)
        {
            Debug.Log($"[RegionUnlockService] Not enough credits: {creditService.credits}/{regionData.unlockRequirement.requiredCredits}");
            return false;
        }

        foreach(var itemReq in regionData.unlockRequirement.requiredItems)
        {
            var inventoryItem = PlayerInventory.Instance.Items.Find(i => i.itemData == itemReq.itemdata);
            int currentAmount = inventoryItem?.quantity ?? 0;

            if (currentAmount < itemReq.requiredAmount)
            {
                Debug.Log($"[RegionUnlockedService] Not enough {itemReq.itemdata.itemName}: {currentAmount}/{itemReq.requiredAmount}");
                return false;
            }
        }

        return true;
    }

    public bool TryUnlockRegion(int regionId, RegionData regionData)
    {
        if(!CanUnlockRegion(regionId, regionData))
        {
            return false;
        }

        long creditsToDeduct = regionData.unlockRequirement.requiredCredits;

        if(creditsToDeduct >0)
        {
            creditService.AddCredit((int)-creditsToDeduct);
            Debug.Log($"[RegionUnlockService] Deducted {creditsToDeduct} credits");
        }

        // 아이템 차감
        foreach(var itemReq in regionData.unlockRequirement.requiredItems)
        {
            bool removed = PlayerInventory.Instance.RemoveItem(itemReq.itemdata, itemReq.requiredAmount);
            if(removed)
            {
                Debug.Log($"[RegionUnlockService] Deducted {itemReq.requiredAmount}x {itemReq.itemdata.itemName}");
            }
        }

        // 지역 해금
        unlockedRegions.Add(regionId);
        SaveUnlockedRegions();

        OnRegionUnlocked?.Invoke(regionId);

        Debug.Log($"[RegionUnlockService] Region {regionId} unlocked");
        return true;
    }

    private void SaveUnlockedRegions()
    {
        string data = string.Join(",", unlockedRegions);
        PlayerPrefs.SetString("UnlcokedRegions", data);
        PlayerPrefs.Save();
        Debug.Log($"[RegionUnlockService]");
    }

    private void LoadUnlockedRegions()
    {
        string data = PlayerPrefs.GetString("UnlockedRegions", "1");
        string[] regionIds = data.Split(',');

        foreach(string id in regionIds)
        {
            if(int.TryParse(id, out int regionId))
            {
                unlockedRegions.Add(regionId);
            }
        }

        Debug.Log($"[RegionUnlockService] Loaded unlocked regions: {data}");
    }

    /// <summary>
    /// 디버그 테스트용
    /// </summary>
    public void ResetAllRegions()
    {
        unlockedRegions.Clear();
        unlockedRegions.Add(1); // 첫 지역만 유지
        SaveUnlockedRegions();
        Debug.Log("[RegionUnlockService] All regions reset");
    }
}