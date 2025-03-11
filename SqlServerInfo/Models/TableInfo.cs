namespace SqlServerInfo.Models;

public sealed class TableInfo(
    string schema,
    string name,
    List<ColumnInfo> columns,
    List<KeyInfo> keys,
    List<IndexInfo> indexes)
{
    public string Schema { get; set; } = schema;

    public string Name { get; set; } = name;

    public List<ColumnInfo> Columns { get; set; } = columns;

    public List<KeyInfo> Keys { get; set; } = keys;

    public List<IndexInfo> Indexes { get; set; } = indexes;
}
