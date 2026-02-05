using System.Collections.Generic;

public class Inventory
{
    public Dictionary<ItemData, int> Items { get; } = new();

    public int TotalCount
    {
        get
        {
            int total = 0;
            foreach (var v in Items.Values)
                total += v;
            return total;
        }
    }

    public void AddInventory(ItemData item, int amount)
    {
        if (!Items.ContainsKey(item))
            Items[item] = 0;

        Items[item] += amount;
    }

    public void Remove(ItemData item, int amount)
    {
        if (!Items.ContainsKey(item))
            return;

        Items[item] -= amount;

        if (Items[item] <= 0)
            Items.Remove(item);
    }
}