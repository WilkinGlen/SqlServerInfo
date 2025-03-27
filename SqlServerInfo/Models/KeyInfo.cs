namespace SqlServerInfo.Models;

public sealed class KeyInfo(
    string name,
    string type,
    string columnName,
    string referencedTable,
    string referencedColumn)
{
    public string Name { get; set; } = name;

    public string Type { get; set; } = type;

    public string ColumnName { get; set; } = columnName;

    public string ReferencedTable { get; set; } = referencedTable;

    public string ReferencedColumn { get; set; } = referencedColumn;
}
