using System;

[Serializable]
public class InventoryItem
{
    public ItemDataSO itemData;
    public int quantity;

    public InventoryItem(ItemDataSO data, int qty)
    {
        itemData = data;
        quantity = qty;
    }
}