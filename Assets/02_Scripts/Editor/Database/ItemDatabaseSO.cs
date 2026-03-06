using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase SO",menuName = "Game/Database/Item Database")]
public class ItemDatabaseSO : ScriptableObject, ICSVDatabase
{
    public List<ItemDataSO> items = new();

    private const string ITEM_FOLDER = "Assets/06_Data/Economy/";

    public void Clear()
    {
        items.Clear();
    }

    public void AddRow(Dictionary<string, string> row)
    {
        string itemName = row["itemName"];
        string path = ITEM_FOLDER + itemName + ".asset";

        ItemDataSO item = AssetDatabase.LoadAssetAtPath<ItemDataSO>(path);

        // 이미 존재하면 로드
        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemDataSO>();
            AssetDatabase.CreateAsset(item, path);
        }

        // 값 업데이트
        item.Id = int.Parse(row["Id"]);
        item.itemName = itemName;

        if (System.Enum.TryParse(row["itemType"], true, out ItemType type))
            item.itemType = type;

        item.sellPrice = int.Parse(row["sellPrice"]);
        item.sellDuration = float.Parse(row["sellDuration"].Replace("f", ""));

        item.inputAmountPerProcess = int.Parse(row["inputAmountPerProcess"]);

        item.backPackRotationOffset = ParseVector3(row["backPackRotationOffset"]);

        EditorUtility.SetDirty(item);

        if (!items.Contains(item))
            items.Add(item);
    }

    public List<Dictionary<string, string>> ExportRows()
    {
        var rows = new List<Dictionary<string, string>>();

        foreach (var item in items)
        {
            if (item == null) continue;

            rows.Add(new Dictionary<string, string>
            {
                { "Id", item.Id.ToString() },
                { "itemName", item.itemName },
                { "itemType", item.itemType.ToString() },
                { "sellPrice", item.sellPrice.ToString() },
                { "sellDuration", item.sellDuration.ToString() },
                { "inputAmountPerProcess", item.inputAmountPerProcess.ToString() },
                { "backPackRotationOffset", $"{item.backPackRotationOffset.x}|{item.backPackRotationOffset.y}|{item.backPackRotationOffset.z}" }
            });
        }

        return rows;
    }

    // 벡터 동기화
    Vector3 ParseVector3(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Vector3.zero;

        var v = value.Split('|');

        return new Vector3(
            float.Parse(v[0]),
            float.Parse(v[1]),
            float.Parse(v[2]));
    }
}
