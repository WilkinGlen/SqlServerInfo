namespace SqlServerUI.Models;

using SqlServerInfo.Models;

public class DroppableKeyInfo
{
    public KeyInfo? KeyInfo { get; set; }

    public string Identifier => this.KeyInfo!.Name;
}
