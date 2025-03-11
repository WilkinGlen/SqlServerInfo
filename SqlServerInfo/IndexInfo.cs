namespace SqlServerInfo;

public class IndexInfo
{
    public IndexInfo(string name, string type)
    {
        this.Name = name;
        this.Type = type;
    }

    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
