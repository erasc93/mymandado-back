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

public class Test_RepoUsers : IClassFixture<MymandadoWebAppFactory>
{


    //private readonly Repo_StoredProcedures _repoStoredPro;
    //private readonly Repo_TableInfos _repoTables;
    //private readonly Repo_AnyTable<MND_PRODUCT> _repoDBProducts;

    //private readonly IRepo_CartItems _repoCartItems;
    //private readonly IRepo_Products _repoProducts;
    //private readonly IRepo_Users _repoUsers;
    //private readonly IRepo_Cart _repoCart;
    //private readonly IQueries _queries;
    private MymandadoWebAppFactory _fac;

    public Test_RepoUsers(MymandadoWebAppFactory fac)
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
    public void TEST_CreateDeleteUsers()
    {
        User userWhenFound;
        _fac.SecureTest((conn, trans) =>
        {
            User? testUser;
            User[]
                allUsers = _fac._repoUsers.GetAll(conn, trans);
            Assert.NotEmpty(allUsers);
            string
                userName = "usertest13";
            testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
            Assert.Null(testUser);

            testUser = _fac._repoUsers.AddByName(userName, conn, trans);
            testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
            Assert.NotNull(testUser);
            userWhenFound = testUser;

            Cart[]
                carts = _fac._repoCart.GetAll(testUser, conn, trans);
            Assert.NotEmpty(carts);
            Cart?
                firstCart = carts[0];
            Assert.NotNull(firstCart);
            Assert.NotNull(firstCart.items);
            Assert.Empty(firstCart.items);

            _fac._repoUsers.Delete(testUser, conn, trans);
            testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
            Assert.Null(testUser);

            carts = _fac._repoCart.GetAll(userWhenFound, conn, trans);
            Assert.Empty(carts);
        });
    }
    [Fact]
    public void TEST_RollBackWorks()
    {
        User? testUser;
        string
            userName = "usertest13";
        try
        {

            _fac.SecureTest((conn, trans) =>
            {
                User[]
                    allUsers = _fac._repoUsers.GetAll(conn, trans);
                Assert.NotEmpty(allUsers);
                testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
                Assert.Null(testUser);

                testUser = _fac._repoUsers.AddByName(userName, conn, trans);
                testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
                Assert.NotNull(testUser);
            });
            throw new TestFailureException();
        }
        catch (TestFailureException)
        {
            _fac.SecureTest((conn, trans) =>
            {
                testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
                Assert.Null(testUser);
            });
        }
    }
    [Fact]
    public void TEST_WhenUserDeleted_CartItemsIsDeleted_CartDeleted()
    {
        User? testUser;
        string
            userName = "usertest13";
        try
        {
            _fac.SecureTest((conn, trans) =>
            {
                User[]
                    allUsers = _fac._repoUsers.GetAll(conn, trans);
                Assert.NotEmpty(allUsers);
                testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
                Assert.Null(testUser);

                testUser = _fac._repoUsers.AddByName(userName, conn, trans);
                testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
                Assert.NotNull(testUser);
            });
            throw new TestFailureException();
        }
        catch (TestFailureException)
        {
            _fac.SecureTest((conn, trans) =>
            {
                testUser = _fac._repoUsers.GetUserByName(userName, conn, trans);
                Assert.Null(testUser);
            });
        }
    }
    private class TestFailureException : Exception { }
}
