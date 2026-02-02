using UnityEngine;

[CreateAssetMenu(fileName = "ShopLevel", menuName = "Game/Economy/ShopLevel")]
public class ShopLevelData : ScriptableObject
{
    public int level;
    public float sellSpeedMultiplier;
}