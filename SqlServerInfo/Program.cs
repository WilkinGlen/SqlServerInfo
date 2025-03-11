using SqlServerInfo.Services;

var connectionString = "Server=localhost;Integrated Security=True;TrustServerCertificate=True;";
ISqlServerInfoService sqlServerInfoService = new SqlServerInfoService();
var _ = await sqlServerInfoService.GetDatabasesAsync(connectionString);

Console.WriteLine("Done");
