﻿namespace SqlServerUI.Components.Pages;

using MudBlazor;
using SqlServerInterrogator.Models;
using SqlServerInterrogator.Services;

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
            this.serverInfo = await ServerInterrogator.GetFullServerInfoAsync(ServerConnectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving server info: {ex.Message}");
        }
    }

    private void SelectedDatabaseChanged(bool newVal)
    {
        this.selectedColumns.Clear();
        this.selectedTable = null;
        this.generatedSql = string.Empty;
    }

    private void SelectedTableChanged(TableInfo tableInfo)
    {
        this.selectedTable = tableInfo;
    }

    private void AddColumn(ColumnInfo columnInfo)
    {
        if (!this.selectedColumns.Contains(columnInfo))
        {
            this.selectedColumns.Add(columnInfo);
            if (this.selectedColumns.Count > 0)
            {
                var firstColumn = this.selectedColumns[0];
                var database = this.serverInfo?.Databases.Single(d => d.Name == firstColumn.DatabaseName);
                if (database is not null)
                {
                    this.generatedSql = SqlGenerator.GenerateSelectStatement(this.selectedColumns, database);
                }
            }
            else
            {
                this.generatedSql = string.Empty;
            }
        }
    }

    private List<TableInfo> GetJoinableTables(DatabaseInfo databaseInfo)
    {
        if (this.selectedColumns == null || this.selectedColumns.Count == 0)
        {
            return databaseInfo.Tables;
        }

        var selectedTableIds = this.selectedColumns.Select(c => c.TableId).ToHashSet();
        var selectedTables = databaseInfo.Tables.Where(t => selectedTableIds.Contains(t.TableId)).ToList();
        var joinableTables = selectedTables
            .SelectMany(t => t.TablesICanJoinTo)
            .Distinct()
            .ToList();

        foreach (var t in selectedTables)
        {
            if (!joinableTables.Contains(t))
            {
                joinableTables.Add(t);
            }
        }

        return joinableTables;
    }

    private void RemoveColumn(ColumnInfo columnInfo)
    {
        if (this.selectedColumns.Contains(columnInfo))
        {
            _ = this.selectedColumns.Remove(columnInfo);
        }
    }
}

