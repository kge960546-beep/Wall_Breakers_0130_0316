using System;
using UnityEngine;

public static class CSVValueConverter
{
    public static string ToCSVValue(object value)
    {
        if (value == null)
            return "";

        switch (value)
        {
            case Vector2 v2:
                return $"{v2.x}|{v2.y}";

            case Vector3 v3:
                return $"{v3.x}|{v3.y}|{v3.z}";

            case Quaternion q:
                return $"{q.x}|{q.y}|{q.z}|{q.w}";

            case bool b:
                return b ? "true" : "false";

            case Enum e:
                return e.ToString();

            default:
                return value.ToString();
        }
    }
}
