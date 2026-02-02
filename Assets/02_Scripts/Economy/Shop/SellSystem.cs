
using System.Collections.Generic;
using UnityEngine;

public class SellSystem
{
    private List<SellSlot> sellSlots;
    private GoldService goldService;
    private PriceTable priceTable;
    private ShopLevelData shopLevel;

    public SellSystem(
        List<SellSlot> sellSlots,
        GoldService goldService,
        PriceTable priceTable,
        ShopLevelData shopLevel
        )
    {
        this.sellSlots = sellSlots;
        this.goldService = goldService;
        this.priceTable = priceTable;
        this.shopLevel = shopLevel;
    }

    public int FastForward(double offlineSeconds)
    {
        int totalEarnedGold = 0;

        foreach(var slot in sellSlots)
        {
            if(slot.amount <= 0)
                continue;

            float sellPreSecond = slot.mineral.baseSellPerSecond * shopLevel.sellSpeedMultiplier;
            int sellable = Mathf.FloorToInt((float)(offlineSeconds * sellPreSecond));
            int sold = Mathf.Min(slot.amount, sellable);

            if (sold <= 0)
                continue;

            slot.amount -= sold;

            int gold = sold * priceTable.GetPrice(slot.mineral);
            goldService.AddGold(gold);
            totalEarnedGold += gold;
        }

        return totalEarnedGold;
    }
}
