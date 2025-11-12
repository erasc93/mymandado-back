using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.Extensions.DependencyInjection;
using models.tables;
using Services.Dapper.Interfaces;
using Services.Repositories;
using tests_mandado.utilities;
using ZstdSharp.Unsafe;

namespace tests_mandado.Repositories;

public class Test_RepoCart : IClassFixture<MymandadoWebAppFactory>
{
    private readonly MymandadoWebAppFactory _fac;

    //private readonly Repo_AnyTable<MND_PRODUCT> _repoDBProducts;

    //private readonly IRepo_CartItems _repoCartItems;
    //private readonly IRepo_Products _repoProducts;
    //private readonly IRepo_Users _repoUsers;
    //private readonly IRepo_Cart _repoCart;
    //private readonly IQueries _queries;

    public Test_RepoCart(MymandadoWebAppFactory fac)
    {
        //IServiceScope
        //    scope = fac.Services.CreateScope();
        //using (scope)
        //{
        //    _repoCart = fac.Svc<IRepo_Cart>(scope)!;
        //    _repoUsers = fac.Svc<IRepo_Users>(scope)!;
        //    _repoProducts = fac.Svc<IRepo_Products>(scope)!;
        //    _repoDBProducts = fac.Svc<Repo_AnyTable<MND_PRODUCT>>(scope)!;
        //    _repoCartItems = fac.Svc<IRepo_CartItems>(scope)!;
        //    _queries = fac.Svc<IQueries>(scope)!;
        //}
        _fac = fac;
    }

    [Fact]
    public void TEST_CreateDeleteCart()
    {
        User userWhenFound;
        _fac.SecureTest(
        (conn, trans) =>
        {
            User? testUser;
            User[]
                allUsers = _fac._repoUsers.GetAll(conn, trans);
            Assert.NotEmpty(allUsers);
            string
                productName = "usertest13";
            testUser = _fac._repoUsers.GetUserByName(productName, conn, trans);
            Assert.Null(testUser);
        });
    }

    private class TestFailureException : Exception { }
}
