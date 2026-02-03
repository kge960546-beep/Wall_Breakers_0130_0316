using UnityEngine;

[CreateAssetMenu(fileName = "ShopLevel", menuName = "Game/Economy/ShopLevel")]
public class ShopLevelData : ScriptableObject
{
    [Header("Shop Level Settings")]
    public int currentLevel = 1;
    public float[] levelSpeedMultipliers = { 1f, 0.9f, 0.8f, 0.7f, 0.6f }; // 레벨별 판매 속도 배수

    public float GetSpeedMultiplier()
    {
        int index = Mathf.Clamp(currentLevel - 1, 0, levelSpeedMultipliers.Length - 1);
        return levelSpeedMultipliers[index];
    }
}