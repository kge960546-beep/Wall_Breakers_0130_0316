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
        string fileName = row["fileName"];
        string path = ITEM_FOLDER + fileName + ".asset";

        ItemDataSO item = AssetDatabase.LoadAssetAtPath<ItemDataSO>(path);

        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemDataSO>();
            AssetDatabase.CreateAsset(item, path);
        }

        item.Id = int.Parse(row["Id"]);
        item.itemName = row["itemName"];

        if (System.Enum.TryParse(row["itemType"], true, out ItemType type))
            item.itemType = type;

        item.sellPrice = int.Parse(row["sellPrice"]);
        item.sellDuration = float.Parse(row["sellDuration"]);
        item.inputAmountPerProcess = int.Parse(row["inputAmountPerProcess"]);

        item.backPackRotationOffset = ParseVector3(row["backPackRotationOffset"]);

        // 프리팹 임포트
        if (row.TryGetValue("prefabPath", out string prefabPath))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
                item.mineralPrefab = prefab;
            else
                Debug.LogWarning($"Prefab not found: {prefabPath}");
        }

        // 아이콘 임포트
        if (row.TryGetValue("iconPath", out string iconPath))
        {
            var icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            if (icon != null)
                item.icon = icon;
            else
                Debug.LogWarning($"Icon not found: {iconPath}");
        }

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
                { "fileName", item.name },
                { "Id", item.Id.ToString() },
                { "itemName", item.itemName },
                { "itemType", item.itemType.ToString() },
                { "sellPrice", item.sellPrice.ToString() },
                { "sellDuration", item.sellDuration.ToString() },
                { "inputAmountPerProcess", item.inputAmountPerProcess.ToString() },
                { "backPackRotationOffset", $"{item.backPackRotationOffset.x}|{item.backPackRotationOffset.y}|{item.backPackRotationOffset.z}" },
                { "prefabPath", AssetDatabase.GetAssetPath(item.mineralPrefab) },
                { "iconPath", AssetDatabase.GetAssetPath(item.icon) }
            });
        }

        return rows;
    }

    /// <summary>
    /// 벡터 동기화
    /// </summary>
    Vector3 ParseVector3(string value)
    {
        var split = value.Split('|');

        if (split.Length != 3)
            return Vector3.zero;

        return new Vector3(
            float.Parse(split[0]),
            float.Parse(split[1]),
            float.Parse(split[2]));
    }
}

