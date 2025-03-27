using SqlServerInfo.Services;

var connectionString = "Server=localhost;Integrated Security=True;TrustServerCertificate=True;";
var cancelProvider = new CancellationTokenSource();
var sqlServerInfoService = new SqlServerInfoService();

var _ = await sqlServerInfoService.GetDatabasesAsync(connectionString);

await foreach (var db in sqlServerInfoService.GetDatabasesAsyncEnumerable(connectionString, cancelProvider.Token))
{
    Console.WriteLine(db.Name);
}

Console.WriteLine("Done");
