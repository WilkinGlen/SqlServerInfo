using SqlServerInfo.Services;

var connectionString = "Server=localhost;Integrated Security=True;TrustServerCertificate=True;";
ISqlServerInfoService sqlServerInfoService = new SqlServerInfoService();
var _ = await sqlServerInfoService.GetDatabasesAsync(connectionString);

var cancelProvider = new CancellationTokenSource();
await foreach(var db in sqlServerInfoService.GetDatabasesAsyncEnumerable(connectionString, cancelProvider.Token))
{
    Console.WriteLine(db.Name);
    cancelProvider.Cancel();
}

Console.WriteLine("Done");
