namespace SqlServerUI.Components.Pages;

using MudBlazor;
using SqlServerInterrogator.Models;

public sealed partial class Home
{
    private const string ServerConnectionString = "Server=localhost;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

    private ServerInfo? serverInfo;

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

    private void SelectedTableChanged(TableInfo tableInfo)
    {
        
    }
}

