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

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldNotIncludeSelf_WhenTableHasSelfReference()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "Table1", SchemaName = "dbo", TableId = 1 }
            ]
        };
        databaseInfo.Tables[0].Keys.Add(new KeyInfo
        {
            IsForeignKey = true,
            ReferencedTableName = "Table1"
        });

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().BeEmpty();
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldHandleCircularReferences()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "A", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "B", SchemaName = "dbo", TableId = 2 },
                new TableInfo { Name = "C", SchemaName = "dbo", TableId = 3 }
            ]
        };
        databaseInfo.Tables[0].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "B" }); // A → B
        databaseInfo.Tables[1].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "C" }); // B → C
        databaseInfo.Tables[2].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "A" }); // C → A

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        foreach (var table in databaseInfo.Tables)
        {
            _ = table.TablesICanJoinTo.Should().HaveCount(2);
        }
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldNotDuplicate_WhenMultipleForeignKeysExist()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "Parent", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "Child", SchemaName = "dbo", TableId = 2 }
            ]
        };
        databaseInfo.Tables[1].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "Parent" });
        databaseInfo.Tables[1].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "Parent" });

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        _ = databaseInfo.Tables[1].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "Parent");
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldNotJoinDisconnectedTables()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "A", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "B", SchemaName = "dbo", TableId = 2 },
                new TableInfo { Name = "C", SchemaName = "dbo", TableId = 3 },
                new TableInfo { Name = "D", SchemaName = "dbo", TableId = 4 }
            ]
        };
        databaseInfo.Tables[0].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "B" }); // A → B
        databaseInfo.Tables[2].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "D" }); // C → D

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "B");
        _ = databaseInfo.Tables[2].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "D");
        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().NotContain(t => t.Name == "C" || t.Name == "D");
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldHandleEmptyTableList()
    {
        var databaseInfo = new DatabaseInfo { Name = "TestDatabase", Tables = [] };

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        _ = databaseInfo.Tables.Should().BeEmpty();
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldHandleMultipleDisconnectedGraphs()
    {
        // Two separate groups: (A <-> B), (C <-> D)
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "A", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "B", SchemaName = "dbo", TableId = 2 },
                new TableInfo { Name = "C", SchemaName = "dbo", TableId = 3 },
                new TableInfo { Name = "D", SchemaName = "dbo", TableId = 4 }
            ]
        };
        databaseInfo.Tables[0].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "B" }); // A → B
        databaseInfo.Tables[1].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "A" }); // B → A
        databaseInfo.Tables[2].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "D" }); // C → D
        databaseInfo.Tables[3].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "C" }); // D → C

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        // A and B can only join to each other, C and D can only join to each other
        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "B");
        _ = databaseInfo.Tables[1].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "A");
        _ = databaseInfo.Tables[2].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "D");
        _ = databaseInfo.Tables[3].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "C");
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldHandleTableWithNoKeys()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "Lonely", SchemaName = "dbo", TableId = 1 }
            ]
        };

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().BeEmpty();
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldHandleCaseInsensitiveTableNames()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "Parent", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "child", SchemaName = "dbo", TableId = 2 }
            ]
        };
        databaseInfo.Tables[1].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "PARENT" });

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        _ = databaseInfo.Tables[1].TablesICanJoinTo.Should().ContainSingle(t => t.Name == "Parent");
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldNotAddDuplicateTables_WhenIndirectAndDirectJoinsExist()
    {
        var databaseInfo = new DatabaseInfo
        {
            Name = "TestDatabase",
            Tables =
            [
                new TableInfo { Name = "A", SchemaName = "dbo", TableId = 1 },
                new TableInfo { Name = "B", SchemaName = "dbo", TableId = 2 },
                new TableInfo { Name = "C", SchemaName = "dbo", TableId = 3 }
            ]
        };
        // A → B (direct), A → C (direct), B → C (direct)
        databaseInfo.Tables[0].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "B" });
        databaseInfo.Tables[0].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "C" });
        databaseInfo.Tables[1].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = "C" });

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        // A can join to B and C, but C should not appear twice
        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().Contain(t => t.Name == "B");
        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().Contain(t => t.Name == "C");
        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().HaveCount(2);
    }

    [Fact]
    public void PopulateTablesICanJoinTo_ShouldHandleLargeNumberOfTables()
    {
        var tables = new List<TableInfo>();
        for (var i = 1; i <= 50; i++)
        {
            tables.Add(new TableInfo { Name = $"T{i}", SchemaName = "dbo", TableId = i });
        }
        // Create a chain: T1 → T2 → T3 ... → T50
        for (var i = 0; i < 49; i++)
        {
            tables[i].Keys.Add(new KeyInfo { IsForeignKey = true, ReferencedTableName = $"T{i + 2}" });
        }

        var databaseInfo = new DatabaseInfo { Name = "TestDatabase", Tables = tables };

        DatabaseInterrogator.PopulateDatabaseForeignAndPrimaryTables(databaseInfo);

        // T1 should be able to join to all others
        _ = databaseInfo.Tables[0].TablesICanJoinTo.Should().HaveCount(49);
        // T50 should be able to join to all others (if bidirectional is correct)
        _ = databaseInfo.Tables[49].TablesICanJoinTo.Should().HaveCount(49);
    }
}
