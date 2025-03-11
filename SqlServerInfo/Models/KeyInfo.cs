namespace SqlServerInfo.Models;

public sealed class KeyInfo(
    string name,
    string type,
    string columnName)
{
    public string Name { get; set; } = name;

    public string Type { get; set; } = type;

    public string ColumnName { get; set; } = columnName;
}
