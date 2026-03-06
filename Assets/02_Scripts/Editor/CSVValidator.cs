using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class CSVValidator
{
    public static void Validate(ScriptableObject db)
    {
        var listField = db.GetType().GetField("items");
        IList list = listField.GetValue(db) as IList;

        if (list == null || list.Count == 0) return;

        var elementType = listField.FieldType.GetGenericArguments()[0];
        var idField = elementType.GetField("id");

        if (idField == null) return;

        HashSet<int> ids = new HashSet<int>();

        foreach (var item in list)
        {
            int id = (int)idField.GetValue(item);

            if (!ids.Add(id))
            {
                Debug.LogError($"중복 ID 발견 ({db.name}): {id}");
            }
        }
    }
}