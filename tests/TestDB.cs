//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado;

using System.Threading.Tasks;
using information_schema.models;
using Microsoft.Extensions.DependencyInjection;
using repositories;
using repositories.infoSchema;
using tests_mandado.utilities;
using Xunit;

public class TestDB : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;


    private readonly Repo_StoredProcedures _repoStoredPro;

    private readonly Repo_Tables _repoTables;
    private readonly Repo_Cart _repoCart;
    private readonly Repo_Users _repoUsers;

    private readonly Repo_Products _repoProducts;

    public TestDB(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            _repoStoredPro = scope.ServiceProvider.GetRequiredService<Repo_StoredProcedures>();
            _repoTables = scope.ServiceProvider.GetRequiredService<Repo_Tables>();

            _repoCart = scope.ServiceProvider.GetRequiredService<Repo_Cart>();
            _repoUsers = scope.ServiceProvider.GetRequiredService<Repo_Users>();
            _repoProducts = scope.ServiceProvider.GetRequiredService<Repo_Products>();
        }
    }

    [Fact]
    public async Task Products_Should_Return_Success()
    {
        StoredProcedure? storedprocedures;
        storedprocedures = _repoStoredPro.GetStoredProcedures()
                                        .Where(x => x.ROUTINE_NAME == "sp_tables")
                                        .First();
        Assert.NotNull(storedprocedures);

        // ---
        string[] tables;
        string? usersTable;
        tables = _repoTables.GetTableNames("mymandado");
        usersTable = tables.Where(x => x == "USERS").First();
        Assert.NotNull(usersTable);

        // ---
        DbInfo_Columns[] columns;
        columns = _repoTables.GetColumns(usersTable);
        Assert.Contains(columns, col => col.Field == "usr_name");

        // ---

    }
}
