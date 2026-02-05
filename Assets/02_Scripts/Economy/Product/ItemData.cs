using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public int sellPrice;
    public float sellDuration = 1f; // 기본 판매 시간
    public Sprite icon;
}

public enum ItemType
{
    // 광물
    Iron,
    Copper,
    Silver,

    // 가공품
    IronIngot,
    CopperIngot,
    SilverIngot
}