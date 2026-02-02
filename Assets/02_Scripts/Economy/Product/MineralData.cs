using UnityEngine;

[CreateAssetMenu(fileName = "MineralData", menuName = "Game/Economy/MineralData")]
public class MineralData : ScriptableObject
{
    public int id;
    public string mineralName;
    public Sprite icon;

    public int basePrice;
    public float baseSellPerSecond;
}
