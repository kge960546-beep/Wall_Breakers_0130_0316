using System;
using System.Collections.Generic;
using UnityEngine;

public static class CSVParser
{
    public static void Parse(string csv, ScriptableObject database)
    {
        if (database is not ICSVDatabase db)
        {
            Debug.LogError("Database가 ICSVDatabase를 구현하지 않았습니다.");
            return;
        }

        db.Clear();

        // 줄 분리
        var lines = csv.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1)
        {
            Debug.LogError("CSV 데이터가 없습니다.");
            return;
        }

        // 헤더
        var headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            try
            {
                var values = lines[i].Split(',');

                var row = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length; j++)
                {
                    string key = headers[j].Trim();
                    string value = values[j].Trim();

                    row[key] = value;
                }

                db.AddRow(row);
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV Parse Error line {i + 1}: {e.Message}");
            }
        }

        Debug.Log($"CSV Parse 완료: {lines.Length - 1} rows");
    }
}