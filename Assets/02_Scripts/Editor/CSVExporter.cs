#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CSVExporter
{
    public static void Export(ScriptableObject database, string path)
    {
        if (database is not ICSVDatabase db)
        {
            Debug.LogError("Database가 ICSVDatabase를 구현하지 않았습니다.");
            return;
        }

        var rows = db.ExportRows();

        if (rows.Count == 0)
        {
            Debug.LogWarning("Export할 데이터 없음");
            return;
        }

        StringBuilder sb = new();

        // 해더
        var headers = new List<string>(rows[0].Keys);
        sb.AppendLine(string.Join(",", headers));

        foreach (var row in rows)
        {
            List<string> values = new();

            foreach (var header in headers)
            {
                object rawValue = row.ContainsKey(header) ? row[header] : null;

                string value = CSVValueConverter.ToCSVValue(rawValue);

                values.Add(CSVValueConverter.ToCSVValue(value));
            }

            sb.AppendLine(string.Join(",", values));
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();

        Debug.Log($"CSV Export 완료: {path}");
    }
}
#endif