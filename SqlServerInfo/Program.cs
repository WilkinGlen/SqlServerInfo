using Microsoft.Data.SqlClient;
using SqlServerInfo;
using System.Data;

var connectionString = "Server=localhost;Integrated Security=True;TrustServerCertificate=True;";

var databases = new List<DatabaseInfo>();

await using var conn = new SqlConnection(connectionString);
await conn.OpenAsync();
var dbSchema = await Task.Run(() => conn.GetSchema("Databases"));

foreach (DataRow dbRow in dbSchema.Rows)
{
    var databaseName = dbRow["database_name"].ToString()!;
    if (databaseName is "master" or "tempdb" or "model" or "msdb")
    {
        continue;
    }

    var tables = new List<TableInfo>();
    await using var dbConn = new SqlConnection($"{connectionString}Database={databaseName};");
    await dbConn.OpenAsync();
    var tableSchema = await Task.Run(() => dbConn.GetSchema("Tables"));
    foreach (DataRow tableRow in tableSchema.Rows)
    {
        var tableSchemaName = tableRow["TABLE_SCHEMA"].ToString()!;
        var tableName = tableRow["TABLE_NAME"].ToString()!;
        var columns = new List<ColumnInfo>();
        var keys = new List<KeyInfo>();
        var indexes = new List<IndexInfo>();

        await using (var cmd = new SqlCommand(@"
                            SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = @TableName", dbConn))
        {
            _ = cmd.Parameters.AddWithValue("@TableName", tableName);
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

        await using (var keyCmd = new SqlCommand(@"
                            SELECT tc.CONSTRAINT_NAME, tc.CONSTRAINT_TYPE, kcu.COLUMN_NAME
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
                            WHERE tc.TABLE_NAME = @TableName", dbConn))
        {
            _ = keyCmd.Parameters.AddWithValue("@TableName", tableName);
            await using var keyReader = await keyCmd.ExecuteReaderAsync();
            while (await keyReader.ReadAsync())
            {
                keys.Add(new KeyInfo(
                    keyReader["CONSTRAINT_NAME"].ToString()!,
                    keyReader["CONSTRAINT_TYPE"].ToString()!,
                    keyReader["COLUMN_NAME"].ToString()!
                ));
            }
        }

        await using var indexCmd = new SqlCommand(@"
                            SELECT i.name AS IndexName, i.type_desc AS IndexType
                            FROM sys.indexes i
                                INNER JOIN sys.tables t ON i.object_id = t.object_id
                            WHERE t.name = @TableName AND i.is_primary_key = 0", dbConn);
        _ = indexCmd.Parameters.AddWithValue("@TableName", tableName);
        await using var indexReader = await indexCmd.ExecuteReaderAsync();
        while (await indexReader.ReadAsync())
        {
            indexes.Add(new IndexInfo(
                indexReader["IndexName"].ToString()!,
                indexReader["IndexType"].ToString()!
            ));
            Console.WriteLine($"    Index: {indexReader["IndexName"]} - {indexReader["IndexType"]}");
        }

        tables.Add(new TableInfo(tableSchemaName, tableName, columns, keys, indexes));
    }

    databases.Add(new DatabaseInfo(databaseName, tables));
}

Console.WriteLine("Done");
