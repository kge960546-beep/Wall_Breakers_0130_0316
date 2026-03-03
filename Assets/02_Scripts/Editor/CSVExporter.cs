#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CSVExporter
{
    public static void Export(ScriptableObject db, string path)
    {
        var listField = db.GetType().GetField("items");
        IList list = listField.GetValue(db) as IList;

        var elementType = listField.FieldType.GetGenericArguments()[0];
        var fields = elementType.GetFields();

        using (StreamWriter writer = new StreamWriter(path))
        {
            for (int i = 0; i < fields.Length; i++)
            {
                writer.Write(fields[i].Name);
                if (i < fields.Length - 1)
                    writer.Write(",");
            }
            writer.WriteLine();

            foreach (var item in list)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    writer.Write(fields[i].GetValue(item));
                    if (i < fields.Length - 1)
                        writer.Write(",");
                }
                writer.WriteLine();
            }
        }

        AssetDatabase.Refresh();
    }
}
#endif