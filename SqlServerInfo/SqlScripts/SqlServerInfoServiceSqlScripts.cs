namespace SqlServerInfo.SqlScripts;

public static class SqlServerInfoServiceSqlScripts
{
    public const string GetColumnsSql =
        @"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
          FROM INFORMATION_SCHEMA.COLUMNS 
          WHERE TABLE_NAME = @tableName";

    public const string GetKeysSql =
        @"SELECT 
              fk.name AS [Name],
          	'FOREIGN KEY' AS [Type],
              tp.name AS ParentTable,
              cp.name AS [ColumnName],
              tr.name AS ReferencedTable,
              cr.name AS ReferencedColumn
          FROM sys.foreign_keys AS fk
          	INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
          		INNER JOIN sys.tables AS tp ON fkc.parent_object_id = tp.object_id
          			INNER JOIN sys.columns AS cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
          				INNER JOIN sys.tables AS tr ON fkc.referenced_object_id = tr.object_id
          					INNER JOIN sys.columns AS cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
          WHERE tp.name = @tableName";

    public const string GetIndexesSql =
        @"SELECT i.name AS IndexName, i.type_desc AS IndexType
          FROM sys.indexes i
              INNER JOIN sys.tables t ON i.object_id = t.object_id
          WHERE t.name = @tableName AND i.is_primary_key = 0";
}
