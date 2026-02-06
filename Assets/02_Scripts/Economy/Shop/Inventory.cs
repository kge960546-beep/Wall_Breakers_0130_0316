using System.Collections.Generic;

public class Inventory
{
    public Dictionary<ItemDataSO, int> Items { get; } = new();

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

    public void AddInventory(ItemDataSO item, int amount)
    {
        if (!Items.ContainsKey(item))
            Items[item] = 0;

        Items[item] += amount;
    }

    public void Remove(ItemDataSO item, int amount)
    {
        if (!Items.ContainsKey(item))
            return;

        Items[item] -= amount;

        if (Items[item] <= 0)
            Items.Remove(item);
    }
}