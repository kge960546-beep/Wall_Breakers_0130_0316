using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public static class CSVParser
{
    public static void Parse(string csv, ScriptableObject db)
    {
        var listField = db.GetType().GetField("items");
        if (listField == null)
        {
            Debug.LogError($"{db.name} 에 items 리스트가 필요합니다.");
            return;
        }

        IList list = listField.GetValue(db) as IList;
        list.Clear();

        var elementType = listField.FieldType.GetGenericArguments()[0];

        var lines = csv.Split('\n');
        if (lines.Length == 0) return;

        var headers = lines[0].Trim().Split(',');

        var fields = elementType.GetFields();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var row = lines[i].Trim().Split(',');

            try
            {
                var element = Activator.CreateInstance(elementType);

                for (int h = 0; h < headers.Length; h++)
                {
                    var field = Array.Find(fields, f => f.Name == headers[h]);
                    if (field == null) continue;

                    object value = ConvertValue(row[h], field.FieldType);
                    field.SetValue(element, value);
                }

                list.Add(element);
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV Parse Error line {i + 1}: {e.Message}");
            }
        }
    }

    static object ConvertValue(string value, Type type)
    {
        if (type.IsEnum)
            return Enum.Parse(type, value);

        if (type == typeof(bool))
            return value == "true" || value == "1";

        return Convert.ChangeType(value, type);
    }
}