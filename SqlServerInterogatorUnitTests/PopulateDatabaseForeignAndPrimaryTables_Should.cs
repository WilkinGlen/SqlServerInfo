namespace SqlServerInterogatorUnitTests;

using FluentAssertions;
using SqlServerInterrogator.Models;
using SqlServerInterrogator.Services;

public class PopulateDatabaseForeignAndPrimaryTables_Should
{
    [Fact]
    public void PopulateTablesICanJoinToCorrectly_WhenThereAreNoJoinsBetweenTheFourTables()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "Table1", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "Table2", SchemaName = "dbo", TableId = 2 },
                new TableInfo { Name = "Table3", SchemaName = "dbo", TableId = 3 },
                new TableInfo { Name = "Table4", SchemaName = "dbo", TableId = 4 }
            ]
        };

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        foreach (var table in databaseInfo.Tables)
        {
            _ = table.TablesICanJoinTo.Should().BeEmpty();
        }
    }

    [Fact]
    public void PopulateTablesICanJoinToCorrectly_WhenThereAreJoinsBetweenAllTheFourTables()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "Table1", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "Table2", SchemaName = "dbo", TableId = 2 },
                new TableInfo { Name = "Table3", SchemaName = "dbo", TableId = 3 },
                new TableInfo { Name = "Table4", SchemaName = "dbo", TableId = 4 }
            ]
        };
        // Table1 can join to Table2 and Table3
        databaseInfo.Tables[0].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table2"
        });
        databaseInfo.Tables[0].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table3"
        });
        // Table2 can join to Table4
        databaseInfo.Tables[1].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table4"
        });
        // Table3 can join to Table4
        databaseInfo.Tables[2].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table4"
        });

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        foreach (var table in databaseInfo.Tables)
        {
            _ = table.TablesICanJoinTo.Should().NotBeEmpty();
            _ = table.TablesICanJoinTo.Should().HaveCount(3);
            if (table.Name == "Table1")
            {
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table2");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table3");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table4");
            }
            else if (table.Name == "Table2")
            {
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table1");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table4");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table3");
            }
            else if (table.Name == "Table3")
            {
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table1");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table4");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table2");
            }
            else if (table.Name == "Table4")
            {
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table2");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table3");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table1");
            }
        }
    }

    [Fact]
    public void PopulateTablesICanJoinToCorrectly_WhenThereAreDirectAndIndirectJoinsBetweenAllTheFourTables()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "Table1", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "Table2", SchemaName = "dbo", TableId = 2 },
                new TableInfo { Name = "Table3", SchemaName = "dbo", TableId = 3 },
                new TableInfo { Name = "Table4", SchemaName = "dbo", TableId = 4 }
            ]
        };
        // Table1 can join to Table2 and Table3 (direct)
        databaseInfo.Tables[0].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table2"
        });
        databaseInfo.Tables[0].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table3"
        });
        // Table2 can join to Table4 (direct)
        databaseInfo.Tables[1].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table4"
        });
        // Table3 can join to Table4 (direct)
        databaseInfo.Tables[2].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table4"
        });

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        foreach (var table in databaseInfo.Tables)
        {
            if (table.Name == "Table1")
            {
                // Table1 can join directly to Table2 and Table3, and indirectly to Table4 via Table2 or Table3
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table2");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table3");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table4");
                _ = table.TablesICanJoinTo.Should().HaveCount(3);
            }
            else if (table.Name == "Table2")
            {
                // Table2 can join directly to Table4, and indirectly to Table1 (via foreign key from Table1) and Table3 (via Table4)
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table1");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table4");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table3");
                _ = table.TablesICanJoinTo.Should().HaveCount(3);
            }
            else if (table.Name == "Table3")
            {
                // Table3 can join directly to Table4, and indirectly to Table1 (via foreign key from Table1) and Table2 (via Table4)
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table1");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table4");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table2");
                _ = table.TablesICanJoinTo.Should().HaveCount(3);
            }
            else if (table.Name == "Table4")
            {
                // Table4 can join to Table2 and Table3 directly, and Table1 indirectly via Table2 or Table3
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table1");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table2");
                _ = table.TablesICanJoinTo.Should().Contain(t => t.Name == "Table3");
                _ = table.TablesICanJoinTo.Should().HaveCount(3);
            }
        }
    }
}
