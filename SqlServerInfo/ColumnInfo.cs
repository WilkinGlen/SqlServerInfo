namespace SqlServerInfo;

public class ColumnInfo
{
    public ColumnInfo(string name, string dataType, object maxLength)
    {
        this.Name = name;
        this.DataType = dataType;
        this.MaxLength = maxLength as int?;
    }

    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public int? MaxLength { get; set; }
}
