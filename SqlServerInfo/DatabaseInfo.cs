namespace SqlServerInfo;

public class DatabaseInfo
{
    public DatabaseInfo(string databaseName, List<TableInfo> tables)
    {
        this.Name = databaseName;
        this.Tables = tables;
    }

    public string Name { get; set; } = string.Empty;
    public List<TableInfo> Tables { get; set; } = [];
}
