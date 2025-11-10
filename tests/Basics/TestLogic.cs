//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0

namespace tests_mandado.Basics;

using System.Data;
using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using models.tables;
using Services.Repositories;
using tests_mandado.utilities;
using Xunit;

public class TestLogic : IClassFixture<MymandadoWebAppFactory>
{


    private readonly Repo_StoredProcedures _repoStoredPro;
    private readonly Repo_TableInfos _repoTables;


    private readonly Repo_AnyTable<MND_PRODUCT> _repoDBProducts;

    private readonly IRepo_Users _repoUsers;
    private readonly IRepo_CartItems _repoCart;
    private readonly IRepo_Products _repoProducts;
    public TestLogic(MymandadoWebAppFactory factory) 
    {
        IServiceScope scope;
        scope = factory.Services.CreateScope();
        using (scope)
        {
            _repoCart = scope.ServiceProvider.GetRequiredService<IRepo_CartItems>();
            _repoUsers = scope.ServiceProvider.GetRequiredService<IRepo_Users>();
            _repoProducts = scope.ServiceProvider.GetRequiredService<IRepo_Products>();
            _repoDBProducts = scope.ServiceProvider.GetRequiredService<Repo_AnyTable<MND_PRODUCT>>();
        }
    }

    [Fact]
    public  void UserCRUD()
    {
        User[] allUsers = _repoUsers.GetAll();
        Assert.NotEmpty(allUsers);

        string userName = "usertest";
        User? u;
        u= _repoUsers.GetUserByName(userName);
        Assert.Null(u);

        u=_repoUsers.AddByName(userName);
        u= _repoUsers.GetUserByName(userName);
        Assert.NotNull(u);

         _repoUsers.Delete(u);
        u= _repoUsers.GetUserByName(userName);
        Assert.Null(u);
    }
    [Fact]
    public  void TABLE_CRUD()
    {
        MND_PRODUCT[] all;
        all= _repoDBProducts.GetAll();

        string notHere = "nothere";
        MND_PRODUCT? p = new MND_PRODUCT() { prd_name = notHere };

        all= _repoDBProducts.GetAll();
        Assert.DoesNotContain(all,x => x.prd_name == notHere);
        _repoDBProducts.Add(ref p);

        all= _repoDBProducts.GetAll();
        p = all.Where(x => x.prd_name == notHere).FirstOrDefault();
        Assert.Contains(all, x => x.prd_id == p.prd_id); // id KEY

        _repoDBProducts.Delete(p);
        all= _repoDBProducts.GetAll();
        Assert.DoesNotContain(all, x => x.prd_name == notHere);

    }
}