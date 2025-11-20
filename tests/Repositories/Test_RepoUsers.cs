using core_mandado.Cart;
using core_mandado.Users;
using tests_mandado.utilities;

namespace tests_mandado.Repositories;

public class Test_RepoUsers(MymandadoWebAppFactory _fac) : IClassFixture<MymandadoWebAppFactory>
{
    [Fact]
    public void TEST_GetALl()
    {
        _fac.SecureTest(() =>
        {
            User[] users = _fac._repoUsers.GetAll();
            Assert.NotEmpty(users);
        });
    }
    [Fact]
    public void TEST_CreateDeleteUsers()
    {
        User userWhenFound;
        _fac.SecureTest(() =>
        {
            User? testUser;
            User[]
                allUsers = _fac._repoUsers.GetAll();
            Assert.NotEmpty(allUsers);
            string
                userName = "usertest13";
            testUser = _fac._repoUsers.GetUserByName(userName);
            Assert.Null(testUser);

            testUser = _fac._repoUsers.AddByName(userName);
            testUser = _fac._repoUsers.GetUserByName(userName);
            Assert.NotNull(testUser);
            userWhenFound = testUser;

            Cart[]
                carts = _fac._repoCart.GetAll(testUser);
            Assert.NotEmpty(carts);
            Cart?
                firstCart = carts[0];
            Assert.NotNull(firstCart);
            Assert.NotNull(firstCart.items);
            Assert.Empty(firstCart.items);

            _fac._repoUsers.Delete(testUser);
            testUser = _fac._repoUsers.GetUserByName(userName);
            Assert.Null(testUser);

            carts = _fac._repoCart.GetAll(userWhenFound);
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
            _fac.SecureTest(() =>
            {
                User[]
                    allUsers = _fac._repoUsers.GetAll();
                Assert.NotEmpty(allUsers);
                testUser = _fac._repoUsers.GetUserByName(userName);
                Assert.Null(testUser);

                testUser = _fac._repoUsers.AddByName(userName);
                testUser = _fac._repoUsers.GetUserByName(userName);
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
            _fac.SecureTest(() =>
            {
                User[]
                    allUsers = _fac._repoUsers.GetAll();
                Assert.NotEmpty(allUsers);
                testUser = _fac._repoUsers.GetUserByName(userName);
                Assert.Null(testUser);

                testUser = _fac._repoUsers.AddByName(userName);
                testUser = _fac._repoUsers.GetUserByName(userName);
                Assert.NotNull(testUser);
            });
            throw new TestFailureException();
        }
        catch (TestFailureException)
        {
            _fac.SecureTest(() =>
            {
                testUser = _fac._repoUsers.GetUserByName(userName);
                Assert.Null(testUser);
            });
        }
    }
    private class TestFailureException : Exception { }
}
