﻿namespace SqlServerInfo.Services;

using Microsoft.Data.SqlClient;
using SqlServerInfo.Models;
using SqlServerInfo.SqlScripts;
using System.Data;
using System.Runtime.CompilerServices;

public interface ISqlServerInfoService
{
    /// <summary>
    /// Retrieves information about all user databases from the SQL Server instance.
    /// </summary>
    /// <param name="connectionString">The connection string to the SQL Server instance.</param>
    /// <returns>A collection of DatabaseInfo objects containing database metadata.</returns>
    Task<IEnumerable<DatabaseInfo>> GetDatabasesAsync(string connectionString);

    /// <summary>
    /// Asynchronously enumerates information about all user databases from the SQL Server instance.
    /// </summary>
    /// <param name="connectionString">The connection string to the SQL Server instance.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of DatabaseInfo objects containing database metadata.</returns>
    IAsyncEnumerable<DatabaseInfo> GetDatabasesAsyncEnumerable(string connectionString, CancellationToken cancellationToken = default);

    void PopulateDatabaseForeignAndPrimaryTables(DatabaseInfo databaseInfo);
}

public sealed class SqlServerInfoService : ISqlServerInfoService
{
    public async Task<IEnumerable<DatabaseInfo>> GetDatabasesAsync(string connectionString)
    {
        var databases = new List<DatabaseInfo>();

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        var dbSchema = await conn.GetSchemaAsync("Databases");

        foreach (DataRow dbRow in dbSchema.Rows)
        {
            var databaseName = dbRow["database_name"].ToString()!;
            if (databaseName is "master" or "tempdb" or "model" or "msdb")
            {
                continue;
            }

            var (tables, dbConn) = await PopulateTables(connectionString, databaseName);
            databases.Add(new DatabaseInfo(databaseName, tables));
        }

        return databases;
    }

    public async IAsyncEnumerable<DatabaseInfo> GetDatabasesAsyncEnumerable(
        string connectionString,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);
        var dbSchema = await conn.GetSchemaAsync("Databases", cancellationToken);

        foreach (DataRow dbRow in dbSchema.Rows)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var databaseName = dbRow["database_name"].ToString()!;
            if (databaseName is "master" or "tempdb" or "model" or "msdb")
            {
                continue;
            }

            var (tables, _) = await PopulateTables(connectionString, databaseName);
            yield return new DatabaseInfo(databaseName, tables);
        }
    }

    public void PopulateDatabaseForeignAndPrimaryTables(DatabaseInfo databaseInfo)
    {
        foreach(var table in databaseInfo.Tables)
        {
            table.TablesWithForeignKeysToMe = 
                [.. databaseInfo.Tables.Where(x => x.Keys.Any(k => k.ReferencedTable == table.Name))];
            table.TablesWithPrimaryKeysFromMe =
                [.. databaseInfo.Tables.Where(primaryTable => table.Keys.Any(k => k.ReferencedTable == primaryTable.Name))];
        }
    }

    private static async Task<(List<TableInfo> tables, SqlConnection dbConn)> PopulateTables(string connectionString, string databaseName)
    {
        var tables = new List<TableInfo>();
        await using var dbConn = new SqlConnection($"{connectionString}Database={databaseName};");
        await dbConn.OpenAsync();
        var tableSchema = await dbConn.GetSchemaAsync("Tables");
        foreach (DataRow tableRow in tableSchema.Rows)
        {
            var tableSchemaName = tableRow["TABLE_SCHEMA"].ToString()!;
            var tableName = tableRow["TABLE_NAME"].ToString()!;
            var columns = new List<ColumnInfo>();
            var keys = new List<KeyInfo>();
            var indexes = new List<IndexInfo>();

            await PopulateTable(dbConn, tableName, columns);
            await PopulateKeys(dbConn, tableName, keys);
            await PopulateIndexes(dbConn, tableName, indexes);

            tables.Add(new TableInfo(tableSchemaName, tableName, columns, keys, indexes));
        }

        return (tables, dbConn);
    }

    private static async Task PopulateTable(
        SqlConnection dbConn,
        string tableName,
        List<ColumnInfo> columns)
    {
        await using var cmd = new SqlCommand(SqlServerInfoServiceSqlScripts.GetColumnsSql, dbConn);
        cmd.Parameters.AddWithValue("@tableName", tableName);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(new ColumnInfo(
                reader["COLUMN_NAME"].ToString()!,
                reader["DATA_TYPE"].ToString()!,
                reader["CHARACTER_MAXIMUM_LENGTH"]
            ));
        }
    }

    private static async Task PopulateKeys(
        SqlConnection dbConn,
        string tableName,
        List<KeyInfo> keys)
    {
        await using var keyCmd = new SqlCommand(SqlServerInfoServiceSqlScripts.GetKeysSql, dbConn);
        keyCmd.Parameters.AddWithValue("@tableName", tableName);
        await using var keyReader = await keyCmd.ExecuteReaderAsync();
        while (await keyReader.ReadAsync())
        {
            keys.Add(new KeyInfo(
                keyReader["Name"].ToString()!,
                keyReader["Type"].ToString()!,
                keyReader["ColumnName"].ToString()!,
                keyReader["ReferencedTable"].ToString()!,
                keyReader["ReferencedColumn"].ToString()!
            ));
        }
    }

    private static async Task<(SqlCommand indexCmd, SqlDataReader indexReader)> PopulateIndexes(
        SqlConnection dbConn,
        string tableName,
        List<IndexInfo> indexes)
    {
        await using var indexCmd = new SqlCommand(SqlServerInfoServiceSqlScripts.GetIndexesSql, dbConn);
        indexCmd.Parameters.AddWithValue("@tableName", tableName);
        await using var indexReader = await indexCmd.ExecuteReaderAsync();
        while (await indexReader.ReadAsync())
        {
            indexes.Add(new IndexInfo(
                indexReader["IndexName"].ToString()!,
                indexReader["IndexType"].ToString()!
            ));
        }

        return (indexCmd, indexReader);
    }
}
