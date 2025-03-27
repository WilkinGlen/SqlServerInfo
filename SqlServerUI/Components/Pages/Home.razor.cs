namespace SqlServerUI.Components.Pages;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SqlServerInfo.Models;
using SqlServerInfo.Services;
using System.Threading.Tasks;

public sealed partial class Home
{
    private List<DatabaseInfo>? databases;
    private DatabaseInfo? selectedDatabase;
    private TableInfo? selectedTable;

    private readonly List<DropItem> droppableItems = [];

    [Inject]
    private ISqlServerInfoService? SqlServerInfoService { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.databases = [.. await this.SqlServerInfoService!.GetDatabasesAsync(
                "Server=localhost;Integrated Security=True;TrustServerCertificate=True;")];
            this.StateHasChanged();
        }
    }

    private void DatabaseSelected(ChangeEventArgs args)
    {
        this.selectedTable = null;
        this.selectedDatabase = this.databases?.FirstOrDefault(x => x.Name.Equals(args.Value));
    }

    private void TableSelected(TableInfo tableInfo)
    {
        this.selectedTable = tableInfo;
        this.droppableItems.Clear();
        foreach (var col in this.selectedTable.Columns)
        {
            this.droppableItems.Add(new DropItem { Name = col.Name, Identifier = "Drop Zone 1" });
        }

        var foreignKeyColumns = tableInfo.Keys
            .Where(x => x.ReferencedColumn != null)
            .Select(x => new DropItem
            {
                Name = $"{x.ReferencedColumn}=>({x.ReferencedTable})",
                Identifier = "Drop Zone 1"
            });
        this.droppableItems.AddRange(foreignKeyColumns);

        List<DropItem> primaryDropItems = [];
        foreach (var table in this.selectedDatabase?.Tables!.Where(x => x.Name != tableInfo.Name)!)
        {
            if (table.Keys.Any(x => x.ReferencedTable == tableInfo.Name))
            {
                primaryDropItems.AddRange(
                    table.Columns.Select(x => new DropItem
                    {
                        Name = $"{x.Name}<=({table.Name})",
                        Identifier = "Drop Zone 1"
                    }));
            }
        }

        this.droppableItems.AddRange(primaryDropItems);

        this.StateHasChanged();
    }

    private static void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
    {
        dropItem!.Item!.Identifier = dropItem.DropzoneIdentifier;
    }
}

public class DropItem
{
    public string? Name { get; init; }
    public string? Identifier { get; set; }
}
