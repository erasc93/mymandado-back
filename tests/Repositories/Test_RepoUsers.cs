using core_mandado.Cart;
using core_mandado.Users;
using tests_mandado.utilities;

namespace tests_mandado.Repositories;

public class Test_RepoUsers(MymandadoWebAppFactory _fac) : IClassFixture<MymandadoWebAppFactory>
{
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

            testUser = _fac._repoUsers.GetUserByName(userName);
            Assert.Null(testUser);
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
