using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "Game/Item/Product")]
public class ProductData : ItemData
{
    public List<Ingredient> ingredients;
    public float processingTime;
}

[System.Serializable]
public class Ingredient
{
    public ItemData Item;
    public int amount;
}