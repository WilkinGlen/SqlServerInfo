namespace SqlServerUI.Components.Pages;

using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using SqlServerInterrogator.Models;

public sealed partial class Home
{
    private const string ServerConnectionString = "Server=localhost;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

    private ServerInfo? serverInfo;
    private TableInfo? selectedTable;
    private readonly List<ColumnInfo> selectedColumns = [];
    private string generatedSql = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            serverInfo = await SqlServerInterrogator.Services.ServerInterrogator.GetServerInfoAsync(ServerConnectionString);
            serverInfo.Databases = await SqlServerInterrogator.Services.ServerInterrogator.GetDatabasesAsync(ServerConnectionString);
            foreach (var database in serverInfo.Databases)
            {
                database.Tables = await SqlServerInterrogator.Services.DatabaseInterrogator.GetTableInfoAsync(
                    ServerConnectionString,
                    database.Name!);
                database.StoredProcedures = await SqlServerInterrogator.Services.DatabaseInterrogator.GetStoredProcedureInfoAsync(
                    ServerConnectionString,
                    database.Name!);
                SqlServerInterrogator.Services.DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(database);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving server info: {ex.Message}");
        }
    }

    private void SelectedTableChanged(TableInfo tableInfo) => this.selectedTable = tableInfo;

    private void AddColumn(ColumnInfo columnInfo)
    {
        if (!this.selectedColumns.Contains(columnInfo))
        {
            this.selectedColumns.Add(columnInfo);
            if(this.selectedColumns.Count > 0)
            {
                var firstColumn = this.selectedColumns[0];
                var database = this.serverInfo?.Databases.Single(d => d.Name == firstColumn.DatabaseName);
                if (database is not null)
                {
                    this.generatedSql = SqlServerInterrogator.Services.SqlGenerator
                        .GenerateSelectStatement(this.selectedColumns, database);
                }
            }
            else
            {
                this.generatedSql = string.Empty;
            }            
        }
    }

    private void RemoveColumn(ColumnInfo columnInfo)
    {
        if (this.selectedColumns.Contains(columnInfo))
        {
            _ = this.selectedColumns.Remove(columnInfo);
        }
    }
}

