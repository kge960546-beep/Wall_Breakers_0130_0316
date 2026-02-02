using System;
using UnityEngine;

[System.Serializable]
public class SellSlot
{
    public MineralData mineral;
    public int amount;

    public void Set(MineralData mineral, int amount)
    {
        this.mineral = mineral;
        this.amount = amount;
    }

    public bool IsEmpty => mineral == null || amount == 0;
}
