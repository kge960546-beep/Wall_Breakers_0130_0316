using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "Game/Item/Product")]
public class ProductData : ItemDataSO
{
    public List<Ingredient> ingredients;
    public float processingTime;
}

[System.Serializable]
public class Ingredient
{
    public ItemDataSO Item;
    public int amount;
}