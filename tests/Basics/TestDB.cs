//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado.Basics;

using System.Threading.Tasks;
using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using models.information_schema;
using Services.Repositories;
using tests_mandado.utilities;
using Xunit;

public class TestDB : IClassFixture<MymandadoWebAppFactory>
{
    private readonly HttpClient _client;


    private readonly Repo_StoredProcedures _repoStoredPro;

    private readonly Repo_TableInfos _repoTables;

    public TestDB(MymandadoWebAppFactory factory)
    {
        _client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            _repoStoredPro = scope.ServiceProvider.GetRequiredService<Repo_StoredProcedures>();
            _repoTables = scope.ServiceProvider.GetRequiredService<Repo_TableInfos>();
        }
    }

    [Fact]
    public async Task Test_StoredProcedures_Exist()
    {
        StoredProcedure? storedprocedures;
        StoredProcedure[] all;
        all = _repoStoredPro.GetStoredProcedures();

        storedprocedures = all.Where(x => x.ROUTINE_NAME == "sp_tables").FirstOrDefault();
        Assert.Null(storedprocedures);
    }
    [Fact]
    public async Task Tests_TableUSERS_HasOneElement()
    {
        string[] tables;
        string? usersTable;
        tables = _repoTables.GetTableNames();

        Assert.NotNull(tables);
        Assert.NotEmpty(tables);
        Assert.Contains("USERS", tables);

        //usersTable

        // ---
        DbInfo_Columns[] columns;
        usersTable = tables.Where(x => x == "USERS").First();
        columns = _repoTables.GetColumns(usersTable);
        Assert.Contains(columns, col => col.Field == "usr_name");

        // ---
    }
}
