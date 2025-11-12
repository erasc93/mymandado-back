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

public class Test_RepoCart(MymandadoWebAppFactory _fac) : IClassFixture<MymandadoWebAppFactory>
{
    [Fact]
    public void TEST_CreateDeleteCart()
    {
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
