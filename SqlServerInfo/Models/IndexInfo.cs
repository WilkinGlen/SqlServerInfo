namespace SqlServerInfo.Models;

public sealed class IndexInfo(string name, string type)
{
    public string Name { get; set; } = name;

    public string Type { get; set; } = type;
}
