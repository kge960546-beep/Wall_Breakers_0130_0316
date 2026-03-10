using System.Collections.Generic;

public interface ICSVDatabase
{
    void Clear();
    void AddRow(Dictionary<string, string> row);
    List<Dictionary<string, string>> ExportRows();
}
