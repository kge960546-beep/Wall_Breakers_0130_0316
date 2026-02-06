using System;
using UnityEngine;

[System.Serializable]
public class SellSlot
{
    public ItemDataSO item;
    public int amount;

    public void Set(ItemDataSO item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public bool IsEmpty => item == null || amount <= 0;
}
