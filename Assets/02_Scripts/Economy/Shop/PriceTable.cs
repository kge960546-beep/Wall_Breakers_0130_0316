using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PriceTable", menuName = "Game/Economy/PriceTable")]
public class PriceTable : ScriptableObject
{
    public List<MineralData> minerals;
    public int GetPrice(MineralData mineral)
    {
        return mineral.basePrice;
    }
}