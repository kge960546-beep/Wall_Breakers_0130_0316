using System;
using UnityEngine;

[Serializable]
public class RegionRequirment
{
    public long requiredCredits;
    public ItemRequirement[] requiredItems;
}

[Serializable]
public class ItemRequirement
{
    public ItemData itemdata;
    public int requiredAmount;
}
