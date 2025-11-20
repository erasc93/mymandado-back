//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado.utilities;

using api_mandado.Controllers;
using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using models.tables;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using Services.Dapper.Queries;
using Services.Repositories;
using System.Data;

public class MymandadoWebAppFactory : ACustomWebApplicationFactory
{
    private readonly Repo_StoredProcedures _repoStoredPro;
    private readonly Repo_TableInfos _repoTables;
    private readonly IQueries _queries;

    public readonly Repo_AnyTable<MND_PRODUCT> _repoDBProducts;
    private readonly Repo_AnyTable<MND_USERS> _USERS;
    private readonly Repo_AnyTable<MND_CART_ITEM> _CART_ITEMS;
    private readonly Repo_AnyTable<MND_CART> _CART;
    private readonly Repo_AnyTable<MND_USER_EXCLUSIONS> _USER_EXCLUSIONS;

    public readonly IRepo_Users _repoUsers;
    public readonly IRepo_Products _repoProducts;
    public readonly IRepo_Cart _repoCart;
    public readonly IRepo_CartItems _repoCartItems;

    private readonly ITransactionHandle _handle;
    public MymandadoWebAppFactory()
    {
        using var scope = this.Services.CreateScope();

        _handle = Svc<ITransactionHandle>(scope)!;
        _queries = Svc<IQueries>(scope)!;

        _repoUsers = Svc<IRepo_Users>(scope)!;
        _repoProducts = Svc<IRepo_Products>(scope)!;
        _repoCart = Svc<IRepo_Cart>(scope)!;
        _repoCartItems = Svc<IRepo_CartItems>(scope)!;

        _repoDBProducts = Svc<Repo_AnyTable<MND_PRODUCT>>(scope)!;
        _USERS = Svc<Repo_AnyTable<MND_USERS>>(scope)!;
        _CART_ITEMS = Svc<Repo_AnyTable<MND_CART_ITEM>>(scope)!;
        _CART = Svc<Repo_AnyTable<MND_CART>>(scope)!;
        _USER_EXCLUSIONS = Svc<Repo_AnyTable<MND_USER_EXCLUSIONS>>(scope)!;

        _queries = Svc<IQueries>(scope)!;
        _repoStoredPro = Svc<Repo_StoredProcedures>(scope)!;
        _repoTables = Svc<Repo_TableInfos>(scope)!;
    }

    private static T? Svc<T>(IServiceScope scope) where T : notnull
        => scope.ServiceProvider.GetRequiredService<T>();
    protected override void AddControllers(IServiceCollection services)
    {
        services.AddScoped<WeatherForecastController>();
        services.AddScoped<UsersController>();
        services.AddScoped<ProductsController>();
        services.AddScoped<CartItemsController>();
    }

    public void SecureTest(Action testfunction)
    {
        _queries.ExecuteInTransaction(() =>
        {
            testfunction();
            if (_handle.transaction is null)
            {
                throw new Exception("Handle was deleted before the end");
            }

        }, immediatRollback: true);
    }

    /// <summary>
    /// Intended to ensure rollback transaction are allways executed on tests;
    /// </summary>
    private class Success : Queries.RollBackException { }
}
