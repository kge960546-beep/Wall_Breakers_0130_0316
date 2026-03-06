using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "",menuName = "Game/Database/Item Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemDataSO> items = new();
}
