namespace SqlServerInfo.SqlScripts;

public static class SqlServerInfoServiceSqlScripts
{
    public const string GetColumnsSql =
        @"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
          FROM INFORMATION_SCHEMA.COLUMNS 
          WHERE TABLE_NAME = @tableName";

    public const string GetKeysSql =
        @"SELECT tc.CONSTRAINT_NAME, tc.CONSTRAINT_TYPE, kcu.COLUMN_NAME
          FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
              INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
          WHERE tc.TABLE_NAME = @tableName";

    public const string GetIndexesSql =
        @"SELECT i.name AS IndexName, i.type_desc AS IndexType
          FROM sys.indexes i
              INNER JOIN sys.tables t ON i.object_id = t.object_id
          WHERE t.name = @tableName AND i.is_primary_key = 0";
}
