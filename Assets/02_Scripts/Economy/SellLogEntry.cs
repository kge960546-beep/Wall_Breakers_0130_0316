using System;

public class SellLogEntry
{
    public float time;
    public string itemName;
    public int soldAmount;
    public int earnedGold;

    public SellLogEntry() { }

    public SellLogEntry(float time, string itemName, int soldAmount, int earnedGold)
    {
        this.time = time;
        this.itemName = itemName;
        this.soldAmount = soldAmount;
        this.earnedGold = earnedGold;
    }
}
