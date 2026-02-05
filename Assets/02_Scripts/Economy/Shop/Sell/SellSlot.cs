using System;
using UnityEngine;

[System.Serializable]
public class SellSlot
{
    public ItemData item;
    public int amount;

    public void Set(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public bool IsEmpty => item == null || amount <= 0;
}
