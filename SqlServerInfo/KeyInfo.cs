namespace SqlServerInfo;

public class KeyInfo
{
    public KeyInfo(string name, string type, string columnName)
    {
        this.Name = name;
        this.Type = type;
        this.ColumnName = columnName;
    }

    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
}
