namespace SqlServerInfo.Models;

public sealed class ColumnInfo(string name, string dataType, object maxLength)
{
    public string Name { get; set; } = name;

    public string DataType { get; set; } = dataType;

    public int? MaxLength { get; set; } = maxLength as int?;
}
