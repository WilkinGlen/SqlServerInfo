namespace SqlServerInfo.Models;

public sealed class DatabaseInfo(string databaseName, List<TableInfo> tables)
{
    public string Name { get; set; } = databaseName;

    public List<TableInfo> Tables { get; set; } = tables;
}
