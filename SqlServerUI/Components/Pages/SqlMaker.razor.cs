namespace SqlServerUI.Components.Pages;

//using Microsoft.AspNetCore.Components;
//using SqlServerInfo.Models;
//using SqlServerInfo.Services;

public sealed partial class SqlMaker
{
    //private List<DatabaseInfo>? databases;
    //private DatabaseInfo? selectedDatabase;

    //[Inject]
    //private ISqlServerInfoService? SqlServerInfoService { get; set; }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //    {
    //        this.databases = [.. await this.SqlServerInfoService!.GetDatabasesAsync(
    //            "Server=localhost;Integrated Security=True;TrustServerCertificate=True;")];
    //        this.StateHasChanged();
    //        this.selectedDatabase = this.databases?.FirstOrDefault(x => x.Name.Equals("ApiSelfService"));
    //        this.SqlServerInfoService!.PopulateDatabaseForeignAndPrimaryTables(this.selectedDatabase!);
    //        this.StateHasChanged();
    //    }
    //}

    //private void GenerateSql()
    //{

    //}
}
