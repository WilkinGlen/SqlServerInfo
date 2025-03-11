namespace SqlServerInfo;

public class TableInfo
{
    public TableInfo(string schema, string name, List<ColumnInfo> columns, List<KeyInfo> keys, List<IndexInfo> indexes)
    {
        this.Schema = schema;
        this.Name = name;
        this.Columns = columns;
        this.Keys = keys;
        this.Indexes = indexes;
    }

    public string Schema { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<ColumnInfo> Columns { get; set; } = [];
    public List<KeyInfo> Keys { get; set; } = [];
    public List<IndexInfo> Indexes { get; set; } = [];
}
