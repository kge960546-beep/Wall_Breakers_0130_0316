using System.Collections.Generic;
using UnityEngine;

public class SellLogService
{
    public List<SellLogEntry> Logs { get; } = new();

    public void Add(string itemName, int sold, int gold)
    {
        Logs.Add(new SellLogEntry
        {
            time = Time.time,
            itemName = itemName,
            soldAmount = sold,
            earnedGold = gold
        });
    }
}
